using CustomObjective.DoorMultipleWaves.DoorWaves;
using CustomExpeditions.HandlerBase;
using CustomExpeditions.Utils;
using CustomExpeditions;
using GameData;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = CustomExpeditions.Logger;
using CustomObjective.DoorMultipleWaves.Replication;
using CustomExpeditions.CustomReplicators;
using Player;
using CustomExpeditions.Messages;

namespace CustomObjective.DoorMultipleWaves
{
    public class DoorWaveHandler : CustomExpHandlerBase
    {
        private DoorData[] DataContext;
        

        private List<Replication.LG_DoorMultiWave> Replicators = new List<Replication.LG_DoorMultiWave>();

        public override void OnSetup()
        {
            if(ConfigUtil.TryGetLocalConfig<DoorWaveDataDTO>("DoorWaves.json", out var cfg))
            {
                foreach(var data in cfg.Datas)
                {
                    if(CurrentExpeditionTier != data.TargetExpeditionTier)
                    {
                        continue;
                    }
                    if (CurrentExpeditionIndex != data.TargetExpeditionIndex)
                    {
                        continue;
                    }
                    DataContext = data.Datas;
                }
                
            }

            if(DataContext == null)
            {
                ConfigUtil.SaveLocalConfig<DoorWaveDataDTO>("DoorWaves.json");

                Logger.Error("UNLOAD SELF!");

                UnloadSelf();
                return;
            }
        }

        public override void OnUnload()
        {
            foreach(var rep in Replicators)
            {
                rep.Clear();
            }
            Replicators.Clear();
        }

        public override void OnBuildDone()
        {
            foreach(var doorData in DataContext)
            {
                var doorZone = Builder.GetZone(doorData.DoorZoneLayer, doorData.DoorZoneIndex);
                var door = Builder.GetSpawnedDoorInZone(doorZone);

                if(door == null)
                {
                    continue;
                }

                var doorWaveManager = new LG_DoorMultiWave();
                doorWaveManager.Door = door;
                doorWaveManager.MessageData = doorData.Messages;
                RegisterUpdateEvent(doorWaveManager.OnUpdate);
                Replicators.Add(doorWaveManager);

                doorWaveManager.Setup(ReplicatorType.LevelInstance, ReplicatorCHType.GameOrderCritical, new DoorWaveState()
                {
                    WaveCount = 0,
                    PhaseStatus = PhaseType.Initialized
                });

                var initPuzzle = ChainedPuzzleUtil.SetupDoor(doorData.ChainedPuzzleToActive, door);
                initPuzzle.SolvedMessageDuration = 0.0f;
                initPuzzle.Solved = () =>
                {
                    doorWaveManager.TriggerDoor();
                };

                var doorLock = door.m_locks.Cast<LG_SecurityDoor_Locks>();
                doorLock.m_intOpenDoor.InteractionMessage = doorData.StartMessage;
                doorLock.m_intCustomMessage.m_message = doorData.LockdownMessage;
                doorLock.ChainedPuzzleToSolve = initPuzzle.Instance;

                var waveCount = doorData.WaveDatas.Length;
                doorWaveManager.WaveInfos = new WaveInfo[waveCount];
                for(int i = 0;i<waveCount;i++)
                {
                    var waveData = doorData.WaveDatas[i];
                    var waveInfo = new WaveInfo
                    {
                        RawData = waveData
                    };
                    doorWaveManager.WaveInfos[i] = waveInfo;

                    //Setup ChainedPuzzle
                    ChainedPuzzleContext puzzle = null;
                    if(ChainedPuzzleUtil.IsValidID(waveData.PuzzleID))
                    {
                        puzzle = ChainedPuzzleUtil.SetupDoor(waveData.PuzzleID, door);
                    }
                    else if(i == 0)
                    {
                        puzzle = ChainedPuzzleUtil.SetupDoor(4, door);
                        Logger.Warning("ChainedPuzzleToActive should always have ValidPuzzle ID!, Replaced with id: 4");
                    }

                    //Assign Puzzle Instance
                    if(puzzle != null)
                    {
                        puzzle.SolvedMessage = string.Empty;
                        puzzle.SolvedMessageDuration = 0.0f;
                        waveInfo.PuzzleToStart = puzzle;
                    }
                    else
                    {
                        Logger.Warning("ChainedPuzzle was null!, Wave: {0}", i + 1);
                    }

                    //Setup Verification Terminal
                    if (waveData.SearchPhaseType == SearchType.SearchTerminal
                        && Builder.TryGetZone(waveData.TerminalZoneLayer, waveData.TerminalZoneIndex, out var terminalZone))
                    {
                        var terminal = Builder.GetSpawnedTerminalInZone(terminalZone, waveData.TerminalPickMode);

                        if (terminal == null)
                        {
                            Logger.Warning("Unable to find Terminal Instance for Search Phase!, Wave: {0}", i+1);
                            continue;
                        }

                        //Add Local Log
                        waveInfo.VerifyTerminal = terminal;
                        waveInfo.ValidFileName = waveData.GeneratedFileName;
                        terminal.AddLocalLog(new TerminalLogFileData()
                        {
                            FileName = waveData.GeneratedFileName,
                            FileContent = waveData.GeneratedFileContent
                        });

                        //Add Controller Command
                        TerminalUtil.AddCommand(terminal, doorData.PushKeyCommand, doorData.PushKeyCommandDesc, (eTerm, arg1, arg2) =>
                        {
                            DoorCommandHandler(doorWaveManager, doorData.PushKeyCommand, eTerm, arg1, arg2);
                        });

                        //Has Valid ChainedPuzzle Setting for Terminal
                        var verifyPuzzleID = waveData.TerminalChainedPuzzleToVerify;
                        if (ChainedPuzzleUtil.TryGetBlock(verifyPuzzleID, out var block))
                        {
                            var terminalPuzzle = ChainedPuzzleUtil.SetupTerminal(block, terminal);
                            waveInfo.PuzzleToVerify = terminalPuzzle;
                        }
                    }
                }
            }
        }

