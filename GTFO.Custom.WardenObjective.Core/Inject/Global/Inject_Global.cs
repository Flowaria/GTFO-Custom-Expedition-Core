using HarmonyLib;

namespace GTFO.CustomObjectives.Inject.Global
{
    [HarmonyPatch(typeof(Globals.Global))]
    internal static class Inject_Global
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnLevelCleanup")]
        internal static void Post_OnLevelCleanup()
        {
            Logger.Verbose("Global: OnLevelCleanup");
            GlobalMessage.OnLevelCleanup?.Invoke();
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnResetSession")]
        internal static void Post_OnResetSession()
        {
            Logger.Verbose("Global: OnResetSession");
            GlobalMessage.OnResetSession?.Invoke();
        }
    }
}