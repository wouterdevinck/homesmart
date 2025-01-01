using System;

namespace Home.Core.Attributes {

    [AttributeUsage(AttributeTargets.Property)]
    public class DevicePropertyAttribute<T> : DevicePropertyAttribute {

        public T Min { get; set; }
        public T Max { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DevicePropertyAttribute : Attribute {

        public string Unit { get; set; }

    }

}