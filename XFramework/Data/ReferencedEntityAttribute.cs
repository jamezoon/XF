using System;

namespace XFramework.Data
{
    public class ReferencedEntityAttribute : Attribute
    {
        public ReferencedEntityAttribute(Type type)
        {
            this.Type = type;
        }
        public Type Type { get; private set; }

        public string Prefix { get; set; }

        public string ConditionalProperty { get; set; }
    }
}
