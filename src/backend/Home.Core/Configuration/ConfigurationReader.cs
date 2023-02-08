using System.Collections.Generic;
using System.IO;
using Home.Core.Configuration.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Home.Core.Configuration {

    public class ConfigurationReader {

        public ConfigurationModel ConfigurationModel { get; private set; }
        public List<Descriptor> ProviderDescriptors { get; private set; }

        public ConfigurationReader(string configPath, List<Descriptor> providerDescriptors) {
            var configYaml = File.ReadAllText(configPath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithNodeTypeResolver(new ConfigurationResolver(providerDescriptors))
                .Build();
            ConfigurationModel = deserializer.Deserialize<ConfigurationModel>(configYaml);
            ProviderDescriptors = providerDescriptors;
        }

    }

}
