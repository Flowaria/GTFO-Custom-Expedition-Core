using CustomObjectives;
using CustomObjectives.Inject.CustomReplicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.ShuffleSeed
{
    public class ShuffleSeedReplicator : CustomReplicatorProvider<SeedState>
    {
        public override void OnStateChange(SeedState oldState, SeedState newState, bool isRecall)
        {
            Logger.Log(newState.SeedNumber);
        }
    }
}
