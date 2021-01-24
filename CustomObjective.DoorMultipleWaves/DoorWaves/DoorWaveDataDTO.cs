using CustomExpeditions.HandlerBase;
using GameData;
using LevelGeneration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomObjective.DoorMultipleWaves.DoorWaves
{
    public enum SearchType
    {
        SearchTerminal,
        IgnoreSearch
    }

    public class DoorWaveDataDTO
    {
        public ExpFilterData[] Datas = new ExpFilterData[] { new ExpFilterData() };
    }

    public class ExpFilterData
    {
        public eRundownTier TargetExpeditionTier = eRundownTier.TierA;
        public int TargetExpeditionIndex;

        public DoorData[] Datas = new DoorData[] { new DoorData() };
    }

    public class DoorData
    {
        public LG_LayerType DoorZoneLayer;
        public eLocalZoneIndex DoorZoneIndex;
        public uint ChainedPuzzleToActive = 4;
        public string StartMessage = "Start Security Scan Sequence <color=red>[WARNING: Class I Verification REQUIRED]</color>";
        public string LockdownMessage = "<color=red>//:Door in Verification Lockdown</color>";

        public string PushKeyCommand = "PUSH_KEY";
        public string PushKeyCommandDesc = "Push public key to Master Door Controller.";

        public DoorWaveHUDSettings Messages = new DoorWaveHUDSettings();
        public DoorWaveData[] WaveDatas = new DoorWaveData[] { new DoorWaveData() };
    }

    public class DoorWaveHUDSettings
    {
        public string SearchSkipPhase = "Bioscan Complete! Verification is not needed!";
        public string SearchingPhase = "Verification is needed. ([WAVE_CURRENT]/[WAVE_MAX]) Push Key-file <color=orange>[KEYFILE]</color> using <color=orange>[TERMINAL]</color> placed inside <color=orange>Z[ZONE_NUMBER]</color>.\n<color=orange>TIME LEFT: [TIMER]</color>";
        public string WaitPuzzlePhaseFirst = "Initial Bioscan will be initiate!\n<color=orange>TIME LEFT: [TIMER]</color>";
        public string WaitPuzzlePhase = "Bioscan will be continue!\n<color=orange>TIME LEFT: [TIMER]</color>";
        public string WaitPuzzlePhaseRetry = "Retrying Bioscan...\n<color=orange>TIME LEFT: [TIMER]</color>";
        public string VerifyFailedPhase = "Verification Failed! Returning to previous Bioscan...\n<color=orange>TIME LEFT: [TIMER]</color>";
        public string FullySolvedPhase = "Verification is Finished! Door Unlocked!";

        public uint WaitPuzzlePhaseSound = 1028489895u;
        public float WaitPuzzlePhaseSoundAtTime = 10.0f;

        public uint PuzzleSolvedSound = 0u;
        public uint VerifySuccessSound = 0u;
        public uint VerifyFailSound = 0u;
    }

    public class DoorWaveData
    {
        public uint PuzzleID;

        public float[] TimeUntilBioscan;
        public float[] TimeForSearchTerminal;
        public float[] TimeForRetry;

        public WardenObjectiveEventData[] EventsOnSolved;

        public string GeneratedFileName;
        public string GeneratedFileContent;

        public SearchType SearchPhaseType = SearchType.SearchTerminal;
        public LG_LayerType TerminalZoneLayer;
        public eLocalZoneIndex TerminalZoneIndex;
        public BuilderSelectMode TerminalPickMode = BuilderSelectMode.Random;
        public uint TerminalChainedPuzzleToVerify = 0;

        public float GetTimeUntilBioscan(int failCounter)
        {
            int index = Mathf.Min(Mathf.Max(failCounter, 0), TimeUntilBioscan.Length - 1);
            return TimeUntilBioscan[index];
        }

        public float GetTimeForSearchTerminal(int failCounter)
        {
            int index = Mathf.Min(Mathf.Max(failCounter, 0), TimeForSearchTerminal.Length-1);
            return TimeForSearchTerminal[index];
        }

        public float GetTimeForRetry(int failCounter)
        {
            int index = Mathf.Min(Mathf.Max(failCounter, 0), TimeForRetry.Length - 1);
            return TimeForRetry[index];
        }
    }
}
