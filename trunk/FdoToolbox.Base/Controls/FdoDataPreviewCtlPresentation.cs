#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using System.ComponentModel;
using OSGeo.FDO.Commands.SQL;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Filter;
using System.Data;

namespace FdoToolbox.Base.Controls
{
    public enum QueryMode
    {
        Standard,
        ExtendedSelect,
        Aggregate,
        SQL
    }

    public interface IFdoDataPreviewView
    {
        List<QueryMode> QueryModes { set; }
        QueryMode SelectedQueryMode { get; }

        IQuerySubView QueryView { get; set; }

        bool CancelEnabled { get; set; }
        bool ClearEnabled { get; set; }
        bool ExecuteEnabled { get; set; }

        string CountMessage { set; }
        FdoFeatureTable ResultTable { set; get; }

        bool MapEnabled { set; }

        void DisplayError(Exception exception);
    }

    public class FdoDataPreviewPresenter
    {
        private readonly IFdoDataPreviewView _view;
        private FdoConnection _connection;
        private FdoFeatureService _service;
        private BackgroundWorker _queryWorker;

        private Dictionary<QueryMode, IQuerySubView> _queryViews;

        public FdoDataPreviewPresenter(IFdoDataPreviewView view, FdoConnection conn)
        {
            _view = view;
            _connection = conn;
            _service = conn.CreateFeatureService();
            _queryViews = new Dictionary<QueryMode, IQuerySubView>();
            _queryWorker = new BackgroundWorker();
            _queryWorker.WorkerReportsProgress = true;
            _queryWorker.WorkerSupportsCancellation = true;
            _queryWorker.DoWork += new DoWorkEventHandler(DoWork);
            _queryWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            _queryWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);

            _view.CountMessage = string.Empty;
            _view.CancelEnabled = false;
            _view.ExecuteEnabled = true;
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _view.CancelEnabled = false;
            _view.ClearEnabled = true;
            _view.ExecuteEnabled = true;
            
            if (e.Error != null)
            {
                _view.DisplayError(e.Error);
            }
        }

