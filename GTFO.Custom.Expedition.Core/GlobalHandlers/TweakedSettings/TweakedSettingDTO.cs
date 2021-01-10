using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.GlobalHandlers.TweakedSettings
{
    internal class TweakedSettingDTO
    {
        public List<TargetSetting> Settings { get; }
    }

    internal class TargetSetting
    {
        public bool InAllExpeditionTier { get; } = false;
        public eRundownTier TargetExpeditionTier { get; }

        public bool InAllExpeditionIndex { get; } = false;
        public int TargetExpeditionIndex { get; }

        public List<uint> ElevatorCargoItems { get; }
        public ScoutWaveSetup ShadowScoutWaveOverride { get; }
        public List<FogRepellerSetting> FogTurbineOverrides { get; }
        public List<FogRepellerSetting> FogRepellerOverrides { get; }
    }

    internal class ScoutWaveSetup
    {
        public uint WaveSetting { get; } = 0u;
        public uint WavePopulation { get; } = 0u;
    }

    internal class FogRepellerSetting
    {
        public uint ItemID { get; } = 0u;
        public float RangeMulti { get; } = 1.0f;
        public float GrowDuration { get; } = 12.0f;
        public float ShrinkDuration { get; } = 20.0f;
        public float LifeDuration { get; } = 10.0f;
        public bool InfiniteDuration { get; } = true;
    }
}
