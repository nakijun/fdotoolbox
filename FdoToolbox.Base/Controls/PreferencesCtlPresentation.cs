using System;
using System.Collections.Generic;
using System.Text;

using Prop = ICSharpCode.Core.PropertyService;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Controls
{
    public interface IPreferencesView : IViewContent
    {

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
            Properties prop = Prop.Get("Base.AddIn.Options", new Properties());

        }
    }
}