        public void DoorCommandHandler(LG_DoorMultiWave manager, string CmdText, LG_ComputerTerminal terminal, string param1, string param2)
        {
            var cmd = terminal.m_command;
            var info = manager.CurrentWaveInfo;

            //Parameter Count Filter
            if (string.IsNullOrEmpty(param1))
            {
                cmd.AddOutput("Argument length mismatching!", true);
                cmd.AddOutput($"Command Usage: {CmdText} -File <FileName>", true);
                return;
            }

            //Parameter Filter
            if(!param1.ToLower().Equals("-file"))
            {
                cmd.AddOutput($"There is no such argument named: '{param1.ToUpper()}'", true);
                cmd.AddOutput($"Command Usage: {CmdText} -File <FileName>", true);
                return;
            }

            if(string.IsNullOrEmpty(param2))
            {
                cmd.AddOutput("FileName Argument is missing!", true);
                cmd.AddOutput($"Command Usage: {CmdText} -File <FileName>", true);
                return;
            }

            //FileName Filter
            if(!terminal.GetLocalLogs().ContainsKey(param2.ToUpper()))
            {
                cmd.AddOutput($"There is no such file named: '{param2.ToUpper()}'", true);
                return;
            }

            cmd.AddOutput("", true);
            cmd.AddOutput(TerminalLineType.SpinningWaitNoDone, "Opening connection towards Master Door Controller", 4f);

            if (manager.State.PhaseStatus != PhaseType.Searching)
            {
                cmd.AddOutput("", true);
                cmd.AddOutput(TerminalLineType.Warning, "ACCESS DENIED :: Push URL is closed from Door Controller. Try again when session is opened!", 0.0f);
                return;
            }

            if (!info.IsSameTerminal(terminal))
            {
                cmd.AddOutput("", true);
                cmd.AddOutput(TerminalLineType.Warning, "ACCESS DENIED :: Push URL is blocked from Door Controller. Try again with other allowed Terminal.", 0.0f);
                return;
            }

            cmd.AddOutput(TerminalLineType.ProgressWait, "Pushing Key file content", 3.5f);
            cmd.AddOutput("", true);
            cmd.AddOutput(TerminalLineType.SpinningWaitNoDone, "Waiting for Controllers respond", 2f);

            if (!info.ValidFileName.ToUpper().Equals(param2.ToUpper()))
            {
                cmd.AddOutput(TerminalLineType.Warning, "ACCESS DENIED :: Given Key file was not correct!");
                return;
            }

            cmd.AddOutput(TerminalLineType.ProgressWait, "Parsing the Controllers respond", 1.0f);
            cmd.AddOutput("", true);
            if (info.PuzzleToVerify != null && !info.PuzzleToVerify.IsSolved)
            {
                var puzzle = info.PuzzleToVerify;
                if (puzzle.Instance.Data.TriggerAlarmOnActivate)
                {
                    cmd.AddOutput(TerminalLineType.Warning, $"Security Scan with [{puzzle.Instance.Data.PublicAlarmName}] is required!");
                }
                else
                {
                    cmd.AddOutput("Security Scan is required!", true);
                }
                cmd.AddOutput("", true);
                cmd.OnEndOfQueue = new Action(() =>
                {
                    puzzle.Trigger();
                });

                puzzle.Solved = () =>
                {
                    if(manager.TryJumpToNextWave())
                    {
                        cmd.AddOutput("SUCCESS! Master Door's Security Scan will be resume in few moment...");
                    }
                    else
                    {
                        cmd.AddOutput(TerminalLineType.Warning, "FAILED! Master Door's Connection has closed due to timeout! Please Try Again!");
                    }
                };
            }
            else
            {
                cmd.OnEndOfQueue = new Action(() =>
                {
                    if (manager.TryJumpToNextWave())
                    {
                        cmd.AddOutput("SUCCESS! Master Door's Security Scan will be resume in few moment...");
                    }
                    else
                    {
                        cmd.AddOutput(TerminalLineType.Warning, "FAILED! Master Door's Connection has closed due to timeout! Please Try Again!");
                    }
                });
            }
        }
    }
}
