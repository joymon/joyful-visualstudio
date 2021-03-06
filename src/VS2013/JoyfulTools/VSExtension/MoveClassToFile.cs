﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace JoyfulTools.VSExtension
{
    [ExportCodeRefactoringProvider(RefactoringId, LanguageNames.CSharp), Shared]
    internal class MoveClassToFileCodeRefactoringProvider : CodeRefactoringProvider
    {
        public const string RefactoringId = "MoveClassToFile";

        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var node = root.FindNode(context.Span);

            // only for a type declaration node that doesn't match the current file name
            // also omit all private classes
            var typeDecl = node as BaseTypeDeclarationSyntax;
            if (typeDecl == null ||
                context.Document.Name.ToLowerInvariant() == string.Format("{0}.cs", typeDecl.Identifier.ToString().ToLowerInvariant()) ||
                typeDecl.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                return;
            }

            var action = CodeAction.Create("Move class to file", c => MoveClassIntoNewFileAsync(context.Document, typeDecl, c));
            context.RegisterRefactoring(action);
        }

        private async Task<Solution> MoveClassIntoNewFileAsync(Document document, BaseTypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            var identifierToken = typeDecl.Identifier;

            // symbol representing the type
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

            // remove type from current files
            var currentSyntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
            var currentRoot = await currentSyntaxTree.GetRootAsync(cancellationToken);
            var replacedRoot = currentRoot.RemoveNode(typeDecl, SyntaxRemoveOptions.KeepNoTrivia);

            document = document.WithSyntaxRoot(replacedRoot);

            // TODO: use Simplifier instead.
            document = await RemoveUnusedImportDirectivesAsync(document, cancellationToken);

            // create new tree for a new file
            // we drag all the usings because we don't know which are needed
            // and there is no easy way to find out which
            var currentUsings = currentRoot.DescendantNodesAndSelf().Where(s => s is UsingDirectiveSyntax);

            var newFileTree = SyntaxFactory.CompilationUnit()
                .WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>(currentUsings.Select(i => ((UsingDirectiveSyntax)i))))
                .WithMembers(
                            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                                SyntaxFactory.NamespaceDeclaration(
                                    SyntaxFactory.IdentifierName(typeSymbol.ContainingNamespace.ToString()))
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(typeDecl))))
                .WithoutLeadingTrivia()
                .NormalizeWhitespace();

            //move to new File
            //TODO: handle name conflicts
            var newDocument = document.Project.AddDocument(string.Format("{0}.cs", identifierToken.Text), SourceText.From(newFileTree.ToFullString()), document.Folders);

            newDocument = await RemoveUnusedImportDirectivesAsync(newDocument, cancellationToken);

            return newDocument.Project.Solution;
        }

        private static async Task<Document> RemoveUnusedImportDirectivesAsync(Document document, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            root = RemoveUnusedImportDirectives(semanticModel, root, cancellationToken);
            document = document.WithSyntaxRoot(root);
            return document;
        }

        private static SyntaxNode RemoveUnusedImportDirectives(SemanticModel semanticModel, SyntaxNode root, CancellationToken cancellationToken)
        {
            var oldUsings = root.DescendantNodesAndSelf().Where(s => s is UsingDirectiveSyntax);
            var unusedUsings = GetUnusedImportDirectives(semanticModel, cancellationToken);
            SyntaxTriviaList leadingTrivia = root.GetLeadingTrivia();

            root = root.RemoveNodes(oldUsings, SyntaxRemoveOptions.KeepNoTrivia);
            var newUsings = SyntaxFactory.List(oldUsings.Except(unusedUsings));
            root = ((CompilationUnitSyntax)root)
                .WithUsings(newUsings)
                .NormalizeWhitespace()
                .WithLeadingTrivia(leadingTrivia);

            return root;
        }

        private static HashSet<SyntaxNode> GetUnusedImportDirectives(SemanticModel model, CancellationToken cancellationToken)
        {
            HashSet<SyntaxNode> unusedImportDirectives = new HashSet<SyntaxNode>();
            SyntaxNode root = model.SyntaxTree.GetRoot(cancellationToken);
            foreach (Diagnostic diagnostic in model.GetDiagnostics(null, cancellationToken).Where(d => d.Id == "CS8019" || d.Id == "CS0105"))
            {
                UsingDirectiveSyntax usingDirectiveSyntax = root.FindNode(diagnostic.Location.SourceSpan, false, false) as UsingDirectiveSyntax;
                if (usingDirectiveSyntax != null)
                {
                    unusedImportDirectives.Add(usingDirectiveSyntax);
                }
            }

            return unusedImportDirectives;
        }
    }
}
