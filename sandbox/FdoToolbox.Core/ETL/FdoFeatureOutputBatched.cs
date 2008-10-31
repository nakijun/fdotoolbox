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
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.Schema;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// FDO feature output source with support for batch insertion
    /// </summary>
    public class FdoFeatureOutputBatched : IFdoOutput, IDisposable
    {
        private IConnection _conn;
        private IInsert _insertCmd;

        public FdoFeatureOutputBatched(IConnection conn, string className, int batchSize) 
        {
            _conn = conn;
            _insertCmd = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert;
            _insertCmd.SetFeatureClassName(className);
            this.BatchSize = batchSize;
        }

        public FdoFeatureOutputBatched(IConnection conn, FeatureSchema schema, string className, int batchSize)
        {
            using (IApplySchema apply = conn.CreateCommand(CommandType.CommandType_ApplySchema) as IApplySchema)
            {
                apply.FeatureSchema = schema;
                apply.Execute();
            }
            _conn = conn;
            _insertCmd = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert;
            _insertCmd.SetFeatureClassName(className);
            this.BatchSize = batchSize;
        }

        const string PARAM_PREFIX = "param_";

        private int _BatchSize;

        public int BatchSize
        {
            get { return _BatchSize; }
            set { _BatchSize = value; }
        }
	

        public IEnumerable<FdoFeature> Process(IEnumerable<FdoFeature> features)
        {
            int count = 0;
            foreach (FdoFeature feat in features)
            {
                ParameterValueCollection paramVals = new ParameterValueCollection();
                foreach (string name in feat.PropertyNames)
                {
                    string paramName = PARAM_PREFIX + name;
                    paramVals.Add(new ParameterValue(paramName, feat[name]));
                }
                _insertCmd.BatchParameterValues.Add(paramVals);
                count++;
                if (count == this.BatchSize)
                {
                    using (IFeatureReader reader = _insertCmd.Execute())
                    {
                        while (reader.ReadNext()) { }
                        reader.Close();
                        count = 0;
                    }
                }
            }
            //There may be some batches left uninserted because count did not reach
            //batchsize before iteration completed. So execute this remainding batch
            if (count > 0)
            {
                using (IFeatureReader reader = _insertCmd.Execute())
                {
                    while (reader.ReadNext()) { }
                    reader.Close();
                    count = 0;
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
