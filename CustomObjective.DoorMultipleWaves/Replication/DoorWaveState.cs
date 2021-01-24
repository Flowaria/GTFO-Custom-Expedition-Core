using CustomExpeditions.CustomReplicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObjective.DoorMultipleWaves.Replication
{
    public enum PhaseType : byte
    {
        Initialized,
        WaitForPuzzle,
        PuzzleStarted,
        PuzzleSolved,
        Searching,
        SkippedSearch,
        VerifyFailed,
        FullySolved
    }

    public class DoorWaveState : StateWrapper
    {
        public int WaveCount
        {
            get => Value1.IntValue;
            set => Value1.IntValue = value;
        }

        public int FailedCount
        {
            get => Value2.IntValue;
            set => Value2.IntValue = value;
        }

        public PhaseType PhaseStatus
        {
            get => (PhaseType)Status;
            set => Status = (byte)value;
        }
    }
}
