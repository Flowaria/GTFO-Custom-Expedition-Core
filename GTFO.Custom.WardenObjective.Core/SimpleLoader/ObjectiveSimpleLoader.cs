using GTFO.CustomObjectives.Utils;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.SimpleLoader
{
    internal static class ObjectiveSimpleLoader
    {
        public static void Setup()
        {
            var lookupPaths = new string[]
            {
                Path.Combine(Imports.GetGameDirectory(), "Mods"),
                Path.Combine(Imports.GetGameDirectory(), "Mods", "CustomObjectives")
            };

            foreach(var path in lookupPaths)
            {
                Logger.Verbose($"Searching Plugin Path: {path}");

                if (!Directory.Exists(path))
                    continue;

                var files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
                foreach(var file in files)
                {
                    try
                    {
                        var asm = Assembly.LoadFile(file);
                        LoadAssembly(asm);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        private static void LoadAssembly(Assembly asm)
        {
            var attribute = asm.GetCustomAttribute<CustomObjectiveAttribute>();

            if (attribute == null)
                return;


            var type = attribute.Entry;

            if (type == null)
                return;

            if (type.IsAbstract)
                return;

            if (type.BaseType == null)
                return;

            if (type.BaseType != typeof(ObjectiveSimpleEntry))
                return;

            Logger.Verbose("Loading Simple Entry dll: {0} / type: {1}", asm.GetName().Name, type.Name);
            var entry = Activator.CreateInstance(type) as ObjectiveSimpleEntry;
            entry.OnStart();
        }
    }
}
