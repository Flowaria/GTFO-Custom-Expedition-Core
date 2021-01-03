using GTFO.CustomObjectives;
using GTFO.CustomObjectives.Inject.Global;
using GTFO.CustomObjectives.SimpleLoader;
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
using static PlayerChatManager;

[assembly: CustomObjective(typeof(Entry))]
namespace GTFO_SeedRandomizer_MP
{
    public class Entry : ObjectiveSimpleEntry
    {
        private static MD5 _Md5;
        public static ShuffleSeedReplicator Replicator;
        public override void OnStart()
        {
            _Md5 = MD5.Create();

            var harmonyInstance = new Harmony("SeedRandomizer_MP");
            harmonyInstance.PatchAll();

            GlobalMessage.OnResetSession += () =>
            {
                GenerateNewSeed();
            };

            GlobalMessage.OnGameInit += () =>
            {
                Replicator = new ShuffleSeedReplicator();
                Replicator.Setup(eSNetReplicatorLifeTime.NeverDestroyed, SNet_ChannelType.SessionOrderCritical);
            };
        }

        public static void ResetToDefault()
        {
            Replicator.SendState.UsingSeed = false;
            Replicator.UpdateState();
        }

        public static void GenerateNewSeed()
        {
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
            Replicator.SendState.SetSeed(seed);
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

            Logger.Log(msg);

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
