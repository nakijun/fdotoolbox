using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Feature;

namespace FdoUtil
{
    public class CreateFileCommand : ConsoleCommand
    {
        private string _file;
        private string _schema;

        public CreateFileCommand(string file)
        {
            _file = file;
        }

        public CreateFileCommand(string file, string schema)
            : this(file)
        {
            _schema = schema;
        }

        public override int Execute()
        {
            CommandStatus retCode = CommandStatus.E_OK;

            bool create = ExpressUtility.CreateFlatFileDataSource(_file);
            if (!create)
            {
                WriteLine("Failed to create file {0}", _file);
                retCode = CommandStatus.E_FAIL_CREATE_DATASTORE;
                return (int)retCode;
            }
            WriteLine("File {0} created", _file);
            if (_schema != null)
            {
                try
                {
                    FdoConnection conn = ExpressUtility.CreateFlatFileConnection(_file);
                    conn.Open();
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.LoadSchemasFromXml(_schema);
                        WriteLine("Schema applied to {0}", _file);
                    }
                    retCode = CommandStatus.E_OK;
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    retCode = CommandStatus.E_FAIL_APPLY_SCHEMA;
                }
            }
            return (int)retCode;
        }
    }
}
