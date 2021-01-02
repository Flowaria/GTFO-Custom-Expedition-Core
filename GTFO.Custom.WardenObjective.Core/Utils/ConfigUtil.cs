using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Linq;

//TODO: Fix Json of this shit
namespace GTFO.CustomObjectives.Utils
{
    public static class ConfigUtil
    {
        public static string GlobalPath { get; private set; }
        public static string LocalPath { get; private set; }
        public static bool HasLocalPath { get; private set; } = false;

        public static readonly JsonSerializerSettings JSONSetting;

        static ConfigUtil()
        {
            GlobalPath = Paths.ConfigPath;
            LocalPath = Paths.ConfigPath;

            if (!Directory.Exists(GlobalPath))
            {
                Directory.CreateDirectory(GlobalPath);
            }

            JSONSetting = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                Converters = new JsonConverter[]
                {
                    new StringEnumConverter()
                }
            };
        }

        public static void SetupLocalConfig(ConfigFile config)
        {
            if (HasLocalPath)
                return;

            return;
            
            var isDataDumperExist = IL2CPPChainloader.Instance.Plugins.ContainsKey("Data-Dumper"); //MAJOR: Fix it to DataDumper GUID later
            if (isDataDumperExist)
            {
                HasLocalPath = true;

                //READ FROM CONFIGMANAGER

                LocalPath = Path.Combine(Paths.ConfigPath, null);
            }
        }

        private static string GetDefaultFolder()
        {
            return "GameData_" + CellBuildData.GetRevision().ToString();
        }

        #region GlobalConfig

        //TODO: Implement This
        public static bool TryReadConfigAsList<W>(string name, out W[] arr)
        {
            arr = null;
            return true;
        }

        public static bool TryGetGlobalConfig<D>(string name, out D obj)
        {
            var content = GetGlobalConfigContent(name);
            if (string.IsNullOrEmpty(content))
            {
                obj = default;
                return false;
            }

            obj = JsonConvert.DeserializeObject<D>(content);
            return true;
        }

        public static string GetGlobalConfigContent(string filename)
        {
            try
            {
                return File.ReadAllText(GetGlobalConfigPath(filename));
            }
            catch
            {
                return null;
            }
        }

        public static string GetGlobalConfigPath(string filename)
        {
            return Path.Combine(GlobalPath, filename);
        }

        #endregion GlobalConfig

        #region LocalConfig

        public static void SaveLocalConfig<D>(string name, D obj)
        {
            var path = GetLocalConfigPath(name);
            if(string.IsNullOrEmpty(path))
            {
                var json = JsonConvert.SerializeObject(obj, JSONSetting);
                File.WriteAllText(path, json);
            }
        }

        public static bool TryGetLocalConfig<D>(string name, out D obj)
        {
            var content = GetLocalConfigContent(name);
            if (string.IsNullOrEmpty(content))
            {
                obj = default;
                return false;
            }

            obj = JsonConvert.DeserializeObject<D>(content, JSONSetting);
            return true;
        }

        public static string GetLocalConfigContent(string filename)
        {
            try
            {
                return File.ReadAllText(GetLocalConfigPath(filename));
            }
            catch
            {
                Logger.Verbose("Can't Read File!: {0}", GetLocalConfigPath(filename));
                return null;
            }
        }

        public static string GetLocalConfigPath(string filename)
        {
            if (HasLocalPath)
            {
                return Path.Combine(LocalPath, filename);
            }
            else
            {
                Logger.Verbose("LocalPath is Missing!");
                return null;
            }
        }

        #endregion LocalConfig
    }
}