using CustomObjective.DoorMultipleWaves.DoorWaves;
using CustomObjective.DoorMultipleWaves.PuzzleContext;
using CustomExpeditions.HandlerBase;
using CustomExpeditions.Utils;
using CustomExpeditions.Utils.ChainedPuzzle;
using GameData;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomObjective.DoorMultipleWaves
{
    public class MainHandler : CustomExpHandlerBase
    {
        private DoorWaveManager doorWaveManager;

        public override void OnSetup()
        {
            DoorWaveManager.Setup();

            doorWaveManager = DoorWaveManager.Current;

            int count = ObjectiveData.ReactorWaves.Count;
            doorWaveManager.Waves = new DoorWaveData[count];
            //doorWaveManager.MainZone = ObjectiveLayerData.ZonePlacementDatas[0][0].LocalIndex;

            for (int i = 0; i < count; i++)
            {
                var ReactWaveData = ObjectiveData.ReactorWaves[i];

                doorWaveManager.Waves[i] = new DoorWaveData()
                {
                    PuzzleID = ReactWaveData.EnemyWaves[0].WaveSettings,
                    EventData = ReactWaveData.Events.ToArray(),

                    Time_Init = ReactWaveData.Warmup,
                    Time_Search = ReactWaveData.Verify,

                    Time_FailInit = ReactWaveData.WarmupFail,
                    Time_FailSearch = ReactWaveData.VerifyFail,

                    GeneratedFileName = $"autogen_{SerialGenerator.GetCodeWordPrefix()}.key",
                    GeneratedFileContent = $"//Generated Key file for BlockCipher:\n\n{GetBase64(GetBase64(GetBase64(SomeBigLongText())))}",
                    TerminalZone = ReactWaveData.ZoneForVerification
                };
            }

            RegisterUpdateEvent(doorWaveManager.OnUpdate);
        }

        public override void OnBuildDone()
        {
            int count = doorWaveManager.Waves.Length;

            var doorZone = PlacementUtil.GetZone(LayerType, doorWaveManager.MainZone);
            doorWaveManager.ChainedDoor = PlacementUtil.GetSpawnedDoor(doorZone);
            //TODO: FIX doorWaveManager.ChainedDoor.m_locks.Cast<LG_SecurityDoor_Locks>();

            for (int i = 0; i < count; i++)
            {
                var waveData = doorWaveManager.Waves[i];

                if(!Builder.TryGetZone(waveData.TerminalZone, out var zone))
                {
                    continue;
                }

                var terminal = Builder.GetSpawnedTerminalInZone(zone);
                if (terminal != null)
                {
                    waveData.Terminal = terminal;

                    terminal.AddLocalLog(new TerminalLogFileData()
                    {
                        FileName = waveData.GeneratedFileName,
                        FileContent = waveData.GeneratedFileContent
                    });

                    TerminalUtil.AddCommand(terminal, "PUSH_KEY", "Push public key to Master Door Controller.", DoorCommandHandler);
                }
            }

            if (SNet.IsMaster)
                DoorWaveManager.Current.AttemptInteract(DoorWaveInteractionType.Startup);
        }

        public void DoorCommandHandler(LG_ComputerTerminal terminal, string param1, string param2)
        {
            var cmd = terminal.m_command;
            if (string.IsNullOrEmpty(param1))
            {
                cmd.AddOutput("Argument length mismatching!", true);
                cmd.AddOutput("Command Usage: PUSH_KEY -File <FileName>", true);
                return;
            }

            if(!param1.ToLower().Equals("-file"))
            {
                cmd.AddOutput($"There is no such argument named: '{param1.ToUpper()}'", true);
                cmd.AddOutput("Command Usage: PUSH_KEY -File <FileName>", true);
                return;
            }

            if(terminal.m_localLogs.TryGetValue(param2.ToUpper(), out var data))
            {
                cmd.AddOutput(TerminalLineType.SpinningWaitNoDone, "Opening connection towards Master Door Controller", 4f);

                if (terminal != doorWaveManager.CurrentWave.Terminal)
                {
                    cmd.AddOutput("", true);
                    cmd.AddOutput(TerminalLineType.Warning, "ACCESS DENIED :: Push URL is blocked from Door Controller. Try again with other allowed Terminal.", 0.0f);
                    return;
                }

                cmd.AddOutput(TerminalLineType.ProgressWait, "Pushing Key file content", 6.5f);
                cmd.AddOutput(TerminalLineType.SpinningWaitDone, "Waiting for Controllers respond", 3f);

                if (!doorWaveManager.CurrentWave.GeneratedFileName.ToUpper().Equals(param2.ToUpper()))
                {
                    cmd.AddOutput(TerminalLineType.Warning, "ACCESS DENIED :: Given Key file was not correct!", 1.0f);
                    return;
                }

                cmd.AddOutput(TerminalLineType.ProgressWait, "Parsing the Controllers respond", 1.0f);
                cmd.AddOutput("", true);
                if(GameDataBlockBase<ChainedPuzzleDataBlock>.HasBlock(ObjectiveData.ChainedPuzzleMidObjective))
                {
                    var puzzle = ChainedPuzzleUtil.Setup<TerminalPuzzleContext>(
                        ObjectiveData.ChainedPuzzleMidObjective,
                        terminal.SpawnNode.m_area,
                        terminal.m_wardenObjectiveSecurityScanAlign);

                    puzzle.Terminal = terminal;
                    puzzle.Handler = this;

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
                }
                else
                {
                    cmd.AddOutput("SUCCESS! Master Door's Security Scan will be resume in few moment...");
                    cmd.OnEndOfQueue = new Action(() =>
                    {
                        doorWaveManager.JumpToNextWave();
                    });
                }
            }
            else
            {
                cmd.AddOutput($"There is no such file named: '{param2.ToUpper()}'", true);
                return;
            }
        }

        public void OnPushSuccess()
        {

        }

        public string SomeBigLongText()
        {
            var builder = new StringBuilder();
            builder.Append("random code is: ");
            builder.Append(SerialGenerator.GetCodeWord());
            builder.AppendLine(", your welcome :)");

            builder.AppendLine("Btw did you realized this is base64 strings huh?");

            builder.AppendLine("Congratulation you actually figured out!");

            builder.AppendLine("Now go back to the game.");

            return builder.ToString();
        }

        public string GetBase64(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }
    }
}
