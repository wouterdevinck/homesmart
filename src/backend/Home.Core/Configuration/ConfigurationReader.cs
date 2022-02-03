using System.Collections.Generic;
using System.IO;
using Home.Core.Configuration.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Home.Core.Configuration {

    public class ConfigurationReader {

        public ConfigurationModel ConfigurationModel { get; private set; }
        public List<ProviderDescription> ProviderDescriptors { get; private set; }

        public ConfigurationReader(string configPath, List<ProviderDescription> providerDescriptors) {
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
