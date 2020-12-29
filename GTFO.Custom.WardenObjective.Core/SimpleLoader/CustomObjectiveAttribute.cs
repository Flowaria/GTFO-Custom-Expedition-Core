using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.SimpleLoader
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public class CustomObjectiveAttribute : Attribute
    {
        public Type Entry;

        public CustomObjectiveAttribute(Type entryPoint)
        {
            Entry = entryPoint;
        }
    }
}
