using AIGraph;
using CustomExpeditions.HandlerBase;
using GameData;
using LevelGeneration;
using System;

namespace CustomExpeditions.Utils
{
    public static class PlacementUtil
    {
        public static bool TryGetRandomPlaceSingleZone(CustomExpHandlerBase handlerContext, out LG_Zone zone, out ZonePlacementWeights weight)
        {
            return LG_DistributionJobUtils.TryGetRandomPlacementZone(handlerContext.Layer, handlerContext.LayerData.ObjectiveData.ZonePlacementDatas, out zone, out weight);
        }

        public static bool TryFetchFunctionMarker(LG_Zone zone, ZonePlacementWeights weight, ExpeditionFunction function, out LG_DistributeItem distItem, out AIG_CourseNode distNode, bool createNew = true)
        {
            float rand = RandomUtil.Value("LG_Distribute_WardenObjective");
            var exist = LG_DistributionJobUtils.TryGetExistingZoneFunctionDistribution(zone, function, rand, weight, out distItem, out distNode);
            if (!exist && createNew)
            {
                PushFunctionMarker(zone, weight, function, out distItem, out distNode);
                return true;
            }

            return exist;
        }

        public static bool TryFetchFunctionMarker(LG_Zone zone, ZonePlacementWeights weight, ExpeditionFunction function, out LG_DistributeItem distItem)
        {
            return TryFetchFunctionMarker(zone, weight, function, out distItem, out _);
        }

        public static void PushFunctionMarker(LG_Zone zone, ZonePlacementWeights weight, ExpeditionFunction function, out LG_DistributeItem distItem, out AIG_CourseNode distNode)
        {
            var randNode = LG_DistributionJobUtils.GetRandomNodeFromZoneForFunction(zone, function, Builder.BuildSeedRandom.Value("FindFunctionMarker"), 1f);
            var newDistItem = new LG_DistributeItem()
            {
                m_function = function,
                m_amount = 1.0f,
                m_assignedNode = randNode
            };

            randNode.m_zone.DistributionData.GenericFunctionItems.Enqueue(newDistItem);

            distItem = newDistItem;
            distNode = randNode;
        }

        public static LG_DistributeItem PushFunctionMarker(LG_Zone zone, ZonePlacementWeights weight, ExpeditionFunction function)
        {
            PushFunctionMarker(zone, weight, function, out var distItem, out _);
            return distItem;
        }

        public static LG_Zone GetZone(LG_LayerType layer, eLocalZoneIndex index)
        {
            if (Builder.Current.m_currentFloor.TryGetZoneByLocalIndex(layer, index, out var zone))
            {
                return zone;
            }

            return null;
        }

        public static bool TryGetZone(LG_LayerType layer, eLocalZoneIndex index, out LG_Zone zone)
        {
            return Builder.Current.m_currentFloor.TryGetZoneByLocalIndex(layer, index, out zone);
        }

        public static LG_SecurityDoor GetSpawnedDoor(LG_Zone zone)
        {
            return zone.m_sourceGate.SpawnedDoor?.Cast<LG_SecurityDoor>();
        }

        public static bool TryGetSpawnedDoor(LG_Zone zone, out LG_SecurityDoor door)
        {
            door = zone.m_sourceGate?.SpawnedDoor?.Cast<LG_SecurityDoor>() ?? null;
            return door != null;
        }

        public static LG_ComputerTerminal GetRandomTerminal(LG_Zone zone)
        {
            return RandomUtil.PickFromList(zone.TerminalsSpawnedInZone);
        }

        public static bool TryGetRandomTerminal(LG_Zone zone, out LG_ComputerTerminal terminal)
        {
            terminal = RandomUtil.PickFromList(zone.TerminalsSpawnedInZone);
            return terminal != null;
        }

        public static void DistributePickupItems()
        {
            throw new NotImplementedException();
        }
    }
}