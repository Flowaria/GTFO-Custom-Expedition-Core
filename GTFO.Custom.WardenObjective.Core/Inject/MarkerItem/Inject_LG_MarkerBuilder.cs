using GTFO.CustomObjectives.Utils;
using Harmony;
using LevelGeneration;
using UnityEngine;

namespace GTFO.CustomObjectives.Inject.MarkerItem
{
    [HarmonyPatch(typeof(LG_FunctionMarkerBuilder), "SetupFunctionGO")]
    internal static class Inject_LG_MarkerBuilder
    {
        internal static void Postfix(LG_FunctionMarkerBuilder __instance, LG_LayerType layer, GameObject GO)
        {
            var guid = ItemUtil.GetGUID(__instance.m_localTerminalLogFiles);

            if (guid == null)
            {
                return;
            }

            var info = ItemUtil.FindInfoByGUID(guid);

            if (info != null)
            {
                var terminal = GO.GetComponentInChildren<LG_ComputerTerminal>();
                if (terminal != null)
                {
                    terminal.m_localLogs.Remove("!PLUGIN_REVERVED_SPECIAL_GUID");
                }

                info.OnGameObjectSpawned?.Invoke(GO);
            }
        }
    }
}