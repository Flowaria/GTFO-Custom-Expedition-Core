using MelonLoader;
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

        public static void SetupLocalConfig()
        {
            if (HasLocalPath)
                return;

            var isDataDumperExist = MelonHandler.Mods.Any(x => x.Info.Name.Equals("Data-Dumper"));
            if (isDataDumperExist)
            {
                if(MelonPrefs.HasKey("Data Dumper", "RundownPackage"))
                {
                    HasLocalPath = true;

                    var path = MelonPrefs.GetString("Data Dumper", "RundownPackage");
                    LocalPath = Path.Combine(MelonLoaderBase.UserDataPath, path != "default" ? path : GetDefaultFolder());
                }
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

        public static bool TryGetLocalConfig<D>(string name, out D obj)
        {
            var content = GetLocalConfigContent(name);
            if (string.IsNullOrEmpty(content))
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