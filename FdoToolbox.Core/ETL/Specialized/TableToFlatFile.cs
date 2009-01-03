using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.ETL.Operations;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// A <see cref="FdoFeatureTable"/> to flat file data source conversion process
    /// </summary>
    public class TableToFlatFile : FdoSpecializedEtlProcess
    {
        private FdoFeatureTable _table;
        private string _outputFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableToFlatFile"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="file">The file.</param>
        public TableToFlatFile(FdoFeatureTable table, string file)
        {
            _table = table;
            _outputFile = file;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            SendMessage("Creating target data source");
            if (!ExpressUtility.CreateFlatFileDataSource(_outputFile))
                throw new FdoETLException(ResourceUtil.GetStringFormatted("ERR_CANNOT_CREATE_DATA_FILE", _outputFile));

            FdoConnection conn = ExpressUtility.CreateFlatFileConnection(_outputFile);
            conn.Open();

            ClassDefinition cd = _table.CreateClassDefinition(true);

            using (FdoFeatureService service = conn.CreateFeatureService())
            {
                SendMessage("Applying schema to target");
                FeatureSchema schema = new FeatureSchema("Schema1", "Default schema");
                schema.Classes.Add(cd);
                service.ApplySchema(schema);
            }

            Register(new FdoFeatureTableInputOperation(_table));
            Register(new FdoOutputOperation(conn, cd.Name));
        }

        /// <summary>
        /// Called when a row is processed.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <param name="dictionary">The dictionary.</param>
        protected override void OnFeatureProcessed(FdoOperationBase op, FdoRow dictionary)
        {
            if (op.Statistics.OutputtedRows % 50 == 0)
            {
                if (op is FdoOutputOperation)
                {
                    string className = (op as FdoOutputOperation).ClassName;
                    SendMessageFormatted("[Conversion => {0}]: {1} features written", className, op.Statistics.OutputtedRows);
                }
            }
        }

        /// <summary>
        /// Called when this process has finished processing.
        /// </summary>
        /// <param name="op">The op.</param>
        protected override void OnFinishedProcessing(FdoOperationBase op)
        {
            if (op is FdoOutputOperation)
            {
                string className = (op as FdoOutputOperation).ClassName;
                SendMessageFormatted("[Conversion => {0}]: {1} features written in {2}", className, op.Statistics.OutputtedRows, op.Statistics.Duration.ToString());
            }
        }
    }
}
