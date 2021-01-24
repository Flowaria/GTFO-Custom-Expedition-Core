using CustomExpeditions.CustomReplicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.Utils
{
    public class ChainedPuzzleContextReplicator : CustomReplicatorProvider
    {
        public Action StateChange;

        public override void OnStateChange()
        {
            StateChange?.Invoke();
        }
    }
}
