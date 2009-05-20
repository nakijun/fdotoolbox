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
using System.Collections.Specialized;
using FdoToolbox.Core.ETL.Operations;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Feature;

namespace FdoToolbox.Core.ETL.Specialized
{
    public class ClassToClassCopy : FdoSpecializedEtlProcess
    {
        private FdoClassCopyOptions _copt;

        public override string Name
        {
            get
            {
                return _copt.SourceClassName + " => " + _copt.TargetClassName;
            }
        }

        private int _ReportFrequency;

        public int ReportFrequency
        {
            get { return _ReportFrequency; }
            set { _ReportFrequency = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ClassToClassCopy"/> class.
        /// </summary>
        /// <param name="copt">The copt.</param>
        public ClassToClassCopy(FdoClassCopyOptions copt)
        {
            _copt = copt;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (_copt.DeleteTarget)
            {
                Info("Deleting data in target class {0} before copying", _copt.TargetClassName);
                using (FdoFeatureService service = _copt.Parent.TargetConnection.CreateFeatureService())
                {
                    using (IDelete del = service.CreateCommand<IDelete>(OSGeo.FDO.Commands.CommandType.CommandType_Delete) as IDelete)
                    {
                        try
                        {
                            del.SetFeatureClassName(_copt.TargetClassName);
                            del.Execute();
                            Info("Data in target class {0} deleted");
                        }
                        catch
                        {

                        }
                    }
                }
            }

            FdoBulkCopyOptions bcpOptions = _copt.Parent;
            IFdoOperation input = new FdoInputOperation(bcpOptions.SourceConnection, CreateSourceQuery(_copt));
            IFdoOperation output = null;
            NameValueCollection propertyMappings = new NameValueCollection();
            if (_copt.SourcePropertyNames.Length > 0)
            {
                foreach (string srcProp in _copt.SourcePropertyNames)
                {
                    propertyMappings.Add(srcProp, _copt.GetTargetProperty(srcProp));
                }
            }
            if (_copt.SourceAliases.Length > 0)
            {
                foreach (string srcAlias in _copt.SourceAliases)
                {
                    propertyMappings.Add(srcAlias, _copt.GetTargetPropertyForAlias(srcAlias));
                }
            }
            if (propertyMappings.Count > 0)
            {
                if (bcpOptions.BatchSize > 0)
                {
                    FdoBatchedOutputOperation b = new FdoBatchedOutputOperation(bcpOptions.TargetConnection, _copt.TargetClassName, propertyMappings, bcpOptions.BatchSize);
                    b.BatchInserted += delegate(object sender, BatchInsertEventArgs e)
                    {
                        SendMessageFormatted("[Bulk Copy => {0}] {1} feature batch written", _copt.TargetClassName, e.BatchSize);
                    };
                    output = b;
                }
                else
                {
                    output = new FdoOutputOperation(bcpOptions.TargetConnection, _copt.TargetClassName, propertyMappings);
                }
            }
            else
            {
                if (bcpOptions.BatchSize > 0)
                {
                    FdoBatchedOutputOperation b = new FdoBatchedOutputOperation(bcpOptions.TargetConnection, _copt.TargetClassName, bcpOptions.BatchSize);
                    b.BatchInserted += delegate(object sender, BatchInsertEventArgs e)
                    {
                        SendMessageFormatted("[Bulk Copy => {0}] {1} feature batch written", _copt.TargetClassName, e.BatchSize);
                    };
                    output = b;
                }
                else
                {
                    output = new FdoOutputOperation(bcpOptions.TargetConnection, _copt.TargetClassName);
                }
            }

            string sourceClass = _copt.SourceClassName;
            string targetClass = _copt.TargetClassName;

            Register(input);
            Register(output);
        }

        /// <summary>
        /// Called when a row is processed.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <param name="dictionary">The dictionary.</param>
        protected override void OnFeatureProcessed(FdoOperationBase op, FdoRow dictionary)
        {
            if (op.Statistics.OutputtedRows % this.ReportFrequency == 0)
            {
                if (op is FdoOutputOperation)
                {
                    string className = (op as FdoOutputOperation).ClassName;
                    SendMessageFormatted("[{0}]: {1} features written", this.Name, op.Statistics.OutputtedRows);
                }
            }
        }

        /// <summary>
        /// Called when this process has finished processing.
        /// </summary>
        /// <param name="op">The op.</param>
        protected override void OnFinishedProcessing(FdoOperationBase op)
        {
            if (op is FdoBatchedOutputOperation)
            {
                FdoBatchedOutputOperation bop = op as FdoBatchedOutputOperation;
                string className = bop.ClassName;
                SendMessageFormatted("[{0}]: {1}", this.Name, op.Statistics.ToString());
            }
            else if (op is FdoOutputOperation)
            {
                string className = (op as FdoOutputOperation).ClassName;
                SendMessageFormatted("[{0}]: {1}", this.Name, op.Statistics.ToString());
            }
        }

        private static FeatureQueryOptions CreateSourceQuery(FdoClassCopyOptions copt)
        {
            FeatureQueryOptions query = new FeatureQueryOptions(copt.SourceClassName);
            query.AddFeatureProperty(copt.SourcePropertyNames);

            foreach (string alias in copt.SourceAliases)
            {
                query.AddComputedProperty(alias, copt.GetExpression(alias));
            }

            if (!string.IsNullOrEmpty(copt.SourceFilter))
                query.Filter = copt.SourceFilter;

            return query;
        }
    }
}
