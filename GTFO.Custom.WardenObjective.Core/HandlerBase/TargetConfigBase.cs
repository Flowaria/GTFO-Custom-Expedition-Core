using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.HandlerBase
{
    public abstract class TargetConfigBase
    {
        public uint TargetLayoutID { get; }
        public bool UseTargetLayoutID { get; }

        public uint TargetObjectiveID { get; }
        public bool UseTargetObjectiveID { get; }

        public uint Target
    }
}
