using CustomExpeditions;
using CustomExpeditions.HandlerBase;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.Basic
{
    public class OverridesHandler : CustomExpHandlerBase
    {
        public override void OnSetup()
        {
            Logger.Log("OVERRIDES TEST: OnSetup");
            RegisterUpdateEvent(OnUpdate, OnFixedUpdate); // It will be automatically unloaded when OnUnload called
        }

        private void OnUpdate() { }
        private void OnFixedUpdate() { }

        public override void OnUnload()
        {
            Logger.Log("OVERRIDES TEST: OnUnload (LevelCleanup)");
        }

        public override void OnBuildDone()
        {
            Logger.Log("OVERRIDES TEST: OnBuildDone (Level is fully built)");
        }

        public override void OnElevatorArrive()
        {
            Logger.Log("OVERRIDES TEST: OnElevatorArrive (When prisoner has landed from Elevator)");
        }

        public override void OnExpeditionFail()
        {
            Logger.Log("OVERRIDES TEST: OnExpeditionFail (Level Failed, Called when Failed Screen appear)");
        }

        public override void OnExpeditionSuccess()
        {
            Logger.Log("OVERRIDES TEST: OnExpeditionSuccess (Level Success, Called when Survived Screen appear)");
        }
    }
}
