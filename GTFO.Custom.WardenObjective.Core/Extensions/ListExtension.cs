using System.Collections.Generic;

namespace CustomObjectives.Extensions
{
    using Il2CppCollection = Il2CppSystem.Collections.Generic;

    public static class ListExtension
    {
        #region 1D Array

        public static Il2CppCollection.List<T> ToIl2CppList<T>(this List<T> list)
        {
            if (list == null)
                return null;

            var newList = new Il2CppCollection.List<T>();
            foreach (var data in list)
                newList.Add(data);

            return newList;
        }

        public static List<T> ToMonoList<T>(this Il2CppCollection.List<T> list)
        {
            if (list == null)
                return null;

            var newList = new List<T>();
            foreach (var data in list)
                newList.Add(data);

            return newList;
        }

        public static T[] ToMonoArray<T>(this Il2CppCollection.List<T> list)
        {
            var array = list.ToArray();

            if (array == null)
                return null;

            if (array.Count == 0)
                return new T[0];

            var newArray = new T[array.Count];
            for (int i = 0; i < newArray.Length; i++)
            {
                newArray[i] = array[i];
            }

            return newArray;
        }

        #endregion 1D Array
    }
}