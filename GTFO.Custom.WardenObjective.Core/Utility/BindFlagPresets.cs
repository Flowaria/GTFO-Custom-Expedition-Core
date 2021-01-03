using System.Reflection;

namespace CustomObjectives.Utility
{
    public static class BindFlagPresets
    {
        public const BindingFlags PUBLIC_INSTANCE = BindingFlags.Public | BindingFlags.Instance;
        public const BindingFlags PUBLIC_STATIC = BindingFlags.Public | BindingFlags.Static;

        public const BindingFlags NONPUBLIC_INSTANCE = BindingFlags.NonPublic | BindingFlags.Instance;
        public const BindingFlags NONPUBLIC_STATIC = BindingFlags.Public | BindingFlags.Static;
    }
}