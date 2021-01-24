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

    public enum ReplicatorType
    {
        LevelInstance,
        Manager
    }

    public enum ReplicatorCHType
    {
        SessionOrderCritical,
        GameOrderCritical,
        GameReceiveCritical,
        GameNonCritical
    }

    public abstract class CustomReplicatorProvider<S> where S : StateWrapper, new()
    {
        public const byte PACKET_OFFSET = 128;

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

        public void Setup(ReplicatorType replicatorType = ReplicatorType.LevelInstance, ReplicatorCHType channelType = ReplicatorCHType.GameOrderCritical, S defaultState = null)
        {
            if (DoneSetup)
            {
                throw new ArgumentException("This replicator already has been setup");
            }

            //Convert Type to LifeTime
            eSNetReplicatorLifeTime lifeTime;
            switch(replicatorType)
            {
                case ReplicatorType.LevelInstance:
                    lifeTime = eSNetReplicatorLifeTime.DestroyedOnLevelReset;
                    break;

                case ReplicatorType.Manager:
                    lifeTime = eSNetReplicatorLifeTime.NeverDestroyed;
                    break;

                default:
                    throw new ArgumentException("Argument: replicatorType is Invalid!");
            }

            //Convert Managed ChannelType into SNet_ChannelType
            SNet_ChannelType snet_channelType;
            switch(channelType)
            {
                case ReplicatorCHType.SessionOrderCritical:
                    snet_channelType = SNet_ChannelType.SessionOrderCritical;
                    break;

                case ReplicatorCHType.GameReceiveCritical:
                    snet_channelType = SNet_ChannelType.GameReceiveCritical;
                    break;

                case ReplicatorCHType.GameOrderCritical:
                    snet_channelType = SNet_ChannelType.GameOrderCritical;
                    break;

                case ReplicatorCHType.GameNonCritical:
                    snet_channelType = SNet_ChannelType.GameNonCritical;
                    break;

                default:
                    throw new ArgumentException("Argument: channelType is Invalid!");
            }

            //Create stuffs
            var guid = Guid.NewGuid().ToString() + "_CustomReplicator_" + UnityEngine.Random.Range(000, 999);
            var syncObj = new GameObject(guid);
            var syncInstance = syncObj.AddComponent<LG_Door_Sync>();

            //Make object not be Destroy On Loaded
            if (lifeTime == eSNetReplicatorLifeTime.NeverDestroyed)
                GameObject.DontDestroyOnLoad(syncObj);

            //Cast Door_Sync to Provider
            SyncProvider = syncInstance;
            var provider = SyncProvider.Cast<ComplexStateReplicatorProvider>();

            //Create Replicator
            var defaultStateStruct = defaultState?.ToOriginal() ?? default;
            OriginalReplicator = ComplexStateReplicator.Create(provider, lifeTime, defaultStateStruct, snet_channelType);
            InnerReplicator = OriginalReplicator.Replicator.Cast<SNet_Replicator>();
            InnerIReplicator = OriginalReplicator.Replicator;

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
            SNet.Replication.RemoveFromAssignedList(InnerIReplicator);

            //Register Replicator to Custom Manager
            CustomReplicatorManager.RegisterReplicator(InnerIReplicator, guid, OnStateChange_Cast, out var newKey);
            InnerReplicator.Key = newKey;
            InnerIReplicator.Key = newKey;

            //Set Packet ID to Custom Area
            SetPacketBytes(OriginalReplicator.m_statePacket, InnerIReplicator.KeyBytes);
            SetPacketBytes(OriginalReplicator.m_statePacket_Recall, InnerIReplicator.KeyBytes);
            SetPacketBytes(OriginalReplicator.m_interactionPacket, InnerIReplicator.KeyBytes);

            SyncProvider.m_stateReplicator = OriginalReplicator;
            SyncProvider.m_syncStruct = OriginalReplicator.GetProviderSyncStruct();

            //Set Default Value
            State.FromOriginal(OriginalReplicator.State);
            SendState.FromOriginal(OriginalReplicator.State);
            DoneSetup = true;
        }

        private void SetPacketBytes<T>(SNet_Packet<T> packet, byte[] keyBytes) where T : struct
        {
            packet.m_internalBytes[0] = keyBytes[0];
            packet.m_internalBytes[1] = keyBytes[1];

            packet.Index += PACKET_OFFSET;
            packet.m_internalBytes[2] = packet.Index;
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
                InnerReplicator.Key = InnerReplicator.Key; //MINOR: Need to find good way
                OriginalReplicator.State = SendState.ToOriginal();
            }
        }

        private void OnStateChange_Cast(pDoorState oldState, pDoorState newState, bool isRecall)
        {
            _OldStateBuffer.FromOriginal(oldState);
            State.FromOriginal(newState);

            OnStateChange(_OldStateBuffer, State, isRecall);
            OnStateChange(_OldStateBuffer, State);
            OnStateChange(isRecall);
            OnStateChange();
        }

        public virtual void OnStateChange(S oldState, S newState, bool isRecall) { }
        public virtual void OnStateChange(S oldState, S newState) { }
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