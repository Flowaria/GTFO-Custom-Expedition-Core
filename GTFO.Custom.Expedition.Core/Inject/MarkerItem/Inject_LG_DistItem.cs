using CustomExpeditions.Utils;
using HarmonyLib;
using LevelGeneration;

namespace CustomExpeditions.Inject.MarkerItem
{
    [HarmonyPatch(typeof(LG_DistributeItem), "AvailableToReuse")]
    internal class Inject_LG_DistItem
    {
        internal static void Postfix(LG_DistributeItem __instance, ref bool __result)
        {
            __result = ItemUtil.IsReusableItem(__instance);
        }
    }
}