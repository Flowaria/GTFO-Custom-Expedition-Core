using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject.CustomReplicator
{
    [HarmonyPatch(typeof(LG_Door_Sync), "OnStateChange")]
    internal static class Inject_LG_Door_Sync
    {
        internal static bool Prefix(LG_Door_Sync __instance, pDoorState oldState, pDoorState newState, bool isDropinState)
        {
            if (ProviderManager.Contains(__instance, out var action))
            {
                action.Invoke(oldState, newState, isDropinState);
                return false; //Skip the original code
            }

            return true;
        }
    }
}
