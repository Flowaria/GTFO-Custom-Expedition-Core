using GameData;
using GTFO.CustomObjectives.Inject.CustomReplicators;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObjective.DoorMultipleWaves.DoorWaves
{
    public class DoorWaveManager : CustomReplicatorProvider<DoorWaveSWrapper, DoorWaveIWrapper>
    {
        public LG_SecurityDoor ChainedDoor;
        public eLocalZoneIndex MainZone;
        public DoorWaveData[] Waves;

        private DoorWaveState CurrentState = DoorWaveState.Idle;

        int _currentWaveIndex = 0;
        public DoorWaveData CurrentWave
        {
            get
            {
                return Waves[_currentWaveIndex];
            }
        }

        public static DoorWaveManager Current { get; private set; }

        public static void Setup()
        {
            Current = new DoorWaveManager();
            Current.Setup(eSNetReplicatorLifeTime.DestroyedOnLevelReset, SNet_ChannelType.GameOrderCritical);
        }

        public void OnUpdate()
        {

        }

        public void StartWave()
        {

        }

        public void StartSearchingPhase()
        {

        }

        public void JumpToNextWave()
        {

        }

        public override void OnStateChange(DoorWaveSWrapper oldState, DoorWaveSWrapper newState, bool isRecall)
        {
            MelonLoader.MelonLogger.Log(string.Format("State Changed: {0} {1} {2}", newState.State, newState.Value1, isRecall));
            Console.WriteLine();
            CurrentState = newState.State;
        }

        public void AttemptInteract(DoorWaveInteractionType type)
        {
            AttemptInteract(new DoorWaveIWrapper()
            {
                Interaction = type
            });
        }

        public override bool ShouldInteract(DoorWaveIWrapper interaction, out DoorWaveSWrapper state)
        {
            switch(interaction.Interaction)
            {
                case DoorWaveInteractionType.Startup:

                    break;

                case DoorWaveInteractionType.StartVerify:

                    break;

                case DoorWaveInteractionType.JumpAndWarmup:

                    break;

                case DoorWaveInteractionType.FailedVerify:

                    break;
            }

            if (interaction.Interaction == DoorWaveInteractionType.Complete)
            {
                state = new DoorWaveSWrapper()
                {
                    State = DoorWaveState.Unlocked,
                    Value1 = 5.0f
                };
            }
            else
            {
                state = new DoorWaveSWrapper()
                {
                    State = DoorWaveState.Idle,
                    Value1 = 32.0f
                };
            }

            return true;
        }
    }
}
