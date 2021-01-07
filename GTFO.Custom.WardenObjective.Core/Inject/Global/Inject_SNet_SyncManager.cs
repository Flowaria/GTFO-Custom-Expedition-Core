using CustomExpeditions.Messages;
using HarmonyLib;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.Inject.Global
{
    [HarmonyPatch(typeof(SNet_SyncManager))]
    class Inject_SNet_SyncManager
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnPlayerJoinedSessionHub")]
        internal void Post_PlayerJoinedHub(SNet_Player player)
        {
            Logger.Verbose("Global (SNet): OnPlayerJoinedSession");
            GlobalMessage.OnPlayerJoinedSession?.Invoke(player);
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnPlayerLeftSessionHub")]
        internal void Post_PlayerLeftHub(SNet_Player player)
        {
            Logger.Verbose("Global (SNet): OnPlayerLeftSession");
            GlobalMessage.OnPlayerLeftSession?.Invoke(player);
        }
    }
}
