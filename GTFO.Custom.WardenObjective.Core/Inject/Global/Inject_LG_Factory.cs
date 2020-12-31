using GTFO.CustomObjectives.Utils;
using Harmony;
using LevelGeneration;
using MelonLoader;

namespace GTFO.CustomObjectives.Inject.Global
{
    [HarmonyPatch(typeof(LG_Factory), "FactoryDone")]
    internal static class Inject_LG_Factory
    {
        internal static void Postfix()
        {
            Logger.Verbose("Global: OnBuildDone");
            GlobalMessage.OnBuildDone?.Invoke();
        }
    }
}