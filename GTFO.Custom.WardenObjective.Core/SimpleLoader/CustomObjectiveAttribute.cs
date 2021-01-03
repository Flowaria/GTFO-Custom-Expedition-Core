using System;

namespace CustomObjectives.SimpleLoader
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