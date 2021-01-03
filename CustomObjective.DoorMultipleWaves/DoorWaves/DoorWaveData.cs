using GameData;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomObjective.DoorMultipleWaves.DoorWaves
{
    public class DoorWaveData
    {
        public LG_ComputerTerminal Terminal;

        public uint PuzzleID;

        public float Time_Init;
        public float Time_Search;

        public float Time_FailInit;
        public float Time_FailSearch;

        public WardenObjectiveEventData[] EventData;

        public string GeneratedFileName;
        public string GeneratedFileContent;

        public eLocalZoneIndex TerminalZone;
    }
}
