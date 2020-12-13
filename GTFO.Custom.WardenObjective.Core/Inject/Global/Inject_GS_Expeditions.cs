using Harmony;
using MelonLoader;

namespace GTFO.CustomObjectives.Inject.Global
{
    [HarmonyPatch(typeof(GS_ExpeditionSuccess), "Enter")]
    internal static class Inject_GS_Success_Enter
    {
        internal static void Postfix()
        {
            MelonLogger.Log("Global: OnLevelSuccess");
            GlobalMessage.OnLevelSuccess?.Invoke();
        }
    }

    [HarmonyPatch(typeof(GS_ExpeditionFail), "Enter")]
    internal static class Inject_GS_Fail_Enter
    {
        internal static void Postfix()
        {
            MelonLogger.Log("Global: OnLevelFail");
            GlobalMessage.OnLevelFail?.Invoke();
        }
    }
}