using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject
{
    [HarmonyPatch(typeof(LG_Distribute_WardenObjective), "Build")]
    internal static class Inject_LG_Dist_WardenObjective
    {
        public static void Postfix(LG_Distribute_WardenObjective __instance)
        {
            if(__instance.m_dataBlockData == null)
            {
                //We don't have datablock data, Can't do anything :(
                return;
            }

            var type = (byte)__instance.m_dataBlockData.Type;
            if(Enum.IsDefined(typeof(eWardenObjectiveType), type))
            {
                //This is stock warden objective type, no needs to be handled
                return;
            }

            CustomObjectiveManager.FireHandler(type, __instance.m_layer, __instance.m_dataBlockData);
            CustomObjectiveManager.FireAllGlobalHandler(__instance.m_layer, __instance.m_dataBlockData);
        }
    }
}
