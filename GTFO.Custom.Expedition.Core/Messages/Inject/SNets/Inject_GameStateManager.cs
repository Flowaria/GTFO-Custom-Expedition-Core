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
    [HarmonyPatch(typeof(GameStateManager))]
    internal class Inject_GameStateManager
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnPlayerGameStateChange")]
        internal static void Post_StateChange(SNet_Player player, pGameState data)
        {
            Logger.Verbose("SNet Global: OnPlayerGameStateChanged");
            SNetMessage.OnPlayerGameStateChanged?.Invoke(player, data.gameState);
        }
    }
}
