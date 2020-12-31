using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives
{
    public static class Logger
    {
        internal static bool IsGlobalEnabled = true;
        internal static bool IsLogEnabled = true;
        internal static bool IsWarnEnabled = true;
        internal static bool IsErrorEnabled = true;
        internal static bool IsVerboseEnabled = false;

        public static void Verbose(string str, params object[] args)
        {
            if (IsGlobalEnabled && IsVerboseEnabled)
                MelonLogger.Log(ConsoleColor.Cyan, $"[Verbose] {str}", args);
        }

        public static void Log(string str, params object[] args)
        {
            if(IsGlobalEnabled && IsLogEnabled)
                MelonLogger.Log(str, args);
        }

        public static void Warning(string str, params object[] args)
        {
            if (IsGlobalEnabled && IsWarnEnabled)
                MelonLogger.LogWarning(str, args);
        }

        public static void Error(string str, params object[] args)
        {
            if (IsGlobalEnabled && IsErrorEnabled)
                MelonLogger.LogError(str, args);
        }
    }
}
