using System;

namespace Home.Core {

    public class DeviceUpdateEventArgs : EventArgs {

        public DeviceUpdateEventArgs(string property, object value, DateTime timestamp) {
            Property = property;
            Value = value;
            Timestamp = timestamp;
        }

        public string Property { get; set; }
        public object Value { get; set; }
        public DateTime Timestamp { get; set; }

    }

}
