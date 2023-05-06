using System.Collections.Generic;

namespace Home.Generator.Models {

    public record CommandModel(string Name, string Method, List<CommandArgument> Arguments);

}