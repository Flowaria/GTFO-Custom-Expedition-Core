using GTFO.CustomObjectives.Inject.Global;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GTFO.CustomObjectives.Inject.CustomReplicators
{
    using ComplexProvider = iSNet_StateReplicatorProvider<pDoorState, pDoorInteraction>;
    using ComplexReplicator = SNet_StateReplicator<pDoorState, pDoorInteraction>;
    using ProviderDict = Dictionary<string, Action<pDoorState, pDoorState, bool>>;
    using StateChangedAction = Action<pDoorState, pDoorState, bool>;

    internal static class ProviderManager
    {
        public static readonly ProviderDict PersistentReplicator;
        public static readonly ProviderDict InstanceReplicator;

        static ProviderManager()
        {
            PersistentReplicator = new ProviderDict();
            InstanceReplicator = new ProviderDict();

            GlobalMessage.OnLevelCleanup += () =>
            {
                InstanceReplicator.Clear();
            };
        }

        public static bool Contains(string guid, out StateChangedAction action)
        {
            if (!PersistentReplicator.TryGetValue(guid, out action))
            {
                if (!InstanceReplicator.TryGetValue(guid, out action))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public abstract class CustomReplicatorProvider<S, I> where S : StateWrapperBase, new() where I : InteractionWrapperBase, new()
    {
        private bool _HasSetup = false;
        private LG_Door_Sync _ProviderSync;

        public void Setup(eSNetReplicatorLifeTime lifeTime = eSNetReplicatorLifeTime.DestroyedOnLevelReset, SNet_ChannelType channelType = SNet_ChannelType.GameOrderCritical)
        {
            var guid = Guid.NewGuid().ToString();
            var syncObj = new GameObject(guid);
            var syncInstance = syncObj.AddComponent<LG_Door_Sync>();

            _ProviderSync = syncInstance;

            var provider = _ProviderSync.Cast<ComplexProvider>();

            _ProviderSync.m_stateReplicator = ComplexReplicator.Create(provider, lifeTime, default, channelType);
            _ProviderSync.m_syncStruct = _ProviderSync.m_stateReplicator.GetProviderSyncStruct();

            if (lifeTime == eSNetReplicatorLifeTime.DestroyedOnLevelReset)
            {
                ProviderManager.InstanceReplicator.Add(guid, OnStateChange_Cast);
            }
            else if (lifeTime == eSNetReplicatorLifeTime.NeverDestroyed)
            {
                ProviderManager.PersistentReplicator.Add(guid, OnStateChange_Cast);
                GameObject.DontDestroyOnLoad(syncObj);
            }

            _HasSetup = true;
        }

        public void AttemptInteract(I interaction)
        {
            if (!_HasSetup)
                return;

            if (ShouldInteract(interaction, out var state))
            {
                _ProviderSync.m_stateReplicator.InteractWithState(state.ToOriginal(), interaction.ToOriginal());
            }
        }

        internal void OnStateChange_Cast(pDoorState oldState, pDoorState newState, bool isRecall)
        {
            var oldWrapper = new S();
            var newWrapper = new S();

            oldWrapper.FromOriginal(oldState);
            newWrapper.FromOriginal(newState);

            OnStateChange(oldWrapper, newWrapper, isRecall);
        }

        public abstract bool ShouldInteract(I interaction, out S state);

        public abstract void OnStateChange(S oldState, S newState, bool isRecall);
    }
}