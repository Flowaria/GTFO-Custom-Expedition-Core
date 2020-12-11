using BepInEx;
using BepInEx.IL2CPP;
using GTFO.CustomObjectives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObjective.DoorMultipleWaves
{
    [BepInDependency("GTFO.CustomObjective.Core", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("GTFO.CO.MultiPuzzleDoor", "(WardenObjective) Multiple Wave Security Door", "1.0")]
    public class Entry : BasePlugin
    {
        public override void Load()
        {
            CustomObjectiveManager.AddHandler<MainHandler>(51);
        }
    }
}
