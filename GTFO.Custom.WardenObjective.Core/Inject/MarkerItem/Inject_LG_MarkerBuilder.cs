using CustomExpeditions.Utils;
using HarmonyLib;
using LevelGeneration;
using UnityEngine;

namespace CustomExpeditions.Inject.MarkerItem
{
    [HarmonyPatch(typeof(LG_FunctionMarkerBuilder), "SetupFunctionGO")]
    internal static class Inject_LG_MarkerBuilder
    {
        internal static void Postfix(LG_FunctionMarkerBuilder __instance, LG_LayerType layer, GameObject GO)
        {
            var guid = ItemUtil.GetGUID(__instance.m_localTerminalLogFiles);

            //ignore item without log data
            if (guid == null)
                return;

            var info = ItemUtil.FindInfoByGUID(guid);

            //ignore item without info
            if (info == null)
                return;

            var terminal = GO.GetComponentInChildren<LG_ComputerTerminal>();
            if (terminal != null)
            {
                terminal.m_localLogs.Remove("!PLUGIN_RESERVED_SPECIAL_GUID");
            }

            info.OnGameObjectSpawned?.Invoke(GO);
        }
    }
}