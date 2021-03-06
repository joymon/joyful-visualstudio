﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

namespace JoyfulTools.VSExtension
{
    class VBCommentedCodeRemover
    {

        internal static string Remove(string inputCode)
        {
            SyntaxTree tree = VisualBasicSyntaxTree.ParseText(inputCode);

            IEnumerable<SyntaxTrivia> commentTrivia = from t in tree.GetRoot().DescendantTrivia()
                                                      where t.Kind() == SyntaxKind.CommentTrivia
                                                      select t;
            SyntaxNode newRoot = tree.GetRoot().ReplaceTrivia(commentTrivia, (t1, t2) => default(SyntaxTrivia));
            return newRoot.ToFullString();
        }

        private static SyntaxNode RemoveImproperDocumentationTrivia(SyntaxNode node)
        {
            IEnumerable<SyntaxTrivia> commentTrivia = from t in node.DescendantTrivia()
                                                      where t.Kind() == SyntaxKind.DocumentationCommentTrivia 
                                                      select t;
            SyntaxNode newRoot = node.ReplaceTrivia(commentTrivia, (t1, t2) => default(SyntaxTrivia));
            return newRoot;
        }
        private static bool IsDocumentationCommentTriviaAndNotProperXML(SyntaxTrivia trivia)
        {
            return (trivia.Kind() == SyntaxKind.DocumentationCommentTrivia &&
                ( trivia.ToString()).IsValidXml() == false);
        } 
        private static string WrapWithRootXML(string xmlNodes)
        {
            return string.Format("<Wrapper>{0}</Wrapper>", xmlNodes);
        }
    }
}
