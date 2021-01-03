using LevelGeneration;
using System.Collections.Generic;

namespace CustomObjectives.Utils
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

        public static float Value(SeederType type = SeederType.Session, string debugTag = "NO_TAG")
        {
            return GetSeeder(type).Value(debugTag);
        }

        public static float Value(string debugTag = "NO_TAG")
        {
            return Value(SeederType.Session, debugTag);
        }

        public static float Range(float min, float max, SeederType type = SeederType.Session, string debugTag = "NO_TAG")
        {
            return GetSeeder(type).Range(min, max, debugTag);
        }

        public static float Range(float min, float max, string debugTag)
        {
            return Range(min, max, SeederType.Session, debugTag);
        }

        public static int Range(int min, int max, SeederType type = SeederType.Session, string debugTag = "NO_TAG")
        {
            return GetSeeder(type).Range(min, max, debugTag);
        }

        public static int Range(int min, int max, string debugTag)
        {
            return Range(min, max, SeederType.Session, debugTag);
        }

        public static void Shuffle<T>(T[] array, SeederType type = SeederType.Session, string debugTag = "NO_TAG")
        {
            var seeder = GetSeeder(type);

            int i = array.Length;
            while (i > 1)
            {
                int num = seeder.Range(0, i--);
                T t = array[i];
                array[i] = array[num];
                array[num] = t;
            }
        }

        public static T PickFromArray<T>(T[] items, SeederType type = SeederType.Session, string debugTag = "NO_TAG")
        {
            if (items == null)
                return default;

            if (items.Length == 0)
                return default;

            return items[GetSeeder(type).Range(0, items.Length, debugTag)];
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