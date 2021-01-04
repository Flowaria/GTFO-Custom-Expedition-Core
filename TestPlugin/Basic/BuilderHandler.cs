using CustomExpeditions;
using CustomExpeditions.HandlerBase;
using CustomExpeditions.Utils;
using GameData;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.Basic
{
    class BuilderHandler : CustomExpHandlerBase
    {
        //Pre-Build Method
        public override void OnSetup()
        {
            //Get Zone from Builder Context
            Builder.TryGetZone(eLocalZoneIndex.Zone_0, out var zone);

            //Or you can also do this!
            var placements = Builder.PickPlacementsStandard(LayerData.ObjectiveData.ZonePlacementDatas, 4);
            //Builder.PickPlacements(LayerData.ObjectiveData.ZonePlacementDatas, PickMode.Random, PickMode.Random, 4);

            foreach(var place in placements)
            {
                if(Builder.TryGetZone(place.LocalIndex, out var placementZone))
                {
                    Logger.Log("Found zone from placement data: {0}", placementZone.Alias);
                }
            }

            //Create Empty PlacementWeights Data
            var weight = new ZonePlacementWeights() { Start = 0.0f, Middle = 0.0f, End = 0.0f };

            //Spawn regular Disinfection Station
            Builder.PlaceDisinfectStation(zone, weight);

            //Place 5 Terminal but With custom logs!
            for (int i = 0; i < 5; i++)
            {
                Builder.PlaceTerminal(zone, weight, (terminal) =>
                {
                    terminal.AddLocalLog(new TerminalLogFileData()
                    {
                        FileName = "HelloWorld.log",
                        FileContent = "UwU\nCuddle me hard senpai~"
                    });
                });
            }

            //Add Sign using PlaceFunction, And Modify the Text too
            for (int i = 0;i<4;i++)
            {
                Builder.PlaceFunction<LG_Sign>(zone, weight, ExpeditionFunction.Sign, (sign)=>
                {
                    sign.m_text.text = "Poggers";
                });
            }
        }

        //Done Build Method
        public override void OnBuildDone()
        {
            //Now Calling Place/Fetch Function will cause Exception, Don't do it
            //Builder.PlaceDisinfectStation(zone, weight); : NONO

            //Unlock Random 5 Door!
            var zones = Builder.GetAllZones();
            RandomUtil.Shuffle(zones);

            for(int i = 0;i<5;i++)
            {
                Builder.GetSpawnedDoorInZone(zones[i])?.AttemptOpenCloseInteraction(true);
            }
        }
    }
}
