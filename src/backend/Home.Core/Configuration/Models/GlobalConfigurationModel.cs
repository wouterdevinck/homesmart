using System;

namespace Home.Core.Configuration.Models {

    public class GlobalConfigurationModel {

        public string Timezone { get; set; }
        public string Culture { get; set; }

        public TimeZoneInfo GetTz() {
            return TimeZoneInfo.FindSystemTimeZoneById(Timezone);
        }

    }

}
