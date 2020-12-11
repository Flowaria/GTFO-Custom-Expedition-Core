using Globals;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject.Global
{
    [HarmonyPatch]
    internal static class Inject_Global
    {
        [HarmonyPatch(typeof(Globals.Global), "OnLevelCleanup")]
        [HarmonyPostfix]
        internal static void Post_LevelCleanup()
        {
            GlobalMessage.OnLevelCleanup?.Invoke();
        }

        [HarmonyPatch(typeof(Globals.Global), "OnResetSession")]
        [HarmonyPostfix]
        internal static void Post_ResetSession()
        {
            GlobalMessage.OnLevelCleanup?.Invoke();
        }
    }
}
