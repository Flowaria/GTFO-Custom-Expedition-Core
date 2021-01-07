﻿using CustomExpeditions.Messages;
using HarmonyLib;

namespace CustomExpeditions.Inject.Global
{
    [HarmonyPatch(typeof(GS_StopElevatorRide), "OnElevatorHasArrived")]
    static class Inject_GS_Elevator
    {
        internal static void Postfix()
        {
            Logger.Verbose("Global: OnElevatorArrive");
            GlobalMessage.OnElevatorArrive?.Invoke();
        }
    }
}