using CustomObjective.DoorMultipleWaves.DoorWaves;
using GTFO.CustomObjectives;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObjective.DoorMultipleWaves.PuzzleContext
{
    public class TerminalPuzzleContext : ChainedPuzzleContext
    {
        public MainHandler Handler;
        public LG_ComputerTerminal Terminal;

        public override void OnSolved()
        {
            Terminal.m_command.AddOutput("SUCCESS! Master Door's Security Scan will be resume in few moment...");
            DoorWaveManager.JumpToNextWave();
        }
    }
}
