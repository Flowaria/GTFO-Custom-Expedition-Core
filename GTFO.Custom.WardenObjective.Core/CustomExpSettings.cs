using GameData;
using LevelGeneration;
using System;

namespace CustomExpeditions
{
    [Flags]
    public enum AllowedLayerType
    {
        All = Main | Secondary | Third,
        Main = 0b001,
        Secondary = 0b010,
        Third = 0b100
    }

    public class CustomExpSettings
    {
        public bool IsRequiresAllFilter = false;
        public AllowedLayerType AllowedLayers = AllowedLayerType.All;
        public Func<WardenObjectiveDataBlock, bool> CustomFilter;

        internal bool ShouldFire_Internal(LG_LayerType layerType, WardenObjectiveDataBlock objectiveData)
        {
            bool shouldFire = false;
            if (IsRequiresAllFilter)
            {
                shouldFire = true;
                shouldFire &= ShouldFire_LayerCheck(layerType);
                shouldFire &= ShouldFire_User(shouldFire, objectiveData);
            }
            else
            {
                shouldFire |= ShouldFire_LayerCheck(layerType);
                shouldFire |= ShouldFire_User(shouldFire, objectiveData);
            }

            return shouldFire;
        }

        private bool ShouldFire_LayerCheck(LG_LayerType layerType)
        {
            switch (layerType)
            {
                case LG_LayerType.SecondaryLayer: return AllowedLayers.HasFlag(AllowedLayerType.Secondary);
                case LG_LayerType.ThirdLayer: return AllowedLayers.HasFlag(AllowedLayerType.Third);
                default: return AllowedLayers.HasFlag(AllowedLayerType.Main);
            }
        }

        private bool ShouldFire_User(bool originalState, WardenObjectiveDataBlock objectiveData)
        {
            return CustomFilter?.Invoke(objectiveData) ?? originalState;
        }

        public static readonly CustomExpSettings ALL_LAYER = new CustomExpSettings()
        {
            AllowedLayers = AllowedLayerType.All
        };

        public static readonly CustomExpSettings MAIN_ONLY = new CustomExpSettings()
        {
            AllowedLayers = AllowedLayerType.Main
        };

        public static readonly CustomExpSettings SUB_ONLY = new CustomExpSettings()
        {
            AllowedLayers = AllowedLayerType.Secondary | AllowedLayerType.Third
        };
    }
}