using GameData;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives
{
    [Flags]
    public enum AllowedLayerType
    {
        All = Main | Secondary | Third,
        Main = 1,
        Secondary = 2,
        Third = 4
    }

    public class CustomObjectiveSettings
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
                shouldFire &= ShouldFire_User(objectiveData);
            }
            else
            {
                shouldFire |= ShouldFire_LayerCheck(layerType);
                shouldFire |= ShouldFire_User(objectiveData);
            }

            return shouldFire;
        }

        private bool ShouldFire_LayerCheck(LG_LayerType layerType)
        {
            switch(layerType)
            {
                case LG_LayerType.SecondaryLayer: return AllowedLayers.HasFlag(AllowedLayerType.Secondary);
                case LG_LayerType.ThirdLayer: return AllowedLayers.HasFlag(AllowedLayerType.Third);
                default: return AllowedLayers.HasFlag(AllowedLayerType.Main);
            }
        }

        private bool ShouldFire_User(WardenObjectiveDataBlock objectiveData)
        {
            return CustomFilter?.Invoke(objectiveData) ?? true;
        }

        public static CustomObjectiveSettings ALL_LAYER = new CustomObjectiveSettings()
        {
            AllowedLayers = AllowedLayerType.All
        };

        public static CustomObjectiveSettings MAIN_ONLY = new CustomObjectiveSettings()
        {
            AllowedLayers = AllowedLayerType.Main
        };

        public static CustomObjectiveSettings SUB_ONLY = new CustomObjectiveSettings()
        {
            AllowedLayers = AllowedLayerType.Secondary | AllowedLayerType.Third
        };
    }
}
