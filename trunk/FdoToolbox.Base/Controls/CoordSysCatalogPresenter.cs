using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.CoordinateSystems;
using System.ComponentModel;

namespace FdoToolbox.Base.Controls
{
    public interface ICoordSysCatalogView
    {
        BindingList<CoordinateSystemDefinition> CoordSysDefinitions { set; }

        CoordinateSystemDefinition SelectedCS { get; }

        bool EditEnabled { set; }
        bool DeleteEnabled { set; }
    }

    public class CoordSysCatalogPresenter
    {
        private readonly ICoordSysCatalogView _view;
        private FdoToolbox.Base.Services.CoordSysCatalog _catalog;
        private BindingList<CoordinateSystemDefinition> _list;

        public CoordSysCatalogPresenter(ICoordSysCatalogView view, FdoToolbox.Base.Services.CoordSysCatalog catalog)
        {
            _view = view;
            _catalog = catalog;
            _view.DeleteEnabled = false;
            _view.EditEnabled = false;
        }

        public void Init()
        {
            _view.CoordSysDefinitions = _list = _catalog.GetAllProjections(); 
        }

        public void AddNew(CoordinateSystemDefinition cs)
        {
            _catalog.AddProjection(cs);
        }

        public void Update(string oldName, CoordinateSystemDefinition cs)
        {
            _catalog.UpdateProjection(cs, oldName);
        }

        public void Delete(CoordinateSystemDefinition cs)
        {
            _catalog.DeleteProjection(cs);
        }

        public void CheckStatus()
        {
            _view.DeleteEnabled = (_view.SelectedCS != null);
            _view.EditEnabled = (_view.SelectedCS != null);
        }
    }
}
