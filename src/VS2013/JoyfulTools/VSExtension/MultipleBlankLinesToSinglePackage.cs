using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using EnvDTE;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text;
using System.Text.RegularExpressions;

namespace JoyfulTools.VSExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidMultipleBlankLinesToSinglePkgString)]
    [ProvideAutoLoad("f1536ef8-92ec-443c-9ed7-fdadf150da82")]
    public sealed class MultipleBlankLinesToSinglePackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public MultipleBlankLinesToSinglePackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidMultipleBlankLinesToSingleCmdSet, (int)PkgCmdIDList.cmdidRemMulBlanks);
                OleMenuCommand menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += new EventHandler(menuItem_BeforeQueryStatus);
                mcs.AddCommand(menuItem);
            }
        }
        #endregion

        private void menuItem_BeforeQueryStatus(object sender, EventArgs args)
        {
            var menuCommand = sender as OleMenuCommand;
            menuCommand.Enabled = ! string.IsNullOrWhiteSpace(GetCurrentFileNameUsingDTE());
        }
        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            ReplaceMultipleBlankLinesWithOne();
        }
        private string GetCurrentFileName()
        {
            string filename = string.Empty;
            IVsTextView textView;
            IVsTextManager txtMgr = (IVsTextManager)GetService(typeof(SVsTextManager));

            int mustHaveFocus = 1;
            txtMgr.GetActiveView(mustHaveFocus, null, out textView);
            if (textView != null)
            {
                IComponentModel componentModel = (IComponentModel)GetService(typeof(SComponentModel));
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
        private void ReplaceMultipleBlankLinesWithOne()
        {
            //TryUsingDTE();
            // Show a Message Box to prove we were here
            string fileNameDTE = GetCurrentFileNameUsingDTE();
            string fileName = GetCurrentFileName();

            IVsTextView textView;
            IVsTextManager txtMgr = (IVsTextManager)GetService(typeof(SVsTextManager));
            int mustHaveFocus = 1;
            txtMgr.GetActiveView(mustHaveFocus, null, out textView);
            if (textView != null)
            {
                IComponentModel componentModel = (IComponentModel)GetService(typeof(SComponentModel));
                IVsEditorAdaptersFactoryService editorAdapterService = componentModel.GetService<IVsEditorAdaptersFactoryService>();

                IVsTextLines lines;
                if (textView.GetBuffer(out lines) == 0)
                {

                    var buffer = lines as IVsTextBuffer;
                    if (buffer != null)
                    {
                        ITextBuffer textBuffer = editorAdapterService.GetDataBuffer(buffer);

                        string text = textBuffer.CurrentSnapshot.GetText();
                        string replacedText = Regex.Replace(text, @"\n\s*\n\s*\n", "\n\n",RegexOptions.Multiline);
                        //
                        //string replacedText = Regex.Replace(text, @"^:b*\n:b*\n", @"\n");
                        ITextEdit edit = textBuffer.CreateEdit();
                        edit.Replace(0, text.Length, replacedText);
                        edit.Apply();

                        //ShowMessage(text);
                    }
                }
            }
            //Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = Package.GetGlobalService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


            //IWpfTextView wpfViewCurrent = editorAdapterService.GetWpfTextView(textView);

            //ITextBuffer textCurrent = wpfViewCurrent.;
            //IVsTextLines textLines;
            //textView.GetBuffer(out textLines);

        }

        private void ShowMessage(string text)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                          0,
                          ref clsid,
                          "MultipleBlankLinesToSingle",
                          text,
                          string.Empty,
                          0,
                          OLEMSGBUTTON.OLEMSGBUTTON_OK,
                          OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                          OLEMSGICON.OLEMSGICON_INFO,
                          0,        // false
                          out result));
        }
        private string GetCurrentFileNameUsingDTE()
        {
            EnvDTE.DTE m_dte = (EnvDTE.DTE)this.GetService(typeof(EnvDTE.DTE));
            //TextDocument td = m_dte.ActiveDocument.Object() as TextDocument;
            return m_dte.ActiveDocument == null ? string.Empty : m_dte.ActiveDocument.FullName;
        }
        private void TryUsingDTE()
        {
            EnvDTE80.DTE2 m_dte = (EnvDTE80.DTE2)this.GetService(typeof(EnvDTE80.DTE2));
            TextDocument td = m_dte.ActiveDocument.Object() as TextDocument;

            EditPoint ep = td.CreateEditPoint(td.StartPoint);
            ep.ReplacePattern(td.StartPoint, @"^:b*\n:b*\n", @"\n");

        }
    }
}
