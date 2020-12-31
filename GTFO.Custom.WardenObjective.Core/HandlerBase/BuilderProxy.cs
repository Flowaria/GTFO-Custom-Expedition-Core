using AIGraph;
using GameData;
using GTFO.CustomObjectives.Extensions;
using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using System;
using UnityEngine;

namespace GTFO.CustomObjectives.HandlerBase
{
    using PlacementDataList = Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<ZonePlacementData>>;

    public enum PickMode
    {
        Random,
        First,
        Last,
        StartToEnd
    }

    public class BuilderProxy
    {
        private CustomObjectiveHandlerBase Base;

        private InvalidOperationException LatePlacementException
        {
            get { return new InvalidOperationException("You cannot call Builder Functions after Level is fully built!\nUse it before OnBuildDone() called!"); }
        }

        private InvalidOperationException EarlyFetchException
        {
            get { return new InvalidOperationException("You cannot call Fetch Functions before Level is fully built!\nUse it after OnBuildDone() called!"); }
        }

        internal BuilderProxy(CustomObjectiveHandlerBase b)
        {
            Base = b;
        }

        #region Layer & Zone Gather

        public bool TryGetLayer(out LG_Layer layer)
        {
            return TryGetLayer(Base.LayerType, out layer);
        }

        public bool TryGetLayer(LG_LayerType layerType, out LG_Layer layer)
        {
            return Builder.CurrentFloor.TryGetLayer(layerType, out layer);
        }

        public bool TryGetAllLayers(out LG_Layer[] layers)
        {
            layers = GetAllLayers();
            return layers != null;
        }

        public LG_Layer[] GetAllLayers()
        {
            return Builder.CurrentFloor.m_layers?.ToMonoArray() ?? null;
        }

        public bool TryGetZone(eLocalZoneIndex zoneIndex, out LG_Zone zone)
        {
            return TryGetZone(Base.LayerType, zoneIndex, out zone);
        }

        public bool TryGetZone(LG_LayerType layerType, eLocalZoneIndex zoneIndex, out LG_Zone zone)
        {
            return Builder.CurrentFloor.TryGetZoneByLocalIndex(layerType, zoneIndex, out zone);
        }

        public bool TryGetZones(LG_LayerType layerType, out LG_Zone[] zones)
        {
            zones = GetZones(layerType);
            return zones != null;
        }

        public LG_Zone[] GetZones(LG_LayerType layerType)
        {
            if (TryGetLayer(layerType, out var layer))
            {
                return GetZones(layer);
            }
            return null;
        }

        public LG_Zone[] GetZones(LG_Layer layer)
        {
            return layer?.m_zones?.ToMonoArray() ?? null;
        }

        public bool TryGetAllZones(out LG_Zone[] zones)
        {
            zones = GetAllZones();
            return zones != null;
        }

        public LG_Zone[] GetAllZones()
        {
            return Builder.CurrentFloor.allZones.ToMonoArray();
        }

        #endregion Layer & Zone Gather

        #region ZonePicker

        public ZonePlacementData[] PickPlacementsStandard(PlacementDataList datas, int count = 1)
        {
            return PickPlacements(datas, PickMode.StartToEnd, PickMode.Random, count);
        }

        public ZonePlacementData[] PickPlacementsStandard(ZonePlacementData[][] datas, int count = 1)
        {
            return PickPlacements(datas, PickMode.StartToEnd, PickMode.Random, count);
        }

        public ZonePlacementData[] PickPlacements(PlacementDataList datas, PickMode mode1D = PickMode.StartToEnd, PickMode mode2D = PickMode.Random, int count = 1)
        {
            var arr = new ZonePlacementData[datas.Count][];
            for (int i = 0; i < arr.Length; i++)
            {
                var dataRow = datas[i];
                arr[i] = new ZonePlacementData[dataRow.Count];
                for (int j = 0; j < dataRow.Count; j++)
                {
                    arr[i][j] = dataRow[j];
                }
            }
            return PickPlacements(arr, mode1D, mode2D, count);
        }

        public ZonePlacementData[] PickPlacements(ZonePlacementData[][] datas, PickMode mode1D = PickMode.StartToEnd, PickMode mode2D = PickMode.StartToEnd, int count = 1)
        {
            if (datas == null)
                return null;

            var result = new ZonePlacementData[count];
            var rowOffset = new int[datas.Length];

            for (int i = 0; i < count; i++)
            {
                var rowIndex = PickItemIndex(mode1D, datas.Length, i);
                var colIndex = PickItemIndex(mode2D, datas[rowIndex].Length, rowOffset[rowIndex]++);

                result[i] = datas[rowIndex][colIndex];
            }

            return result;
        }

        private int PickItemIndex(PickMode mode, int length, int offset)
        {
            switch (mode)
            {
                case PickMode.StartToEnd:
                    return offset % length;

                case PickMode.First:
                    return 0;

                case PickMode.Last:
                    return length - 1;

                case PickMode.Random:
                default:
                    return RandomUtil.Range(0, length);
            }
        }

        #endregion ZonePicker

        #region Pre-Build Placements

        public void PlaceTerminal(LG_Zone zone, ZonePlacementWeights weight, Action<LG_ComputerTerminal> onSpawned = null)
        {
            PlaceFunction(zone, weight, ExpeditionFunction.Terminal, onSpawned);
        }

        public void PlaceGenerator(LG_Zone zone, ZonePlacementWeights weight, Action<LG_PowerGenerator_Core> onSpawned = null)
        {
            PlaceFunction(zone, weight, ExpeditionFunction.PowerGenerator, onSpawned);
        }

