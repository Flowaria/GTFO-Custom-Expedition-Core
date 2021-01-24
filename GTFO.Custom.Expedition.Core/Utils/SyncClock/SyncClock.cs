using CustomExpeditions.CustomReplicators;
using CustomExpeditions.Messages;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.Utils
{
    public class SyncClock
    {
        public static SyncClock Global { get; private set; }

        static SyncClock()
        {
            //Global = new SyncClock(true);
            GlobalMessage.OnResetSession += () =>
            {
                //SNet.SessionHub.is
                Global.TimeInSession = Clock.TimeInSessionHub;
            };
        }

        public static SyncClock CreateNew()
        {
            return new SyncClock();
        }

        public SyncClock()
        {
            Initialize();
        }

        private SyncClock(bool isGlobal)
        {
            IsGlobal = isGlobal;
            Initialize();
        }

        private SyncClockReplicator Replicator = new SyncClockReplicator();

        private void Initialize()
        {
            var lifeTime = IsGlobal ? ReplicatorType.Manager : ReplicatorType.Manager;
            Replicator.Setup(lifeTime, ReplicatorCHType.GameReceiveCritical);

            GlobalMessage.OnUpdate += Update;
        }

        private void Update()
        {

        }

        public bool IsGlobal { get; private set; } = false;

        public float TimeInSession { get; private set; }
    }
}
