using CustomExpeditions.Messages;
using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomExpeditions.Messages.Inject.MarkerItem
{
    [HarmonyPatch(typeof(LG_PickupItemBuilder), "SetupFunctionGO")]
    internal class Inject_LG_PickupItemBuilder
    {
        internal static void Postfix(LG_PickupItemBuilder __instance, LG_LayerType layer, GameObject GO)
        {
            var casted = __instance.Cast<LG_FunctionMarkerBuilder>();
            var result = Inject_LG_FunctionMarkerBuilder.TryGetGUID(casted, out var guid);
            if(result)
            {
                ItemMessage.OnItemSpawned?.Invoke(guid, GO);
                ItemMessage.OnPickupItemSpawned?.Invoke(guid, GO);
            }
        }
    }
}
