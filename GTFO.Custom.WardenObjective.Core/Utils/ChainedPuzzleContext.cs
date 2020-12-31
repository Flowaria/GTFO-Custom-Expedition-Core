using ChainedPuzzles;

namespace GTFO.CustomObjectives.Utils
{
    public class ChainedPuzzleContext
    {
        public ChainedPuzzleInstance Instance { get; internal set; }
        public string SolvedMessage { get; set; }
        public float SolvedMessageDuration { get; set; } = 10.0f;
        public ePUIMessageStyle SolvedMessageStyle { get; set; } = ePUIMessageStyle.Bioscan;

        internal void Solved()
        {
            if (!string.IsNullOrWhiteSpace(SolvedMessage) || SolvedMessageDuration <= 0.0f)
            {
                GuiManager.InteractionLayer.SetTimedMessage(SolvedMessage, SolvedMessageDuration, SolvedMessageStyle, 0);
            }

            OnSolved();
        }

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