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
using System.Collections.Specialized;
using OSGeo.FDO.Commands.DataStore;

namespace FdoToolbox.Core
{
    /// <summary>
    /// IConnection wrapper service object.
    /// </summary>
    public class FeatureService : IDisposable
    {
        private IConnection _conn;

        private FgfGeometryFactory _GeomFactory;

        /// <summary>
        /// Constructor. The passed connection must already be open.
        /// </summary>
        /// <param name="conn"></param>
        public FeatureService(IConnection conn)
        {
            _conn = conn;
            _GeomFactory = new FgfGeometryFactory();
        }

        public void Dispose()
        {
            _GeomFactory.Dispose();
        }

        /// <summary>
        /// The underlying FDO connection
        /// </summary>
        public IConnection Connection
        {
            get { return _conn; }
        }

        /// <summary>
        /// Loads and applies a defined feature schema definition file into the
        /// current connection
        /// </summary>
        /// <param name="xmlFile"></param>
        public void LoadSchemasFromXml(string xmlFile)
        {
            FeatureSchemaCollection schemas = new FeatureSchemaCollection(null);
            schemas.ReadXml(xmlFile);
            foreach (FeatureSchema fs in schemas)
            {
                ApplySchema(fs);
            }
        }

        /// <summary>
        /// Checks if a given FDO command is supported
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool SupportsCommand(OSGeo.FDO.Commands.CommandType cmd)
        {
            return Array.IndexOf<int>(_conn.CommandCapabilities.Commands, (int)cmd) >= 0;
        }

        /// <summary>
        /// Utility method to clone a feature schema (via in-memory XML serialization)
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Applies a feature schema to the current connection
        /// </summary>
        /// <param name="fs"></param>
        public void ApplySchema(FeatureSchema fs)
        {
            using (IApplySchema apply = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema) as IApplySchema)
            {
                apply.FeatureSchema = fs;
                apply.Execute();
            }
        }

        /// <summary>
        /// Dumps a given feature schema (by name) to an xml file
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="xmlFile"></param>
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

        /// <summary>
        /// Clones all the feature schemas in the current connection (via in-memory
        /// XML serialization)
        /// </summary>
        /// <returns></returns>
        public FeatureSchemaCollection CloneSchemas()
        {
            FeatureSchemaCollection schemas = DescribeSchema();
            FeatureSchemaCollection newSchemas = new FeatureSchemaCollection(null);
            using (IoMemoryStream stream = new IoMemoryStream())
            {
                schemas.WriteXml(stream);
                stream.Reset();
                newSchemas.ReadXml(stream);
            }
            return newSchemas;
        }

        /// <summary>
        /// Gets a feature schema by name
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        /// <returns>The feature schema. null if the schema was not found.</returns>
        public FeatureSchema GetSchemaByName(string schemaName)
        {
            if (string.IsNullOrEmpty(schemaName))
                return null;

            FeatureSchemaCollection schemas = DescribeSchema();

            foreach (FeatureSchema schema in schemas)
            {
                if (schema.Name == schemaName)
                    return schema;
            }

            return null;
        }

        /// <summary>
        /// Enumerates all the known feature schemas in the current connection.
        /// </summary>
        /// <returns></returns>
        public FeatureSchemaCollection DescribeSchema()
        {
            FeatureSchemaCollection schemas = null;
            using (IDescribeSchema describe = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                schemas = describe.Execute();
            }
            return schemas;
        }

