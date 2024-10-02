using System.Collections.Generic;
using System.IO;
using Home.Core.Configuration.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace Home.Core.Configuration {

    public class ConfigurationReader {

        public ConfigurationModel ConfigurationModel { get; private set; }
        public List<Descriptor> ProviderDescriptors { get; private set; }

        public ConfigurationReader(string configPath, string secretsPath, List<Descriptor> providerDescriptors) {

            if (File.Exists(configPath) && File.Exists(secretsPath)) {

                // Secrets
                var secretsYaml = File.ReadAllText(secretsPath);
                var secretsDeserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var secrets = secretsDeserializer.Deserialize<SecretsModel>(secretsYaml);

                // Config
                var configYaml = File.ReadAllText(configPath);
                var configDeserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithNodeTypeResolver(new ConfigurationResolver(providerDescriptors))
                    .WithNodeDeserializer(inner => new SecretDeserializer(inner, secrets),
                        s => s.InsteadOf<ScalarNodeDeserializer>())
                    .Build();
                ConfigurationModel = configDeserializer.Deserialize<ConfigurationModel>(configYaml);

            } else {

                // Blank configuration
                ConfigurationModel = new ConfigurationModel();

            }

            // Providers
            ProviderDescriptors = providerDescriptors;

        }

    }

}
