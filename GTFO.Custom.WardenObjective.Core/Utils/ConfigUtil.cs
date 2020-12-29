using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

//TODO: Fix Json of this shit
namespace GTFO.CustomObjectives.Utils
{
    public static class ConfigUtil
    {
        public static string GlobalPath { get; private set; }
        public static string LocalPath { get; private set; }

        public static readonly JsonSerializerSettings JSONSetting;
        

        static ConfigUtil()
        {
            GlobalPath = Path.Combine(MelonLoaderBase.UserDataPath, "CustomObjective");

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

        #region GlobalConfig

        public static bool TryGetGlobalConfig<D>(string name, out D obj)
        {
            var content = GetGlobalConfigContent(name);
            if (string.IsNullOrEmpty(content))
            {
                obj = default;
                return false;
            }

            if (!File.Exists(content))
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

        #endregion

        #region LocalConfig

        public static bool TryGetLocalConfig<D>(string name, out D obj)
        {
            var content = GetLocalConfigContent(name);
            if (string.IsNullOrEmpty(content))
            {
                obj = default;
                return false;
            }
                
            if(!File.Exists(content))
            {
                obj = default;
                return false;
            }

            obj = JsonConvert.DeserializeObject<D>(content);
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
                return null;
            }
        }

        public static string GetLocalConfigPath(string filename)
        {
            return Path.Combine(LocalPath, filename);
        }

        #endregion
    }
}