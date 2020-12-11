using BepInEx;
using BepInEx.IL2CPP;
using GTFO.CustomObjectives;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWardenObjective.ChainedPuzzle.Uplink
{
    [BepInDependency("GTFO.CustomObjective.Core", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("GTFO.CO.BioUplink", "(WardenObjective) Bioscan Uplink", "1.0")]
    public class Entry : BasePlugin
    {
        public override void Load()
        {
            
            CustomObjectiveManager.AddHandler<CP_UplinkHandler>(50);
        }
    }
}
