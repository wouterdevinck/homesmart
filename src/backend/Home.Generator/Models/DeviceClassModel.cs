namespace Home.Generator.Models {

    public record DeviceClassModel {
        
        public string Namespace { get; init; }
        public string Classname { get; init; }
        public DeviceModel Device { get; init; }

    }

}