using GTFO.CustomObjectives.Extensions;
using GTFO.CustomObjectives.Inject.Global;
using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;

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


    public class TimedObjectiveHandler : CustomObjectiveHandler
    {
        bool IsCountdownMission = false;
        bool IsCountdownEnable {
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

        bool _isCountdownEnable = false;
        

        StartEventType StartType = StartEventType.ElevatorArrive;
        EndEventType EndType = EndEventType.OnGotoWin;

        float TimeUntilFail = 120.0f;
        float Timer = 0.0f;

        string BaseMessage = "";


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
            if(File.Exists(file))
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
            MelonLogger.Log($"Elevator Called");
            if (IsCountdownMission)
            {
                RegisterUpdateEvent(OnUpdate);
                MelonLogger.Log($"Event Registered");

                if (StartType == StartEventType.ElevatorArrive)
                {
                    IsCountdownEnable = true;
                }
            }
        }

        void OnUpdate()
        {
            if (ObjectiveStatus == eWardenObjectiveStatus.WardenObjectiveItemSolved)
            {
                if (EndType == EndEventType.OnGotoWin)
                {
                    IsCountdownEnable = false;
                }

                if(StartType == StartEventType.OnGotoWin)
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

                if(Timer >= TimeUntilFail)
                {
                    ForceFail();
                }
            }
        }

        string TimerFormat(float remainingTime)
        {
            int min = (int)remainingTime / 60;
            int sec = (int)remainingTime % 60;

            return $"{min:D2}:{sec:D2}";
        }
    }
}
