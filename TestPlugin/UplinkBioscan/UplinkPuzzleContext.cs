using CustomExpeditions.Utils;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.UplinkBioscan
{
    public class UplinkPuzzleContext : ChainedPuzzleContext
    {
        public UplinkBioscanHandler Handler;
        public LG_ComputerTerminal Terminal;

        public override void OnSolved()
        {
            Terminal.ObjectiveItemSolved = true;

            Terminal.m_command.AddOutput(TerminalLineType.Normal, "SUCCESS! Uplink Established Successfully!", 2f);

            Handler.WinConditions.SolvedObjectiveItem(Terminal.Cast<iWardenObjectiveItem>());
        }
    }
}
