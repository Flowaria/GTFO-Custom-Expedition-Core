using CustomExpeditions.CustomReplicators;
using CustomExpeditions.Inject.CustomReplicators;
using LevelGeneration;
using System;
using UnityEngine;

namespace CustomExpeditions.Utils.FogLevel
{
    public class FogLevelSetting
    {
        public uint FogSetting;
        public float TransitionTime;
    }

    public class FogLevelState : StateWrapper
    {
        public int Level
        {
            get => Value1.IntValue;
            set => Value1.IntValue = value;
        }

        public bool UsingFogController
        {
            get => Flag1;
            set => Flag1 = value;
        }
    }

    public class FogLevelReplicator : CustomReplicatorProvider<FogLevelState>
    {
        public Action<int> OnLevelChanged;

        public void SetLevel(int level)
        {
            if(State.Level != level && CanSendState)
            {
                SendState.Level = level;
                UpdateState();
            }
        }

        public override void OnStateChange()
        {
            OnLevelChanged?.Invoke(State.Level);
        }
    }
}