        const int PRG_INIT_GRID = 1;
        const int PRG_REFRESH = 2;

        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == PRG_INIT_GRID)
            {
                _view.ResultTable = e.UserState as FdoFeatureTable;
            }
        }

        void DoWork(object sender, DoWorkEventArgs e)
        {
            IFdoReader reader = null;
            using (FdoFeatureService service = _connection.CreateFeatureService())
            {
                try
                {
                    if (e.Argument is FeatureAggregateOptions)
                        reader = service.SelectAggregates((FeatureAggregateOptions)e.Argument);
                    else if (e.Argument is StandardQuery)
                        reader = service.SelectFeatures((e.Argument as StandardQuery).query, (e.Argument as StandardQuery).Limit);
                    else if (e.Argument is string)
                        reader = service.ExecuteSQLQuery(e.Argument.ToString());

                    //Init the data grid view
                    FdoFeatureTable table = new FdoFeatureTable();
                    table.InitTable(reader);
                    _queryWorker.ReportProgress(PRG_INIT_GRID, table);

                    System.Threading.Thread.Sleep(50);

                    while (reader.ReadNext() && !_queryWorker.CancellationPending)
                    {
                        //Pass processed feature to data grid view
                        FdoFeature feat = table.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string name = reader.GetName(i);
                            if (!reader.IsNull(name))
                            {
                                FdoPropertyType pt = reader.GetFdoPropertyType(name);
                                switch (pt)
                                {
                                    //case FdoPropertyType.Association:
                                    case FdoPropertyType.BLOB:
                                        feat[name] = reader.GetLOB(name).Data;
                                        break;
                                    case FdoPropertyType.Boolean:
                                        feat[name] = reader.GetBoolean(name);
                                        break;
                                    case FdoPropertyType.Byte:
                                        feat[name] = reader.GetByte(name);
                                        break;
                                    case FdoPropertyType.CLOB:
                                        feat[name] = reader.GetLOB(name).Data;
                                        break;
                                    case FdoPropertyType.DateTime:
                                        feat[name] = reader.GetDateTime(name);
                                        break;
                                    case FdoPropertyType.Decimal:
                                        feat[name] = reader.GetDouble(name);
                                        break;
                                    case FdoPropertyType.Double:
                                        feat[name] = reader.GetDouble(name);
                                        break;
                                    case FdoPropertyType.Geometry:
                                        byte[] fgf = reader.GetGeometry(name);
                                        feat[name] = service.GeometryFactory.CreateGeometryFromFgf(fgf);
                                        break;
                                    case FdoPropertyType.Int16:
                                        feat[name] = reader.GetInt16(name);
                                        break;
                                    case FdoPropertyType.Int32:
                                        feat[name] = reader.GetInt32(name);
                                        break;
                                    case FdoPropertyType.Int64:
                                        feat[name] = reader.GetInt64(name);
                                        break;
                                    //case FdoPropertyType.Object:
                                    //case FdoPropertyType.Raster:
                                    case FdoPropertyType.Single:
                                        feat[name] = reader.GetSingle(name);
                                        break;
                                    case FdoPropertyType.String:
                                        feat[name] = reader.GetString(name);
                                        break;
                                }
                            }
                        }
                        table.AddRow(feat);

                        if (table.Rows.Count % 50 == 0)
                        {
                            System.Threading.Thread.Sleep(50);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if(reader != null)
                        reader.Close();
                    if (_queryWorker.CancellationPending)
                        e.Cancel = true;
                }
            }
        }

        public void Init()
        {
            List<QueryMode> modes = new List<QueryMode>();
            if (_service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select))
            {
                modes.Add(QueryMode.Standard);
                _queryViews.Add(QueryMode.Standard, new FdoStandardQueryCtl(_connection));
            }
            if (_service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates))
            {
                modes.Add(QueryMode.Aggregate);
                _queryViews.Add(QueryMode.Aggregate, new FdoAggregateQueryCtl(_connection));
            }
            if (_service.SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand))
            {
                modes.Add(QueryMode.SQL);
                _queryViews.Add(QueryMode.SQL, new FdoSqlQueryCtl());
            }
            foreach (IQuerySubView qv in _queryViews.Values)
            {
                qv.MapPreviewStateChanged += new MapPreviewStateEventHandler(OnMapPreviewStateChanged);
            }
            _view.QueryModes = modes;
        }

        void OnMapPreviewStateChanged(object sender, bool enabled)
        {
            _view.MapEnabled = enabled;
        }

        public void QueryModeChanged()
        {
            _view.QueryView = _queryViews[_view.SelectedQueryMode];
            //_view.MapEnabled = (_view.SelectedQueryMode == QueryMode.Standard);
        }

        class StandardQuery
        {
            public FeatureQueryOptions query;
            public int Limit;
        }

        public void ExecuteQuery()
        {
            object query = null;
            switch (_view.SelectedQueryMode)
            {
                case QueryMode.Aggregate:
                    {
                        query = (_view.QueryView as IFdoAggregateQueryView).QueryObject;
                    }
                    break;
                case QueryMode.SQL:
                    {
                        query = (_view.QueryView as IFdoSqlQueryView).SQLString;
                    }
                    break;
                case QueryMode.Standard:
                    {
                        StandardQuery qry = new StandardQuery();
                        qry.query = (_view.QueryView as IFdoStandardQueryView).QueryObject;
                        qry.Limit = (_view.QueryView as IFdoStandardQueryView).Limit;
                        query = qry;
                    }
                    break;
            }
            if (query != null)
            {
                Clear();
                _view.CancelEnabled = true;
                _view.ClearEnabled = false;
                _view.ExecuteEnabled = false;
                _queryWorker.RunWorkerAsync(query);
            }
        }

        public void CancelCurrentQuery()
        {
            _queryWorker.CancelAsync();
        }

        public long GetFeatureCount()
        {
            IFdoStandardQueryView qv = _view.QueryView as IFdoStandardQueryView;
            if(qv == null)
                return 0;

            ClassDefinition classDef = qv.SelectedClass;
            string filter = qv.Filter;

            return _service.GetFeatureCount(classDef, filter);
        }

        public void Clear()
        {
            _view.ResultTable = null;   
        }
    }
}
