using AssetShards;
using BepInEx;
using BepInEx.IL2CPP;
using GTFO.CustomObjectives.Inject.Global;
using GTFO.CustomObjectives.Utils;
using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace GTFO.CustomObjectives
{
    [BepInProcess("GTFO.exe")]
    [BepInPlugin("GTFO.CustomObjective.Core", "Custom WardenObjective Core", "1.0")]
    public class Entry : BasePlugin
    {
        public override void Load()
        {
            var harmony = new Harmony("gtfo.customobjective.core.harmony");
            harmony.PatchAll();

            ClassInjector.RegisterTypeInIl2Cpp<GlobalMessageComponent>();

            var persistentObject = new GameObject();
            persistentObject.AddComponent<GlobalMessageComponent>();

            GameObject.DontDestroyOnLoad(persistentObject);
        }
    }
}
