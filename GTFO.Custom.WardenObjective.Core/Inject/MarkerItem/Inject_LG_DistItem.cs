using CustomExpeditions.Utils;
using HarmonyLib;
using LevelGeneration;

namespace CustomExpeditions.Inject.MarkerItem
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