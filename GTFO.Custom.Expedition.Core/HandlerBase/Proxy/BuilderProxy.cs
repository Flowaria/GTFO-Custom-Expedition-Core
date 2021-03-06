﻿using AIGraph;
using GameData;
using CustomExpeditions.Extensions;
using CustomExpeditions.Utils;
using LevelGeneration;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

namespace CustomExpeditions.HandlerBase
{
    using PlacementDataList = Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<ZonePlacementData>>;

    internal class TerminalPlacementInfo
    {
        public LG_ComputerTerminal Terminal;
        public float DistanceFromZoneGate;
        public int AreaIndex;
    }

    public enum BuilderPickMode
    {
        Random,
        First,
        Last,
        StartToEnd
    }

    public enum BuilderSelectMode
    {
        Start,
        Middle,
        End,
        Random
    }

    public class BuilderProxy
    {
        private readonly CustomExpHandlerBase Base;

        private InvalidOperationException LatePlacementException
        {
            get { return new InvalidOperationException("You cannot call Builder Functions after Level is fully built!\nUse it before OnBuildDone() called!"); }
        }

        private InvalidOperationException EarlyFetchException
        {
            get { return new InvalidOperationException("You cannot call Fetch Functions before Level is fully built!\nUse it after OnBuildDone() called!"); }
        }

        internal BuilderProxy(CustomExpHandlerBase b)
        {
            Base = b;
        }

        #region Layer & Zone Gather
        

        //
        //
        // Current
        public LG_Floor CurrentFloor
        {
            get => Builder.CurrentFloor;
        }

        public LG_Layer CurrentLayer
        {
            get => Base.Layer;
        }

        public LG_Floor GetCurrentFloor()
        {
            return CurrentFloor;
        }

        public LG_Layer GetCurrentLayer()
        {
            return CurrentLayer;
        }


        //
        //
        // Starting Area
        public LG_Zone StartingZone
        {
            get => Builder.GetElevatorZone();
        }

        public LG_Area StartingArea
        {
            get => Builder.GetElevatorArea();
        }

        public LG_Zone GetStartingZone()
        {
            return StartingZone;
        }

        public LG_Area GetStartingArea()
        {
            return StartingArea;
        }
        
        
        //
        //
        // Get Layer
        public bool TryGetLayer(out LG_Layer layer)
        {
            return TryGetLayer(Base.LayerType, out layer);
        }

        public bool TryGetLayer(LG_LayerType layerType, out LG_Layer layer)
        {
            return CurrentFloor.TryGetLayer(layerType, out layer);
        }

        public LG_Layer GetLayer()
        {
            return Base.Layer;
        }

        public LG_Layer GetLayer(LG_LayerType layerType)
        {
            var exist = CurrentFloor.TryGetLayer(layerType, out var layer);
            return exist ? layer : null;
        }


        //
        //
        // Get All Layer
        public bool TryGetAllLayers(out LG_Layer[] layers)
        {
            layers = GetAllLayers();
            return layers != null;
        }

        public LG_Layer[] GetAllLayers()
        {
            return CurrentFloor.m_layers?.ToMonoArray() ?? null;
        }


        //
        //
        // Get Zone
        public bool TryGetZone(eLocalZoneIndex zoneIndex, out LG_Zone zone)
        {
            return TryGetZone(Base.LayerType, zoneIndex, out zone);
        }

        public bool TryGetZone(LG_LayerType layerType, eLocalZoneIndex zoneIndex, out LG_Zone zone)
        {
            return CurrentFloor.TryGetZoneByLocalIndex(layerType, zoneIndex, out zone);
        }

        public LG_Zone GetZone(eLocalZoneIndex zoneIndex)
        {
            return GetZone(CurrentLayer, zoneIndex);
        }

        public LG_Zone GetZone(LG_Layer layer, eLocalZoneIndex zoneIndex)
        {
            return GetZone(layer.m_type, zoneIndex);   
        }

        public LG_Zone GetZone(LG_LayerType layerType, eLocalZoneIndex zoneIndex)
        {
            var exist = CurrentFloor.TryGetZoneByLocalIndex(layerType, zoneIndex, out var zone);
            return exist ? zone : null;
        }


        //
        //
        // Get All Zone in Floor
        public bool TryGetAllZonesInFloor(out LG_Zone[] zones)
        {
            zones = GetAllZonesInFloor();
            return zones != null;
        }

        public LG_Zone[] GetAllZonesInFloor()
        {
            return CurrentFloor?.allZones?.ToMonoArray() ?? null;
        }


        //
        //
        // Get All Zone in Layer
        public bool TryGetAllZonesInLayer(out LG_Zone[] zones)
        {
            return TryGetAllZonesInLayer(CurrentLayer, out zones);
        }

