using GTFO.CustomObjectives.Inject.Global;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GTFO.CustomObjectives.Inject.CustomReplicators
{
    using ComplexStateReplicatorProvider = iSNet_StateReplicatorProvider<pDoorState, pDoorInteraction>;
    using ComplexStateReplicator = SNet_StateReplicator<pDoorState, pDoorInteraction>;
    using ProviderDict = Dictionary<string, Action<pDoorState, pDoorState, bool>>;
    using StateChangedAction = Action<pDoorState, pDoorState, bool>;

    internal static class ProviderManager
    {
        internal static ushort IdSlot = 20000;
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

    public abstract class CustomReplicatorProvider<S> where S : StateWrapperBase, new()
    {
        public bool DoneSetup { get; private set; }
        public LG_Door_Sync SyncProvider { get; private set; }
        public ComplexStateReplicator Replicator { get; private set; }
        public SNet_Replicator InnerReplicator { get; private set; }
        public IReplicator InnerIReplicator { get; private set; }
        public S State { get; private set; }

        public void Setup(eSNetReplicatorLifeTime lifeTime = eSNetReplicatorLifeTime.DestroyedOnLevelReset, SNet_ChannelType channelType = SNet_ChannelType.GameOrderCritical)
        {
            var guid = Guid.NewGuid().ToString();
            var syncObj = new GameObject(guid);
            var syncInstance = syncObj.AddComponent<LG_Door_Sync>();

            SyncProvider = syncInstance;

            var provider = SyncProvider.Cast<ComplexStateReplicatorProvider>();

            Replicator = ComplexStateReplicator.Create(provider, lifeTime, default, channelType);
            InnerReplicator = Replicator.Replicator.Cast<SNet_Replicator>();
            InnerIReplicator = Replicator.Replicator;

            var oldKey = InnerReplicator.Key; //TODO: FIX GODAMN BUG
            var newKey = InnerReplicator.Key = ProviderManager.IdSlot++; //Use Custom ID Area
            SNet_Replication.ClearReplicatorKey(newKey, InnerIReplicator);
            SNet_Replication.s_replicatorSlots[oldKey] = null;
            SNet_Replication.s_replicatorSlots[newKey] = InnerIReplicator;

            SyncProvider.m_stateReplicator = Replicator;
            SyncProvider.m_syncStruct = Replicator.GetProviderSyncStruct();

            if (lifeTime == eSNetReplicatorLifeTime.DestroyedOnLevelReset)
            {
                SNet_Replication.s_highestSlotUsed_SelfManaged--; //Roll-back the Slot Usage
                ProviderManager.InstanceReplicator.Add(guid, OnStateChange_Cast);
            }
            else if (lifeTime == eSNetReplicatorLifeTime.NeverDestroyed)
            {
                SNet_Replication.s_highestSlotUsed_Manager--; //Roll-back the Slot Usage
                ProviderManager.PersistentReplicator.Add(guid, OnStateChange_Cast);
                GameObject.DontDestroyOnLoad(syncObj);
            }

            var newState = new S();
            newState.FromOriginal(Replicator.State);
            State = newState;
            DoneSetup = true;
        }

        public void ChangeState(S state)
        {
            State = state;
            UpdateState();
        }

        public void UpdateState()
        {
            Replicator.State = State.ToOriginal();
            Logger.Log("Got Key {0} {1}", InnerReplicator.Key, InnerReplicator.KeyBytes);
        }

        internal void OnStateChange_Cast(pDoorState oldState, pDoorState newState, bool isRecall)
        {
            var oldWrapper = new S();
            var newWrapper = new S();

            oldWrapper.FromOriginal(oldState);
            newWrapper.FromOriginal(newState);

            OnStateChange(oldWrapper, newWrapper, isRecall);
        }

        public abstract void OnStateChange(S oldState, S newState, bool isRecall);
    }

    public abstract class CustomReplicatorProvider<S, I> : CustomReplicatorProvider<S> where S : StateWrapperBase, new() where I : InteractionWrapperBase, new()
    {
        public bool AllowInteractionByUser { get; set; } = true;

        public void AttemptInteract(I interaction)
        {
            if (!DoneSetup)
                return;

            if (!AllowInteractionByUser && !SNet.IsMaster)
                return;

            if (ShouldInteract(interaction, out var state))
            {
                Replicator.InteractWithState(state.ToOriginal(), interaction.ToOriginal());
            }
        }

        public abstract bool ShouldInteract(I interaction, out S state);
    }
}