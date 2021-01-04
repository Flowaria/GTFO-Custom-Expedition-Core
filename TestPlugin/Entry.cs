using CustomExpeditions;
using CustomExpeditions.SimpleLoader;
using HarmonyLib;
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

[assembly: ExpPlugin(typeof(Entry))]
namespace TestPlugin
{
    internal class Entry : ExpSimpleEntry
    {
        //ShuffleSeedReplicator Replicator;

        public override void OnStart()
        {
            var harmonyInstance = new Harmony("TestPlugin");
            harmonyInstance.PatchAll();

            //Replicator = new ShuffleSeedReplicator();
            //Replicator.Setup(eSNetReplicatorLifeTime.NeverDestroyed, SNet_ChannelType.GameOrderCritical);

            //Replicator.State.SeedNumber++;
            //Replicator.UpdateState();

            CustomExpHandlerManager.AddGlobalHandler<OverridesHandler>("OverrideTest", CustomExpSettings.MAIN_ONLY);
            //CustomObjectiveManager.AddGlobalHandler<BuilderHandler>("BuilderTest", CustomObjectiveSettings.ALL_LAYER);
            //CustomObjectiveManager.AddHandler<UplinkBioscanHandler>(50/*, CustomObjectiveSettings.ALL_LAYER*/);
        }
    }
}
