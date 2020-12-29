using GTFO.CustomObjectives;
using GTFO.CustomObjectives.SimpleLoader;
using GTFO.CustomObjectives.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPlugin;

[assembly: CustomObjective(typeof(Entry))]
namespace TestPlugin
{
    internal class Entry : ObjectiveSimpleEntry
    {
        public override void OnStart()
        {
            CustomObjectiveManager.AddGlobalHandler<TestHandler>(CustomObjectiveSettings.MAIN_ONLY);
        }
    }
}
