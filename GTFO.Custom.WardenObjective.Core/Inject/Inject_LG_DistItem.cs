using GTFO.CustomObjectives.Utils;
using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject
{
    [HarmonyPatch]
    internal static class Inject_LG_DistItem
    {
        [HarmonyPatch(typeof(LG_DistributeItem), "AvailableToReuse")]
        [HarmonyPostfix]
        internal static void Alter_Reusable(LG_DistributeItem __instance, ref bool __result)
        {
            __result = !ItemUtil.IsWardenObjectiveItem(__instance);
        }
    }
}
