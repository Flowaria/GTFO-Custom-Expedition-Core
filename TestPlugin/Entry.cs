using GTFO.CustomObjectives;
using GTFO.CustomObjectives.SimpleLoader;
using GTFO.CustomObjectives.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPlugin;
using TestPlugin.Basic;
using TestPlugin.UplinkBioscan;

[assembly: CustomObjective(typeof(Entry))]
namespace TestPlugin
{
    internal class Entry : ObjectiveSimpleEntry
    {
        public override void OnStart()
        {
            CustomObjectiveManager.AddGlobalHandler<OverridesHandler>("OverrideTest", CustomObjectiveSettings.MAIN_ONLY);
            CustomObjectiveManager.AddGlobalHandler<BuilderHandler>("BuilderTest", CustomObjectiveSettings.ALL_LAYER);

            CustomObjectiveManager.AddHandler<UplinkBioscanHandler>(50/*, CustomObjectiveSettings.ALL_LAYER*/);
        }
    }
}
