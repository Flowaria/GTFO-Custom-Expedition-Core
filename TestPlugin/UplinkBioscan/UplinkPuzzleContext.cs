using GTFO.CustomObjectives;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.UplinkBioscan
{
    class CP_UplinkPuzzleContext : ChainedPuzzleContext
    {
        public UplinkBioscanHandler Handler;
        public LG_ComputerTerminal Terminal;

        public override void OnSolved()
        {
            Terminal.ObjectiveItemSolved = true;
            Terminal.UplinkPuzzle.Solved = true;

            Terminal.m_command.AddOutput(TerminalLineType.Normal, "SUCCESS! Uplink Established Successfully!", 2f);

            Handler.StopAllWave();

            Handler.WinConditions.SolvedObjectiveItem(Terminal.Cast<iWardenObjectiveItem>());
        }
    }
}
