using System.Collections.Generic;
using Newtonsoft.Json;

namespace Home.Generator.Models {

    public record DeviceModel {
        
        [JsonIgnore]
        public List<PropertyModel> Properties { get; }
        
        public DeviceModel(List<PropertyModel> properties) => (Properties) = (properties);
        
    }

}