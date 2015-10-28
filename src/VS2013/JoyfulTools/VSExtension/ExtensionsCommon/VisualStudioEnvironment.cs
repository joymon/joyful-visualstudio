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
        internal static string GetContentsFromActiveVisualStudioEditor()
        {
            string text = string.Empty;
            ITextBuffer textBuffer = GetTextBuffer();
            text = textBuffer.CurrentSnapshot.GetText();
            return text;
        }
        private static ITextBuffer GetTextBuffer()
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
        internal static void SetContensToActiveVisualStudioEditor(string oldText, string newText)
        {
            ITextBuffer textBuffer = GetTextBuffer();
            ReplaceContentsInsideTextBuffer(textBuffer, oldText, newText);
        }
        private static void ReplaceContentsInsideTextBuffer(ITextBuffer textBuffer, string oldText, string newText)
        {
            ITextEdit edit = textBuffer.CreateEdit();
            edit.Replace(0, oldText.Length, newText);
            edit.Apply();
        }
    }
}
