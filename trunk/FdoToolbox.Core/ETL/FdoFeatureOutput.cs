#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands.Schema;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// FDO feature output source
    /// </summary>
    public class FdoFeatureOutput : IFdoOutput, IDisposable
    {
        private IConnection _conn;
        private IInsert _insertCmd;

        public event EventHandler FeatureInserted = delegate { };

        public FdoFeatureOutput(IConnection conn, string className)
        {
            _conn = conn;
            _insertCmd = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert;
            _insertCmd.SetFeatureClassName(className);
        }

        public FdoFeatureOutput(IConnection conn, FeatureSchema schema, string className)
        {
            using (IApplySchema apply = conn.CreateCommand(CommandType.CommandType_ApplySchema) as IApplySchema)
            {
                apply.FeatureSchema = schema;
                apply.Execute();
            }
            _conn = conn;
            _insertCmd = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert;
            _insertCmd.SetFeatureClassName(className);
        }

        public IEnumerable<FdoFeature> Process(IEnumerable<FdoFeature> features)
        {
            foreach (FdoFeature feat in features)
            {
                _insertCmd.PropertyValues.Clear();
                PropertyValueCollection propVals = feat.ToValueCollection();
                foreach (PropertyValue p in propVals)
                {
                    _insertCmd.PropertyValues.Add(p);
                }
                using (IFeatureReader reader = _insertCmd.Execute())
                {
                    while (reader.ReadNext()) { }
                    reader.Close();
                }
            }
            yield break;
        }

        public void Dispose()
        {
            _insertCmd.Dispose();
        }
    }
}
