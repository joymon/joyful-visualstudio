using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JoyfulTools.VSExtension
{
    internal class CSharpClassExtractor
    {
        internal string[] ExtractFromString(string codeFragmentWhichMayContainMultipleClasses)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(codeFragmentWhichMayContainMultipleClasses);
            IEnumerable<SyntaxNode> classNodes = tree.GetRoot()
                .DescendantNodes()
                .Where((node) => node.Kind() == SyntaxKind.ClassDeclaration)
                .Where((node) => ! IsPrivateOrProtectedClass(node));
            
            string[] result = classNodes.Select((node) => node.ToFullString()).ToArray<string>();
            return result;
        }

        private bool IsPrivateOrProtectedClass(SyntaxNode node)
        {
            ClassDeclarationSyntax classNode = node as ClassDeclarationSyntax;
            if (classNode == null) throw new ArgumentException("Expected class but its different");
            return classNode.Modifiers.Any(SyntaxKind.PrivateKeyword)
                | classNode.Modifiers.Any(SyntaxKind.ProtectedKeyword);
        }
    }
}
