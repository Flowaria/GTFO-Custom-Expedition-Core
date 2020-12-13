using Globals;
using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject.Global
{
    [HarmonyPatch(typeof(Globals.Global), "OnLevelCleanup")]
    internal static class Inject_Global_LevelCleanup
    {
        
        internal static void Postfix()
        {
            MelonLogger.Log("Global: OnLevelCleanup");
            GlobalMessage.OnLevelCleanup?.Invoke();
        }
    }

    [HarmonyPatch(typeof(Globals.Global), "OnResetSession")]
    internal static class Inject_Global_ResetSession
    {
        internal static void Postfix()
        {
            MelonLogger.Log("Global: OnResetSession");
            GlobalMessage.OnResetSession?.Invoke();
        }
    }
}
