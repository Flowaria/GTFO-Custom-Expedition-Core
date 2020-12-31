using GTFO.CustomObjectives.Inject.CustomReplicators;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO.CustomObjectives.Utils
{
    public class FogLevelSetting
    {
        public uint FogSetting;
        public float TransitionTime;
    }

    public class FogLevelState : StateWrapperBase
    {
        public int Level;

        public override void FromOriginal(pDoorState state)
        {
            Level = Mathf.RoundToInt(state.animProgress);
        }

        public override pDoorState ToOriginal()
        {
            return new pDoorState()
            {
                animProgress = Level
            };
        }
    }

    public enum FogLevelInteractionType : byte
    {
        Raise, Lower
    }

    public class FogLevelInteraction : InteractionWrapperBase
    {
        public FogLevelInteractionType InteractionType;

        public override void FromOriginal(pDoorInteraction interaction)
        {
            InteractionType = (FogLevelInteractionType)interaction.type;
        }

        public override pDoorInteraction ToOriginal()
        {
            return new pDoorInteraction()
            {
                type = (eDoorInteractionType)InteractionType
            };
        }
    }

    public class FogLevelReplicator : CustomReplicatorProvider<FogLevelState, FogLevelInteraction>
    {
        public Action<int> OnLevelChanged;

        public override void OnStateChange(FogLevelState oldState, FogLevelState newState, bool isRecall)
        {
            OnLevelChanged?.Invoke(newState.Level);
        }

        public override bool ShouldInteract(FogLevelInteraction interaction, out FogLevelState state)
        {
            if (interaction.InteractionType == FogLevelInteractionType.Raise)
            {
                state = new FogLevelState()
                {
                    Level = FogLevelUtil.CurrentLevel - 1
                };
                return true;
            }
            else if (interaction.InteractionType == FogLevelInteractionType.Lower)
            {
                state = new FogLevelState()
                {
                    Level = FogLevelUtil.CurrentLevel + 1
                };
                return true;
            }

            state = null;
            return false;
        }

        public void Raise()
        {
            AttemptInteract(new FogLevelInteraction()
            {
                InteractionType = FogLevelInteractionType.Raise
            });
        }

        public void Lower()
        {
            AttemptInteract(new FogLevelInteraction()
            {
                InteractionType = FogLevelInteractionType.Raise
            });
        }
    }
}
