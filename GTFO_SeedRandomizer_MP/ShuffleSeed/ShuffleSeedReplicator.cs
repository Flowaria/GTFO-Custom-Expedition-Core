using CustomExpeditions;
using CustomExpeditions.Inject.CustomReplicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_SeedRandomizer_MP
{
    public class ShuffleSeedReplicator : CustomReplicatorProvider<SeedState>
    {
        public override void OnStateChange(SeedState oldState, SeedState newState, bool isRecall)
        {
            Logger.Log("Seed Change: Seed1: {0}, Seed2: {1}, Seed3: {2}", newState.Seed1, newState.Seed2, newState.Seed3);
        }
    }
}
