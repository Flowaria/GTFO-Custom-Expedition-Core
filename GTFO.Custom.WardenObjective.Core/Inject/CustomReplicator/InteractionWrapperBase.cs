using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO.CustomObjectives.Inject.CustomReplicator
{
    public abstract class InteractionWrapperBase
    {
        public abstract pDoorInteraction ToOriginal();
        public abstract void FromOriginal(pDoorInteraction interaction);
    }
}
