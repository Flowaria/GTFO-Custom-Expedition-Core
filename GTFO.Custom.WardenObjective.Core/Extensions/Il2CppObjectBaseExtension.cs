using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;

namespace GTFO.CustomObjectives.Extensions
{
    public static class Il2CppObjectBaseExtension
    {

        public static Il2CppSystem.Object DynamicCast(this Il2CppSystem.Object obj, Il2CppSystem.Type type)
        {
            var mi = obj.GetIl2CppType().GetMethod("Cast");

            var genericArg = new Il2CppReferenceArray<Il2CppSystem.Type>(1);
            genericArg[0] = type;

            var fooRef = mi.MakeGenericMethod(genericArg);
            return fooRef.Invoke(obj, null);
        }
    }
}
