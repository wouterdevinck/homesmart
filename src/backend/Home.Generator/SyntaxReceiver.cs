using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Home.Generator {

    internal class SyntaxReceiver : ISyntaxReceiver {

        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is ClassDeclarationSyntax s && s.AttributeLists.Count > 0) {
                CandidateClasses.Add(s);
            }
        }

    }

}