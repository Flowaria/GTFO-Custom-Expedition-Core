using GTFO.CustomObjectives.Utils;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;

namespace GTFO.CustomObjectives.HandlerBase
{
    public class WinConditionProxy
    {
        private CustomObjectiveHandlerBase Base;
        internal WinConditionProxy(CustomObjectiveHandlerBase b) { Base = b; }

        public ObjectiveItem CreateEmptyObjectiveItem()
        {
            var item = new ObjectiveItem(Base.LayerType);

            RegisterObjectiveItemForCollection(item.ItemCast);

            return item;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public void RegisterObjectiveItem(iWardenObjectiveItem item)
        {
            WardenObjectiveManager.RegisterObjectiveItem(Base.LayerType, item);
        }

        public void RegisterObjectiveItemForCollection(iWardenObjectiveItem item)
        {
            WardenObjectiveManager.RegisterObjectiveItemForCollection(Base.LayerType, item);
        }

        public void RegisterRequiredScanItem(iWardenObjectiveItem item)
        {
            var array = new Il2CppReferenceArray<iWardenObjectiveItem>(1);
            array[0] = item;

            ElevatorShaftLanding.Current.AddRequiredScanItems(array);
        }
    }
}
