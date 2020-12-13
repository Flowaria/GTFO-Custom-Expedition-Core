using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject.CustomReplicators
{
    public abstract class StateWrapperBase
    {
        public abstract pDoorState ToOriginal();
        public abstract void FromOriginal(pDoorState state);
    }
}
