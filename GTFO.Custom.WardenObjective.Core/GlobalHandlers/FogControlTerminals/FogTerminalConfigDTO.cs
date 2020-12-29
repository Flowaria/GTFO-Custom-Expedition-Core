using GameData;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.GlobalHandlers.FogControlTerminals
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
        public LG_LayerType Layer;
        public eLocalZoneIndex Zone;
        public FogTerminalType Type = FogTerminalType.Increment;
        public int Level = 1;
        public int AllowedCount = 0;
    }
}
