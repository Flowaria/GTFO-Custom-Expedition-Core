using CustomExpeditions.Messages;
using HarmonyLib;
using LevelGeneration;

namespace CustomExpeditions.Inject.Global
{
    [HarmonyPatch(typeof(LG_Factory), "FactoryDone")]
    static class Inject_LG_Factory
    {
        internal static void Postfix()
        {
            Logger.Verbose("Global: OnBuildDone");
            GlobalMessage.OnBuildDone?.Invoke();
        }
    }
}