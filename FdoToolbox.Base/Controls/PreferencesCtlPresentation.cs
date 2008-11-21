using System;
using System.Collections.Generic;
using System.Text;

using Prop = ICSharpCode.Core.PropertyService;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    public interface IPreferencesView : IViewContent
    {
        IList<IPreferenceSheet> Sheets { get; set; }
    }

    public class PreferencesCtlPresenter
    {
        private readonly IPreferencesView _view;

        public PreferencesCtlPresenter(IPreferencesView view)
        {
            _view = view;
        }

        public void LoadPreferences()
        {
            List<IPreferenceSheet> sheets = AddInTree.BuildItems<IPreferenceSheet>("/FdoToolbox/Preferences", this);
            _view.Sheets = sheets;
            //Properties prop = Prop.Get("Base.AddIn.Options", new Properties());
        }

        public void SaveChanges()
        {
            foreach (IPreferenceSheet sh in _view.Sheets)
            {
                sh.ApplyChanges();
            }
            PropertyService.Save();
        }
    }
}
