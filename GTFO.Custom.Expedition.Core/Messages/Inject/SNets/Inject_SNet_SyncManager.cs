using CustomExpeditions.Messages;
using HarmonyLib;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.Messages.Inject.SNets
{
    [HarmonyPatch(typeof(SNet_SyncManager))]
    internal class Inject_SNet_SyncManager
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnPlayerJoinedSessionHub")]
        internal static void Post_PlayerJoinedHub(SNet_Player player)
        {
            Logger.Verbose("SNet Global: OnPlayerJoinedSession");
            SNetMessage.OnPlayerJoinedSession?.Invoke(player);
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnPlayerLeftSessionHub")]
        internal static void Post_PlayerLeftHub(SNet_Player player)
        {
            Logger.Verbose("SNet Global: OnPlayerLeftSession");
            SNetMessage.OnPlayerLeftSession?.Invoke(player);
        }
    }
}
