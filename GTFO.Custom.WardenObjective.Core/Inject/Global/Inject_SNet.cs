using CustomExpeditions.Messages;
using HarmonyLib;
using SNetwork;

namespace CustomExpeditions.Inject.Global
{
    [HarmonyPatch(typeof(SNet))]
    internal static class Inject_SNet
    {
        [HarmonyPostfix]
        [HarmonyPatch("ResetSession")]
        internal static void Post_ResetSession()
        {
            Logger.Verbose("Global (SNet): OnResetSession_SNet");
            GlobalMessage.OnResetSession_SNet?.Invoke();
        }

        [HarmonyPostfix]
        [HarmonyPatch("ValidateMasterData")]
        internal static void Post_ValidateMaster()
        {
            Logger.Verbose("Global (SNet): ValidateMasterData");
            GlobalMessage.OnValidateMasterData?.Invoke();
        }
    }
}