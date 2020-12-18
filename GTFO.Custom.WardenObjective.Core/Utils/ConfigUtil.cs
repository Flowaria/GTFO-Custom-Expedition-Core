using MelonLoader;
using System.IO;

//TODO: Fix Json of this shit
namespace GTFO.CustomObjectives.Utils
{
    public static class ConfigUtil
    {
        public static readonly string BasePath;
        private static string LocalPath;

        static ConfigUtil()
        {
            BasePath = Path.Combine(MelonLoaderBase.UserDataPath, "CustomObjective");

            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
        }

        public static string GetConfigPath(string name, string extension = ".json")
        {
            return Path.Combine(BasePath, $"{name}{extension}");
        }
    }
}