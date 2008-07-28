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
using OSGeo.FDO.Commands.SpatialContext;
using OSGeo.FDO.Geometry;

namespace FdoToolbox.Core
{
    public class FeatureService : IDisposable
    {
        private IConnection _conn;

        private FeatureSchemaCollection _Schemas;

        private FgfGeometryFactory _GeomFactory;

        internal FeatureService(IConnection conn)
        {
            _conn = conn;
            _GeomFactory = new FgfGeometryFactory();
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

        public static FeatureSchema CloneSchema(FeatureSchema fs)
        {
            FeatureSchemaCollection newSchemas = new FeatureSchemaCollection(null);
            using (IoMemoryStream stream = new IoMemoryStream())
            {
                //Clone selected schema
                fs.WriteXml(stream);
                stream.Reset();
                newSchemas.ReadXml(stream);
                stream.Close();
            }
            return newSchemas[0];
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
            if (_Schemas == null)
                CacheSchemas();

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

        public void WriteSchemaToXml(string schemaFile)
        {
            _Schemas.WriteXml(schemaFile);
        }

        public List<SpatialContextInfo> GetSpatialContexts()
        {
            List<SpatialContextInfo> contexts = new List<SpatialContextInfo>();
            using (IGetSpatialContexts get = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_GetSpatialContexts) as IGetSpatialContexts)
            {
                get.ActiveOnly = false;
                using (ISpatialContextReader reader = get.Execute())
                {
                    while (reader.ReadNext())
                    {
                        SpatialContextInfo info = new SpatialContextInfo(reader);
                        contexts.Add(info);
                    }
                }
            }
            return contexts;
        }

        public SpatialContextInfo GetSpatialContext(string name)
        {
            List<SpatialContextInfo> contexts = GetSpatialContexts();
            return contexts.Find(delegate(SpatialContextInfo info) { return info.Name == name; });
        }

        public void CreateSpatialContext(SpatialContextInfo ctx, bool updateExisting)
        {
            using (ICreateSpatialContext create = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateSpatialContext) as ICreateSpatialContext)
            {
                IGeometry geom = null;
                create.CoordinateSystem = ctx.CoordinateSystem;
                create.CoordinateSystemWkt = ctx.CoordinateSystemWkt;
                create.Description = ctx.Description;
                create.ExtentType = ctx.ExtentType;
                if (create.ExtentType == SpatialContextExtentType.SpatialContextExtentType_Static)
                {
                    geom = _GeomFactory.CreateGeometry(ctx.ExtentGeometryText);
                    create.Extent = _GeomFactory.GetFgf(geom);
                }
                create.Name = ctx.Name;
                create.UpdateExisting = updateExisting;
                create.XYTolerance = ctx.XYTolerance;
                create.ZTolerance = ctx.ZTolerance;
                create.Execute();
                if(geom != null)
                    geom.Dispose();
            }
        }

        public void DestroySpatialContext(SpatialContextInfo ctx)
        {
            DestroySpatialContext(ctx.Name);
        }

        public void DestroySpatialContext(string name)
        {
            using (IDestroySpatialContext destroy = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext) as IDestroySpatialContext)
            {
                destroy.Name = name;
                destroy.Execute();
            }
        }

        public void Dispose()
        {
            _GeomFactory.Dispose();
        }
    }
}
