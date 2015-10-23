using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace JoyfulTools.VSExtension
{
    internal static class VisualStudioEnvironment
    {
        internal static string GetCurrentFileNameUsingDTE()
        {
            //TextDocument td = m_dte.ActiveDocument.Object() as TextDocument;
            return VisualStudioServicesProvider.DTE.Value.ActiveDocument == null ? string.Empty :
                 VisualStudioServicesProvider.DTE.Value.ActiveDocument.FullName;
        }
        private static string GetCurrentFileName()
        {
            string filename = string.Empty;
            IVsTextView textView;
            IVsTextManager txtMgr = VisualStudioServicesProvider.VSTextManager.Value;

            int mustHaveFocus = 1;
            txtMgr.GetActiveView(mustHaveFocus, null, out textView);
            if (textView != null)
            {
                IComponentModel componentModel = VisualStudioServicesProvider.ComponentModel.Value;
                IVsEditorAdaptersFactoryService editorAdapterService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
                IWpfTextView wpfView = editorAdapterService.GetWpfTextView(textView);
                ITextDocument document = null;
                wpfView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document);
                filename = document.FilePath;
                //IVsTextLines lines;
                //if (textView.GetBuffer(out lines) == 0)
                //{
                //    var buffer = lines as IVsTextBuffer;
                //    if (buffer != null)
                //    {
                //        ITextBuffer textBuffer = editorAdapterService.GetDocumentBuffer(buffer);
                //        textBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document);
                //        filename = document.FilePath;
                //    }
                //}
            }
            return filename;
        }
    }
}
