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
        internal static bool IsLogEnabled = false;
        internal static bool IsWarnEnabled = false;
        internal static bool IsErrorEnabled = false;

        public static void Log(string str, params object[] args)
        {
            if(IsGlobalEnabled)
                MelonLogger.Log(str, args);
        }

        public static void Warning(string str, params object[] args)
        {
            if (IsGlobalEnabled)
                MelonLogger.LogWarning(str, args);
        }

        public static void Error(string str, params object[] args)
        {
            if (IsGlobalEnabled)
                MelonLogger.LogError(str, args);
        }
    }
}
