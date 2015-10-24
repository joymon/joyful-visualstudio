// Guids.cs
// MUST match guids.h
using System;

namespace JoyfulTools.VSExtension
{
    internal static class GuidList
    {
        public const string guidMultipleBlankLinesToSinglePkgString = "4eb4ee6b-e540-423f-9f2e-2ae38710d11b";
        public const string guidMultipleBlankLinesToSingleCmdSetString = "1af85a00-8bb7-47fd-84f0-108651db01dc";
        public static readonly Guid guidMultipleBlankLinesToSingleCmdSet = new Guid(guidMultipleBlankLinesToSingleCmdSetString);
        public static readonly Guid RemoveCommentedCodeCmd = new Guid("1af85a00-8bb7-47fd-84f0-108651db01dd");
    };
}