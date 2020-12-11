using BepInEx;
using GTFO.CustomObjectives.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;

//TODO: Fix Json of this shit
namespace GTFO.CustomObjectives.Utils
{
    using ConverterList = Il2CppSystem.Collections.Generic.IList<JsonConverter>;

    public static class ConfigUtil
    {
        

        public static readonly string BasePath;
        public static readonly JsonSerializerSettings Setting;

        static ConfigUtil()
        {
            BasePath = Path.Combine(Paths.ConfigPath, "CustomObjective");

            var converters = new List<JsonConverter>
            {
                new StringEnumConverter()
            }.ToIl2CppList().Cast<ConverterList>();

            Setting = new JsonSerializerSettings()
            {
                Converters = converters
            };

            if(!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
        }

        public static dynamic ReadConfig<C>(string name, bool createNew = false) where C : class
        {
            var type = Il2CppType.From(typeof(C));
            var file = Path.Combine(BasePath, $"{name}.json");

            if (File.Exists(file))
            {   
                var obj = JsonConvert.DeserializeObject(File.ReadAllText(file), type, Setting);

                return obj;
            }
            else if (createNew)
            {
                var instance = Il2CppSystem.Activator.CreateInstance(type);
                File.WriteAllText(file, JsonConvert.SerializeObject(instance));

                return instance;
            }

            return null;
        }
    }
}
