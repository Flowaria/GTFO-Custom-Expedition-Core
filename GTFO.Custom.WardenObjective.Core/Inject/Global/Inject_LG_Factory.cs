using Harmony;
using LevelGeneration;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject.Global
{
    [HarmonyPatch(typeof(LG_Factory), "FactoryDone")]
    internal static class Inject_LG_Factory
    {
        internal static void Postfix()
        {
            MelonLogger.Log("Global: OnBuildDone");
            GlobalMessage.OnBuildDone?.Invoke();
        }
    }
}
