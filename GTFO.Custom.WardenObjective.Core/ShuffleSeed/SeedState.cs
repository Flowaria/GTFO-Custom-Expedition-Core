using GTFO.CustomObjectives.Inject.CustomReplicators;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.ShuffleSeed
{
    public class SeedState : StateWrapperBase
    {
        public bool UsingSeed = false;
        public int SeedNumber = 0;

        public override void FromOriginal(pDoorState state)
        {
            UsingSeed = state.hasBeenOpenedDuringGame;
            SeedNumber = FloatByteToInt(state.animProgress);
        }

        public override pDoorState ToOriginal()
        {
            return new pDoorState()
            {
                animProgress = IntByteToFloat(SeedNumber)
            };
        }
    }
}
