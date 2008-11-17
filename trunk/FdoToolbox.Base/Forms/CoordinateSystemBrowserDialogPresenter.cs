using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FdoToolbox.Core.CoordinateSystems;

namespace FdoToolbox.Base.Forms
{
    public interface ICoordinateSystemBrowserView
    {
        BindingList<CoordinateSystemDefinition> CoordinateSystems { set; }
        CoordinateSystemDefinition SelectedCS { get; }

        bool OkEnabled { set; }
    }

    public class CoordinateSystemBrowserDialogPresenter
    {
        private readonly ICoordinateSystemBrowserView _view;
        private ICoordinateSystemCatalog _catalog;

        public CoordinateSystemBrowserDialogPresenter(ICoordinateSystemBrowserView view, ICoordinateSystemCatalog cat)
        {
            _view = view;
            _catalog = cat;
            _view.OkEnabled = false;
        }

        public void Init()
        {
            _view.CoordinateSystems = _catalog.GetAllProjections();
        }

        public void CoordinateSystemSelected()
        {
            CoordinateSystemDefinition cs = _view.SelectedCS;
            if (cs != null)
            {
                _view.OkEnabled = true;
            }
        }
    }
}
