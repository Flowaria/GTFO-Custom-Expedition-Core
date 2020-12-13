using CustomObjective.DoorMultipleWaves;
using GTFO.CustomObjectives;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: MelonInfo(typeof(Entry), "TestPlugin", "1.0", "Flowaria")]
[assembly: MelonGame("10 Chambers Collective", "GTFO")]
namespace CustomObjective.DoorMultipleWaves
{
    public class Entry : MelonMod
    {
        public override void OnApplicationStart()
        {
            CustomObjectiveManager.AddGlobalHandler<MainHandler>();
        }
    }
}
