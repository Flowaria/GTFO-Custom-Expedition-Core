using LevelGeneration;

namespace CustomExpeditions.Inject.CustomReplicators
{
    public abstract class InteractionWrapperBase
    {
        public abstract pDoorInteraction ToOriginal();

        public abstract void FromOriginal(pDoorInteraction interaction);
    }
}