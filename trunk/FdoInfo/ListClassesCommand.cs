using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Feature;

namespace FdoInfo
{
    public class ListClassesCommand : ConsoleCommand
    {
        private string _schema;
        private string _provider;
        private string _connstr;

        public ListClassesCommand(string provider, string connStr, string schemaName)
        {
            _schema = schemaName;
            _provider = provider;
            _connstr = connStr;
        }

        public override int Execute()
        {
            IConnection conn = null;
            try
            {
                conn = CreateConnection(_provider, _connstr);
                conn.Open();
            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                WriteException(ex);
                return (int)CommandStatus.E_FAIL_CONNECT;
            }

            using (FdoFeatureService service = new FdoFeatureService(conn))
            {
                FeatureSchema fs = service.GetSchemaByName(_schema);
                if (fs != null)
                {
                    using (fs)
                    {
                        Console.WriteLine("\nClasses in schema {0}: {1}\n", fs.Name, fs.Classes.Count);
                        foreach (ClassDefinition cd in fs.Classes)
                        {
                            Console.WriteLine("Name: {0} ({1})\n\n\tQualified Name: {2}", cd.Name, cd.ClassType, cd.QualifiedName);
                            Console.WriteLine("\tDescription: {0}", cd.Description);
                            Console.WriteLine("\tIs Abstract: {0}\n\tIs Computed: {1}", cd.IsAbstract, cd.IsComputed);
                            if (cd.BaseClass != null)
                                Console.WriteLine("\tBase Class: {0}", cd.BaseClass.Name);
                            Console.WriteLine("\tAttributes:");
                            WriteAttributes(cd.Attributes);
                            Console.WriteLine("");
                        }
                    }
                }
                else
                {
                    Console.Error.WriteLine("Could not find schema: {0}", _schema);
                    return (int)CommandStatus.E_FAIL_SCHEMA_NOT_FOUND;
                }
            }

            conn.Close();
            return (int)CommandStatus.E_OK;
        }

        private void WriteAttributes(SchemaAttributeDictionary schemaAttributeDictionary)
        {
            if (schemaAttributeDictionary.AttributeNames.Length > 0)
            {
                foreach (string name in schemaAttributeDictionary.AttributeNames)
                {
                    Console.WriteLine("\t\t- {0} : {1}", name, schemaAttributeDictionary.GetAttributeValue(name));
                }
            }
            else
            {
                Console.WriteLine("\t\tNone");
            }
        }
    }
}
