using CustomExpeditions.Messages;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomExpeditions.CustomReplicators
{
    using ComplexStateReplicator = SNet_StateReplicator<pDoorState, pDoorInteraction>;
    using ComplexStateReplicatorProvider = iSNet_StateReplicatorProvider<pDoorState, pDoorInteraction>;

    public abstract class CustomReplicatorProvider<S> where S : StateWrapper, new()
    {
        public bool CanSendState
        {
            get { return DoneSetup && (!SNet.HasMaster || SNet.IsMaster); }
        }

        public bool DoneSetup { get; private set; }
        public LG_Door_Sync SyncProvider { get; private set; }
        public SNet_Replicator InnerReplicator { get; private set; }
        public IReplicator InnerIReplicator { get; private set; }
        public S SendState { get; private set; } = new S();
        public S State { get; private set; } = new S();

        private readonly S _OldStateBuffer = new S();

        internal ComplexStateReplicator OriginalReplicator { get; set; }

        public void Setup(eSNetReplicatorLifeTime lifeTime = eSNetReplicatorLifeTime.DestroyedOnLevelReset, SNet_ChannelType channelType = SNet_ChannelType.GameOrderCritical)
        {
            if(lifeTime == eSNetReplicatorLifeTime.DynamicallyDestroyed)
            {
                throw new ArgumentException("DynamicallyDestroyed is not supported!");
            }

            //Create stuffs
            var guid = Guid.NewGuid().ToString();
            var syncObj = new GameObject(guid);
            var syncInstance = syncObj.AddComponent<LG_Door_Sync>();

            //Make object not be Destroy On Loaded
            if (lifeTime == eSNetReplicatorLifeTime.NeverDestroyed)
                GameObject.DontDestroyOnLoad(syncObj);

            //Cast Door_Sync to Provider
            SyncProvider = syncInstance;
            var provider = SyncProvider.Cast<ComplexStateReplicatorProvider>();

            //Create Replicator
            OriginalReplicator = ComplexStateReplicator.Create(provider, lifeTime, default, channelType);
            InnerReplicator = OriginalReplicator.Replicator.Cast<SNet_Replicator>();
            InnerIReplicator = OriginalReplicator.Replicator;

            //Set Packet ID to Custom Area
            foreach(var packet in InnerReplicator.m_packets)
            {
                packet.Index += 128;
            }

            //Remove Replicator from Official Manager Class
            var oldKey = InnerIReplicator.Key;
            switch (InnerIReplicator.Type)
            {
                case SNet_ReplicatorType.Manager:
                    SNet_Replication.s_replicatorSlots[oldKey] = null;
                    SNet_Replication.s_highestSlotUsed_Manager--;
                    break;

                case SNet_ReplicatorType.SelfManaged:
                    SNet_Replication.s_replicatorSlots[oldKey] = null;
                    SNet_Replication.s_highestSlotUsed_SelfManaged--;
                    break;
            }

            //Register Replicator to Custom Manager
            CustomReplicatorManager.RegisterReplicator(InnerIReplicator, guid, OnStateChange_Cast);

            SyncProvider.m_stateReplicator = OriginalReplicator;
            SyncProvider.m_syncStruct = OriginalReplicator.GetProviderSyncStruct();

            SendState.FromOriginal(OriginalReplicator.State);
            DoneSetup = true;
        }

        public void ChangeState(S state)
        {
            if (CanSendState)
            {
                SendState = state;
                UpdateState();
            }
        }

        public void UpdateState()
        {
            if (CanSendState)
            {
                OriginalReplicator.State = SendState.ToOriginal();
            }

            InnerReplicator.Key = InnerReplicator.Key; //MINOR: Need to find good way
        }

        private void OnStateChange_Cast(pDoorState oldState, pDoorState newState, bool isRecall)
        {
            _OldStateBuffer.FromOriginal(oldState);
            State.FromOriginal(newState);

            OnStateChange(_OldStateBuffer, State, isRecall);
            OnStateChange(isRecall);
            OnStateChange();
        }

        public virtual void OnStateChange(S oldState, S newState, bool isRecall) { }
        public virtual void OnStateChange(bool isRecall) { }
        public virtual void OnStateChange() { }
    }

    public abstract class CustomReplicatorProvider<S, I> : CustomReplicatorProvider<S> where S : StateWrapper, new() where I : InteractionWrapper, new()
    {
        public bool AllowInteractionByUser { get; set; } = true;

        public void AttemptInteract(I interaction)
        {
            if (!DoneSetup)
                return;

            if (!AllowInteractionByUser && !SNet.IsMaster)
                return;

            var newSendState = new S();
            if (ShouldInteract(interaction, ref newSendState))
            {
                OriginalReplicator.InteractWithState(newSendState.ToOriginal(), interaction.ToOriginal());
            }
        }

        public abstract bool ShouldInteract(I interaction, ref S state);
    }

    public abstract class CustomReplicatorProvider : CustomReplicatorProvider<StateWrapper>
    {

    }
}