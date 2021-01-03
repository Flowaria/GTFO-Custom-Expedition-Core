using ChainedPuzzles;
using System;

namespace CustomObjectives.Utils.ChainedPuzzle
{
    public class ChainedPuzzleContext
    {
        public ChainedPuzzleInstance Instance { get; internal set; }
        public string SolvedMessage { get; set; } = "Security Scan Complete, door unlocked.";
        public float SolvedMessageDuration { get; set; } = 10.0f;
        public ePUIMessageStyle SolvedMessageStyle { get; set; } = ePUIMessageStyle.Bioscan;

        internal void Solved_Internal()
        {
            if (!string.IsNullOrWhiteSpace(SolvedMessage) && SolvedMessageDuration >= 0.0f)
            {
                GuiManager.InteractionLayer.SetTimedMessage(SolvedMessage, SolvedMessageDuration, SolvedMessageStyle, 0);
            }

            Solved?.Invoke();
            OnSolved();
        }

        public Action Solved;

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