        public bool TryGetAllZonesInLayer(LG_LayerType layerType, out LG_Zone[] zones)
        {
            return TryGetAllZonesInLayer(GetLayer(layerType), out zones);
        }

        public bool TryGetAllZonesInLayer(LG_Layer layer, out LG_Zone[] zones)
        {
            zones = GetAllZonesInLayer(layer);
            return zones != null;
        }

        public LG_Zone[] GetAllZonesInLayer()
        {
            return GetAllZonesInLayer(CurrentLayer);
        }

        public LG_Zone[] GetAllZonesInLayer(LG_LayerType layerType)
        {
            return GetAllZonesInLayer(GetLayer(layerType));
        }

        public LG_Zone[] GetAllZonesInLayer(LG_Layer layer)
        {
            return layer?.m_zones?.ToMonoArray() ?? null;
        }

        #endregion Layer & Zone Gather

        #region ZonePicker

        public ZonePlacementData[] PickPlacementsStandard(PlacementDataList datas, int count = 1)
        {
            return PickPlacements(datas, BuilderPickMode.StartToEnd, BuilderPickMode.Random, count);
        }

        public ZonePlacementData[] PickPlacementsStandard(ZonePlacementData[][] datas, int count = 1)
        {
            return PickPlacements(datas, BuilderPickMode.StartToEnd, BuilderPickMode.Random, count);
        }

        public ZonePlacementData[] PickPlacements(PlacementDataList datas, BuilderPickMode mode1D = BuilderPickMode.StartToEnd, BuilderPickMode mode2D = BuilderPickMode.Random, int count = 1)
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

        public ZonePlacementData[] PickPlacements(ZonePlacementData[][] datas, BuilderPickMode mode1D = BuilderPickMode.StartToEnd, BuilderPickMode mode2D = BuilderPickMode.StartToEnd, int count = 1)
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

        private int PickItemIndex(BuilderPickMode mode, int length, int offset)
        {
            switch (mode)
            {
                case BuilderPickMode.StartToEnd:
                    return offset % length;

                case BuilderPickMode.First:
                    return 0;

                case BuilderPickMode.Last:
                    return length - 1;

                case BuilderPickMode.Random:
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

            ItemUtil.RegisterItem(distItem, onSpawned);
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

        public LG_ComputerTerminal GetSpawnedTerminalInZone(LG_Zone zone, BuilderSelectMode mode)
        {
            if (!IsBuildDone())
                throw EarlyFetchException;

            var terminals = GetSpawnedTerminalsInZone(zone);
            if (terminals == null)
                return null;

            if (terminals.Length <= 0)
                return null;

            if (terminals.Length <= 1)
                return terminals[0];

            if (mode == BuilderSelectMode.Random)
                return RandomUtil.PickFromArray(terminals);

            var termInfoList = new List<TerminalPlacementInfo>();
            foreach (var terminal in terminals)
            {
                var c = terminal.SpawnNode.m_area.m_navInfo.Suffix.ToCharArray()[0];
                var areaIndex = (int)c - 65;

                var distance = -1.0f;
                var meshPath = new NavMeshPath();
                var from = terminal.SpawnNode.m_zone.m_sourceGate.m_position;
                var to = terminal.SpawnNode.Position;
                if (NavMesh.CalculatePath(from, to, -1, meshPath))
                {
                    if ((meshPath.status != NavMeshPathStatus.PathInvalid) && (meshPath.corners.Length > 1))
                    {
                        distance = 0.0f;

                        for (var i = 1; i < meshPath.corners.Length; ++i)
                        {
                            distance += Vector3.Distance(meshPath.corners[i - 1], meshPath.corners[i]);
                        }
                    }
                }

                var termInfo = new TerminalPlacementInfo()
                {
                    Terminal = terminal,
                    AreaIndex = areaIndex,
                    DistanceFromZoneGate = distance
                };
                Logger.Verbose($"Term: {termInfo.Terminal.m_terminalItem.TerminalItemKey}, Area: {termInfo.AreaIndex}, Distance: {termInfo.DistanceFromZoneGate}");
                termInfoList.Add(termInfo);
            }

            termInfoList.OrderBy(x=>x.AreaIndex).ThenBy(x=>x.DistanceFromZoneGate);
            switch(mode)
            {
                case BuilderSelectMode.Start:
                    return termInfoList.First().Terminal;

                case BuilderSelectMode.Middle:
                    var index = termInfoList.Count / 2;
                    return termInfoList[index].Terminal;

                default:
                case BuilderSelectMode.End:
                    return termInfoList.Last().Terminal;
            }
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