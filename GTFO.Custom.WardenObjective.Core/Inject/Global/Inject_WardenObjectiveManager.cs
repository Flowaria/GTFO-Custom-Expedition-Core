using Harmony;

namespace GTFO.CustomObjectives.Inject.Global
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