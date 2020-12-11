using GameData;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObjective.DoorMultipleWaves
{
    public static class DoorWaveManager
    {
        public static LG_SecurityDoor ChainedDoor;

        public static eLocalZoneIndex MainZone;
        public static DoorWaveData[] Waves;

        static int _currentWaveIndex = 0;
        public static DoorWaveData CurrentWave
        {
            get
            {
                return Waves[_currentWaveIndex];
            }
        }

        public static void OnUpdate()
        {

        }

        public static void StartWave()
        {

        }

        public static void StartSearchingPhase()
        {

        }

        public static void JumpToNextWave()
        {

        }
    }
}
