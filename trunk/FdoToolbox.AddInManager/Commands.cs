using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;

namespace FdoToolbox.AddInManager
{
    public class ShowCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.ShowForm();
        }
    }

    public class StartupCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ResourceService.RegisterNeutralStrings(Strings.ResourceManager);
        }
    }

    public class AddInManagerAddInStateConditionEvaluator : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            string states = condition.Properties["states"];
            string action = ((AddInControl)caller).AddIn.Action.ToString();
            foreach (string state in states.Split(','))
            {
                if (state == action)
                    return true;
            }
            return false;
        }
    }

    public class DisableCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.Disable);
        }
    }

    public class EnableCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.Enable);
        }
    }

    public class AbortInstallCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.Uninstall);
        }
    }

    public class AbortUpdateCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.Instance.TryRunAction(((AddInControl)Owner).AddIn, AddInAction.InstalledTwice);
        }
    }

    public class UninstallCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ManagerForm.Instance.TryUninstall(((AddInControl)Owner).AddIn);
        }
    }

    public class OpenHomepageCommand : AbstractMenuCommand
    {
        public override bool IsEnabled
        {
            get
            {
                return ((AddInControl)Owner).AddIn.Properties["url"].Length > 0;
            }
        }

        public override void Run()
        {
			try {
				System.Diagnostics.Process.Start(((AddInControl)Owner).AddIn.Properties["url"]);
			} catch {}
            ManagerForm.Instance.Close();
        }
    }
}