        /// <summary>
        /// Gets a class definition by name
        /// </summary>
        /// <param name="schemaName">The parent schema name</param>
        /// <param name="className">The class name</param>
        /// <returns>The class definition if found. null if it wasn't</returns>
        public ClassDefinition GetClassByName(string schemaName, string className)
        {
            if (string.IsNullOrEmpty(className))
                return null;

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

        /// <summary>
        /// Dumps all feature schemas in the current connection to an xml file.
        /// </summary>
        /// <param name="schemaFile"></param>
        public void WriteSchemaToXml(string schemaFile)
        {
            FeatureSchemaCollection schemas = DescribeSchema();
            schemas.WriteXml(schemaFile);
        }

        /// <summary>
        /// Enumerates all spatial contexts in the current connection
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a spatial context by name
        /// </summary>
        /// <param name="name">The name of the spatial context</param>
        /// <returns>The spatial context information if found. null if otherwise</returns>
        public SpatialContextInfo GetSpatialContext(string name)
        {
            List<SpatialContextInfo> contexts = GetSpatialContexts();
            return contexts.Find(delegate(SpatialContextInfo info) { return info.Name == name; });
        }

        /// <summary>
        /// Creates a spatial context
        /// </summary>
        /// <param name="ctx">The spatial context</param>
        /// <param name="updateExisting">If true, will replace any existing spatial context of the same name</param>
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

        /// <summary>
        /// Destroys a spatial context
        /// </summary>
        /// <param name="ctx">The spatial context to destroy</param>
        public void DestroySpatialContext(SpatialContextInfo ctx)
        {
            DestroySpatialContext(ctx.Name);
        }

        /// <summary>
        /// Destroys a spatial context
        /// </summary>
        /// <param name="name">The name of the spatial context to destroy</param>
        public void DestroySpatialContext(string name)
        {
            using (IDestroySpatialContext destroy = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySpatialContext) as IDestroySpatialContext)
            {
                destroy.Name = name;
                destroy.Execute();
            }
        }

        /// <summary>
        /// Destroys a feature schema
        /// </summary>
        /// <param name="schemaName">The name of the schema to destroy</param>
        public void DestroySchema(string schemaName)
        {
            using (IDestroySchema destroy = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySchema) as IDestroySchema)
            {
                destroy.SchemaName = schemaName;
                destroy.Execute();
            }
        }

        /// <summary>
        /// Destroys a data store
        /// </summary>
        /// <param name="dataStoreString">A connection-string style string describing the data store parameters</param>
        public void DestroyDataStore(string dataStoreString)
        {
            NameValueCollection parameters = ExpressUtility.ConvertFromString(dataStoreString);
            using (IDestroyDataStore destroy = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroyDataStore) as IDestroyDataStore)
            {
                foreach (string key in parameters.AllKeys)
                {
                    destroy.DataStoreProperties.SetProperty(key, parameters[key]);
                }
                destroy.Execute();
            }
        }

        /// <summary>
        /// Creates a data store
        /// </summary>
        /// <param name="dataStoreString">A connection-string style string describing the data store parameters</param>
        public void CreateDataStore(string dataStoreString)
        {
            NameValueCollection parameters = ExpressUtility.ConvertFromString(dataStoreString);
            using (ICreateDataStore create = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore) as ICreateDataStore)
            {
                foreach (string key in parameters.AllKeys)
                {
                    create.DataStoreProperties.SetProperty(key, parameters[key]);
                }
                create.Execute();
            }
        }

        /// <summary>
        /// Enumerates all the datastores in the current connection
        /// </summary>
        /// <param name="onlyFdoEnabled">If true, only fdo-enabled datastores are returned</param>
        /// <returns>A list of datastores</returns>
        public List<DataStoreInfo> ListDataStores(bool onlyFdoEnabled)
        {
            List<DataStoreInfo> stores = new List<DataStoreInfo>();
            using (IListDataStores dlist = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ListDataStores) as IListDataStores)
            {
                using (IDataStoreReader reader = dlist.Execute())
                {
                    while (reader.ReadNext())
                    {
                        if (onlyFdoEnabled)
                        {
                            if (reader.GetIsFdoEnabled())
                            {
                                DataStoreInfo dinfo = new DataStoreInfo(reader);
                                stores.Add(dinfo);
                            }
                        }
                        else
                        {
                            DataStoreInfo dinfo = new DataStoreInfo(reader);
                            stores.Add(dinfo);
                        }
                    }
                }
            }
            return stores;
        }
    }
}
