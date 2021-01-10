using CustomExpeditions.CustomReplicators;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_SeedRandomizer_MP
{
    public class SeedState : StateWrapper
    {
        public bool UsingSeed
        {
            get => Flag1;
            set => Flag1 = value;
        }
        public int Seed1
        {
            get => Value1.IntValue;
            set => Value1.IntValue = value;
        }
        public int Seed2
        {
            get => Value2.IntValue;
            set => Value2.IntValue = value;
        }
        public int Seed3
        {
            get => Value3.IntValue;
            set => Value3.IntValue = value;
        }
        public void SetSeed(string seed)
        {
            Seed1 = seed.GetHashCode();
            Seed2 = (seed + "newCode").GetHashCode();
            Seed3 = (seed + "nnewCode").GetHashCode();
        }
    }
}
