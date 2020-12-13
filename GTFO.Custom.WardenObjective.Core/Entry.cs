using GTFO.CustomObjectives;
using GTFO.CustomObjectives.GlobalHandlers.TimedObjectives;
using GTFO.CustomObjectives.Inject.Global;
using MelonLoader;

[assembly: MelonInfo(typeof(Entry), "Custom WardenObjective Core", "1.0", "Flowaria")]
[assembly: MelonGame("10 Chambers Collective", "GTFO")]

namespace GTFO.CustomObjectives
{
    internal class Entry : MelonMod
    {
        public override void OnApplicationStart()
        {
            Start_RegisterGlobalHandlers();
        }

        private void Start_RegisterGlobalHandlers()
        {
            CustomObjectiveManager.AddGlobalHandler<TimedObjectiveHandler>();
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