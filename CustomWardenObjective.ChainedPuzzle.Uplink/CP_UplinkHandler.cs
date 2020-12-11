using ChainedPuzzles;
using GameData;
using GTFO.CustomObjectives;
using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomWardenObjective.ChainedPuzzle.Uplink
{
    public class CP_UplinkHandler : CustomObjectiveHandler
    {
        public string UplinkIP;

        public override void OnSetup()
        {
            if(!PlacementUtil.TryGetRandomPlaceSingleZone(this, out var zone, out var weight))
            {
                return;
            }
            PlacementUtil.FetchFunctionMarker(zone, weight, ExpeditionFunction.Terminal, out var distItem, out var distNode);

            Console.WriteLine("{0} {1}",zone.ID, weight.Middle);
            Console.WriteLine(distItem.m_function);

            ListenItemSpawnEvent(distItem, true, OnObjectiveTerminalSpawned);

            Console.WriteLine("OnPreSetupCalled");
        }

        public override void OnElevatorArrive()
        {
            Console.WriteLine("Elevator Arrive");
        }

        public override void OnBuildDone()
        {
            Console.WriteLine("Build Done");
        }

        public void OnObjectiveTerminalSpawned(GameObject GO)
        {
            var terminal = GO.GetComponentInChildren<LG_ComputerTerminal>();
            if(terminal == null)
            {
                return;
            }

            RegisterObjectiveItemForCollection(terminal.Cast<iWardenObjectiveItem>());

            var ip = "1.1.1.1";
            //var ip = SerialGenerator.GetIpAddress();
            UplinkIP = ip;

            Console.WriteLine("IP: {0}", ip);

            //Register Info text
            SetObjectiveTextFragment(eWardenTextFragment.ITEM_SERIAL, terminal.ItemKey);
            SetObjectiveTextFragment(eWardenTextFragment.UPLINK_ADDRESS, ip);

            //Add Terminal Command
            TerminalUtil.AddCommand(terminal, "UPLINK_CONNECT", "ESTABLISH AN EXTERNAL UPLINK CONNECTION", UplinkCommandHandler);

            terminal.m_isWardenObjective = true;
            terminal.ObjectiveItemSolved = false;
        }

        public void UplinkCommandHandler(LG_ComputerTerminal terminal, string param1, string param2)
        {
            var cmd = terminal.m_command;
            if(param1.Equals(UplinkIP))
            {
                if(terminal.ObjectiveItemSolved)
                {
                    cmd.AddOutput("UPLINK ERROR: Uplink is already opened with ip: '" + param1 + "'.", true);
                    return;
                }

                //Create Puzzle Context
                var puzzle = ChainedPuzzleUtil.Setup<CP_UplinkPuzzleContext>(
                    this.ObjectiveData.ChainedPuzzleToActive,
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
