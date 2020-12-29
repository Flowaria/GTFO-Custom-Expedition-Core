using GameData;
using GTFO.CustomObjectives.Extensions;
using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO.CustomObjectives.HandlerBase
{
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

        public bool TryGetZone(eLocalZoneIndex zoneIndex, out LG_Zone zone)
        {
            return TryGetZone(Base.LayerType, zoneIndex, out zone);
        }

        public bool TryGetZone(LG_LayerType layerType, eLocalZoneIndex zoneIndex, out LG_Zone zone)
        {
            return Builder.CurrentFloor.TryGetZoneByLocalIndex(layerType, zoneIndex, out zone);
        }

        #endregion

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

        #endregion

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
            if(terminals != null)
            {
                if(index < 0)
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

        #endregion

        /// <summary>
        /// Listen to the actual gameObject spawn event for LG_DistributeItem
        /// </summary>
        /// <param name="distItem">Target LG_DistributeItem</param>
        /// <param name="isWardenObjectiveItem">Is the warden objective item? (needed for reason)</param>
        /// <param name="onSpawned">delegate for spawned event</param>
        public void ListenItemSpawnEvent(LG_DistributeItem distItem, bool isWardenObjectiveItem, Action<GameObject> onSpawned)
        {
            if (!IsBuildDone())
                throw EarlyFetchException;

            ItemUtil.RegisterItem(distItem, isWardenObjectiveItem, onSpawned);
        }

        private bool IsBuildDone()
        {
            return Base.IsBuildDone;
        }
    }
}
