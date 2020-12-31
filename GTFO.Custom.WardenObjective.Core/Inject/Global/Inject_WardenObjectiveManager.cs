using GTFO.CustomObjectives.Utils;
using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject.Global
{
    [HarmonyPatch(typeof(WardenObjectiveManager), "OnStateChange")]
    internal class Inject_WardenObjectiveManager
    {
        internal static void Postfix(pWardenObjectiveState oldState, pWardenObjectiveState newState, bool isRecall)
        {
            Logger.Verbose("Global: ObjectiveStateChanged");
            GlobalMessage.OnObjectiveStateChanged?.Invoke(oldState, newState, isRecall);
        }
    }
}
