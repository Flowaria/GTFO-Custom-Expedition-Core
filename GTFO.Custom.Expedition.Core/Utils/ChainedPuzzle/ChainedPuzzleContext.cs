using ChainedPuzzles;
using CustomExpeditions.CustomReplicators;
using System;
using UnityEngine;

namespace CustomExpeditions.Utils
{
    public class ChainedPuzzleContext
    {
        public ChainedPuzzleInstance Instance { get; internal set; }
        public string SolvedMessage { get; set; } = "Security Scan Complete.";
        public float SolvedMessageDuration { get; set; } = 10.0f;
        public ePUIMessageStyle SolvedMessageStyle { get; set; } = ePUIMessageStyle.Bioscan;

        public bool IsSolved { get => Instance?.IsSolved ?? false; }

        private ChainedPuzzleContextReplicator _Replicator = new ChainedPuzzleContextReplicator();
        private int _RegenerateCount = 0;
        private Vector3 _Position;

        private ChainedPuzzleInstance _RegenBufferInstance = null;
        private Action _OnRegenDone_Once;

        internal void Setup(Vector3 pos)
        {
            _Position = pos;

            _RegenBufferInstance = CopyNewInstance();

            _Replicator.Setup(ReplicatorType.LevelInstance, ReplicatorCHType.GameOrderCritical);
            _Replicator.StateChange += () =>
            {
                for (; _RegenerateCount < _Replicator.State.Value1.IntValue; _RegenerateCount++)
                {
                    RegenInstance();
                }
            };
        }

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

        private void AttemptInteract(eChainedPuzzleInteraction interaction)
        {
            if(Instance != null)
            {
                Instance.AttemptInteract(interaction);
            }
            else
            {
                Logger.Error("ChainedPuzzle Instance was null, Please call AttemptInteract method after OnBuildDone has called!");
            }
        }

        public void Trigger() => AttemptInteract(eChainedPuzzleInteraction.Activate);
        public void Activate() => AttemptInteract(eChainedPuzzleInteraction.Activate);
        public void Deactivate() => AttemptInteract(eChainedPuzzleInteraction.Deactivate);
        public void ForceSolve() => AttemptInteract(eChainedPuzzleInteraction.Solve);

        public void ForceRegenAndTrigger()
        {
            _OnRegenDone_Once = () =>
            {
                Trigger();
            };

            SendRegenerateState();
        }

        public void TriggerAvailable()
        {
            if (Instance == null)
                return;

            if (Instance.IsSolved)
            {
                ForceRegenAndTrigger();
            }
            else
            {
                Trigger();
            }
        }

        private void SendRegenerateState()
        {
            if(_Replicator.CanSendState)
            {
                _Replicator.SendState.Value1.IntValue++;
                _Replicator.UpdateState();
            }
        }

        private void RegenInstance()
        {
            Instance = _RegenBufferInstance;
            _RegenBufferInstance = CopyNewInstance();

            Instance.add_OnPuzzleSolved(new Action(Solved_Internal));

            _OnRegenDone_Once?.Invoke();
            _OnRegenDone_Once = null;
        }

        public ChainedPuzzleInstance CopyNewInstance()
        {
            return ChainedPuzzleManager.CreatePuzzleInstance(Instance.Data, Instance.m_sourceArea, _Position, Instance.m_parent);
        }
    }
}