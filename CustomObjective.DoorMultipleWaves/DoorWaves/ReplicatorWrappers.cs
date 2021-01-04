using CustomExpeditions.Inject.CustomReplicators;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObjective.DoorMultipleWaves.DoorWaves
{
    public enum DoorWaveInteractionType : byte
    {
        Startup,
        TriggerPuzzleWave,
        StartVerify,
        JumpAndWarmup,
        FailedVerify,
        Complete
    }

    public enum DoorWaveState : byte
    {
        Idle,

        ChainedPuzzle_Active,
        ChainedPuzzle_Warmup,

        ChainedPuzzle_Warmup_Second,
        ChainedPuzzle_Active_Second,
        
        RequireTerminal,

        RequireTerminal_Second,

        Unlocked
    }

    public class DoorWaveIWrapper : InteractionWrapperBase
    {
        public DoorWaveInteractionType Interaction;

        public override void FromOriginal(pDoorInteraction interaction)
        {
            Interaction = (DoorWaveInteractionType)interaction.type;
        }

        public override pDoorInteraction ToOriginal()
        {
            return new pDoorInteraction()
            {
                type = (eDoorInteractionType)Interaction
            };
        }
    }

    public class DoorWaveSWrapper : StateWrapperBase
    {
        public DoorWaveState State;
        public float Value1;
        public float Value2;

        public override void FromOriginal(pDoorState state)
        {
            State = (DoorWaveState)state.status;
            Value1 = state.animProgress;
            Value2 = state.damageTaken;
        }

        public override pDoorState ToOriginal()
        {
            return new pDoorState()
            {
                status = (eDoorStatus)State,
                animProgress = Value1,
                damageTaken = Value2
            };
        }
    }
}
