using CustomObjectives.Inject.CustomReplicators;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_SeedRandomizer_MP
{
    public class SeedState : StateWrapperBase
    {
        public bool UsingSeed = false;
        public int Seed1;
        public int Seed2;
        public int Seed3;

        public void SetSeed(string seed)
        {
            Seed1 = seed.GetHashCode();
            Seed2 = (seed + "newCode").GetHashCode();
            Seed3 = (seed + "nnewCode").GetHashCode();
        }

        public override void FromOriginal(pDoorState state)
        {
            UsingSeed = state.hasBeenOpenedDuringGame;
            Seed1 = FloatByteToInt(state.animProgress);
            Seed2 = FloatByteToInt(state.damageTaken);
            Seed3 = FloatByteToInt(state.glueRel);
        }

        public override pDoorState ToOriginal()
        {
            return new pDoorState()
            {
                hasBeenOpenedDuringGame = UsingSeed,
                animProgress = IntByteToFloat(Seed1),
                damageTaken = IntByteToFloat(Seed2),
                glueRel = IntByteToFloat(Seed3)
            };
        }
    }
}
