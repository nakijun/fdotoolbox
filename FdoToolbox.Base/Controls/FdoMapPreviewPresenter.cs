using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Forms;

namespace FdoToolbox.Base.Controls
{
    public interface IFdoMapView
    {
        bool ZoomInChecked { set; }
        bool ZoomOutChecked { set; }
        bool SelectChecked { set; }
        bool PanChecked { set; }
        string StatusText { set; }
    }

    public class FdoMapPreviewPresenter
    {
        private readonly IFdoMapView _view;
        private readonly MapImage _mapImage;

        public FdoMapPreviewPresenter(IFdoMapView view, MapImage mimg)
        {
            _view = view;
            _mapImage = mimg;
            _mapImage.MouseMove += new MapImage.MouseEventHandler(MapMouseMove);

            switch (_mapImage.ActiveTool)
            {
                case MapImage.Tools.Pan:
                    _view.PanChecked = true;
                    break;
                case MapImage.Tools.Query:
                    _view.SelectChecked = true;
                    break;
                case MapImage.Tools.ZoomIn:
                    _view.ZoomInChecked = true;
                    break;
                case MapImage.Tools.ZoomOut:
                    _view.ZoomOutChecked = true;
                    break;
            }
        }

        void MapMouseMove(SharpMap.Geometries.Point WorldPos, System.Windows.Forms.MouseEventArgs ImagePos)
        {
            _view.StatusText = string.Format("X: {0} Y: {1}", WorldPos.X, WorldPos.Y);
        }

        private void ChangeTool(SharpMap.Forms.MapImage.Tools tool)
        {
            _mapImage.ActiveTool = tool;

            _view.SelectChecked = false;
            _view.ZoomInChecked = false;
            _view.ZoomOutChecked = false;
            _view.PanChecked = false;

            switch (tool)
            {
                case SharpMap.Forms.MapImage.Tools.Pan:
                    _view.PanChecked = true;
                    break;
                case SharpMap.Forms.MapImage.Tools.ZoomIn:
                    _view.ZoomInChecked = true;
                    break;
                case SharpMap.Forms.MapImage.Tools.ZoomOut:
                    _view.ZoomOutChecked = true;
                    break;
                case SharpMap.Forms.MapImage.Tools.Query:
                    _view.SelectChecked = true;
                    break;
            }
        }

        private bool HasLayers()
        {
            return _mapImage.Map != null && _mapImage.Map.Layers.Count > 0;
        }

        public void ZoomIn()
        {
            if (HasLayers())
                ChangeTool(SharpMap.Forms.MapImage.Tools.ZoomIn);
        }

        public void ZoomOut()
        {
            if (HasLayers())
                ChangeTool(SharpMap.Forms.MapImage.Tools.ZoomOut);
        }

        public void Pan()
        {
            if (HasLayers())
                ChangeTool(SharpMap.Forms.MapImage.Tools.Pan);
        }

        public void Select()
        {
            if (HasLayers())
                ChangeTool(SharpMap.Forms.MapImage.Tools.Query);
        }

        public void ZoomExtents()
        {
            if (HasLayers())
            {
                _mapImage.Map.ZoomToExtents();
                _mapImage.Refresh();
            }
        }
    }
}
