using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.Inject.Checksums
{
    [HarmonyPatch(typeof(RundownManager), "GetLocalRundownKey", typeof(uint))]
    class Inject_RundownManager
    {
        internal static void Postfix(ref string __result)
        {
            __result += "CustomExp";
        }
    }
}
