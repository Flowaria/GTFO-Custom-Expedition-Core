using AIGraph;
using CustomExpeditions.Extensions;
using CustomExpeditions.Messages;
using CustomExpeditions.Utils;
using GameData;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomExpeditions.HandlerBase
{
    public abstract class CustomExpHandlerBase
    {
        internal string HandlerGUID;

        public Action OnSetupEvent;
        public Action OnUnloadEvent;

        public LG_Layer Layer { get; private set; }
        public LG_LayerType LayerType { get; private set; }
        public LayerData LayerData { get; private set; }
        public WardenObjectiveDataBlock ObjectiveData { get; private set; }
        public eRundownTier CurrentExpeditionTier { get; private set; }
        public int CurrentExpeditionIndex { get; private set; }

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

            var data = RundownManager.GetActiveExpeditionData();
            CurrentExpeditionTier = data.tier;
            CurrentExpeditionIndex = data.expeditionIndex;

            Builder = new BuilderProxy(this);
            WinConditions = new WinConditionProxy(this);
            ObjectiveStatus = new ObjectiveStatusProxy(this);


            var buildDone = new Action(BuildDone);
            var elevatorArrive = new Action(ElevatorArrive);
            var levelSuccess = new Action(OnExpeditionSuccess);
            var levelFail = new Action(OnExpeditionFail);

            GlobalMessage.OnBuildDoneLate += buildDone;
            GlobalMessage.OnElevatorArrive += elevatorArrive;

            GlobalMessage.OnLevelSuccess += levelSuccess;
            GlobalMessage.OnLevelFail += levelFail;

            OnUnloadEvent += () =>
            {
                GlobalMessage.OnBuildDoneLate -= buildDone;
                GlobalMessage.OnElevatorArrive -= elevatorArrive;

                GlobalMessage.OnLevelSuccess -= levelSuccess;
                GlobalMessage.OnLevelFail -= levelFail;
            };

            OnSetupEvent?.Invoke();

            OnSetup();

            //Check WinCondition has been set after Setup
            if(!IsGlobalHandler && (!IsDefaultObjective && !ObjectiveStatus.Is))
            {
                Logger.Error("Every Custom Type Handler should set their ObjectiveStatus Update Behaviour Inside OnSetup()! Handler has been Unloaded!\n - typeID: {0}\n - handler {1}", (byte)ObjectiveData.Type, GetType().Name);
                UnloadSelf();
                return;
            }
        }

        internal void Unload()
        {
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

            if (ConfigUtil.TryGetGlobalConfig<T[]>(configName, out var globalCfg))
            {
                cfgList.AddRange(globalCfg);
            }

            if (ConfigUtil.TryGetLocalConfig<T[]>(configName, out var localCfg))
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
                GlobalMessage.OnUpdate_Level += update;
                if (false)
                {
                    GlobalMessage.OnUpdate += update;

                    //It will fire only once
                    void cleanupHandler()
                    {
                        GlobalMessage.OnUpdate -= update;
                        OnUnloadEvent -= cleanupHandler;
                    }

                    OnUnloadEvent += cleanupHandler;
                }
                
                
            }

            if (fixedUpdate != null)
            {
                GlobalMessage.OnFixedUpdate_Level += fixedUpdate;

                if(false)
                {
                    GlobalMessage.OnFixedUpdate += fixedUpdate;

                    //It will fire only once
                    void cleanupHandler()
                    {
                        GlobalMessage.OnFixedUpdate -= fixedUpdate;
                        OnUnloadEvent -= cleanupHandler;
                    }

                    OnUnloadEvent += cleanupHandler;
                }
            }
        }

        /// <summary>
        /// Trigger OnActive WaveSetting from DataBlock
        /// </summary>
        /// <param name="node">Spawnnode</param>
        /// <param name="type">SpawnType</param>
        public void TriggerOnActiveWave(AIG_CourseNode node = null, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            if (WardenObjectiveManager.HasValidWaveSettings(ObjectiveData.WavesOnActivate))
            {
                TriggerWave(ObjectiveData.WavesOnActivate, node, type);
            }
        }

        public void SpawnWave(uint waveSetting, uint wavePopulation, float delay = 0.0f, AIG_CourseNode node = null, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            TriggerWave(new GenericEnemyWaveData()
            {
                WaveSettings = waveSetting,
                WavePopulation = wavePopulation,
                SpawnDelay = delay,
                TriggerAlarm = false
            }, node, type);
        }

        public void TriggerWave(uint waveSetting, uint wavePopulation, float delay = 0.0f, AIG_CourseNode node = null, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            TriggerWave(new GenericEnemyWaveData()
            {
                WaveSettings = waveSetting,
                WavePopulation = wavePopulation,
                SpawnDelay = delay,
                TriggerAlarm = true
            }, node, type);
        }

        /// <summary>
        /// Trigger Single WaveSetting
        /// </summary>
        /// <param name="waveData">WaveData to use for spawn</param>
        /// <param name="node">Spawnnode</param>
        /// <param name="type">SpawnType</param>
        public void TriggerWave(GenericEnemyWaveData waveData, AIG_CourseNode node = null, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            TriggerWave(new GenericEnemyWaveData[] { waveData }.ToList(), node, type);
        }

        /// <summary>
        /// Trigger Multiple WaveSetting
        /// </summary>
        /// <param name="waveData">WaveData(s) to use for spawn</param>
        /// <param name="node">Spawnnode</param>
        /// <param name="type">SpawnType</param>
        public void TriggerWave(List<GenericEnemyWaveData> waveData, AIG_CourseNode node = null, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            TriggerWave(waveData.ToIl2CppList(), node, type);
        }

        public void TriggerWave(Il2CppSystem.Collections.Generic.List<GenericEnemyWaveData> waveData, AIG_CourseNode node = null, SurvivalWaveSpawnType type = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer)
        {
            if(node == null)
            {
                node = Builder.GetStartingArea().m_courseNode;
            }
            WardenObjectiveManager.TriggerEnemyWaves(waveData, node, type);
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
                WardenObjectiveManager.OnWinConditionSolved(LG_LayerType.MainLayer);
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
            CustomExpHandlerManager.UnloadHandler(this);
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