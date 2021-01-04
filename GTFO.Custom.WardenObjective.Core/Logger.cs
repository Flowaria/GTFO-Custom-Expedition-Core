using BepInEx.Logging;

namespace CustomExpeditions
{
    public static class Logger
    {
        internal static ManualLogSource LogInstance;

        internal static bool IsGlobalEnabled = true;
        internal static bool IsLogEnabled = true;
        internal static bool IsWarnEnabled = true;
        internal static bool IsErrorEnabled = true;
        internal static bool IsVerboseEnabled = false;

        public static void Verbose(object obj) => Verbose(obj.ToString());

        public static void Verbose(string str, params object[] args)
        {
            if (IsGlobalEnabled && IsVerboseEnabled)
                LogInstance?.LogInfo(string.Format($"[Verbose] {str}", args));
        }

        public static void Log(object obj) => Log(obj.ToString());

        public static void Log(string str, params object[] args)
        {
            if (IsGlobalEnabled && IsLogEnabled)
                LogInstance?.LogInfo(string.Format(str, args));
        }

        public static void Warning(object obj) => Warning(obj.ToString());

        public static void Warning(string str, params object[] args)
        {
            if (IsGlobalEnabled && IsWarnEnabled)
                LogInstance?.LogWarning(string.Format(str, args));
        }

        public static void Error(object obj) => Error(obj.ToString());

        public static void Error(string str, params object[] args)
        {
            if (IsGlobalEnabled && IsErrorEnabled)
                LogInstance?.LogError(string.Format(str, args));
        }
    }
}