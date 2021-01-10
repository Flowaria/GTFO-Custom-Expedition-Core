using CustomExpeditions.CustomReplicators;
using HarmonyLib;
using LevelGeneration;

namespace CustomExpeditions.Inject.CustomReplicators
{
    [HarmonyPatch(typeof(LG_Door_Sync), "OnStateChange")]
    static class Inject_LG_Door_Sync
    {
        internal static bool Prefix(LG_Door_Sync __instance, pDoorState oldState, pDoorState newState, bool isDropinState)
        {
            if(CustomReplicatorManager.TryInvokeStateChange(__instance.gameObject.name, oldState, newState, isDropinState))
            {
                return false; //Skip the original code
            }

            return true;
        }
    }
}