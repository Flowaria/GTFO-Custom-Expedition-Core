using CustomObjective.DoorMultipleWaves;
using CustomObjectives;
using CustomObjectives.SimpleLoader;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CustomObjective(typeof(Entry))]
namespace CustomObjective.DoorMultipleWaves
{
    internal class Entry : ObjectiveSimpleEntry
    {
        public override void OnStart()
        {
            CustomObjectiveManager.AddGlobalHandler<MainHandler>("MultiWaveDoor");
        }
    }
}
