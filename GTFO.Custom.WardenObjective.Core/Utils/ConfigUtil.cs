using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

//TODO: Fix Json of this shit
namespace GTFO.CustomObjectives.Utils
{
    public static class ConfigUtil
    {
        public const string DATADUMPER_GUID = "Data-Dumper";

        private static IL2CPPChainloader ChainLoader;
        public static string GlobalPath { get; private set; }
        public static string LocalPath { get; private set; }
        public static bool HasLocalPath { get; private set; } = false;

        public static readonly JsonSerializerSettings JSONSetting;

        static ConfigUtil()
        {
            ChainLoader = IL2CPPChainloader.Instance;
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

        public static void SetupLocalConfig()
        {
            if (HasLocalPath)
                return;

            if (ChainLoader.Plugins.TryGetValue(DATADUMPER_GUID, out var info))
            {
                try
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var ddAsm = assemblies.First(a => a.Location == info.Location);

                    var types = ddAsm.GetTypes();
                    var cfgManagerType = types.First(t => t.Name == "ConfigManager");

                    var dataPathProp = cfgManagerType.GetProperty("GameDataPath", BindingFlags.Public | BindingFlags.Static);
                    var customPathProp = cfgManagerType.GetProperty("CustomPath", BindingFlags.Public | BindingFlags.Static);

                    var dataPath = dataPathProp.GetValue(null) as string;
                    var customPath = customPathProp.GetValue(null) as string;

                    HasLocalPath = true;
                    LocalPath = customPath;
                }
                catch(Exception e)
                {
                    Logger.Error("Exception thrown while reading path from Data Dumper:\n{0}", e.ToString());
                    HasLocalPath = false;
                    LocalPath = null;
                }
            }
        }

        #region GlobalConfig

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