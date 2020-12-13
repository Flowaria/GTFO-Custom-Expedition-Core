using ChainedPuzzles;

namespace GTFO.CustomObjectives
{
    public class ChainedPuzzleContext
    {
        public ChainedPuzzleInstance Instance { get; internal set; }

        public virtual void OnSolved()
        {
        }

        public void AttemptInteract(eChainedPuzzleInteraction interaction)
        {
            Instance?.AttemptInteract(interaction);
        }

        public void Trigger()
        {
            AttemptInteract(eChainedPuzzleInteraction.Activate);
        }
    }
}