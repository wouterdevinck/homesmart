using Home.Core.Configuration.Interfaces;

namespace Home.Remote {

    public class AzureRemoteConfiguration : IRemoteConfiguration {

        public string ApiUrl { get; set; }
        public AzureRemoteIotConfiguration Iot { get; set; }
        public AzureRemoteNotificationsConfiguration Notifications { get; set; }

    }

    public class AzureRemoteIotConfiguration {

        public string Host { get; set; }
        public string Id { get; set; }
        public string Key { get; set; }

    }

    public class AzureRemoteNotificationsConfiguration {

        public string Host { get; set; }
        public string Key { get; set; }

    }

}
