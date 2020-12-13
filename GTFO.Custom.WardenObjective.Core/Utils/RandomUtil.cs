using LevelGeneration;
using System.Collections.Generic;

namespace GTFO.CustomObjectives.Utils
{
    public enum SeederType
    {
        Build,
        Session,
        HostID
    }

    public class RandomUtil
    {
        public static SeedRandom GetSeeder(SeederType type)
        {
            switch (type)
            {
                case SeederType.Build: return Builder.BuildSeedRandom;
                case SeederType.Session: return Builder.SessionSeedRandom;
                case SeederType.HostID: return Builder.HostIDSeedRandom;
            }

            return Builder.SessionSeedRandom;
        }

        public static float Next01(SeederType type = SeederType.Session, string debugTag = "NO_TAG")
        {
            return GetSeeder(type).Value(debugTag);
        }

        public static float NextRange(float min, float max, SeederType type = SeederType.Session, string debugTag = "NO_TAG")
        {
            return GetSeeder(type).Range(min, max, debugTag);
        }

        public static T PickFromList<T>(Il2CppSystem.Collections.Generic.List<T> items, SeederType type = SeederType.Session, string debugTag = "NO_TAG")
        {
            if (items == null)
                return default;

            if (items.Count == 0)
                return default;

            return items[GetSeeder(type).Range(0, items.Count, debugTag)];
        }

        public static T PickFromList<T>(List<T> items, SeederType type = SeederType.Session, string debugTag = "NO_TAG")
        {
            if (items == null)
                return default;

            if (items.Count == 0)
                return default;

            return items[GetSeeder(type).Range(0, items.Count, debugTag)];
        }
    }
}