using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using MelonLoader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GTFO.CustomObjectives.GlobalHandlers.TimedObjectives
{
    public enum StartEventType
    {
        ElevatorArrive,
        OnGotoWin
    }

    public enum EndEventType
    {
        OnGotoWin,
        Persistent
    }

    public class TimedObjectiveConfigDTO
    {
        public List<TimedObjectiveDefinition> Definitions;
    }

    public class TimedObjectiveDefinition
    {
        public uint TargetObjectiveID;
        public float FailTimer;
        public string BaseMessage;
        public StartEventType StartType;
        public EndEventType EndType;
    }

    public class TimedObjectiveHandler : CustomObjectiveHandlerBase
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

        private StartEventType StartType = StartEventType.ElevatorArrive;
        private EndEventType EndType = EndEventType.OnGotoWin;

        private float TimeUntilFail = 120.0f;
        private float Timer = 0.0f;

        private string BaseMessage = "";

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

            MelonLogger.Log($"{TimeUntilFail} {BaseMessage}");

            IsCountdownMission = true;
        }

        public override void OnUnload()
        {
            MelonLogger.Log($"Unloaded");
        }

        private bool TryGetConfig(uint id)
        {
            var file = ConfigUtil.GetConfigFilePath("TimedObjective");
            if (File.Exists(file))
            {
                var content = File.ReadAllText(file);
                var config = JsonConvert.DeserializeObject<TimedObjectiveConfigDTO>(content);

                if (config.Definitions?.Count > 0)
                {
                    var def = config.Definitions.FirstOrDefault(x => x.TargetObjectiveID == id);
                    if (def != null)
                    {
                        TimeUntilFail = def.FailTimer;
                        BaseMessage = def.BaseMessage;
                        StartType = def.StartType;
                        EndType = def.EndType;
                        return true;
                    }
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
                });

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

                if (StartType == StartEventType.ElevatorArrive)
                {
                    IsCountdownEnable = true;
                }
            }
        }

        private void OnUpdate()
        {
            if (ObjectiveStatus == eWardenObjectiveStatus.WardenObjectiveItemSolved)
            {
                if (EndType == EndEventType.OnGotoWin)
                {
                    IsCountdownEnable = false;
                }

                if (StartType == StartEventType.OnGotoWin)
                {
                    IsCountdownEnable = true;
                }
            }

            if (IsCountdownEnable)
            {
                Timer += Clock.Delta;
                var message = BaseMessage.Replace("[TIMER]", TimerFormat(TimeUntilFail - Timer));

                GUIUtil.SetMessage(message);
                GUIUtil.SetTimer(Timer / TimeUntilFail);

                if (Timer >= TimeUntilFail)
                {
                    ForceFail();
                }
            }
        }

        private string TimerFormat(float remainingTime)
        {
            int min = (int)remainingTime / 60;
            int sec = (int)remainingTime % 60;

            return $"{min:D2}:{sec:D2}";
        }
    }
}