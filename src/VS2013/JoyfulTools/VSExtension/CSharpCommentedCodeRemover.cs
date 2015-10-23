using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace JoyfulTools.VSExtension
{
    class CSharpCommentedCodeRemover
    {
        internal static string Remove(string inputCode)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(inputCode);
            IEnumerable<SyntaxTrivia> commentTrivias =tree.GetRoot().DescendantTrivia();
            IEnumerable<SyntaxTrivia> commentTrivia = from t in tree.GetRoot().DescendantTrivia()
                                where t.Kind() == SyntaxKind.SingleLineCommentTrivia ||
                                      t.Kind() == SyntaxKind.MultiLineCommentTrivia
                                select t;

            SyntaxNode newRoot = tree.GetRoot().ReplaceTrivia(commentTrivia, (t1, t2) =>  default( SyntaxTrivia));
            return newRoot.ToFullString();
        }
    }
}
