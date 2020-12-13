using LevelGeneration;

namespace GTFO.CustomObjectives.Inject.CustomReplicators
{
    public abstract class StateWrapperBase
    {
        public abstract pDoorState ToOriginal();

        public abstract void FromOriginal(pDoorState state);
    }
}