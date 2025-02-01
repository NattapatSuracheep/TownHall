using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class GoogleSheetParseToClass
{
    public object ParseToClass(object obj, int dataIndex, List<List<object>> data)
    {
        // Get all properties in the class
        var allPropertyInfos = obj.GetType().GetProperties();

        //Get all nested class types
        var allNestedTypes = new List<Type>();
        foreach (var item in allPropertyInfos)
        {
            //if item is Class and in the same assembly
            if (item.PropertyType.IsClass && item.PropertyType.Assembly == Assembly.GetExecutingAssembly())
                allNestedTypes.Add(item.PropertyType);
        }

        var nestedNameAndTypeDict = new Dictionary<string, Type>();
        var nestedTypeAndPropertyInfoDict = new Dictionary<Type, PropertyInfo[]>();
        foreach (var nestedType in allNestedTypes)
        {
            //key: get the name of the property info that match the current nested type (aka variable name)
            //value: current nested type (aka variable type)
            nestedNameAndTypeDict.Add(allPropertyInfos.FirstOrDefault(x => x.PropertyType == nestedType)?.Name, nestedType);

            //key: current nested type
            //value: get all property info in current nested type
            nestedTypeAndPropertyInfoDict.Add(nestedType, nestedType.GetProperties());
        }

        // Log.Logging("allPropertyInfos");
        // Log.Array(allPropertyInfos);
        // Log.Logging("allNestedTypes");
        // Log.Array(nestedNameAndTypeDict);
        // foreach (var item in nestedTypeAndPropertyInfoDict)
        // {
        //     Log.Logging(item.Key);
        //     Log.Array(item.Value);
        // }

        var firstRow = data[0];
        for (var i = 0; i < firstRow.Count; i++)
        {
            var propertyName = firstRow[i]?.ToString();
            if (string.IsNullOrEmpty(propertyName)) continue;

            // Try to match a property name
            var property = allPropertyInfos.FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            if (property == null)
                continue;

            // Try to match a nested type
            if (nestedNameAndTypeDict.TryGetValue(propertyName, out var nestedType))
            {
                var nestedPropertyInfos = nestedTypeAndPropertyInfoDict[nestedType];
                for (var j = 0; j < nestedPropertyInfos.Length; j++)
                {
                    var nestedPropertyInfo = nestedPropertyInfos[j];
                    var nestedPropertyName = nestedPropertyInfo.Name;

                    var secondRow = data[1];
                    for (var k = 0; k < secondRow.Count; k++)
                    {
                        var dataNestedPropertyName = secondRow[k]?.ToString();
                        if (string.IsNullOrEmpty(dataNestedPropertyName) || !string.Equals(dataNestedPropertyName, nestedPropertyName, StringComparison.OrdinalIgnoreCase))
                            continue;

                        var dataRowNested = data[dataIndex];
                        var nestedPropertyValue = dataRowNested[k]; //get the value of the nested data (itemC) according to itemB index

                        // Log.Logging($"nestedPropertyName: {nestedPropertyName}, nestedPropertyValue: {nestedPropertyValue}");
                        nestedPropertyValue = ProcessData(nestedPropertyValue, nestedPropertyInfo);

                        //set the value of the nested property
                        //I have no idea how does property.GetValue(obj) works, but it works :D
                        nestedPropertyInfo.SetValue(property.GetValue(obj), Convert.ChangeType(nestedPropertyValue, nestedPropertyInfo.PropertyType));
                    }
                }
                continue;
            }

            var dataRow = data[dataIndex];
            var dataValue = dataRow[i]; //get the value of the nested data (itemC) according to itemB index

            // Log.Logging($"propertyName: {propertyName}, dataValue: {dataValue}");
            dataValue = ProcessData(dataValue, property);

            property.SetValue(obj, Convert.ChangeType(dataValue, property.PropertyType));
        }

        return obj;
    }

    private object ProcessData(object dataValue, PropertyInfo property)
    {
        var parseType = property.PropertyType;

        if (parseType.IsArray)
            return ProcessArrayData(dataValue, property);
        else if (parseType.IsGenericType && typeof(IList).IsAssignableFrom(parseType))
            return ProcessGenericListData(dataValue, property);

        return dataValue;
    }

    private object ProcessArrayData(object dataValue, PropertyInfo property)
    {
        var valueString = dataValue.ToString();
        if (string.IsNullOrEmpty(valueString))
            return dataValue;

        // Get type of the array element type (ex. int[], string[], etc.)
        var elementType = property.PropertyType.GetElementType();

        if (elementType == null)
            throw new InvalidOperationException("Unable to determine the element type of the array.");

        // Split the dataValue string
        var splitValues = valueString.Split(',');

        // Create an array of the element type with the desired size
        var convertedArray = Array.CreateInstance(elementType, splitValues.Length);

        for (int i = 0; i < splitValues.Length; i++)
        {
            // Convert 'splitValues' to the element type
            var elementValue = Convert.ChangeType(splitValues[i], elementType);

            // Set the element to the array
            convertedArray.SetValue(elementValue, i);
        }

        return convertedArray;
    }

    private object ProcessGenericListData(object dataValue, PropertyInfo property)
    {
        var valueString = dataValue.ToString();
        if (string.IsNullOrEmpty(valueString))
            return dataValue;

        // Get type of List element type (ex. List<int>, List<string>, etc.)
        var elementType = property.PropertyType.GetGenericArguments()[0];

        // Split the dataValue string
        var splitValues = valueString.Split(',');

        // Create an array of the element type
        var listType = typeof(List<>).MakeGenericType(elementType);
        var listInstance = Activator.CreateInstance(listType);

        // Convert instance to List
        var list = listInstance as IList;

        // Add items to the list using reflection
        foreach (var value in splitValues)
        {
            // Convert 'value' to the element type
            var convertedValue = Convert.ChangeType(value, elementType);

            // Add data to the element
            list.Add(convertedValue);
        }

        return list;
    }
}