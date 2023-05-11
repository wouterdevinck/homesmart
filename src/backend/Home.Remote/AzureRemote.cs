using Home.Core.Configuration;
using Home.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Home.Remote {

    public class AzureRemote : IRemote {

        public static Descriptor Descriptor = new("azure", typeof(AzureRemote), typeof(AzureRemoteConfiguration), DescriptorType.Remote);

        public string Type => "Azure Remote";

        public bool Started { get; private set;  }

        public AzureRemote(ILogger logger, AzureRemoteConfiguration configuration) {

        }

        public void Install(IDeviceProvider provider) {

        }
        
        protected void Start() {
            Started = true;
        }

    }

}
