using GameData;
using GTFO.CustomObjectives.Inject.CustomReplicators;
using GTFO.CustomObjectives.Inject.Global;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
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

    public static class FogLevelUtil
    {
        private static bool _IsInGame = false;
        private static float _EndTime = 0.0f;
        private readonly static List<FogLevelSetting> _FogSettings;
        private readonly static FogLevelReplicator _Replicator;

        public static int CurrentLevel { get; private set; }
        public static int LevelCount { get; private set; }

        static FogLevelUtil()
        {
            _Replicator = new FogLevelReplicator();
            _FogSettings = new List<FogLevelSetting>();

            _Replicator.AllowInteractionByUser = false;
            _Replicator.OnLevelChanged += OnLevelChanged;

            GlobalMessage.OnBuildDone += () =>
            {
                GlobalMessage.OnUpdate += OnUpdate;
                _IsInGame = true;
            };

            GlobalMessage.OnLevelCleanup += () =>
            {
                GlobalMessage.OnUpdate -= OnUpdate;
                _IsInGame = false;
            };
        }

        public static void Setup() //We need it since Replicator heavily relys on Creation order.
        {
            _Replicator.Setup(eSNetReplicatorLifeTime.NeverDestroyed, SNet_ChannelType.GameOrderCritical);
        }

        private static void OnLevelChanged(int level)
        {
            if (!(0 <= level && level < LevelCount))
            {
                return;
            }

            CurrentLevel = level;
            _EndTime = Clock.Time + _FogSettings[CurrentLevel].TransitionTime;

            var block = GameDataBlockBase<FogSettingsDataBlock>.GetBlock(_FogSettings[CurrentLevel].FogSetting);
            LocalPlayerAgentSettings.Current.SetTargetFogSettings(block);
        }

        private static void OnUpdate()
        {
            if (!_IsInGame)
                return;

            return; //TODO: Wip

            var remainingTime = _EndTime - Clock.TimeInSessionHub;
            var percent = 1.0f - (remainingTime / _FogSettings[CurrentLevel].TransitionTime);

            LocalPlayerAgentSettings.Current.UpdateBlendTowardsTargetFogSetting(percent, true);

            if (percent >= 1.0f)
            {
            }
        }

        //TODO: Allow user to jump without increament or decreament (ex: 1 -> 3)
        public static void SetLevel(int level)
        {
            if (!(0 <= level && level < LevelCount))
            {
                return;
            }

            if (level < CurrentLevel)
            {
                _Replicator.Lower();
            }

            if (level > CurrentLevel)
            {
                _Replicator.Raise();
            }
        }
    }
}