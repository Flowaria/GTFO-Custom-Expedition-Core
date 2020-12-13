using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject.Global
{
    [HarmonyPatch(typeof(GS_StopElevatorRide), "OnElevatorHasArrived")]
    internal static class Inject_GS_Elevator
    {
        internal static void Postfix()
        {
            MelonLogger.Log("Global: OnElevatorArrive");
            GlobalMessage.OnElevatorArrive?.Invoke();
        }
    }
}
