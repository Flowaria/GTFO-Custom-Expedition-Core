using GTFO.CustomObjectives.Utils;
using Harmony;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject
{
    [HarmonyPatch(typeof(LG_DistributeItem), "AvailableToReuse")]
    internal static class Inject_LG_DistItem
    {
        internal static void Postfix(LG_DistributeItem __instance, ref bool __result)
        {
            __result = !ItemUtil.IsWardenObjectiveItem(__instance);
        }
    }
}
