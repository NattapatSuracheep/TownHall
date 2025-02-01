using System.Collections.Generic;

public class ListExtension
{
    public static List<(T1, T2)> MapLists<T1, T2>(List<T1> list1, List<T2> list2)
    {
        List<(T1, T2)> result = new List<(T1, T2)>();

        // Nested loop to create every possible pair
        foreach (var item1 in list1)
        {
            foreach (var item2 in list2)
            {
                result.Add(new(item1, item2));
            }
        }

        return result;
    }
}