using System;

namespace Home.Core {

    public class DeviceUpdateEventArgs : EventArgs {

        public DeviceUpdateEventArgs(string property, object value) {
            Property = property;
            Value = value;
        }

        public string Property { get; set; }
        public object Value { get; set; }

    }

}
