using System.Collections.Generic;
using Home.Core.Interfaces;

namespace Home.Core {

    public abstract class AbstractAutomation : MultiDeviceConsumer, IAutomation {

        public string Description { get; set; }
        public abstract string Type { get; }

        public AbstractAutomation(List<string> deviceIds) : base(deviceIds) {}

    }

}