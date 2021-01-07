using CustomExpeditions.CustomReplicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.GlobalHandlers.TimedObjectives
{
    internal enum TimerStatus : byte
    {
        Disabled,
        CountingDelay,
        CountingTimer
    }

    internal class TimedObjectiveReplicator : CustomReplicatorProvider
    {
        public Action StateChanged;

        public int Step
        {
            get => State.Value1.IntValue;
            set => SendState.Value1.IntValue = value;
        }
        public float InitTimerTime
        {
            get => State.Value2.FloatValue;
            set => SendState.Value2.FloatValue = value;
        }
        
        public TimerStatus Status
        {
            get => (TimerStatus)State.Status;
            set => SendState.Status = (byte)value;
        }

        public override void OnStateChange()
        {
            StateChanged?.Invoke();
        }
    }
}