        public void PlaceGeneratorCluster(LG_Zone zone, ZonePlacementWeights weight, Action<LG_PowerGeneratorCluster> onSpawned = null)
        {
            PlaceFunction(zone, weight, ExpeditionFunction.GeneratorCluster, onSpawned);
        }

        public void PlaceHSU(LG_Zone zone, ZonePlacementWeights weight, Action<LG_HSU> onSpawned = null)
        {
            PlaceFunction(zone, weight, ExpeditionFunction.HydroStatisUnit, onSpawned);
        }

        public void PlaceDisinfectStation(LG_Zone zone, ZonePlacementWeights weight, Action<LG_DisinfectionStation> onSpawned = null)
        {
            PlaceFunction(zone, weight, ExpeditionFunction.DisinfectionStation, onSpawned);
        }

        public void PlaceBulkheadController(LG_Zone zone, ZonePlacementWeights weight, Action<LG_BulkheadDoorController_Core> onSpawned = null)
        {
            PlaceFunction(zone, weight, ExpeditionFunction.BulkheadDoorController, onSpawned);
        }

        public void PlaceFunction(LG_Zone zone, ZonePlacementWeights weight, ExpeditionFunction function, Action<GameObject> onSpawned = null)
        {
            if (IsBuildDone())
                throw LatePlacementException;

            PlacementUtil.PushFunctionMarker(zone, weight, function, out var distItem, out var distNode);

            if (onSpawned != null)
            {
                ListenItemSpawnEvent(distItem, false, onSpawned);
            }
        }

        public void PlaceFunction<C>(LG_Zone zone, ZonePlacementWeights weight, ExpeditionFunction function, Action<C> onSpawned = null) where C : MonoBehaviour
        {
            if (IsBuildDone())
                throw LatePlacementException;

            PlacementUtil.PushFunctionMarker(zone, weight, function, out var distItem, out var distNode);

            if (onSpawned != null)
            {
                ListenItemSpawnEvent(distItem, false, (gameObject) =>
                {
                    var comp = gameObject.GetComponentInChildren<C>();
                    if (comp != null)
                    {
                        onSpawned(comp);
                    }
                });
            }
        }

        public void FetchTerminal(LG_Zone zone, ZonePlacementWeights weight, out LG_DistributeItem distItem, out AIG_CourseNode distNode, Action<LG_ComputerTerminal> onSpawned = null)
        {
            FetchFunction(zone, weight, ExpeditionFunction.Terminal, out distItem, out distNode, onSpawned);
        }

        public void FetchFunction<C>(LG_Zone zone, ZonePlacementWeights weight, ExpeditionFunction function, out LG_DistributeItem distItem, out AIG_CourseNode distNode, Action<C> onSpawned = null) where C : MonoBehaviour
        {
            if (IsBuildDone())
                throw LatePlacementException;

            if (PlacementUtil.TryFetchFunctionMarker(zone, weight, function, out distItem, out distNode, false))
            {
                if (onSpawned == null)
                    return;

                ListenItemSpawnEvent(distItem, false, (gameObject) =>
                {
                    var comp = gameObject.GetComponentInChildren<C>();
                    if (comp != null)
                    {
                        onSpawned(comp);
                    }
                });
            }
        }

        /// <summary>
        /// Listen to the actual gameObject spawn event for LG_DistributeItem
        /// </summary>
        /// <param name="distItem">Target LG_DistributeItem</param>
        /// <param name="isWardenObjectiveItem">Is the warden objective item? (needed for reason)</param>
        /// <param name="onSpawned">delegate for spawned event</param>
        public void ListenItemSpawnEvent(LG_DistributeItem distItem, bool isWardenObjectiveItem, Action<GameObject> onSpawned)
        {
            if (IsBuildDone())
                throw LatePlacementException;

            ItemUtil.RegisterItem(distItem, isWardenObjectiveItem, onSpawned);
        }

        #endregion Pre-Build Placements

        #region Post-Build Get

        public LG_SecurityDoor GetSpawnedDoorInZone(LG_Zone zone)
        {
            if (!IsBuildDone())
                throw EarlyFetchException;

            return zone?.m_sourceGate?.SpawnedDoor?.Cast<LG_SecurityDoor>() ?? null;
        }

        /// <summary>
        /// Get Placed Terminal in Zone
        /// </summary>
        /// <param name="zone">Zone to search</param>
        /// <param name="index">Index of Terminal, Pick random if it's below zero</param>
        /// <returns></returns>
        public LG_ComputerTerminal GetSpawnedTerminalInZone(LG_Zone zone, int index = -1)
        {
            if (!IsBuildDone())
                throw EarlyFetchException;

            var terminals = zone?.TerminalsSpawnedInZone ?? null;
            if (terminals != null)
            {
                if (index < 0)
                {
                    return RandomUtil.PickFromList(terminals);
                }
                else
                {
                    return terminals.Count > index ? terminals[index] : null;
                }
            }

            return null;
        }

        public LG_ComputerTerminal[] GetSpawnedTerminalsInZone(LG_Zone zone)
        {
            if (!IsBuildDone())
                throw EarlyFetchException;

            return zone?.TerminalsSpawnedInZone?.ToMonoArray() ?? null;
        }

        #endregion Post-Build Get

        private bool IsBuildDone()
        {
            return Base.IsBuildDone;
        }
    }
}