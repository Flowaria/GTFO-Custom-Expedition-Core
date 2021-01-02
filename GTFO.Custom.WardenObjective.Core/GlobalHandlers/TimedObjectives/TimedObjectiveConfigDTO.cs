using GameData;
using System.Collections.Generic;

namespace GTFO.CustomObjectives.GlobalHandlers.TimedObjectives
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
        ForceFail,
        ForceWin,
        ExecuteEvents
    }

    internal class TimedObjectiveConfigDTO
    {
        public List<TimedObjectiveDefinition> Definitions;
    }

    internal class TimedObjectiveDefinition
    {
        public uint TargetObjectiveID = 0;
        public float Duration = 3600.0f;
        public bool FillOut = false;
        public string BaseMessage = "";

        public string EndMessage = "";
        public float EndMessageDuration = -1.0f;

        public StartEventType StartType = StartEventType.ElevatorArrive;
        public EndEventType EndType = EndEventType.OnGotoWin;

        public DoneEventType DoneType = DoneEventType.ForceFail;
        public bool StopAllWaveWhenDone = false;
        public List<WardenObjectiveEventData> DoneEvents;
    }
}