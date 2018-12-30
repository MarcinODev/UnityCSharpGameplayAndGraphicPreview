using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ListExtensions
{
    public static int SetOrAdd<T>(this List<T> list, int index, T value)
    {
        if(list.Count <= index || index < 0)
        {
            list.Add(value);
            return list.Count - 1;
        }

        list[index] = value;
        return index;
    }
}
