using GTFO.CustomObjectives;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWardenObjective.ChainedPuzzle.Uplink
{
    class CP_UplinkPuzzleContext : ChainedPuzzleContext
    {
        public CP_UplinkHandler Handler;
        public LG_ComputerTerminal Terminal;

        public override void OnSolved()
        {
            Terminal.ObjectiveItemSolved = true;

            Terminal.m_command.AddOutput(TerminalLineType.Normal, "SUCCESS! Uplink Established Successfully!", 2f);

            Handler.StopAllWave();

            if (SNet.IsMaster)
            {
                if(Handler.ObjectiveStatus != eWardenObjectiveStatus.WardenObjectiveItemSolved)
                {
                    Handler.UpdateObjectiveStatus(
                        eWardenObjectiveInteractionType.SolveWardenObjectiveItem,
                        eWardenSubObjectiveStatus.FindLocationInfoHelp);
                }
            }
        }
    }
}
