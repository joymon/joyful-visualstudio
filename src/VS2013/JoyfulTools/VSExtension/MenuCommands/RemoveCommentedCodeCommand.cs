using System;
using System.ComponentModel.Design;
using System.IO;

namespace JoyfulTools.VSExtension
{
    class RemoveCommentedCodeCommand : OleMenuCommandBase
    {
        internal RemoveCommentedCodeCommand()
            : base(new CommandID(GuidList.guidMultipleBlankLinesToSingleCmdSet, (int)PkgCmdIDList.cmdidRemoveCommentedCode))
        {

        }
        #region OleDBCommandBase Overrides
        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            EnableMeIfThereIsFileOpenedInVisualStudio();
        }
        protected override void OnMenuClicked(object sender, EventArgs args)
        {
            RemoveCommentedCode();
        }
        #endregion

        private void EnableMeIfThereIsFileOpenedInVisualStudio()
        {
            this.Enabled = string.IsNullOrWhiteSpace(VisualStudioEnvironment.GetCurrentFileNameUsingDTE()) ? false : true;
        }

        internal void RemoveCommentedCode()
        {
            string text = VisualStudioEnvironment.GetContentsFromActiveVisualStudioEditor();
            string replacedText = GetAfterCommentRemovalBasedOnLanguage(text);
            VisualStudioEnvironment.SetContensToActiveVisualStudioEditor(text, replacedText);
        }

        private static string GetAfterCommentRemovalBasedOnLanguage(string text)
        {
            if (Path.GetExtension(VisualStudioEnvironment.GetCurrentFileNameUsingDTE()).Equals(".cs"))
            {
                return CSharpCommentedCodeRemover.Remove(text);
            }
            else if (Path.GetExtension(VisualStudioEnvironment.GetCurrentFileNameUsingDTE()).Equals(".vb"))
            {
                return VBCommentedCodeRemover.Remove(text);
            }
            return text;
        }
    }
}
