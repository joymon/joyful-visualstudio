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
    class RemoveMultipleBlankLinesCommand :OleMenuCommand
    {
        public RemoveMultipleBlankLinesCommand() : base((sender,args) =>{ MenuItemCallback(sender, args); }, 
            new CommandID(GuidList.guidMultipleBlankLinesToSingleCmdSet, (int)PkgCmdIDList.cmdidRemMulBlanks)) {
            this.BeforeQueryStatus += OleMenuCommand_BeforeQueryStatus;
        }

        private void OleMenuCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            this.Enabled = !string.IsNullOrWhiteSpace(VisualStudioEnvironment.GetCurrentFileNameUsingDTE());
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
        private void ReplaceMultipleBlankLinesWithOne()
        {
            string fileNameDTE = VisualStudioEnvironment.GetCurrentFileNameUsingDTE();

            IVsTextView textView;
            IVsTextManager txtMgr = VisualStudioServicesProvider.VSTextManager.Value;
            int mustHaveFocus = 1;
            txtMgr.GetActiveView(mustHaveFocus, null, out textView);
            if (textView != null)
            {
                IComponentModel componentModel = VisualStudioServicesProvider.ComponentModel.Value;
                IVsEditorAdaptersFactoryService editorAdapterService = componentModel.GetService<IVsEditorAdaptersFactoryService>();

                IVsTextLines lines;
                if (textView.GetBuffer(out lines) == 0)
                {

                    var buffer = lines as IVsTextBuffer;
                    if (buffer != null)
                    {
                        ITextBuffer textBuffer = editorAdapterService.GetDataBuffer(buffer);

                        string text = textBuffer.CurrentSnapshot.GetText();
                        string replacedText = Regex.Replace(text, @"\n\s*\n\s*\n", "\n\n", RegexOptions.Multiline);
                        ITextEdit edit = textBuffer.CreateEdit();
                        edit.Replace(0, text.Length, replacedText);
                        edit.Apply();
                    }
                }
            }
            //Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = Package.GetGlobalService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


            //IWpfTextView wpfViewCurrent = editorAdapterService.GetWpfTextView(textView);

            //ITextBuffer textCurrent = wpfViewCurrent.;
            //IVsTextLines textLines;
            //textView.GetBuffer(out textLines);
        }
        private void TryUsingDTE()
        {
            TextDocument td = VisualStudioServicesProvider.DTE2.Value.ActiveDocument.Object() as TextDocument;

            EditPoint ep = td.CreateEditPoint(td.StartPoint);
            ep.ReplacePattern(td.StartPoint, @"^:b*\n:b*\n", @"\n");

        }

    }
}
