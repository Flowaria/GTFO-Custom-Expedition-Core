using GTFO.CustomObjectives.Inject.Global;
using LevelGeneration;
using System;

namespace GTFO.CustomObjectives.HandlerBase
{
    public partial class ObjectiveStatusProxy
    {
        private CustomObjectiveHandlerBase Base;
        private Action DefaultObjectiveUpdate;
        private Action OnObjectiveStatusChange;

        internal ObjectiveStatusProxy(CustomObjectiveHandlerBase b)
        {
            Base = b;
            Base.RegisterUpdateEvent(OnUpdateWardenObjective);

            GlobalMessage.OnObjectiveStateChanged += OnStateChanged;
            Base.OnUnloadEvent += () => { GlobalMessage.OnObjectiveStateChanged -= OnStateChanged; };
        }

        private void OnStateChanged(pWardenObjectiveState oldState, pWardenObjectiveState newState, bool isRecall)
        {
        }

        private void OnUpdateWardenObjective()
        {
            DefaultObjectiveUpdate?.Invoke();
        }

        public void SetupAsDefaultBehaviour(eWardenObjectiveType objType)
        {
            switch (objType)
            {
                case eWardenObjectiveType.ClearAPath:
                    DefaultObjectiveUpdate = Update_ClearPath;
                    break;

                case eWardenObjectiveType.GatherSmallItems:
                    DefaultObjectiveUpdate = Update_Gather;
                    break;

                case eWardenObjectiveType.RetrieveBigItems:
                    DefaultObjectiveUpdate = Update_RetrieveBig;
                    break;

                case eWardenObjectiveType.ActivateSmallHSU:
                    DefaultObjectiveUpdate = Update_ActivateHSU;
                    break;

                case eWardenObjectiveType.PowerCellDistribution:
                case eWardenObjectiveType.TerminalUplink:
                case eWardenObjectiveType.SpecialTerminalCommand:
                case eWardenObjectiveType.HSU_FindTakeSample:
                case eWardenObjectiveType.CentralGeneratorCluster:
                case eWardenObjectiveType.Reactor_Startup:
                case eWardenObjectiveType.Reactor_Shutdown:
                    DefaultObjectiveUpdate = Update_FindAndSolve;
                    break;
            }
        }

        /// <summary>
        /// Set Warden Objective hint Fragment (such as [ITEM_SERIAL])
        /// </summary>
        /// <param name="type">Fragment Type</param>
        /// <param name="text">String to be placed in selected Fragment</param>
        public void SetObjectiveTextFragment(eWardenTextFragment type, string text)
        {
            WardenObjectiveManager.SetObjectiveTextFragment(Base.LayerType, type, text);
        }

        public void ForceUpdateObjectiveStatus(eWardenObjectiveInteractionType type, eWardenSubObjectiveStatus status)
        {
            WardenObjectiveManager.Current.AttemptInteract(new pWardenObjectiveInteraction
            {
                inLayer = Base.LayerType,
                type = type,
                newSubObj = status,
                forceUpdate = true
            });
        }

        public eWardenObjectiveStatus ObjectiveStatus
        {
            get
            {
                var currentState = WardenObjectiveManager.CurrentState;
                switch (Base.LayerType)
                {
                    case LG_LayerType.MainLayer:
                        return currentState.main_status;

                    case LG_LayerType.SecondaryLayer:
                        return currentState.second_status;

                    case LG_LayerType.ThirdLayer:
                        return currentState.third_status;
                }

                return eWardenObjectiveStatus.NotDiscovered;
            }
        }

        public eWardenSubObjectiveStatus SubObjectiveStatus
        {
            get
            {
                var currentState = WardenObjectiveManager.CurrentState;
                switch (Base.LayerType)
                {
                    case LG_LayerType.MainLayer:
                        return currentState.main_subObj;

                    case LG_LayerType.SecondaryLayer:
                        return currentState.second_subObj;

                    case LG_LayerType.ThirdLayer:
                        return currentState.third_subObj;
                }

                return eWardenSubObjectiveStatus.FindLocationInfo;
            }
        }
    }
}