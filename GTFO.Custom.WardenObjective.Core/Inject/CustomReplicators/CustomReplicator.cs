using CustomObjectives.Messages;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomObjectives.Inject.CustomReplicators
{
    using ComplexStateReplicator = SNet_StateReplicator<pDoorState, pDoorInteraction>;
    using ComplexStateReplicatorProvider = iSNet_StateReplicatorProvider<pDoorState, pDoorInteraction>;
    using ProviderDict = Dictionary<string, Action<pDoorState, pDoorState, bool>>;
    using StateChangedAction = Action<pDoorState, pDoorState, bool>;

    internal static class ProviderManager
    {
        private static ushort IdSlot_SelfManaged = 20000;
        public static readonly ProviderDict PersistentReplicator;
        public static readonly ProviderDict InstanceReplicator;

        static ProviderManager()
        {
            PersistentReplicator = new ProviderDict();
            InstanceReplicator = new ProviderDict();

            GlobalMessage.OnResetSession_SNet += () =>
            {
                InstanceReplicator.Clear();
                IdSlot_SelfManaged = 20000;
            };
        }

        public static void AssignReplicator(IReplicator replicator, string guid, StateChangedAction onStateChange)
        {
            var replicator_casted = replicator.Cast<SNet_Replicator>();
            switch (replicator.Type)
            {
                case SNet_ReplicatorType.Manager:
                    ProviderManager.PersistentReplicator.Add(guid, onStateChange);
                    break;

                case SNet_ReplicatorType.SelfManaged:
                    ProviderManager.InstanceReplicator.Add(guid, onStateChange);
                    break;
            }

            AssignKey(replicator_casted, replicator);
        }

        public static void AssignKey(SNet_Replicator replicator, IReplicator replicator_i)
        {
            ushort oldKey = replicator.Key;
            ushort newKey = 0;
            switch (replicator.Type)
            {
                case SNet_ReplicatorType.Manager:
                    newKey = oldKey;
                    break;

                case SNet_ReplicatorType.SelfManaged:
                    newKey = IdSlot_SelfManaged++;
                    SNet_Replication.s_highestSlotUsed_SelfManaged--;
                    break;
            }

            if (newKey == 0)
            {
                Logger.Warning("Cannot Assign Key for Replicator!");
                return;
            }

            replicator.Key = newKey;

            SNet_Replication.AssignReplicatorKey(replicator_i, newKey);
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
        public SNet_Replicator InnerReplicator { get; private set; }
        public IReplicator InnerIReplicator { get; private set; }
        public S SendState { get; private set; } = new S();
        public S State { get; private set; } = new S();

        internal ComplexStateReplicator OriginalReplicator { get; set; }

        public void Setup(eSNetReplicatorLifeTime lifeTime = eSNetReplicatorLifeTime.DestroyedOnLevelReset, SNet_ChannelType channelType = SNet_ChannelType.GameOrderCritical)
        {
            //Create stuffs
            var guid = Guid.NewGuid().ToString();
            var syncObj = new GameObject(guid);
            var syncInstance = syncObj.AddComponent<LG_Door_Sync>();

            //Cast Door_Sync to Provider
            SyncProvider = syncInstance;
            var provider = SyncProvider.Cast<ComplexStateReplicatorProvider>();

            //Create Replicator
            OriginalReplicator = ComplexStateReplicator.Create(provider, lifeTime, default, channelType);
            InnerReplicator = OriginalReplicator.Replicator.Cast<SNet_Replicator>();
            InnerIReplicator = OriginalReplicator.Replicator;

            //Assign Replicator
            ProviderManager.AssignReplicator(InnerIReplicator, guid, OnStateChange_Cast);

            SyncProvider.m_stateReplicator = OriginalReplicator;
            SyncProvider.m_syncStruct = OriginalReplicator.GetProviderSyncStruct();

            if (lifeTime == eSNetReplicatorLifeTime.NeverDestroyed)
            {
                GameObject.DontDestroyOnLoad(syncObj);
            }

            SendState.FromOriginal(OriginalReplicator.State);
            DoneSetup = true;
        }

        public void ChangeState(S state)
        {
            if (!SNet.HasMaster || SNet.IsMaster)
            {
                SendState = state;
                UpdateState();
            }
        }

        public void UpdateState()
        {
            if (!SNet.HasMaster || SNet.IsMaster)
            {
                OriginalReplicator.State = SendState.ToOriginal();
            }

            InnerReplicator.Key = InnerReplicator.Key; //MINOR: Need to find good way
        }

        internal void OnStateChange_Cast(pDoorState oldState, pDoorState newState, bool isRecall)
        {
            var oldWrapper = new S();

            oldWrapper.FromOriginal(oldState);
            State.FromOriginal(newState);

            OnStateChange(oldWrapper, State, isRecall);
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
                OriginalReplicator.InteractWithState(state.ToOriginal(), interaction.ToOriginal());
            }
        }

        public abstract bool ShouldInteract(I interaction, out S state);
    }
}