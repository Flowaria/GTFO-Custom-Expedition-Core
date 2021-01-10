using CustomExpeditions.Messages;
using CustomExpeditions.Utils;
using HarmonyLib;
using LevelGeneration;
using UnityEngine;

namespace CustomExpeditions.Messages.Inject.MarkerItem
{
    [HarmonyPatch(typeof(LG_FunctionMarkerBuilder), "SetupFunctionGO")]
    internal static class Inject_LG_FunctionMarkerBuilder
    {
        internal static void Postfix(LG_FunctionMarkerBuilder __instance, LG_LayerType layer, GameObject GO)
        {
            var result = TryGetGUID(__instance, out var guid);
            if (result)
            {
                var terminal = GO.GetComponentInChildren<LG_ComputerTerminal>();
                if (terminal != null)
                {
                    terminal.m_localLogs.Remove("!PLUGIN_RESERVED_SPECIAL_GUID");
                }

                ItemMessage.OnItemSpawned?.Invoke(guid, GO);
                ItemMessage.OnStandardItemSpawned?.Invoke(guid, GO);
            }
        }

        internal static bool TryGetGUID(LG_FunctionMarkerBuilder builder, out string guid)
        {
            guid = ItemUtil.GetGUID(builder.m_localTerminalLogFiles);
            return guid != null;
        }
    }
}