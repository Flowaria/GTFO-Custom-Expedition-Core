using System.Collections.Generic;

namespace GTFO.CustomObjectives.Extensions
{
    public static class ListExtension
    {
        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this List<T> list)
        {
            var newList = new Il2CppSystem.Collections.Generic.List<T>();

            foreach (var data in list)
            {
                newList.Add(data);
            }

            return newList;
        }
    }
}