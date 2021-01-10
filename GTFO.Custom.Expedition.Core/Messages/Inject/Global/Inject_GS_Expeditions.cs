using CustomExpeditions.Messages;
using HarmonyLib;

namespace CustomExpeditions.Messages.Inject.Global
{
    [HarmonyPatch(typeof(GS_ExpeditionSuccess), "Enter")]
    static class Inject_GS_Success_Enter
    {
        internal static void Postfix()
        {
            Logger.Verbose("Global: OnLevelSuccess");
            GlobalMessage.OnLevelSuccess?.Invoke();
        }
    }

    [HarmonyPatch(typeof(GS_ExpeditionFail), "Enter")]
    internal static class Inject_GS_Fail_Enter
    {
        internal static void Postfix()
        {
            Logger.Verbose("Global: OnLevelFail");
            GlobalMessage.OnLevelFail?.Invoke();
        }
    }
}