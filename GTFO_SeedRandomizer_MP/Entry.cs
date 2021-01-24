using CustomExpeditions;
using CustomExpeditions.CustomReplicators;
using CustomExpeditions.Messages;
using CustomExpeditions.SimpleLoader;
using GTFO_SeedRandomizer_MP;
using HarmonyLib;
using LevelGeneration;
using Player;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

[assembly: ExpPlugin(typeof(Entry))]
namespace GTFO_SeedRandomizer_MP
{
    public class Entry : ExpSimpleEntry
    {
        public static ShuffleSeedReplicator Replicator;
        public override void OnStart()
        {
            var harmonyInstance = new Harmony("SeedRandomizer_MP");
            harmonyInstance.PatchAll();

            GlobalMessage.OnResetSession += () =>
            {
                ResetToDefault();
            };

            GlobalMessage.OnGameInit += () =>
            {
                Replicator = new ShuffleSeedReplicator();
                Replicator.Setup(ReplicatorType.Manager, ReplicatorCHType.GameOrderCritical);
            };
        }

        public static void ResetToDefault()
        {
            if (!Replicator.CanSendState)
                return;

            Replicator.SendState.UsingSeed = false;
            Replicator.UpdateState();

            Logger.Log("Seed has been reset");
        }

        public static void GenerateNewSeed()
        {
            if (!Replicator.CanSendState)
                return;

            var rand = new Random();
            
            Replicator.SendState.UsingSeed = true;
            var seed1 = Replicator.SendState.Seed1 = rand.Next(int.MinValue, int.MaxValue);
            var seed2 = Replicator.SendState.Seed2 = rand.Next(int.MinValue, int.MaxValue);
            var seed3 = Replicator.SendState.Seed3 = rand.Next(int.MinValue, int.MaxValue);
            Replicator.UpdateState();

            Logger.Log("Setting New Seed: Seed1: {0}, Seed2: {1}, Seed3: {2}", seed1, seed2, seed3);
        }

        public static void GenerateSeed(string seed)
        {
            if (!Replicator.CanSendState)
                return;

            Replicator.SendState.SetSeed(seed);
            Replicator.SendState.UsingSeed = true;
            var seed1 = Replicator.SendState.Seed1;
            var seed2 = Replicator.SendState.Seed2;
            var seed3 = Replicator.SendState.Seed3;
            Replicator.UpdateState();

            Logger.Log("Setting New Seed from String: Seed1: {0}, Seed2: {1}, Seed3: {2}", seed1, seed2, seed3);
        }

        public static void ApplySeed()
        {
            var state = Replicator.State;

            if (!state.UsingSeed)
                return;

            var rand1 = new Random(state.Seed1);
            var rand2 = new Random(state.Seed2);
            var rand3 = new Random(state.Seed3);

            var values = new int[16];
            for(int i = 0;i<values.Length;i++)
            {
                values[i] = ((i & 2) == 0) ? rand1.Next(int.MinValue, int.MaxValue) : rand2.Next(int.MinValue, int.MaxValue);
            }

            Builder.BuildSeedRandom.Seed = state.Seed1;

            var build = Builder.BuildSeedRandom.Seed = values[rand3.Next(0, values.Length - 1)];
            var std = Builder.StandardMarkerSeedOffset = values[rand3.Next(0, values.Length - 1)];
            var func = Builder.FunctionMarkerSeedOffset = values[rand3.Next(0, values.Length - 1)];
            var light = Builder.LightJobSeedOffset = values[rand3.Next(0, values.Length - 1)];

            Logger.Log("Apply Seed: Seed1: {0}, Seed2: {1}, Seed3: {2}", state.Seed1, state.Seed2, state.Seed3);
            Logger.Log(" - Result: Build: {0}, StdMarker: {1}, FucMarker: {2}, Light: {3}", build, std, func, light);
        }
    }

    

    [HarmonyPatch(typeof(LG_LevelBuilder), "BuildFloor")]
    public class Patch_Seed
    {
        public static void Prefix()
        {
            Entry.ApplySeed();
        }
    }

    [HarmonyPatch(typeof(PlayerChatManager), "PostMessage")]
    public class Patch_Chat
    {
        public static void Prefix(PlayerChatManager __instance)
        {
            var msg = __instance.m_lastValue;

            if (string.IsNullOrEmpty(msg))
                return;

            if (msg.Equals("/reset", StringComparison.OrdinalIgnoreCase))
            {
                Entry.ResetToDefault();
            }
            else if (msg.Equals("/reroll", StringComparison.OrdinalIgnoreCase))
            {
                Entry.GenerateNewSeed();
            }
            else if (msg.StartsWith("/seed", StringComparison.OrdinalIgnoreCase))
            {
                Entry.GenerateSeed(msg.Substring(5));
            }
        }
    }
}
