using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace JoyfulTools.VSExtension
{
    public class OleMenuCommandBase : OleMenuCommand
    {
        public OleMenuCommandBase(CommandID id) : base((sender, args) => { MenuItemCallback(sender, args); },id)
        {
            this.BeforeQueryStatus += OnBeforeQueryStatus;
        }

        protected virtual void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            
        }

        private static void MenuItemCallback(object sender, EventArgs args)
        {
            (sender as OleMenuCommandBase).OnMenuClicked(sender, args);
        }

        protected virtual void OnMenuClicked(object oleMenuCommandBase, EventArgs args)
        {

        }
    }
}
