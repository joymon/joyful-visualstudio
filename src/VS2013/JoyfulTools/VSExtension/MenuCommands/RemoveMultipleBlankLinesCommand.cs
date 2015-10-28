using System;
using System.ComponentModel.Design;
using EnvDTE;

namespace JoyfulTools.VSExtension
{
    class RemoveMultipleBlankLinesCommand : OleMenuCommandBase
    {
        #region Constructor
        public RemoveMultipleBlankLinesCommand() 
            : base(new CommandID(GuidList.guidMultipleBlankLinesToSingleCmdSet, (int)PkgCmdIDList.cmdidRemMulBlanks))
        {
            
        }
        #endregion

        #region MenuCommand override
        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
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
        protected override void OnMenuClicked(object sender, EventArgs e)
        {
            ReplaceMultipleBlankLinesWithOne();
        }
        #endregion

        #region Actual Logic
        private void ReplaceMultipleBlankLinesWithOne()
        {
            string text = VisualStudioEnvironment.GetContentsFromActiveVisualStudioEditor();
            string replacedText = MultipleBlankLinesRemover.GetReplacedText(text);
            VisualStudioEnvironment.SetContensToActiveVisualStudioEditor(text, replacedText);
        }
        #endregion

        #region Helpers
        private void TryUsingDTE()
        {
            TextDocument td = VisualStudioServicesProvider.DTE2.Value.ActiveDocument.Object() as TextDocument;

            EditPoint ep = td.CreateEditPoint(td.StartPoint);
            ep.ReplacePattern(td.StartPoint, @"^:b*\n:b*\n", @"\n");

        }
        #endregion
    }
}
