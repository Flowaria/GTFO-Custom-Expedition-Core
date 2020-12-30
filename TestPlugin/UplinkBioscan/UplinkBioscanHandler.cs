using ChainedPuzzles;
using GameData;
using GTFO.CustomObjectives;
using GTFO.CustomObjectives.HandlerBase;
using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TestPlugin.UplinkBioscan
{
    public class UplinkBioscanHandler : CustomObjectiveHandlerBase
    {
        public override void OnSetup()
        {
            var placeData = LayerData.ObjectiveData.ZonePlacementDatas;
            var terminalNumber = ObjectiveData.Uplink_NumberOfTerminals;
            var placements = Builder.PickPlacementsStandard(placeData, terminalNumber);

            foreach(var placement in placements)
            {
                if(Builder.TryGetZone(placement.LocalIndex, out var zone))
                {
                    Builder.FetchTerminal(zone, placement.Weights, out var distItem, out var distNode, (terminal) =>
                    {
                        OnObjectiveTerminalSpawned(terminal);
                    });
                }
            }
        }

        public void OnObjectiveTerminalSpawned(LG_ComputerTerminal terminal)
        {
            if(terminal == null)
            {
                return;
            }

            terminal.UplinkPuzzle = new TerminalUplinkPuzzle()
            {
                TerminalUplinkIP = SerialGenerator.GetIpAddress(),
                Connected = false
            };

            WinConditions.RegisterObjectiveItemForCollection(terminal.Cast<iWardenObjectiveItem>());

            //Register Info text
            ObjectiveStatus.SetObjectiveTextFragment(eWardenTextFragment.ITEM_SERIAL, terminal.ItemKey);
            ObjectiveStatus.SetObjectiveTextFragment(eWardenTextFragment.UPLINK_ADDRESS, terminal.UplinkPuzzle.TerminalUplinkIP);

            //Add Terminal Command
            TerminalUtil.AddCommand(terminal, "UPLINK_CONNECT", "ESTABLISH AN EXTERNAL UPLINK CONNECTION", UplinkCommandHandler);

            terminal.m_isWardenObjective = true;
            terminal.ObjectiveItemSolved = false;
        }

        public void UplinkCommandHandler(LG_ComputerTerminal terminal, string param1, string param2)
        {
            var cmd = terminal.m_command;
            if(param1.Equals(terminal.UplinkPuzzle.TerminalUplinkIP))
            {
                if(terminal.UplinkPuzzle.Connected)
                {
                    cmd.AddOutput("UPLINK ERROR: Uplink is already opened with ip: '" + param1 + "'.", true);
                    return;
                }

                terminal.UplinkPuzzle.Connected = true;

                //Create Puzzle Context
                var puzzle = ChainedPuzzleUtil.Setup<CP_UplinkPuzzleContext>(
                    ObjectiveData.ChainedPuzzleToActive,
                    terminal.SpawnNode.m_area,
                    terminal.m_wardenObjectiveSecurityScanAlign);

                //Assign Context objects for OnSolved Event
                puzzle.Handler = this;
                puzzle.Terminal = terminal;
                
                //Console Stuffs
                cmd.AddOutput(TerminalLineType.ProgressWait, "Creating external uplink to address " + param1 + ", please wait", 3f);
                cmd.AddOutput(TerminalLineType.SpinningWaitNoDone, "Initial Uplink establish, awaiting verification key", 3f);
                cmd.AddOutput("", true);
                if(puzzle.Instance.Data.TriggerAlarmOnActivate)
                {
                    cmd.AddOutput(TerminalLineType.Warning, "WARNING! Breach detected!", 0.8f);
                    cmd.AddOutput(TerminalLineType.Warning, "WARNING! Breach detected!", 0.8f);
                    cmd.AddOutput(TerminalLineType.Warning, "WARNING! Breach detected!", 0.8f);
                    cmd.AddOutput("", true);
                    cmd.AddOutput(TerminalLineType.Warning, $"Security Scan with [{puzzle.Instance.Data.PublicAlarmName}] is required for finalizing the connection!");
                }
                else
                {
                    cmd.AddOutput($"Security Scan is required for finalizing the connection!", true);
                }
                
                cmd.OnEndOfQueue = new Action (() =>
                {
                    //Trigger Alarm, Attempt to Start ChainedPuzzle
                    puzzle.Trigger();
                });
            }
            else
            {
                cmd.AddOutput("UPLINK ERROR: Could not connect to '" + param1 + "'. Make sure it is a valid address for an external uplink.", true);
            }
        }
    }
}
