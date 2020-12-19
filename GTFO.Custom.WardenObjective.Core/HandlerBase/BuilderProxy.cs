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

        internal BuilderProxy(CustomObjectiveHandlerBase b) { Base = b; }

        #region Pre-Build Placements

        public void PlaceTerminal(LG_Zone zone, Action<LG_ComputerTerminal> onSpawned = null)
        {
            PlaceFunction(ExpeditionFunction.Terminal, onSpawned);
        }

        public void PlaceGenerator(LG_Zone zone, Action<LG_PowerGenerator_Core> onSpawned = null)
        {
            PlaceFunction(ExpeditionFunction.PowerGenerator, onSpawned);
        }

        public void PlaceGeneratorCluster(LG_Zone zone, Action<LG_PowerGeneratorCluster> onSpawned = null)
        {
            PlaceFunction(ExpeditionFunction.GeneratorCluster, onSpawned);
        }

        public void PlaceHSU(LG_Zone zone, Action<LG_HSU> onSpawned = null)
        {
            PlaceFunction(ExpeditionFunction.HydroStatisUnit, onSpawned);
        }

        public void PlaceDisinfectStation(LG_Zone zone, Action<LG_DisinfectionStation> onSpawned = null)
        {
            PlaceFunction(ExpeditionFunction.DisinfectionStation, onSpawned);
        }

        public void PlaceBulkheadController(LG_Zone zone, Action<LG_BulkheadDoorController_Core> onSpawned = null)
        {
            PlaceFunction(ExpeditionFunction.BulkheadDoorController, onSpawned);
        }

        public void PlaceFunction(ExpeditionFunction function, Action<GameObject> onSpawned = null)
        {
            if (IsBuildDone())
                throw LatePlacementException;

            if(onSpawned != null)
            {
                ListenItemSpawnEvent(new LG_DistributeItem(), false, onSpawned);
            }
        }

        private void PlaceFunction<C>(ExpeditionFunction function, Action<C> onSpawned = null) where C : MonoBehaviour
        {
            if (IsBuildDone())
                throw LatePlacementException;

            if (onSpawned != null)
            {
                ListenItemSpawnEvent(new LG_DistributeItem(), false, (gameObject) =>
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
            return zone?.m_sourceGate?.SpawnedDoor?.Cast<LG_SecurityDoor>() ?? null;
        }

        public LG_ComputerTerminal GetSpawnedTerminalInZone(LG_Zone zone, int index = -1)
        {
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
            ItemUtil.RegisterItem(distItem, isWardenObjectiveItem, onSpawned);
        }

        private bool IsBuildDone()
        {
            return Base.IsBuildDone;
        }
    }
}
