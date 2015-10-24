using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;

namespace JoyfulTools.VSExtension
{
    class RemoveCommentedCodeCommand : OleMenuCommand
    {
        internal RemoveCommentedCodeCommand() : base((sender, args) => { MenuItemCallback(sender, args); },
            new CommandID(GuidList.guidMultipleBlankLinesToSingleCmdSet, (int)PkgCmdIDList.cmdidRemoveCommentedCode))
        {
            this.BeforeQueryStatus += OleMenuCommand_BeforeQueryStatus;
        }

        private void OleMenuCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            EnableMeIfThereIsFileOpenedInVisualStudio();
        }
        private void EnableMeIfThereIsFileOpenedInVisualStudio()
        {
            this.Enabled = string.IsNullOrWhiteSpace(VisualStudioEnvironment.GetCurrentFileNameUsingDTE()) ? false : true;
        }
        private static void MenuItemCallback(object sender, EventArgs args)
        {
            (sender as RemoveCommentedCodeCommand).RemoveCommentedCode();
        }
        internal void RemoveCommentedCode()
        {
            string text = GetContentsFromActiveVisualStudioEditor();
            string replacedText = GetAfterCommentRemovalBasedOnLanguage(text);
            SetContensToActiveVisualStudioEditor(text, replacedText);
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

        private string GetContentsFromActiveVisualStudioEditor()
        {
            string text = string.Empty;
            ITextBuffer textBuffer = GetTextBuffer();
            text = textBuffer.CurrentSnapshot.GetText();
            return text;
        }
        private void SetContensToActiveVisualStudioEditor(string oldText, string newText)
        {
            ITextBuffer textBuffer = GetTextBuffer();
            ReplaceContentsInsideTextBuffer(textBuffer, oldText, newText);
        }
        private ITextBuffer GetTextBuffer()
        {
            ITextBuffer textBuffer = null;
            IVsTextView textView = GetVSTextView();
            if (textView != null)
            {
                IVsTextLines lines;
                if (textView.GetBuffer(out lines) == 0)
                {
                    textBuffer = GetTextBufferFromVSTextLines(lines);
                }
            }
            return textBuffer;
        }
        private static void ReplaceContentsInsideTextBuffer(ITextBuffer textBuffer, string oldText, string newText)
        {
            ITextEdit edit = textBuffer.CreateEdit();
            edit.Replace(0, oldText.Length, newText);
            edit.Apply();
        }
        private static IVsTextView GetVSTextView()
        {
            IVsTextView textView;
            IVsTextManager txtMgr = VisualStudioServicesProvider.VSTextManager.Value;
            int mustHaveFocus = 1;
            int result = txtMgr.GetActiveView(mustHaveFocus, null, out textView);
            return textView;
        }
        private static ITextBuffer GetTextBufferFromVSTextLines(IVsTextLines lines)
        {
            ITextBuffer textBuffer = null;
            var buffer = lines as IVsTextBuffer;
            if (buffer != null)
            {
                IVsEditorAdaptersFactoryService editorAdapterService = GetVSEditorAdaptorsFactoryServiceFromCompomentModel();
                textBuffer = editorAdapterService.GetDataBuffer(buffer);
            }

            return textBuffer;
        }
        private static IVsEditorAdaptersFactoryService GetVSEditorAdaptorsFactoryServiceFromCompomentModel()
        {
            IComponentModel componentModel = VisualStudioServicesProvider.ComponentModel.Value;
            IVsEditorAdaptersFactoryService editorAdapterService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            return editorAdapterService;
        }
    }
}
