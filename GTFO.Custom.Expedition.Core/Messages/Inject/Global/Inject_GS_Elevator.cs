using CustomExpeditions.Messages;
using HarmonyLib;

namespace CustomExpeditions.Messages.Inject.Global
{
    [HarmonyPatch(typeof(GS_StopElevatorRide), "OnElevatorHasArrived")]
    internal class Inject_GS_Elevator
    {
        internal static void Postfix()
        {
            Logger.Verbose("Global: OnElevatorArrive");
            GlobalMessage.OnElevatorArrive?.Invoke();
        }
    }
}