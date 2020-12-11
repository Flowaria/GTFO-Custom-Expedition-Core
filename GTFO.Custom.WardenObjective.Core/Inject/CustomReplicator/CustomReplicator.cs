using HarmonyLib;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject.CustomReplicator
{
    using ComplexReplicator = SNet_StateReplicator<pDoorState, pDoorInteraction>;
    using ComplexProvider = iSNet_StateReplicatorProvider<pDoorState, pDoorInteraction>;

    using StateChangedAction = Action<pDoorState, pDoorState, bool>;
    using ProviderDict = Dictionary<LG_Door_Sync, Action<pDoorState, pDoorState, bool>>;

    internal static class ProviderManager
    {
        public static readonly ProviderDict PersistentReplicator;
        public static readonly ProviderDict InstanceReplicator;

        static ProviderManager()
        {
            PersistentReplicator = new ProviderDict();
            InstanceReplicator = new ProviderDict();
        }

        public static bool Contains(LG_Door_Sync doorSync, out StateChangedAction action)
        {
            if(!PersistentReplicator.TryGetValue(doorSync, out action))
            {
                if(!InstanceReplicator.TryGetValue(doorSync, out action))
                {
                    return false;
                }
            }

            return true;
        }
    }


    public abstract class CustomReplicatorProvider<S, I> where S : StateWrapperBase, new() where I : InteractionWrapperBase, new()
    {
        private LG_Door_Sync _ProviderSync;

        private CustomReplicatorProvider() { }

        public CustomReplicatorProvider(eSNetReplicatorLifeTime lifeTime = eSNetReplicatorLifeTime.DestroyedOnLevelReset, SNet_ChannelType channelType = SNet_ChannelType.GameOrderCritical)
        {
            _ProviderSync = new LG_Door_Sync();

            var provider = _ProviderSync.Cast<ComplexProvider>();

            _ProviderSync.m_stateReplicator = ComplexReplicator.Create(provider, lifeTime, default, channelType);
            _ProviderSync.m_syncStruct = _ProviderSync.m_stateReplicator.GetProviderSyncStruct();

            if(lifeTime == eSNetReplicatorLifeTime.DestroyedOnLevelReset)
            {
                ProviderManager.InstanceReplicator.Add(_ProviderSync, OnStateChange_Cast);
            }
            else if(lifeTime == eSNetReplicatorLifeTime.NeverDestroyed)
            {
                ProviderManager.PersistentReplicator.Add(_ProviderSync, OnStateChange_Cast);
            }
        }

        public void AttemptInteract(I interaction)
        {
            if(ShouldInteract(interaction, out var state))
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
