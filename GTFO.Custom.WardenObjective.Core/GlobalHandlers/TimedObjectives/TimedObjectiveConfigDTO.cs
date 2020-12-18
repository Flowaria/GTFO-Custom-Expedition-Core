using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    internal class TimedObjectiveConfigDTO
    {
        public List<TimedObjectiveDefinition> Definitions;
    }

    internal class TimedObjectiveDefinition
    {
        public uint TargetObjectiveID;
        public float FailTimer;
        public string BaseMessage;
        public StartEventType StartType;
        public EndEventType EndType;
    }
}
