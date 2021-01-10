using CustomExpeditions;
using CustomExpeditions.SimpleLoader;
using HarmonyLib;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPlugin;
using TestPlugin.Basic;
using TestPlugin.HackWave;
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

            CustomExpHandlerManager.AddGlobalHandler<OverridesHandler>("OverrideTest", CustomExpSettings.MAIN_ONLY);
            CustomExpHandlerManager.AddGlobalHandler<HackMissWaveHandler>("HackMissWave", CustomExpSettings.MAIN_ONLY);
            CustomExpHandlerManager.AddGlobalHandler<BuilderHandler>("BuilderTest", CustomExpSettings.ALL_LAYER);
            CustomExpHandlerManager.AddHandler<UplinkBioscanHandler>(50, CustomExpSettings.ALL_LAYER);
        }
    }
}
