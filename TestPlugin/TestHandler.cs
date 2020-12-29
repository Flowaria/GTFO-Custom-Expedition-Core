using GameData;
using GTFO.CustomObjectives.HandlerBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    class TestHandler : CustomObjectiveHandlerBase
    {
        public override void OnSetup()
        {
            Builder.TryGetZone(eLocalZoneIndex.Zone_0, out var zone);

            for(int i = 0;i<30;i++)
            {
                Builder.PlaceTerminal(zone, new ZonePlacementWeights() { Start = 0.0f, Middle = 0.0f, End = 0.0f });
            }
        }
    }
}
