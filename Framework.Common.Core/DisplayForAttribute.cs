using System;

namespace Framework.Common.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DisplayForAttribute : Attribute
    {
        public DisplayForAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
