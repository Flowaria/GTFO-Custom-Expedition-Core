using AIGraph;
using GameData;
using GTFO.CustomObjectives.Extensions;
using GTFO.CustomObjectives.Inject.Global;
using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using Newtonsoft.Json;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using UnhollowerBaseLib;
using UnityEngine;

namespace GTFO.CustomObjectives.HandlerBase
{
    public abstract class CustomObjectiveHandlerBase
    {
        internal string HandlerGUID;

        public Action OnSetupEvent;
        public Action OnUnloadEvent;

        public LG_Layer Layer { get; private set; }
        public LG_LayerType LayerType { get; private set; }
        public LayerData LayerData { get; private set; }
        public WardenObjectiveDataBlock ObjectiveData { get; private set; }

        public WinConditionProxy WinConditions { get; private set; }
        public ObjectiveStatusProxy ObjectiveStatus { get; private set; }
        public BuilderProxy Builder { get; private set; }

        public bool IsBuildDone { get; private set; } = false;
        public bool IsDefaultObjective { get; private set; } = false;
        public bool IsGlobalHandler { get; private set; } = false;

        internal void Setup(LG_Layer layer, WardenObjectiveDataBlock objectiveData, bool isGlobalHandler)
        {
            Layer = layer;
            LayerType = layer.m_type;
            ObjectiveData = objectiveData;

            IsDefaultObjective = Enum.IsDefined(typeof(eWardenObjectiveType), ObjectiveData.Type);
            IsGlobalHandler = isGlobalHandler;

            var activeExpedition = RundownManager.ActiveExpedition;
            switch (LayerType)
            {
                case LG_LayerType.MainLayer:
                    LayerData = activeExpedition.MainLayerData;
                    break;

                case LG_LayerType.SecondaryLayer:
                    LayerData = activeExpedition.SecondaryLayerData;
                    break;

                case LG_LayerType.ThirdLayer:
                    LayerData = activeExpedition.ThirdLayerData;
                    break;
            }

            Builder = new BuilderProxy(this);
            WinConditions = new WinConditionProxy(this);
            ObjectiveStatus = new ObjectiveStatusProxy(this);

            GlobalMessage.OnBuildDone += BuildDone;
            GlobalMessage.OnElevatorArrive += ElevatorArrive;

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

        private void BuildDone()
        {
            IsBuildDone = true;

            OnBuildDone();
        }

        private void ElevatorArrive()
        {
            OnElevatorArrive();
        }

        public void FetchTargetConfig<T>(string configName) where T : TargetConfigBase, new()
        {
            var cfgList = new List<T>();

            if(ConfigUtil.TryGetGlobalConfig<T[]>(configName, out var globalCfg))
            {
                cfgList.AddRange(globalCfg);
            }

            if(ConfigUtil.TryGetLocalConfig<T[]>(configName, out var localCfg))
            {
                cfgList.AddRange(localCfg);
            }

            //TODO: Implement This
            //JsonConvert.DeserializeObject<T[]>();
        }

        /// <summary>
        /// Register Update Event from Unity MonoBehaviour
        /// It will be unregister automatically when Handler Unloads
        /// </summary>
        /// <param name="update">Update Function Delegate</param>
        /// <param name="fixedUpdate">Fixed Update Function Delegate</param>
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