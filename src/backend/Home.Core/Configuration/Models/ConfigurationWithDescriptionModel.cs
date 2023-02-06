namespace Home.Core.Configuration.Models {

    public class ConfigurationWithDescriptionModel<T> {

        public string Description { get; set; }
        public T Configuration { get; set; }

    }

}
