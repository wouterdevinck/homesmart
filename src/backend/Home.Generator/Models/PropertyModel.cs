namespace Home.Generator.Models {

    public record PropertyModel {

        public string FieldName { get; }

        public PropertyModel(string fieldName) => (FieldName) = (fieldName);

    }

}