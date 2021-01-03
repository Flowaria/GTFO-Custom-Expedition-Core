using CustomObjectives.SimpleLoader;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPlugin;
using TestPlugin.Basic;
using TestPlugin.ShuffleSeed;
using TestPlugin.UplinkBioscan;
using UnityEngine;
using Logger = CustomObjectives.Logger;

[assembly: CustomObjective(typeof(Entry))]
namespace TestPlugin
{
    internal class Entry : ObjectiveSimpleEntry
    {
        //ShuffleSeedReplicator Replicator;

        public override void OnStart()
        {
            //Replicator = new ShuffleSeedReplicator();
            //Replicator.Setup(eSNetReplicatorLifeTime.NeverDestroyed, SNet_ChannelType.GameOrderCritical);

            //Replicator.State.SeedNumber++;
            //Replicator.UpdateState();

            //CustomObjectiveManager.AddGlobalHandler<OverridesHandler>("OverrideTest", CustomObjectiveSettings.MAIN_ONLY);
            //CustomObjectiveManager.AddGlobalHandler<BuilderHandler>("BuilderTest", CustomObjectiveSettings.ALL_LAYER);
            //CustomObjectiveManager.AddHandler<UplinkBioscanHandler>(50/*, CustomObjectiveSettings.ALL_LAYER*/);
        }
    }
}
