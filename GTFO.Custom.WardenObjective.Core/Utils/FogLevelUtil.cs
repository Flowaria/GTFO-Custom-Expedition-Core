using GameData;
using GTFO.CustomObjectives.Inject.Global;
using System.Collections.Generic;

namespace GTFO.CustomObjectives.Utils
{
    public class FogLevelSetting
    {
        public uint FogSetting;
        public float TransitionTime;
    }

    public static class FogLevelUtil
    {
        private static float _EndTime = 0.0f;
        private readonly static List<FogLevelSetting> _FogSettings;

        public static int CurrentLevel { get; private set; }
        public static int LevelCount { get; private set; }

        static FogLevelUtil()
        {
            _FogSettings = new List<FogLevelSetting>();

            GlobalMessage.OnBuildDone += OnUpdate;
            GlobalMessage.OnLevelCleanup += () =>
            {
                GlobalMessage.OnUpdate -= OnUpdate;
            };
        }

        private static void OnUpdate()
        {
            return; //TODO: Wip

            var remainingTime = _EndTime - Clock.TimeInSessionHub;
            var percent = 1.0f - (remainingTime / _FogSettings[CurrentLevel].TransitionTime);

            LocalPlayerAgentSettings.Current.UpdateBlendTowardsTargetFogSetting(percent, true);

            if (percent >= 1.0f)
            {
            }
        }

        public static void Setup()
        {
        }

        public static void SetLevel(int level)
        {
            if (!(0 <= level && level < LevelCount))
            {
                return;
            }

            CurrentLevel = level;
            _EndTime = Clock.TimeInSessionHub + _FogSettings[CurrentLevel].TransitionTime;

            var block = GameDataBlockBase<FogSettingsDataBlock>.GetBlock(_FogSettings[CurrentLevel].FogSetting);
            LocalPlayerAgentSettings.Current.SetTargetFogSettings(block);
        }
    }
}