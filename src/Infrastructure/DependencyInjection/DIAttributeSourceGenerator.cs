using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Infrastructure.DependencyInjection
{
    [Generator]
    public class DIAttributeSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new DIAttributeSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not DIAttributeSyntaxReceiver receiver)
                return;

            var sourceBuilder = new StringBuilder(@"
using Microsoft.Extensions.DependencyInjection;
namespace DI.Generated
{
    public static class DIRegistration
    {
        public static IServiceCollection AddGeneratedDIRegistrations(this IServiceCollection services)
        {
");

            foreach (var (typeSymbol, attribute) in receiver.TypesWithDIAttribute)
            {
                var lifetime = attribute.NamedArguments.FirstOrDefault(na => na.Key == "Lifetime").Value.Value?.ToString() ?? "Scoped";
                var implementingType = attribute.NamedArguments.FirstOrDefault(na => na.Key == "ImplementingType").Value.Value?.ToString();

                if (string.IsNullOrEmpty(implementingType))
                {
                    sourceBuilder.AppendLine($@"
                        services.Add{lifetime}<{typeSymbol}>();");
                }
                else
                {
                    sourceBuilder.AppendLine($@"
                        services.Add{lifetime}<{implementingType}, {typeSymbol}>();");
                }
            }

            sourceBuilder.AppendLine(@"
            return services;
        }
    }
}");

            context.AddSource("DIRegistration.g.cs", sourceBuilder.ToString());
        }
    }

    internal class DIAttributeSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<(INamedTypeSymbol, AttributeData)> TypesWithDIAttribute { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not ClassDeclarationSyntax classDecl)
                return;

            var typeSymbol = context.SemanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
            if (typeSymbol == null)
                return;

            var diAttribute = typeSymbol.GetAttributes().FirstOrDefault(attr =>
                attr.AttributeClass?.Name == "DIAttribute" ||
                attr.AttributeClass?.ToDisplayString().EndsWith(".DIAttribute") == true);

            if (diAttribute != null)
            {
                TypesWithDIAttribute.Add((typeSymbol, diAttribute));
            }
        }
    }

}