using GTFO.CustomObjectives;
using GTFO.CustomObjectives.GlobalHandlers.FogControlTerminals;
using GTFO.CustomObjectives.GlobalHandlers.TimedObjectives;
using GTFO.CustomObjectives.Inject.Global;
using GTFO.CustomObjectives.SimpleLoader;
using GTFO.CustomObjectives.Utils;
using MelonLoader;

[assembly: MelonInfo(typeof(Entry), "Custom WardenObjective Core", "1.0", "Flowaria")]
[assembly: MelonGame("10 Chambers Collective", "GTFO")]

namespace GTFO.CustomObjectives
{
    internal class Entry : MelonMod
    {
        public override void OnApplicationStart()
        {
            Setup_GlobalHandlers();
            //Setup_DefaultReplicator();

            ObjectiveSimpleLoader.Setup();
        }

        private void Read_Configs()
        {

        }

        private void Setup_GlobalHandlers()
        {
            CustomObjectiveManager.AddGlobalHandler<TimedObjectiveHandler>(CustomObjectiveSettings.MAIN_ONLY);
            //CustomObjectiveManager.AddGlobalHandler<FogControlTerminalHandler>(); later
            //TODO: New Global Handler: Change Light Settings (color / type / or whatever)
            //TODO: New Global Handler: Change Alarm Settings for existing door
            //TODO: New Global Handler: Regen Hibernating Enemies on other zone
        }

        private void Setup_DefaultReplicator()
        {
            FogLevelUtil.Setup();
        }

        public override void OnUpdate()
        {
            GlobalMessage.OnUpdate?.Invoke();
        }

        public override void OnFixedUpdate()
        {
            GlobalMessage.OnFixedUpdate?.Invoke();
        }
    }
}