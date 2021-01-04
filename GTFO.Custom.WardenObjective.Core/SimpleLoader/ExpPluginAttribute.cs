using System;

namespace CustomExpeditions.SimpleLoader
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public class ExpPluginAttribute : Attribute
    {
        public Type Entry;

        public ExpPluginAttribute(Type entryPoint)
        {
            Entry = entryPoint;
        }
    }
}