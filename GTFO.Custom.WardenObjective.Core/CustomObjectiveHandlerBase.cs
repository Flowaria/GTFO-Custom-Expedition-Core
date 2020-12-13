using AIGraph;
using GameData;
using GTFO.CustomObjectives.Extensions;
using GTFO.CustomObjectives.Inject.Global;
using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using UnhollowerBaseLib;
using UnityEngine;

namespace GTFO.CustomObjectives
{
    public abstract class CustomObjectiveHandlerBase
    {
        public Action OnSetupEvent;
        public Action OnUnloadEvent;

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
            switch (LayerType)
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

            GlobalMessage.OnBuildDone += OnBuildDone;
            GlobalMessage.OnElevatorArrive += OnElevatorArrive;

            GlobalMessage.OnLevelSuccess += OnExpeditionSuccess;
            GlobalMessage.OnLevelFail += OnExpeditionFail;

            OnSetupEvent?.Invoke();

            OnSetup();
        }

        internal void Unload()
        {
            GlobalMessage.OnBuildDone -= OnBuildDone;
            GlobalMessage.OnElevatorArrive -= OnElevatorArrive;

            GlobalMessage.OnLevelSuccess -= OnExpeditionSuccess;
            GlobalMessage.OnLevelFail -= OnExpeditionFail;

            OnUnloadEvent?.Invoke();

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
            if (update != null)
            {
                GlobalMessage.OnUpdate += update;

                //It will fire only once
                void cleanupHandler()
                {
                    GlobalMessage.OnUpdate -= update;
                    GlobalMessage.OnLevelCleanup -= cleanupHandler;
                }

                GlobalMessage.OnLevelCleanup += cleanupHandler;
                OnUnloadEvent += cleanupHandler;
            }

            if (fixedUpdate != null)
            {
                GlobalMessage.OnFixedUpdate += fixedUpdate;

                //It will fire only once
                void cleanupHandler()
                {
                    GlobalMessage.OnFixedUpdate -= fixedUpdate;
                    GlobalMessage.OnLevelCleanup -= cleanupHandler;
                }

                GlobalMessage.OnLevelCleanup += cleanupHandler;
                OnUnloadEvent += cleanupHandler;
            }
        }

        /// <summary>
        /// Trigger OnActive WaveSetting from DataBlock
        /// </summary>
        /// <param name="node">Spawnnode</param>
        /// <param name="type">SpawnType</param>
        public void TriggerOnActiveWave(AIG_CourseNode node, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            if (WardenObjectiveManager.HasValidWaveSettings(ObjectiveData.WavesOnActivate))
            {
                WardenObjectiveManager.TriggerEnemyWaves(ObjectiveData.WavesOnActivate, node, type);
            }
        }

        /// <summary>
        /// Trigger Single WaveSetting
        /// </summary>
        /// <param name="waveData">WaveData to use for spawn</param>
        /// <param name="node">Spawnnode</param>
        /// <param name="type">SpawnType</param>
        public void TriggerWave(GenericEnemyWaveData waveData, AIG_CourseNode node, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            TriggerWave(new GenericEnemyWaveData[] { waveData }.ToList(), node, type);
        }

        /// <summary>
        /// Trigger Multiple WaveSetting
        /// </summary>
        /// <param name="waveData">WaveData(s) to use for spawn</param>
        /// <param name="node">Spawnnode</param>
        /// <param name="type">SpawnType</param>
        public void TriggerWave(List<GenericEnemyWaveData> waveData, AIG_CourseNode node, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            WardenObjectiveManager.TriggerEnemyWaves(waveData.ToIl2CppList(), node, type);
        }

        /// <summary>
        /// Stop All Wave
        /// </summary>
        public void StopAllWave()
        {
            WardenObjectiveManager.StopAllWardenObjectiveEnemyWaves();
        }

        /// <summary>
        /// Set Warden Objective hint Fragment (such as [ITEM_SERIAL])
        /// </summary>
        /// <param name="type">Fragment Type</param>
        /// <param name="text">String to be placed in selected Fragment</param>
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

        public void ForceWin()
        {
            if (SNet.IsMaster)
            {
                GameStateManager.ChangeState(eGameStateName.ExpeditionSuccess);
            }
        }

        public void ForceFail()
        {
            if (SNet.IsMaster)
            {
                GameStateManager.ChangeState(eGameStateName.ExpeditionFail);
            }
        }

        public void UnloadSelf()
        {
            CustomObjectiveManager.UnloadHandler(this);
        }

        //Forwards
        public abstract void OnSetup();

        public virtual void OnBuildDone()
        {
        }

        public virtual void OnElevatorArrive()
        {
        }

        public virtual void OnExpeditionSuccess()
        {
        }

        public virtual void OnExpeditionFail()
        {
        }

        public virtual void OnUnload()
        {
        }
    }
}