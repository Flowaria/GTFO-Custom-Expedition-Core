using CustomExpeditions;
using CustomExpeditions.CustomReplicators;
using CustomExpeditions.Extensions;
using CustomExpeditions.Utils;
using CustomObjective.DoorMultipleWaves.DoorWaves;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObjective.DoorMultipleWaves.Replication
{
    public class WaveInfo
    {
        public DoorWaveData RawData;

        public ChainedPuzzleContext PuzzleToStart;
        public ChainedPuzzleContext PuzzleToVerify;
        public LG_ComputerTerminal VerifyTerminal;
        public string ValidFileName = string.Empty;
        public bool FailedOnce = false;

        public bool HasDoorChainedPuzzle()
        {
            return PuzzleToStart != null;
        }

        public bool HasTerminalChainedPuzzle()
        {
            return PuzzleToVerify != null;
        }

        public bool HasVerifyTerminal()
        {
            return VerifyTerminal != null;
        }

        public bool IsSameTerminal(LG_ComputerTerminal terminal)
        {
            if (VerifyTerminal == null || terminal == null)
                return false;

            return terminal.GetInstanceID() == VerifyTerminal.GetInstanceID();
        }
    }

    public class LG_DoorMultiWave : CustomReplicatorProvider<DoorWaveState>
    {
        public Timer Timer = new Timer() { IsOnDoneOnce = true };

        internal LG_SecurityDoor Door;
        internal WaveInfo[] WaveInfos;

        public DoorWaveHUDSettings MessageData;

        private string BaseMessage = string.Empty;

        public int MaxWave
        {
            get => WaveInfos.Length - 1;
        }

        public int CurrentWave
        {
            get => State.WaveCount;
        }

        public WaveInfo CurrentWaveInfo
        {
            get => WaveInfos[CurrentWave];
        }

        public string WaveInfoString
        {
            get => $"({CurrentWave+1}/{MaxWave+1})";
        }

        private string GetFormattedMessage()
        {
            var message = BaseMessage;
            message = message.Replace("[TIMER]", Timer.ToTimerString());
            message = message.Replace("[PERCENT]", Timer.ToPercentString());
            message = message.Replace("[WAVE_CURRENT]", (CurrentWave + 1).ToString());
            message = message.Replace("[WAVE_MAX]", (MaxWave + 1).ToString());

            var info = CurrentWaveInfo;
            if(info != null)
            {
                if (info?.VerifyTerminal != null)
                {
                    message = message.Replace("[TERMINAL]", info.VerifyTerminal.m_terminalItem.TerminalItemKey);
                    message = message.Replace("[ZONE_NUMBER]", info.VerifyTerminal.SpawnNode.m_zone.NavInfo.Number.ToString());
                    message = message.Replace("[KEYFILE]", info.ValidFileName);
                }
            }

            return message;
        }

        private bool _countdownPlayed = false;
        private bool _eventTriggered = false;

        public override void OnStateChange(DoorWaveState oldState, DoorWaveState newState, bool isRecall)
        {
            if(oldState.PhaseStatus != newState.PhaseStatus)
            {
                //When Exit Status
                switch (oldState.PhaseStatus)
                {
                    case PhaseType.PuzzleStarted:
                        PlaySound(MessageData.PuzzleSolvedSound);
                        break;

                    case PhaseType.Searching:
                        if(newState.PhaseStatus == PhaseType.VerifyFailed)
                        {
                            PlaySound(MessageData.VerifyFailSound);
                        }
                        else if(newState.PhaseStatus == PhaseType.WaitForPuzzle)
                        {
                            PlaySound(MessageData.VerifySuccessSound);
                        }
                        break;
                }
            }
        }

        public override void OnStateChange()
        {
            Logger.Log(State.PhaseStatus);

            var info = CurrentWaveInfo;
            var raw = info.RawData;

            switch (State.PhaseStatus)
            {
                case PhaseType.WaitForPuzzle:
                    GUIUtil.SetMessageVisible(true);
                    GUIUtil.SetTimerVisible(true);
                    if (State.FailedCount <= 0)
                    {
                        BaseMessage = (CurrentWave == 0) ? MessageData.WaitPuzzlePhaseFirst : MessageData.WaitPuzzlePhase;
                    }
                    else
                    {
                        BaseMessage = MessageData.WaitPuzzlePhaseRetry;
                    }

                    _countdownPlayed = MessageData.WaitPuzzlePhaseSound == 0u;

                    if (Timer.IsWorking)
                    {
                        Logger.Log("WTF");
                    }

                    Logger.Log("Timer Trigger");

                    Timer.Start(raw.GetTimeUntilBioscan(State.FailedCount), () =>
                    {
                        Logger.Log("State Change Called");
                        _countdownPlayed = false;
                        SetPhaseStatus(PhaseType.PuzzleStarted);
                    });
                    break;

                case PhaseType.PuzzleStarted:
                    GUIUtil.SetMessageVisible(false);
                    GUIUtil.SetTimerVisible(false);

                    info.PuzzleToStart.TriggerAvailable();
                    info.PuzzleToStart.Solved = () =>
                    {
                        SetPhaseStatus(PhaseType.PuzzleSolved);
                    };
                    break;

                case PhaseType.PuzzleSolved:
                    GUIUtil.SetMessageVisible(false);
                    GUIUtil.SetTimerVisible(false);

                    if (State.FailedCount <= 0 && info.RawData.EventsOnSolved != null && !_eventTriggered)
                    {
                        foreach (var e in info.RawData.EventsOnSolved)
                        {
                            CoroutineManager.StartCoroutine(WardenObjectiveManager.ExcecuteEvent(e));
                        }
                        _eventTriggered = true;
                    }

                    if (CurrentWaveInfo.HasVerifyTerminal())
                    {
                        SetPhaseStatus(PhaseType.Searching);
                    }
                    else
                    {
                        SetPhaseStatus(PhaseType.SkippedSearch);
                    }
                    break;

                case PhaseType.SkippedSearch:
                    GUIUtil.SetMessageVisible(true);
                    GUIUtil.SetTimerVisible(true);
                    BaseMessage = MessageData.SearchSkipPhase;

                    Timer.Start(raw.GetTimeForSearchTerminal(State.FailedCount), () =>
                    {
                        TryJumpToNextWave();
                    });
                    break;

                case PhaseType.Searching:
                    GUIUtil.SetMessageVisible(true);
                    GUIUtil.SetTimerVisible(true);
                    BaseMessage = MessageData.SearchingPhase;

                    Timer.Start(raw.GetTimeForSearchTerminal(State.FailedCount), () =>
                    {
                        SendState.PhaseStatus = PhaseType.VerifyFailed;
                        SendState.FailedCount++;
                        UpdateState();
                    });
                    break;

                case PhaseType.VerifyFailed:
                    GUIUtil.SetMessageVisible(true);
                    GUIUtil.SetTimerVisible(true);
                    BaseMessage = MessageData.VerifyFailedPhase;

                    Timer.Start(raw.GetTimeForRetry(State.FailedCount), () =>
                    {
                        SetPhaseStatus(PhaseType.WaitForPuzzle);
                    });
                    break;

                case PhaseType.FullySolved:
                    GUIUtil.SetTimerVisible(false);
                    GUIUtil.SetTimedMessage(MessageData.FullySolvedPhase, 10.0f, ePUIMessageStyle.Bioscan);
                    Door.AttemptOpenCloseInteraction(true);
                    Timer.Stop();
                    break;
            }
        }

        public void OnUpdate()
        {
            var info = CurrentWaveInfo;
            var raw = info.RawData;

            switch (State.PhaseStatus)
            {
                case PhaseType.WaitForPuzzle:
                    if (Timer.IsWorking)
                    {
                        GUIUtil.SetTimer(Timer.ProgressPercent);
                        GUIUtil.SetMessage(GetFormattedMessage());

                        if (!_countdownPlayed && Timer.RemainingTime <= MessageData.WaitPuzzlePhaseSoundAtTime)
                        {
                            PlaySound(MessageData.WaitPuzzlePhaseSound);
                            _countdownPlayed = true;
                        }
                    }
                    break;

                case PhaseType.SkippedSearch:
                    if (Timer.IsWorking)
                    {
                        GUIUtil.SetTimer(Timer.ProgressPercent);
                        GUIUtil.SetMessage(GetFormattedMessage(), ePUIMessageStyle.Bioscan);
                    }
                    break;

                case PhaseType.Searching:
                    if(Timer.IsWorking)
                    {
                        GUIUtil.SetTimer(Timer.ProgressPercent);
                        GUIUtil.SetMessage(GetFormattedMessage(), ePUIMessageStyle.Bioscan);
                    }
                    break;

                case PhaseType.VerifyFailed:
                    if(Timer.IsWorking)
                    {
                        GUIUtil.SetTimer(Timer.ProgressPercent);
                        GUIUtil.SetMessage(GetFormattedMessage(), ePUIMessageStyle.Default);
                    }
                    break;
            }
        }

        public void PlaySound(uint id = 0u)
        {
            if(id != 0u)
            {
                Door.m_sound.Post(id);
            }
        }

        public void TriggerDoor()
        {
            SetPhaseStatus(PhaseType.WaitForPuzzle);
        }

        public bool TryJumpToNextWave()
        {
            if(State.PhaseStatus == PhaseType.Searching || State.PhaseStatus == PhaseType.SkippedSearch)
            {
                _eventTriggered = false;

                SendState.FailedCount = 0;

                if(CurrentWave+1 < WaveInfos.Length)
                {
                    SendState.WaveCount++;
                    SendState.PhaseStatus = PhaseType.WaitForPuzzle;
                }
                else
                {
                    SendState.PhaseStatus = PhaseType.FullySolved;
                }
                UpdateState();

                return true;
            }

            return false;
        }

        public void Clear()
        {
            Timer.Stop();
        }

        private void SetPhaseStatus(PhaseType type)
        {
            SendState.PhaseStatus = type;
            UpdateState();
        }
    }
}
