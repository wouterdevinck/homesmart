namespace Home.Devices.SolarEdge.Models {

    public class SiteDetailsResponseModel {

        public SiteDetailsModel Details { get; set; }

    }

    // The below is a partial implementation. Other fields like peakPower, installationDate, full address & solar panel make/model are available.

    public class SiteDetailsModel {

        public LocationModel Location { get; set; }

    }

    public class LocationModel {

        public string TimeZone { get; set; }

    }

}
