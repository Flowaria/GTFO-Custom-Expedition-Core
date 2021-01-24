using CustomObjective.DoorMultipleWaves;
using CustomExpeditions;
using CustomExpeditions.SimpleLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: ExpPlugin(typeof(EntryPoint))]
namespace CustomObjective.DoorMultipleWaves
{
    internal class EntryPoint : ExpSimpleEntry
    {
        public override void OnStart()
        {
            CustomExpHandlerManager.AddGlobalHandler<DoorWaveHandler>("MultiWaveDoor", CustomExpSettings.MAIN_ONLY);
        }
    }
}
