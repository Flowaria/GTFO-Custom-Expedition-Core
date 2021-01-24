using CustomExpeditions.Messages;
using HarmonyLib;
using SNetwork;

namespace CustomExpeditions.Messages.Inject.SNets
{
    [HarmonyPatch(typeof(SNet))]
    internal class Inject_SNet
    {
        [HarmonyPostfix]
        [HarmonyPatch("ResetSession")]
        internal static void Post_ResetSession()
        {
            Logger.Verbose("SNet Global: OnResetSession_SNet");
            SNetMessage.OnResetSession_SNet?.Invoke();
        }

        [HarmonyPostfix]
        [HarmonyPatch("ValidateMasterData")]
        internal static void Post_ValidateMaster()
        {
            Logger.Verbose("SNet Global: ValidateMasterData");
            SNetMessage.OnValidateMasterData?.Invoke();
        }
    }
}