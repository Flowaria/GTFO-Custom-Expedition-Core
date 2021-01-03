using CustomObjectives.Messages;
using HarmonyLib;

namespace CustomObjectives.Inject.Global
{
    [HarmonyPatch(typeof(GS_StopElevatorRide), "OnElevatorHasArrived")]
    internal static class Inject_GS_Elevator
    {
        internal static void Postfix()
        {
            Logger.Verbose("Global: OnElevatorArrive");
            GlobalMessage.OnElevatorArrive?.Invoke();
        }
    }
}