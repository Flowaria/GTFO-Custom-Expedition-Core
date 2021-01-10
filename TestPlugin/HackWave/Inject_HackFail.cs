using AIGraph;
using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.HackWave
{
    [HarmonyPatch(typeof(LG_GenericHackable), "SyncHackingMiss")]
    public static class Inject_HackFail
    {
        public static Action<AIG_CourseNode> OnCalled;

        public static void Prefix(AIG_CourseNode node)
        {
            OnCalled?.Invoke(node);
        }
    }
}
