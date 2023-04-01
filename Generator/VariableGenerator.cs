using LanguageExt;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static LanguageExt.Prelude;

namespace HogWarp.Generator
{
    [Generator]
    public class VariableGenerator : IIncrementalGenerator
    {
        private const string AttributeName = "HogWarp.Lib.Interop.Attributes.VariableAttribute";

        public static string GetNamespaceFrom(SyntaxNode s) =>
            s.Parent switch
            {
                NamespaceDeclarationSyntax namespaceDeclarationSyntax => namespaceDeclarationSyntax.Name.ToString(),
                null => string.Empty,
                _ => GetNamespaceFrom(s.Parent)
            };

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var collector = context.SyntaxProvider
               .ForAttributeWithMetadataName(
                   AttributeName,
                   static (node, _) => node is PropertyDeclarationSyntax
                   {
                       Parent: ClassDeclarationSyntax, 
                       Parent.Parent: ClassDeclarationSyntax,
                       AttributeLists.Count: > 0
                   },
                   static (context, _) =>
                   {
                       ClassDeclarationSyntax classSyntax = (ClassDeclarationSyntax)context.TargetNode.Parent!.Parent!;

                       PropertyDeclarationSyntax methodSyntax = (PropertyDeclarationSyntax)context.TargetNode;
                       IPropertySymbol propertySymbol = (IPropertySymbol)context.TargetSymbol;

                       return (classSyntax, propertySymbol);
                   });

            var groupedStructInfoWithMemberInfos =
                collector.Collect().SelectMany(static (item, _) =>
                    item.GroupBy(static items => items.classSyntax, static items => items.propertySymbol,
                    (key, g) => (Key: key, Variables: g.ToList())));

            context.RegisterSourceOutput(groupedStructInfoWithMemberInfos,
            (sourceContext, data) =>
            {
                IndentedStringBuilder builder = new IndentedStringBuilder();

                builder.AppendLine("// autogen");
                builder.AppendLine("using System.Runtime.InteropServices;");

                var klass = data.Key;
                var ns = GetNamespaceFrom(klass);
                var klassName = klass.Identifier.Value;

                builder.AppendLine($"namespace {ns}");
                builder.AppendLine("{");
                builder.IncrementIndent();
                builder.AppendLine($"public partial class {klassName}");
                builder.AppendLine("{");
                builder.IncrementIndent();

                builder.AppendLine("");

                builder.AppendLine("#pragma warning disable CS0649");
                builder.AppendLine("[StructLayout(LayoutKind.Sequential)]");
                builder.AppendLine($"internal struct InitializationVariableParameters");
                builder.AppendLine("{");
                builder.IncrementIndent();
                foreach (var v in data.Variables)
                {
                    if(!v.GetMethod.IsNull())
                        builder.AppendLine($"internal IntPtr Get{v.Name};");

                    if (!v.SetMethod.IsNull())
                        builder.AppendLine($"internal IntPtr Set{v.Name};");
                }
                builder.DecrementIndent();
                builder.AppendLine("}");
                builder.AppendLine("#pragma warning restore CS0649");
                builder.AppendLine("");

                foreach (var v in data.Variables)
                {
                    if (!v.GetMethod.IsNull())
                        builder.AppendLine($"static internal HogWarp.Lib.Interop.Delegates.Get{v.Type}Delegate Get{v.Name};");

                    if (!v.SetMethod.IsNull())
                        builder.AppendLine($"static internal HogWarp.Lib.Interop.Delegates.Set{v.Type}Delegate Set{v.Name};");
                }

                builder.AppendLine($"internal static void Initialize(InitializationVariableParameters Params)");
                builder.AppendLine("{");
                builder.IncrementIndent();
                foreach (var v in data.Variables)
                {
                    if (!v.GetMethod.IsNull())
                        builder.AppendLine($"Get{v.Name} = (HogWarp.Lib.Interop.Delegates.Get{v.Type}Delegate)Marshal.GetDelegateForFunctionPointer(Params.Get{v.Name}, typeof(HogWarp.Lib.Interop.Delegates.Get{v.Type}Delegate));");

                    if (!v.SetMethod.IsNull())
                        builder.AppendLine($"Set{v.Name} = (HogWarp.Lib.Interop.Delegates.Set{v.Type}Delegate)Marshal.GetDelegateForFunctionPointer(Params.Set{v.Name}, typeof(HogWarp.Lib.Interop.Delegates.Set{v.Type}Delegate));");
                }
                builder.DecrementIndent();
                builder.AppendLine("}");
                builder.AppendLine("");


                foreach (var v in data.Variables)
                {
                    builder.AppendLine($"public {v.Type} {v.Name}");
                    builder.AppendLine("{");
                    builder.IncrementIndent();

                    if (!v.GetMethod.IsNull())
                        builder.AppendLine("get { return Get" + v.Name + "((IntPtr)Address); }");
                    if (!v.SetMethod.IsNull())
                        builder.AppendLine("set { Set" + v.Name + "((IntPtr)Address, value); }");

                    builder.DecrementIndent();
                    builder.AppendLine("}");
                    builder.AppendLine("");
                }

                builder.DecrementIndent();
                builder.AppendLine("}");
                builder.DecrementIndent();
                builder.AppendLine("}");
                builder.AppendLine("");

                var s = builder.ToString();

                sourceContext.AddSource($"VariableMemberGenerator.{ns}.{klassName}.g.cs", s);
            });
        }
    }
}