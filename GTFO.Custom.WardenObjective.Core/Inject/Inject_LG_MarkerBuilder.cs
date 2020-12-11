using GTFO.CustomObjectives.Utils;
using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO.CustomObjectives.Inject
{
    [HarmonyPatch]
    internal static class Inject_LG_MarkerBuilder
    {
        [HarmonyPatch(typeof(LG_FunctionMarkerBuilder), "SetupFunctionGO")]
        [HarmonyPostfix]
        internal static void Post_OnFunctionSpawn(LG_FunctionMarkerBuilder __instance, LG_LayerType layer, GameObject GO)
        {
            var guid = ItemUtil.GetGUID(__instance.m_localTerminalLogFiles);
            
            if(guid == null)
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
