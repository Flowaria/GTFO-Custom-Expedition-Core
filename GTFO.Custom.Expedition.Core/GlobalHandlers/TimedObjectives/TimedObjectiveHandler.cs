using CustomExpeditions.HandlerBase;
using CustomExpeditions.Messages;
using CustomExpeditions.Utils;
using SNetwork;
using System.Collections.Generic;
using System.Linq;

namespace CustomExpeditions.GlobalHandlers.TimedObjectives
{
    internal class TimedObjectiveHandler : CustomExpHandlerBase
    {
        private TimedObjectiveReplicator Replicator;
        private TimedObjectiveDefinition TimerContext;

        private TimerStatus CurrentStatus = TimerStatus.Disabled;
        private int CurrentStep = 0;
        private float Timer = 0.0f;
        private float SyncTimer = 0.0f;

        private TimerStepData CurrentStepData { get { return TimerContext.Steps[CurrentStep]; } }

        public override void OnSetup()
        {
            SNetMessage.OnPlayerJoinedSession += OnPlayerJoinedSession;

            Replicator = new TimedObjectiveReplicator();
            Replicator.Setup(eSNetReplicatorLifeTime.DestroyedOnLevelReset, SNet_ChannelType.GameOrderCritical);
            Replicator.StateChanged += () =>
            {
                if(CurrentStatus != Replicator.Status || CurrentStep != Replicator.Step)
                {
                    CurrentStatus = Replicator.Status;
                    Timer = Replicator.InitTimerTime;
                    CurrentStep = Replicator.Step;
                }
            };

            if (!TryGetConfig(ObjectiveData.persistentID)) //check config
            {
                UnloadSelf();
                return;
            }

            if(TimerContext.Steps.Count <= 0)
            {
                Logger.Warning("This Mission doesn't have valid Step Setting for TimedObjective Config");
                UnloadSelf();
                return;
            }

            if(TimerContext.Steps.Any(step => step.Duration <= 0.0f))
            {
                Logger.Warning("This Mission doesn't have valid Duration for Timer Setting: Duration Can't be below Zero");
                UnloadSelf();
                return;
            }

            RegisterUpdateEvent(OnUpdate);
        }

        private void OnPlayerJoinedSession(SNet_Player player)
        {
            SyncReplicator();
        }

        private void SyncReplicator()
        {
            Replicator.Status = CurrentStatus;
            Replicator.Step = CurrentStep;
            Replicator.InitTimerTime = Timer;
            Replicator.UpdateState();
        }

        private bool TryGetConfig(uint id)
        {
            if (ConfigUtil.TryGetLocalConfig<TimedObjectiveConfigDTO>("TimedObjective.json", out var config))
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
            CurrentStatus = TimerStatus.Disabled;
        }

        public override void OnExpeditionFail()
        {
            CurrentStatus = TimerStatus.Disabled;
        }

        public override void OnElevatorArrive()
        {
            if (TimerContext.StartType == StartEventType.ElevatorArrive)
            {
                CurrentStatus = TimerStatus.CountingDelay;
                SyncReplicator();
            }
        }

        private void OnUpdate()
        {
            if (ObjectiveStatus.ObjectiveStatus == eWardenObjectiveStatus.WardenObjectiveItemSolved)
            {
                if (TimerContext.EndType == EndEventType.OnGotoWin)
                {
                    CurrentStatus = TimerStatus.Disabled;
                }

                if (TimerContext.StartType == StartEventType.OnGotoWin)
                {
                    CurrentStatus = TimerStatus.CountingDelay;
                }
            }

            var timerData = CurrentStepData;

            switch (CurrentStatus)
            {
                case TimerStatus.CountingDelay:
                    Timer += Clock.Delta;
                    SyncTimer += Clock.Delta;

                    GUIUtil.SetMessageVisible(false);
                    GUIUtil.SetTimerVisible(false);

                    //Done Counting
                    if (Timer >= timerData.Delay)
                    {
                        Timer = 0.0f;
                        CurrentStatus = TimerStatus.CountingTimer;

                        if(timerData.TriggerWaveData.Count > 0)
                        {
                            TriggerWave(timerData.TriggerWaveData, Builder.GetStartingArea().m_courseNode);
                        }

                        SyncReplicator();
                    }
                    break;

                case TimerStatus.CountingTimer:
                    Timer += Clock.Delta;
                    SyncTimer += Clock.Delta;

                    GUIUtil.SetMessageVisible(true);
                    GUIUtil.SetTimerVisible(true);

                    //Set Message
                    var message = timerData.BaseMessage;
                    message = message.Replace("[TIMER]", TimerFormat(timerData.Duration, Timer));
                    message = message.Replace("[PERCENT]", PercentFormat(timerData.Duration, Timer));
                    message = message.Replace("[PERCENT_INVERT]", InvertPercentFormat(timerData.Duration, Timer));
                    GUIUtil.SetMessage(message);

                    //Set Percent
                    var percent = (Timer / timerData.Duration);
                    switch(timerData.FillStyle)
                    {
                        case TimerStyle.InvertedPercent:
                            percent = 1.0f - percent;
                            break;

                        case TimerStyle.Zero:
                            percent = 0.0f;
                            break;

                        case TimerStyle.One:
                            percent = 1.0f;
                            break;
                    }
                    GUIUtil.SetTimer(percent);
                    

                    //Done Counting
                    if (Timer >= timerData.Duration)
                    {
                        Timer = 0.0f;

                        switch(timerData.DoneType)
                        {
                            case DoneEventType.ForceFail:
                                ForceFail();
                                break;

                            case DoneEventType.ForceWin:
                                ForceWin();
                                break;
                        }

                        //Stop All Wave
                        if (timerData.StopAllWaveWhenDone)
                        {
                            StopAllWave();
                        }

                        //Execute Done Events
                        if (timerData.DoneEvents.Count > 0)
                        {
                            foreach (var e in timerData.DoneEvents)
                            {
                                WardenObjectiveManager.ExcecuteEvent(e);
                            }
                        }

                        //Jump to Next Step or Disable whole Timer
                        if(CurrentStep < TimerContext.Steps.Count - 1)
                        {
                            CurrentStep++;
                            CurrentStatus = TimerStatus.CountingDelay;
                        }
                        else
                        {
                            CurrentStatus = TimerStatus.Disabled;
                        }
                        
                        SyncReplicator();
                    }
                    break;
            }

            if (SyncTimer >= 3.0f)
            {
                SyncTimer = 0.0f;
                SyncReplicator();
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

        private string InvertPercentFormat(float maxTime, float time)
        {
            float percent = time / maxTime;

            return percent.ToString("N2");
        }
    }
}