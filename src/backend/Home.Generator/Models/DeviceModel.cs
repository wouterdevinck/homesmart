using System.Collections.Generic;

namespace Home.Generator.Models {

    public record DeviceModel(List<PropertyModel> Properties, List<CommandModel> Commands);

}