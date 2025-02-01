using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class GoogleSheetParser
{
    public class GoogleSheetsResponse
    {
        public List<List<object>> values;
    }

    private int beginDataRow;
    private List<List<object>> parseData;
    private GoogleSheetParseToClass parser = new();

    public T ParseToClass<T>(string rawData, int beginDataRowInGoogleSheet)
    {
        Log.Logging($"Parse to class: {typeof(T).Name} / beginDataRow: {beginDataRowInGoogleSheet}");

        this.beginDataRow = beginDataRowInGoogleSheet - 1;

        //convert csv string data to object
        parseData = JsonConvert.DeserializeObject<GoogleSheetsResponse>(rawData).values;

        var result = ParseToClass<T>();

        Log.Logging(JsonConvert.SerializeObject(result, Formatting.Indented));

        return result;
    }

    private T ParseToClass<T>()
    {
        var parseType = typeof(T);

        //check if the type is array
        if (parseType.IsArray)
        {
            return (T)HandleArrayType<T>();
        }

        // parseType.IsGenericType: check if the type is generic 
        else if (parseType.IsGenericType)
        {
            // typeof(IList).IsAssignableFrom(parseType): check if IList is assignable from type (aka check if type is List<T>)
            if (typeof(IList).IsAssignableFrom(parseType))
            {
                return (T)HandleGenericListType<T>();
            }

            // typeof(IDictionary).IsAssignableFrom(parseType): check if IDictionary is assignable from type (aka check if type is Dictionary<T1,T2>)
            else if (typeof(IDictionary).IsAssignableFrom(parseType))
            {
                var keyType = parseType.GetGenericArguments()[0];
                var valueType = parseType.GetGenericArguments()[1];
                return (T)HandleGenericDictionaryType(keyType, valueType);
            }
        }

        //if the type is not array or generic, parse it as a single object
        return (T)HandleSingleType<T>();
    }

    private object HandleArrayType<T>()
    {
        var parseType = typeof(T);

        // Get type of the array element type (ex. int[], string[], etc.)
        var elementType = parseType.GetElementType();

        /*

            Create an array of the element type with the desired size
            - Array.CreateInstance(Type elementType, int length)
        */
        var arrayObj = Array.CreateInstance(elementType, parseData.Count - beginDataRow);

        for (var i = 0; i < arrayObj.Length; i++)
        {
            // Create an instance of the element type
            var newObj = Activator.CreateInstance(elementType);

            // Set data to the element
            newObj = parser.ParseToClass(newObj, beginDataRow + i, parseData);

            // Set the element to the array
            arrayObj.SetValue(newObj, i);
        }

        return (T)(object)arrayObj;
    }


    private object HandleGenericListType<T>()
    {
        var parseType = typeof(T);

        // Get type of List element type (ex. List<int>, List<string>, etc.)
        var elementType = parseType.GetGenericArguments()[0];

        // Create an instance of the List<T> type
        var instance = Activator.CreateInstance(parseType);

        // Convert instance to List
        var list = instance as IList;

        for (var i = 0; i < parseData.Count - beginDataRow; i++)
        {
            var value = new object();

            try
            {
                value = Activator.CreateInstance(elementType);
            }
            catch { }

            // Set data to the element
            value = parser.ParseToClass(value, beginDataRow + i, parseData);

            list.Add(value);
        }

        return instance;
    }

    private object HandleGenericDictionaryType(Type keyType, Type elementType)
    {
        var parseType = typeof(Dictionary<,>);

        // Create the dictionary type definition
        var constructedType = parseType.MakeGenericType(keyType, elementType);

        // Create an instance of the dictionary<T1,T2> type
        var instance = Activator.CreateInstance(constructedType);

        // Convert instance to dictionary
        var dictionary = instance as IDictionary;

        for (var i = 0; i < parseData.Count - beginDataRow; i++)
        {
            // Get and set key data as first value in the data row
            var key = parseData[beginDataRow + i][0];

            var value = new object();

            try
            {
                value = Activator.CreateInstance(elementType);
            }
            catch { }

            // Set value data
            value = parser.ParseToClass(value, beginDataRow + i, parseData);  // Your parsing method for the value

            // Add the key-value pair to the dictionary
            dictionary.Add(key, value);
        }

        return instance;
    }

    private object HandleSingleType<T>()
    {
        var singleObj = Activator.CreateInstance<T>();
        return (T)parser.ParseToClass(singleObj, beginDataRow, parseData);
    }
}