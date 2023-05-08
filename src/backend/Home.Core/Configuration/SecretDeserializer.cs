using System;
using Home.Core.Configuration.Models;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Home.Core.Configuration {

    public class SecretDeserializer : INodeDeserializer {

        private readonly INodeDeserializer _nodeDeserializer;
        private readonly SecretsModel _secrets;

        public SecretDeserializer(INodeDeserializer nodeDeserializer, SecretsModel secrets) {
            _nodeDeserializer = nodeDeserializer;
            _secrets = secrets;
        }

        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value) {
            if (_nodeDeserializer.Deserialize(reader, expectedType, nestedObjectDeserializer, out value)) {
                if (expectedType == typeof(string)) {
                    var key = (value as string)!;
                    if (key.StartsWith("$")) {
                        key = (value as string)![1..];
                        if (_secrets.Secrets.TryGetValue(key, out var secret)) {
                            value = secret;
                        }
                    }
                }
                return true;
            }
            return false;
        }

    }

}
