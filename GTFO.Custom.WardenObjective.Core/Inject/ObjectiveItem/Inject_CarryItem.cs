namespace GTFO.CustomObjectives.Inject.ObjectiveItem
{
    //[HarmonyPatch(typeof(CarryItemPickup_Core), "ActivateWardenObjectiveItem")]
    internal static class Inject_CarryItem_Active
    {
        internal static void Postfix(CarryItemPickup_Core __instance)
        {
        }
    }

    //[HarmonyPatch(typeof(CarryItemPickup_Core), "DeactivateWardenObjectiveItem")]
    internal static class Inject_CarryItem_Deactive
    {
        internal static void Postfix(CarryItemPickup_Core __instance)
        {
        }
    }
}