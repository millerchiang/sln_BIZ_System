using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace prj_BIZ_System.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] ArrayMerge<T>(this T[] array1, T[] array2)
        {
            T[] newArray = new T[array1.Length + array2.Length];
            Array.Copy(array1, newArray, array1.Length);
            Array.Copy(array2, 0, newArray, array1.Length, array2.Length);
            return newArray;
        }
    }
}