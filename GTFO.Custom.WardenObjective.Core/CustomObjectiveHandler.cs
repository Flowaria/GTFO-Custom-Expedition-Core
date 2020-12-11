using GameData;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTFO.CustomObjectives.Extensions;
using AIGraph;
using ChainedPuzzles;
using UnityEngine;
using UnhollowerBaseLib;
using GTFO.CustomObjectives.Utils;
using GTFO.CustomObjectives.Inject;
using GTFO.CustomObjectives.Inject.Global;

namespace GTFO.CustomObjectives
{
    public abstract class CustomObjectiveHandler
    {
        public LG_Layer Layer { get; private set; }
        public LG_LayerType LayerType { get; private set; }
        public WardenObjectiveDataBlock ObjectiveData { get; private set; }
        public WardenObjectiveLayerData ObjectiveLayerData { get; private set; }

        public eWardenObjectiveStatus ObjectiveStatus
        {
            get
            {
                var currentState = WardenObjectiveManager.CurrentState;
                switch (LayerType)
                {
                    case LG_LayerType.MainLayer:
                        return currentState.main_status;

                    case LG_LayerType.SecondaryLayer:
                        return currentState.second_status;

                    case LG_LayerType.ThirdLayer:
                        return currentState.third_status;
                }

                return eWardenObjectiveStatus.NotDiscovered;
            }
        }

        public eWardenSubObjectiveStatus SubObjectiveStatus
        {
            get
            {
                var currentState = WardenObjectiveManager.CurrentState;
                switch (LayerType)
                {
                    case LG_LayerType.MainLayer:
                        return currentState.main_subObj;

                    case LG_LayerType.SecondaryLayer:
                        return currentState.second_subObj;

                    case LG_LayerType.ThirdLayer:
                        return currentState.third_subObj;
                }

                return eWardenSubObjectiveStatus.FindLocationInfo;
            }
        }

        internal void Setup(LG_Layer layer, WardenObjectiveDataBlock objectiveData)
        {
            Layer = layer;
            LayerType = layer.m_type;
            ObjectiveData = objectiveData;

            var activeExpedition = RundownManager.ActiveExpedition;
            switch(LayerType)
            {
                case LG_LayerType.MainLayer:
                    ObjectiveLayerData = activeExpedition.MainLayerData.ObjectiveData;
                    break;

                case LG_LayerType.SecondaryLayer:
                    ObjectiveLayerData = activeExpedition.SecondaryLayerData.ObjectiveData;
                    break;

                case LG_LayerType.ThirdLayer:
                    ObjectiveLayerData = activeExpedition.ThirdLayerData.ObjectiveData;
                    break;
            }

            LG_Factory.add_OnFactoryBuildDone(new Action(OnBuildDone));
            ElevatorRide.add_OnElevatorHasArrived(new Action(OnElevatorArrive));

            OnSetup();
        }

        internal void Unload()
        {
            LG_Factory.remove_OnFactoryBuildDone(new Action(OnBuildDone));
            ElevatorRide.remove_OnElevatorHasArrived(new Action(OnElevatorArrive));

            OnUnload();
        }

        /// <summary>
        /// Listen to the actual gameObject spawn event for LG_DistributeItem
        /// </summary>
        /// <param name="distItem">Target LG_DistributeItem</param>
        /// <param name="isWardenObjectiveItem">Is the warden objective item? (needed for reason)</param>
        /// <param name="onSpawned">delegate for spawned event</param>
        public void ListenItemSpawnEvent(LG_DistributeItem distItem, bool isWardenObjectiveItem, Action<GameObject> onSpawned)
        {
            ItemUtil.RegisterItem(distItem, isWardenObjectiveItem, onSpawned);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void RegisterObjectiveItem(iWardenObjectiveItem item)
        {
            WardenObjectiveManager.RegisterObjectiveItem(LayerType, item);
        }

        public void RegisterObjectiveItemForCollection(iWardenObjectiveItem item)
        {
            WardenObjectiveManager.RegisterObjectiveItemForCollection(LayerType, item);
        }

        public void RegisterRequiredScanItem(iWardenObjectiveItem item)
        {
            var array = new Il2CppReferenceArray<iWardenObjectiveItem>(1);
            array[0] = item;

            ElevatorShaftLanding.Current.AddRequiredScanItems(array);
        }

        public void RegisterUpdateEvent(Action update = null, Action fixedUpdate = null)
        {
            if(update != null)
            {
                GlobalMessage.OnUpdate += update;

                Action cleanupHandler = null;
                cleanupHandler = () =>
                {
                    GlobalMessage.OnUpdate -= update;
                    GlobalMessage.OnLevelCleanup -= cleanupHandler;
                };
                GlobalMessage.OnLevelCleanup += cleanupHandler;
            }

            if(fixedUpdate != null)
            {
                GlobalMessage.OnFixedUpdate += fixedUpdate;

                Action cleanupHandler = null;
                cleanupHandler = () =>
                {
                    GlobalMessage.OnFixedUpdate -= fixedUpdate;
                    GlobalMessage.OnLevelCleanup -= cleanupHandler;
                };
                GlobalMessage.OnLevelCleanup += cleanupHandler;
            }
        }

        public void TriggerOnActiveWave(AIG_CourseNode node, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            if (WardenObjectiveManager.HasValidWaveSettings(ObjectiveData.WavesOnActivate))
            {
                WardenObjectiveManager.TriggerEnemyWaves(ObjectiveData.WavesOnActivate, node, type);
            }
        }

        public void TriggerWave(GenericEnemyWaveData waveData, AIG_CourseNode node, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            TriggerWave(new GenericEnemyWaveData[] { waveData }.ToList(), node, type);
        }


        public void TriggerWave(List<GenericEnemyWaveData> waveData, AIG_CourseNode node, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            WardenObjectiveManager.TriggerEnemyWaves(waveData.ToIl2CppList(), node, type);
        }

        public void StopWave()
        {
            WardenObjectiveManager.StopAllWardenObjectiveEnemyWaves();
        }

        public void SetObjectiveTextFragment(eWardenTextFragment type, string text)
        {
            WardenObjectiveManager.SetObjectiveTextFragment(LayerType, type, text);
        }

        public void UpdateObjectiveStatus(eWardenObjectiveInteractionType type, eWardenSubObjectiveStatus status)
        {
            WardenObjectiveManager.Current.AttemptInteract(new pWardenObjectiveInteraction
            {
                inLayer = LayerType,
                type = type,
                newSubObj = status,
                forceUpdate = true
            });
        }

        //Forwards
        public abstract void OnSetup();
        public virtual void OnBuildDone() { }
        public virtual void OnElevatorArrive() { }
        public virtual void OnUnload() { }
    }
}
