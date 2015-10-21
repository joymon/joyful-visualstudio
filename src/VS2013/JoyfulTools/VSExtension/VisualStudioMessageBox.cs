using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;

namespace JoyfulTools.VSExtension
{
    internal class VisualStudioMessageBox
    {
        private void Show(string text)
        {
            IVsUIShell uiShell = VisualStudioServicesProvider.VSUIShell.Value;
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
    }
}
