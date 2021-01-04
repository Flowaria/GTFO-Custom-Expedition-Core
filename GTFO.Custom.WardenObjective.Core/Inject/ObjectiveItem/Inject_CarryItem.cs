namespace CustomExpeditions.Inject.ObjectiveItem
{
    //[HarmonyPatch(typeof(CarryItemPickup_Core))]
    internal static class Inject_CarryItem
    {
        //[HarmonyPostfix]
        //[HarmonyPatch("ActivateWardenObjectiveItem")]
        internal static void Post_Activate(CarryItemPickup_Core __instance)
        {
        }

        //[HarmonyPostfix]
        //[HarmonyPatch("DeactivateWardenObjectiveItem")]
        internal static void Post_Deactivate(CarryItemPickup_Core __instance)
        {
        }
    }
}