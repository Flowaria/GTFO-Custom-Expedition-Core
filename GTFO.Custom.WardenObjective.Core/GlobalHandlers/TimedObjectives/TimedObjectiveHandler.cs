using GTFO.CustomObjectives.HandlerBase;
using GTFO.CustomObjectives.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GTFO.CustomObjectives.GlobalHandlers.TimedObjectives
{
    internal class TimedObjectiveHandler : CustomObjectiveHandlerBase
    {
        private bool IsCountdownMission = false;

        private bool IsCountdownEnable
        {
            get
            {
                return _isCountdownEnable;
            }
            set
            {
                GUIUtil.SetMessageVisible(value);
                GUIUtil.SetTimerVisible(value);

                _isCountdownEnable = value;
            }
        }

        private bool _isCountdownEnable = false;

        private bool IsEndMessageCountdown = false;

        private TimedObjectiveDefinition TimerContext;
        private float Timer;

        public override void OnSetup()
        {
            if (!TryGetConfig(ObjectiveData.persistentID)) //check config
            {
                UnloadSelf();
                return;
            }

            IsCountdownMission = true;
        }

        private bool TryGetConfig(uint id)
        {
            if(ConfigUtil.TryGetLocalConfig<TimedObjectiveConfigDTO>("TimedObjective.json", out var config))
            {
                if (config.Definitions?.Count > 0)
                {
                    var def = config.Definitions.FirstOrDefault(x => x.TargetObjectiveID == id);
                    TimerContext = def;
                }
            }
            else
            {
                ConfigUtil.SaveLocalConfig("TimedObjective.json", new TimedObjectiveConfigDTO()
                {
                    Definitions = new List<TimedObjectiveDefinition>()
                    {
                        new TimedObjectiveDefinition(){ }
                    }
                });
            }

            return false;
        }

        public override void OnExpeditionSuccess()
        {
            IsCountdownEnable = false;
        }

        public override void OnExpeditionFail()
        {
            IsCountdownEnable = false;
        }

        public override void OnElevatorArrive()
        {
            if (IsCountdownMission)
            {
                RegisterUpdateEvent(OnUpdate);

                if (TimerContext.StartType == StartEventType.ElevatorArrive)
                {
                    IsCountdownEnable = true;
                    if (TimerContext.EndMessageDuration > 0.0f)
                    {
                        IsEndMessageCountdown = true;
                    }
                }
            }
        }

        private void OnUpdate()
        {
            if (ObjectiveStatus.ObjectiveStatus == eWardenObjectiveStatus.WardenObjectiveItemSolved)
            {
                if (TimerContext.EndType == EndEventType.OnGotoWin)
                {
                    if (TimerContext.EndMessageDuration > 0.0f)
                    {
                        IsEndMessageCountdown = true;
                    }
                    else
                    {
                        IsCountdownEnable = false;
                    }
                }

                if (TimerContext.StartType == StartEventType.OnGotoWin)
                {
                    IsCountdownEnable = true;
                }
            }

            if (IsCountdownEnable)
            {
                Timer += Clock.Delta;
                var message = TimerContext.BaseMessage;
                message = message.Replace("[TIMER]", TimerFormat(TimerContext.Duration, Timer));
                message = message.Replace("[PERCENT]", PercentFormat(TimerContext.Duration, Timer));

                GUIUtil.SetMessage(message);
                GUIUtil.SetTimer(Timer / TimerContext.Duration);

                if (Timer >= TimerContext.Duration)
                {
                    ForceFail();
                }
            }
        }

        private string TimerFormat(float maxTime, float time)
        {
            float remainingTime = maxTime - time;
            int min = (int)remainingTime / 60;
            int sec = (int)remainingTime % 60;

            return $"{min:D2}:{sec:D2}";
        }

        private string PercentFormat(float maxTime, float time)
        {
            float percent = time / maxTime;

            return percent.ToString("N2");
        }
    }
}