using CustomObjective.DoorMultipleWaves;
using CustomExpeditions;
using CustomExpeditions.SimpleLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: ExpPlugin(typeof(Entry))]
namespace CustomObjective.DoorMultipleWaves
{
    internal class Entry : ExpSimpleEntry
    {
        public override void OnStart()
        {
            CustomExpHandlerManager.AddGlobalHandler<MainHandler>("MultiWaveDoor");
        }
    }
}
