using ChainedPuzzles;
using CustomObjectives;
using CustomObjectives.Extensions;
using CustomObjectives.HandlerBase;
using CustomObjectives.Utils;
using CustomObjectives.Utils.ChainedPuzzle;
using GameData;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.UplinkBioscan
{
    public class UplinkInfo
    {
        public UplinkPuzzleContext Puzzle;
        public string Ip;
        public bool CommandUsed;
    }

    public class UplinkBioscanHandler : CustomObjectiveHandlerBase
    {
        public Dictionary<int, UplinkInfo> TerminalInfo;

        public override void OnSetup()
        {
            TerminalInfo = new Dictionary<int, UplinkInfo>();

            var placeData = LayerData.ObjectiveData.ZonePlacementDatas;
            var terminalNumber = ObjectiveData.Uplink_NumberOfTerminals;
            var placements = Builder.PickPlacementsStandard(placeData, terminalNumber);

            foreach(var placement in placements)
            {
                Logger.Log("{0}", placement.LocalIndex);
                if(Builder.TryGetZone(placement.LocalIndex, out var zone))
                {
                    Logger.Log("{0}", zone.Alias);
                    Builder.FetchTerminal(zone, placement.Weights, out var distItem, out var distNode, (terminal) =>
                    {
                        Logger.Log("{0}", terminal.m_serialNumber);
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

            var ip = "1.1.1.1";
            //var ip = SerialGenerator.GetIpAddress();

            WinConditions.RegisterObjectiveItemForCollection(terminal.Cast<iWardenObjectiveItem>());

            //Register Info text
            ObjectiveStatus.SetObjectiveTextFragment(eWardenTextFragment.ITEM_SERIAL, terminal.ItemKey);
            ObjectiveStatus.SetObjectiveTextFragment(eWardenTextFragment.UPLINK_ADDRESS, ip);

            //Add Terminal Command
            TerminalUtil.AddCommand(terminal, "UPLINK_CONNECT", "ESTABLISH AN EXTERNAL UPLINK CONNECTION", UplinkCommandHandler);
            TerminalUtil.RegisterEnterEvent(terminal, UplinkTerminalDiscovered);

            terminal.m_isWardenObjective = false;
            terminal.ObjectiveItemSolved = false;

            //Create Puzzle Context
            var puzzle = ChainedPuzzleUtil.Setup<UplinkPuzzleContext>(ObjectiveData.ChainedPuzzleToActive, terminal);

            //Assign Context objects for OnSolved Event
            puzzle.Handler = this;
            puzzle.Terminal = terminal;
            puzzle.SolvedMessage = "Security Scan Complete. Uplink Connection has been Established.";

            TerminalInfo.Add(terminal.GetInstanceID(), new UplinkInfo() {
                Puzzle = puzzle,
                Ip = ip,
                CommandUsed = false
            });
        }

        public void UplinkTerminalDiscovered(LG_ComputerTerminal terminal)
        {
            if (!TerminalInfo.TryGetValue(terminal.GetInstanceID(), out var info))
            {
                return;
            }

            WinConditions.FoundObjectiveItem(terminal.Cast<iWardenObjectiveItem>());
            ObjectiveStatus.SetObjectiveTextFragment(eWardenTextFragment.ITEM_SERIAL, terminal.ItemKey);
            ObjectiveStatus.SetObjectiveTextFragment(eWardenTextFragment.UPLINK_ADDRESS, info.Ip);
        }

        public void UplinkCommandHandler(LG_ComputerTerminal terminal, string param1, string param2)
        {
            var cmd = terminal.m_command;

            if (!TerminalInfo.TryGetValue(terminal.GetInstanceID(), out var info))
            {
                Logger.Error("THIS TERMINAL DOESN'T HAVE ASSIGNED CHAINEDPUZZLE!");
                return;
            }

            if (param1.Equals(info.Ip))
            {
                if(info.CommandUsed)
                {
                    cmd.AddOutput($"UPLINK ERROR: Uplink is already opened with ip: '{param1}'.", true);
                    return;
                }

                info.CommandUsed = true;

                var puzzle = info.Puzzle;

                //Console Stuffs
                cmd.AddOutput(TerminalLineType.ProgressWait, $"Creating external uplink to address {param1}, please wait", 3f);
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

                cmd.SetEndOfQueue(() =>
                {
                    //Trigger Alarm, Attempt to Start ChainedPuzzle
                    puzzle.Trigger();
                });
            }
            else
            {
                cmd.AddOutput($"UPLINK ERROR: Could not connect to '{param1}'. Make sure it is a valid address for an external uplink.", true);
            }
        }
    }
}
