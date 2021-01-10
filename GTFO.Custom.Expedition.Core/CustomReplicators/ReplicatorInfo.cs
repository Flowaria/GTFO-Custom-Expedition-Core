using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.CustomReplicators
{
    public class ReplicatorInfo
    {
        public IReplicator Replicator;
        public string GUID;
        public Action<pDoorState, pDoorState, bool> OnStateChanged;
    }
}
