using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Home.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;

namespace Home.Generator {

    [Generator]
    public class HomeSourceGenerator : ISourceGenerator {

        public void Execute(GeneratorExecutionContext context) {
            if (context.SyntaxReceiver is not SyntaxReceiver receiver) {
                return;
            }
            var templateString = ReadTemplate("device.scriban");
            var template = Template.Parse(templateString);
            if (template.HasErrors) {
                throw new InvalidOperationException($"Template parse error: {template.Messages}");
            }
            var models = new Dictionary<string, DeviceClassModel>();
            foreach (var classSyntax in receiver.CandidateClasses) {
                var model = ProcessDeviceClass(classSyntax, context.Compilation);
                if (model is null) break;
                models.Add(model.Classname, model);
            }
            foreach (var model in models.Values) {
                var result = template.Render(model, member => member.Name);
                context.AddSource($"{model.Classname}.Generated.cs", result);
            }
        }

        public void Initialize(GeneratorInitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private DeviceClassModel ProcessDeviceClass(ClassDeclarationSyntax classSyntax, Compilation compilation) {

            // Get symbol
            var model = compilation.GetSemanticModel(classSyntax.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(classSyntax) as ITypeSymbol;

            // Get attribute
            var attr = symbol?.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == "DeviceAttribute");
            if (attr is null) return null;

            // Get all device class members
            var members = symbol.GetMembers();

            // Find and process the members annotated with a property attribute
            var properties = new List<PropertyModel>();
            var propertyMembers = members.Where(x => x.GetAttributes().Any(y => y.AttributeClass?.Name == "DevicePropertyAttribute"));
            foreach (var property in propertyMembers) {
                properties.Add(new PropertyModel(property.Name));
            }

            // Return a model
            return new DeviceClassModel {
                IsAbstract = symbol.IsAbstract,
                Classname = classSyntax.Identifier.Text,
                Namespace = symbol.ContainingNamespace.ToDisplayString(),
                Device = new DeviceModel(properties)
            };

        }

        private static string ReadTemplate(string name) {
            var assembly = Assembly.GetExecutingAssembly();
            name = assembly.GetName().Name + ".Templates." + name;
            using var resourceStream = assembly.GetManifestResourceStream(name);
            if (resourceStream == null) return null;
            using var streamReader = new StreamReader(resourceStream);
            return streamReader.ReadToEnd();
        }

    }

}