using GameData;
using LevelGeneration;
using System.Collections.Generic;

namespace CustomExpeditions.GlobalHandlers.FogControlTerminals
{
    internal enum FogTerminalType
    {
        Increment,
        Decrement,
        ToMax,
        ToMin,
        ToLevel
    }

    internal class FogTerminalConfigDTO
    {
        public bool EnableListCommand = false;
        public List<FogTerminalPlacementData> Placements;
    }

    internal class FogTerminalPlacementData
    {
        public LG_LayerType Layer = LG_LayerType.MainLayer;
        public eLocalZoneIndex Zone = eLocalZoneIndex.Zone_0;
        public ZonePlacementWeights Weight;
        public FogTerminalType Type = FogTerminalType.Increment;
        public int Level = 1;
        public int AllowedInteractionCount = 1;
    }
}