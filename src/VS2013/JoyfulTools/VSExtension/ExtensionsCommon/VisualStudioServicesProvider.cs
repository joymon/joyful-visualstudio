using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace JoyfulTools.VSExtension
{
    internal class VisualStudioServicesProvider
    {
        internal static Lazy<DTE> DTE = new Lazy<DTE>(() => (EnvDTE.DTE)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)));
        internal static Lazy<DTE2> DTE2 = new Lazy<DTE2>(() => (EnvDTE80.DTE2)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE80.DTE2)));

        internal static Lazy<IVsTextManager> VSTextManager = new Lazy<IVsTextManager>(() => (IVsTextManager)ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager)));
        internal static Lazy<IComponentModel> ComponentModel = new Lazy<IComponentModel>(() => (IComponentModel)ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)));
        public static Lazy<IVsUIShell> VSUIShell = new Lazy<IVsUIShell>(() => (IVsUIShell)ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShell)));
    }
}
