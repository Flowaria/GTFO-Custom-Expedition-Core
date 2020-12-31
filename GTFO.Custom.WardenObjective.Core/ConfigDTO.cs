using System.Collections.Generic;

namespace GTFO.CustomObjectives
{
    internal class ConfigDTO
    {
        public bool UsingLogger = false;
        public bool UsingLogger_Log = false;
        public bool UsingLogger_Warn = false;
        public bool UsingLogger_Err = false;
    }

    internal class LocalConfigDTO
    {
        public string[] EnabledModules;
    }
}