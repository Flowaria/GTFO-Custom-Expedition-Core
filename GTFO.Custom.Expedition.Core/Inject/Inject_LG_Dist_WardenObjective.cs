using HarmonyLib;
using LevelGeneration;
using System;

namespace CustomExpeditions.Inject
{
    [HarmonyPatch(typeof(LG_Distribute_WardenObjective), "Build")]
    internal class Inject_LG_Dist_WardenObjective
    {
        internal static void Postfix(LG_Distribute_WardenObjective __instance)
        {
            if (__instance.m_dataBlockData == null)
            {
                //We don't have datablock data, Can't do anything :(
                return;
            }

            var type = (byte)__instance.m_dataBlockData.Type;
            if (!Enum.IsDefined(typeof(eWardenObjectiveType), type))
            {
                //Custom Handler!
                CustomExpHandlerManager.FireHandler(type, __instance.m_layer, __instance.m_dataBlockData);
            }

            CustomExpHandlerManager.FireAllGlobalHandler(__instance.m_layer, __instance.m_dataBlockData);
        }
    }
}