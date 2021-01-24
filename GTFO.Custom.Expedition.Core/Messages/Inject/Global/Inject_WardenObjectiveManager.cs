using CustomExpeditions.Messages;
using HarmonyLib;

namespace CustomExpeditions.Messages.Inject.Global
{
    [HarmonyPatch(typeof(WardenObjectiveManager), "OnStateChange")]
    internal class Inject_WardenObjectiveManager
    {
        internal static void Postfix(pWardenObjectiveState oldState, pWardenObjectiveState newState, bool isRecall)
        {
            Logger.Verbose("Global: ObjectiveStateChanged");
            GlobalMessage.OnObjectiveStateChanged?.Invoke(oldState, newState, isRecall);
        }
    }
}