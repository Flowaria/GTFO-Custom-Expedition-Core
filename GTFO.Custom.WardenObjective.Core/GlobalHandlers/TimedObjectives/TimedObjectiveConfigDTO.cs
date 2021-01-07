using GameData;
using System.Collections.Generic;

namespace CustomExpeditions.GlobalHandlers.TimedObjectives
{
    internal enum StartEventType
    {
        ElevatorArrive,
        OnGotoWin
    }

    internal enum EndEventType
    {
        OnGotoWin,
        Persistent
    }

    internal enum DoneEventType
    {
        None,
        ForceFail,
        ForceWin
    }

    internal enum TimerStyle
    {
        Percent,
        InvertedPercent,
        Zero,
        One
    }

    internal class TimedObjectiveConfigDTO
    {
        public List<TimedObjectiveDefinition> Definitions;
    }

    internal class TimedObjectiveDefinition
    {
        public uint TargetObjectiveID = 0;

        public StartEventType StartType = StartEventType.ElevatorArrive;
        public EndEventType EndType = EndEventType.OnGotoWin;

        public List<TimerStepData> Steps;
    }

    internal class TimerStepData
    {
        public float Delay = 0.0f;
        public float Duration = 100.0f;
        public TimerStyle FillStyle = TimerStyle.Percent;
        public string BaseMessage = "";

        public DoneEventType DoneType = DoneEventType.None;
        public bool StopAllWaveWhenDone = false;
        public List<GenericEnemyWaveData> TriggerWaveData;
        public List<WardenObjectiveEventData> DoneEvents;
    }
}