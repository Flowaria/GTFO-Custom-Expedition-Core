using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Utility
{
    public static class BindFlagPresets
    {
        public const BindingFlags PUBLIC_INSTANCE = BindingFlags.Public | BindingFlags.Instance;
        public const BindingFlags PUBLIC_STATIC = BindingFlags.Public | BindingFlags.Static;

        public const BindingFlags NONPUBLIC_INSTANCE = BindingFlags.NonPublic | BindingFlags.Instance;
        public const BindingFlags NONPUBLIC_STATIC = BindingFlags.Public | BindingFlags.Static;
    }
}
