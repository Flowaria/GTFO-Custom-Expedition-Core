using CustomExpeditions.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.Utils
{
    public class Timer
    {
        private Action _OnUpdate;

        public Timer()
        {
            _OnUpdate = new Action(OnUpdate);
            GlobalMessage.OnUpdate += _OnUpdate;
        }
        ~Timer()
        {
            GlobalMessage.OnUpdate -= _OnUpdate;
        }

        public float EndTime { get; private set; }
        public float ElapsedTime { get; private set; }
        public float RemainingTime { get; private set; }
        public float ProgressPercent { get; private set; }

        public bool IsWorking { get; private set; } = false;

        public bool IsOnDoneOnce { get; set; } = true;

        public Action OnDone;

        public void Start(float endTime, bool forceReset = true)
        {
            if (!IsWorking || forceReset)
            {
                if(forceReset && IsOnDoneOnce)
                {
                    OnDone = null;
                }

                if(endTime <= 0.0f)
                {
                    return;
                }

                ElapsedTime = 0.0f;
                EndTime = RemainingTime = endTime;

                IsWorking = true;
            }
        }

        public void Start(float endTime, Action onDone, bool forceReset = true)
        {
            Start(endTime, forceReset);
            OnDone = onDone;
        }

        public void Stop()
        {
            if(IsWorking)
            {
                ElapsedTime = 0.0f;
                RemainingTime = 0.0f;
                IsWorking = false;
            }
        }

        private void OnUpdate()
        {
            if(!IsWorking)
            {
                return;
            }

            ElapsedTime += Clock.Delta;

            RemainingTime = EndTime - ElapsedTime;
            ProgressPercent = ElapsedTime / EndTime;

            if (ElapsedTime >= EndTime)
            {
                IsWorking = false;

                RemainingTime = 0.0f;
                ProgressPercent = 1.0f;

                if(OnDone != null)
                {
                    var onDone = new Action(OnDone);
                    if (IsOnDoneOnce)
                    {
                        OnDone = null;
                    }

                    onDone.Invoke();
                }
            }
        }

        public string ToTimerString()
        {
            int min = (int)RemainingTime / 60;
            int sec = (int)RemainingTime % 60;

            return $"{min:D2}:{sec:D2}";
        }

        public string ToPercentString()
        {
            return ProgressPercent.ToString("N2");
        }

        public string ToInvertPercentString()
        {
            return (1.0f - ProgressPercent).ToString("N2");
        }
    }
}
