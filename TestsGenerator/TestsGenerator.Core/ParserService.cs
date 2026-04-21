using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestsGenerator.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace TestsGenerator.Core
{
    public class ParserService
    {
        public List<ClassInfo> Parse(string sourceCode)
        {
            var tree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = tree.GetRoot();

            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            var result = new List<ClassInfo>();

            foreach (var classDeclaration in classDeclarations)
            {
                var constructor = classDeclaration.Members.OfType<ConstructorDeclarationSyntax>()
                    .FirstOrDefault(c => c.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)));

                var constructorDependencies = constructor?.ParameterList.Parameters
                    .Select(p => p.Type.ToString())
                    .ToList() ?? new List<string>();

                var methods = new List<MethodInfo>();
                var methodDeclarations = classDeclaration.Members.OfType<MethodDeclarationSyntax>()
                    .Where(m => m.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PublicKeyword)));

                foreach (var method in methodDeclarations)
                {
                    var parameterTypes = method.ParameterList.Parameters
                        .Select(p => p.Type.ToString())
                        .ToList();

                    var returnType = method.ReturnType.ToString();

                    methods.Add(new MethodInfo(method.Identifier.Text, parameterTypes, returnType));
                }

                result.Add(new ClassInfo(classDeclaration.Identifier.Text, methods, constructorDependencies));
            }
            return result;
        }
    }
}