using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;

namespace JoyfulTools.VSExtension
{
    class RemoveMultipleBlankLinesCommand : OleMenuCommand
    {
        #region Constructor
        public RemoveMultipleBlankLinesCommand() : base((sender, args) => { MenuItemCallback(sender, args); },
            new CommandID(GuidList.guidMultipleBlankLinesToSingleCmdSet, (int)PkgCmdIDList.cmdidRemMulBlanks))
        {
            this.BeforeQueryStatus += OleMenuCommand_BeforeQueryStatus;
        }
        #endregion

        #region MenuCommand Related
        private void OleMenuCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            EnableMeIfThereIsFileOpenedInVisualStudio();
        }

        private void EnableMeIfThereIsFileOpenedInVisualStudio()
        {
            this.Enabled = string.IsNullOrWhiteSpace(VisualStudioEnvironment.GetCurrentFileNameUsingDTE()) ? false : true;
        }
        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private static void MenuItemCallback(object sender, EventArgs e)
        {
            (sender as RemoveMultipleBlankLinesCommand).ReplaceMultipleBlankLinesWithOne();
        }
        #endregion

        #region Actual Logic
        private void ReplaceMultipleBlankLinesWithOne()
        {
            string text = GetContentsFromActiveVisualStudioEditor();
            string replacedText = MultipleBlankLinesRemover.GetReplacedText(text);
            SetContensToActiveVisualStudioEditor(text, replacedText);
        }
        #endregion

        #region Helpers

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

        private static IVsTextView GetVSTextView()
        {
            IVsTextView textView;
            IVsTextManager txtMgr = VisualStudioServicesProvider.VSTextManager.Value;
            int mustHaveFocus = 1;
            int result = txtMgr.GetActiveView(mustHaveFocus, null, out textView);
            return textView;
        }

        private static IVsEditorAdaptersFactoryService GetVSEditorAdaptorsFactoryServiceFromCompomentModel()
        {
            IComponentModel componentModel = VisualStudioServicesProvider.ComponentModel.Value;
            IVsEditorAdaptersFactoryService editorAdapterService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            return editorAdapterService;
        }

        private static void ReplaceContentsInsideTextBuffer(ITextBuffer textBuffer, string oldText, string newText)
        {
            ITextEdit edit = textBuffer.CreateEdit();
            edit.Replace(0, oldText.Length, newText);
            edit.Apply();
        }

        private void TryUsingDTE()
        {
            TextDocument td = VisualStudioServicesProvider.DTE2.Value.ActiveDocument.Object() as TextDocument;

            EditPoint ep = td.CreateEditPoint(td.StartPoint);
            ep.ReplacePattern(td.StartPoint, @"^:b*\n:b*\n", @"\n");

        }
        #endregion
    }
}
