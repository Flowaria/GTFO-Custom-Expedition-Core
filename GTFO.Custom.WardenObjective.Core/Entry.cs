using AssetShards;
using GTFO.CustomObjectives;
using GTFO.CustomObjectives.GlobalHandlers.TimedObjectives;
using GTFO.CustomObjectives.Inject.Global;
using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;

[assembly: MelonInfo(typeof(Entry), "Custom WardenObjective Core", "1.0", "Flowaria")]
[assembly: MelonGame("10 Chambers Collective", "GTFO")]
namespace GTFO.CustomObjectives
{
    public class Entry : MelonMod
    {
        public override void OnApplicationStart()
        {
            CustomObjectiveManager.AddGlobalHandler<TimedObjectiveHandler>();
        }

        public override void OnUpdate()
        {
            GlobalMessage.OnUpdate?.Invoke();
        }

        public override void OnFixedUpdate()
        {
            GlobalMessage.OnFixedUpdate?.Invoke();
        }
    }
}
