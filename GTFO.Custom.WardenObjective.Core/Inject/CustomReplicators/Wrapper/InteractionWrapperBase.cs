using LevelGeneration;

namespace GTFO.CustomObjectives.Inject.CustomReplicators
{
    public abstract class InteractionWrapperBase
    {
        public abstract pDoorInteraction ToOriginal();

        public abstract void FromOriginal(pDoorInteraction interaction);
    }
}