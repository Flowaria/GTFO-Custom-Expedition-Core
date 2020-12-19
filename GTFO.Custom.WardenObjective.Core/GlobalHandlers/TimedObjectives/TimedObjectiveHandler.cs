using GTFO.CustomObjectives.HandlerBase;
using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using MelonLoader;
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
        private float EndMessageTimer;

        public override void OnSetup()
        {
            if (LayerType != LG_LayerType.MainLayer)
            {
                UnloadSelf();
                return;
            }

            if (!TryGetConfig(ObjectiveData.persistentID)) //check config
            {
                UnloadSelf();
                return;
            }

            IsCountdownMission = true;
        }

        private bool TryGetConfig(uint id)
        {
            var file = ConfigUtil.GetConfigPath("TimedObjective");
            if (File.Exists(file))
            {
                var content = File.ReadAllText(file);
                var config = JsonConvert.DeserializeObject<TimedObjectiveConfigDTO>(content);

                if (config.Definitions?.Count > 0)
                {
                    var def = config.Definitions.FirstOrDefault(x => x.TargetObjectiveID == id);
                    TimerContext = def;
                }
            }
            else
            {
                var content = JsonConvert.SerializeObject(new TimedObjectiveConfigDTO()
                {
                    Definitions = new List<TimedObjectiveDefinition>()
                    {
                        new TimedObjectiveDefinition(){ }
                    }
                }, Formatting.Indented);

                File.WriteAllText(file, content);
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
                    if(TimerContext.EndMessageDuration > 0.0f)
                    {
                        IsEndMessageCountdown = true;
                    }
                }
            }
        }

        private void OnUpdate()
        {
            if (ObjectiveStatus == eWardenObjectiveStatus.WardenObjectiveItemSolved)
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