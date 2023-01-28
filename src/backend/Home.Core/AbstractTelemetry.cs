using System.Collections.Generic;
using Home.Core.Interfaces;

namespace Home.Core {

    public abstract class AbstractTelemetry : MultiDeviceConsumer, ITelemetry {

        public AbstractTelemetry(List<string> deviceIds) : base(deviceIds) { }

    }

}
