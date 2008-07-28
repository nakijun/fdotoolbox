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
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Common.Io;

namespace FdoToolbox.Core
{
    public class FeatureService
    {
        private IConnection _conn;

        private FeatureSchemaCollection _Schemas;

        internal FeatureService(IConnection conn)
        {
            _conn = conn;
        }

        public IConnection Connection
        {
            get { return _conn; }
        }

        public void LoadSchemasFromXml(string xmlFile)
        {
            _Schemas.ReadXml(xmlFile);
            foreach (FeatureSchema fs in _Schemas)
            {
                ApplySchema(fs);
            }
        }

        public void ApplySchema(FeatureSchema fs)
        {
            using (IApplySchema apply = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema) as IApplySchema)
            {
                apply.FeatureSchema = fs;
                apply.Execute();
            }
        }

        public void WriteSchemaToXml(string schemaName, string xmlFile)
        {
            FeatureSchema schema = GetSchemaByName(schemaName);
            if (schema != null)
            {
                schema.WriteXml(schemaName);
            }
            else
            {
                throw new FeatureServiceException("Schema " + schemaName + " not found");
            }
        }

        public FeatureSchemaCollection CloneSchemas()
        {
            FeatureSchemaCollection newSchemas = new FeatureSchemaCollection(null);
            using (IoMemoryStream stream = new IoMemoryStream())
            {
                _Schemas.WriteXml(stream);
                stream.Reset();
                newSchemas.ReadXml(stream);
            }
            return newSchemas;
        }

        public FeatureSchema GetSchemaByName(string schemaName)
        {
            foreach (FeatureSchema schema in _Schemas)
            {
                if (schema.Name == schemaName)
                    return schema;
            }

            return null;
        }

        private void CacheSchemas()
        {
            if (_Schemas == null)
            {
                using (IDescribeSchema describe = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    _Schemas = describe.Execute();
                }
            }
        }

        public FeatureSchemaCollection DescribeSchema()
        {
            if (_Schemas != null)
                return _Schemas;

            CacheSchemas();

            return _Schemas;
        }

        public ClassDefinition GetClassByName(string schemaName, string className)
        {
            FeatureSchema schema = GetSchemaByName(schemaName);
            if (schema != null)
            {
                foreach (ClassDefinition classDef in schema.Classes)
                {
                    if (classDef.Name == className)
                        return classDef;
                }
            }
            return null;
        }
    }
}
