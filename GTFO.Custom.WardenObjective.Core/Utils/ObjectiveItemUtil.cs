using LevelGeneration;
using System;

namespace GTFO.CustomObjectives.Utils
{
    public class ObjectiveItem
    {
        public Action OnFoundEvent;
        public Action OnSolvedEvent;

        public string GUID { get; }
        public LG_LayerType Layer { get; }
        public iWardenObjectiveItem ItemCast { get; }
        private CarryItemPickup_Core Item { get; }

        public ObjectiveItem(LG_LayerType layerType)
        {
            Layer = layerType;
            Item = new CarryItemPickup_Core();
            ItemCast = Item.Cast<iWardenObjectiveItem>();

            GUID = Guid.NewGuid().ToString();
            Item.m_itemKey = GUID;
        }

        public virtual void OnFound()
        {
        }

        public virtual void OnSolved()
        {
        }

        public void Found()
        {
            WardenObjectiveManager.OnLocalPlayerFoundObjectiveItem(Layer, ItemCast);
            OnFoundEvent?.Invoke();
            OnFound();
        }

        public void Solved()
        {
            WardenObjectiveManager.OnLocalPlayerSolvedObjectiveItem(Layer, ItemCast);
            OnSolvedEvent?.Invoke();
            OnSolved();
        }
    }

    public class ObjectiveItemUtil
    {
    }
}