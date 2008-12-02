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
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Connections.Capabilities;
using OSGeo.FDO.Commands.SQL;
using OSGeo.FDO.Filter;
using OSGeo.FDO.Commands;
using Iesi.Collections.Generic;
using System.Collections.ObjectModel;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.Connections;

using Res = FdoToolbox.Core.ResourceUtil;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// IConnection wrapper service object.
    /// </summary>
    public class FdoFeatureService : IDisposable
    {
        private static IList<FdoProviderInfo> _providerInfo;

        /// <summary>
        /// Gets the list of providers from the provider registry
        /// </summary>
        /// <returns></returns>
        public static IList<FdoProviderInfo> GetProviders()
        {
            if (_providerInfo == null)
            {
                _providerInfo = new List<FdoProviderInfo>();

                ProviderCollection providers = FeatureAccessManager.GetProviderRegistry().GetProviders();
                foreach (Provider prov in providers)
                {
                    FdoProviderInfo pi = new FdoProviderInfo();
                    pi.Description = prov.Description;
                    pi.DisplayName = prov.DisplayName;
                    pi.FeatureDataObjectsVersion = prov.FeatureDataObjectsVersion;
                    pi.IsManaged = prov.IsManaged;
                    pi.LibraryPath = prov.LibraryPath;
                    pi.Name = prov.Name;
                    pi.Version = prov.Version;

                    IConnection conn = null;
                    try
                    {
                        conn = FeatureAccessManager.GetConnectionManager().CreateConnection(pi.Name);
                        pi.IsFlatFile = conn.ConnectionInfo.ProviderDatastoreType == ProviderDatastoreType.ProviderDatastoreType_File;
                        _providerInfo.Add(pi);
                    }
                    catch
                    { }
                    finally
                    {
                        if(conn != null)
                            conn.Dispose();
                    }
                }
            }
            return _providerInfo;
        }

        private static Dictionary<string, IList<DictionaryProperty>> _connectProperties = new Dictionary<string, IList<DictionaryProperty>>();
        
        /// <summary>
        /// Gets the parameters required to create a connection
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IList<DictionaryProperty> GetConnectProperties(string provider)
        {
            if (_connectProperties.ContainsKey(provider))
                return _connectProperties[provider];

            _connectProperties[provider] = new List<DictionaryProperty>();
            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
            IConnectionPropertyDictionary dict = conn.ConnectionInfo.ConnectionProperties;
            string [] names = dict.PropertyNames;
            foreach (string n in names)
            {
                DictionaryProperty p = null;
                if (dict.IsPropertyEnumerable(n))
                {
                    EnumerableDictionaryProperty ep = new EnumerableDictionaryProperty();
                    try
                    {
                        ep.Values = dict.EnumeratePropertyValues(n);
                        ep.RequiresConnection = false;
                    }
                    catch
                    {
                        ep.RequiresConnection = true;
                    }
                    p = ep;
                }
                else 
                {
                    p = new DictionaryProperty();
                }
                p.IsFile = dict.IsPropertyFileName(n);
                p.IsPath = dict.IsPropertyFilePath(n);
                p.DefaultValue = dict.GetPropertyDefault(n);
                p.LocalizedName = dict.GetLocalizedName(n);
                p.Name = n;
                p.Required = dict.IsPropertyRequired(n);
                p.Protected = dict.IsPropertyProtected(n);
                _connectProperties[provider].Add(p);
            }
            dict.Dispose();
            conn.Dispose();
            return _connectProperties[provider];
        }

        private static Dictionary<string, IList<DictionaryProperty>> _createDataStoreProperties = new Dictionary<string, IList<DictionaryProperty>>();
        private static Dictionary<string, IList<DictionaryProperty>> _destroyDataStoreProperties = new Dictionary<string, IList<DictionaryProperty>>();

        /// <summary>
        /// Gets the parameters required to create a datastore
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IList<DictionaryProperty> GetCreateDataStoreProperties(string provider)
        {
            if (_createDataStoreProperties.ContainsKey(provider))
                return _createDataStoreProperties[provider];

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
            if (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)CommandType.CommandType_CreateDataStore) >= 0)
            {
                _createDataStoreProperties[provider] = new List<DictionaryProperty>();
                using (ICreateDataStore create = conn.CreateCommand(CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                {
                    IDataStorePropertyDictionary dict = create.DataStoreProperties;
                    string[] names = dict.PropertyNames;
                    foreach (string n in names)
                    {
                        DictionaryProperty p = null;
                        if (dict.IsPropertyEnumerable(n))
                        {
                            EnumerableDictionaryProperty ep = new EnumerableDictionaryProperty();
                            ep.Values = dict.EnumeratePropertyValues(n);
                            p = ep;
                        }
                        else
                        {
                            p = new DictionaryProperty();
                        }
                        p.IsFile = dict.IsPropertyFileName(n);
                        p.IsPath = dict.IsPropertyFilePath(n);
                        p.DefaultValue = dict.GetPropertyDefault(n);
                        p.LocalizedName = dict.GetLocalizedName(n);
                        p.Name = n;
                        p.Required = dict.IsPropertyRequired(n);
                        p.Protected = dict.IsPropertyProtected(n);
                        _createDataStoreProperties[provider].Add(p);
                    }
                    dict.Dispose();
                }
                conn.Dispose();
                return _createDataStoreProperties[provider];
            }
            return null;
        }

        /// <summary>
        /// Gets the parameters required to destory a data store
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IList<DictionaryProperty> GetDestroyDataStoreProperties(string provider)
        {
            if (_destroyDataStoreProperties.ContainsKey(provider))
                return _destroyDataStoreProperties[provider];

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(provider);
            if (Array.IndexOf<int>(conn.CommandCapabilities.Commands, (int)CommandType.CommandType_CreateDataStore) >= 0)
            {
                _destroyDataStoreProperties[provider] = new List<DictionaryProperty>();
                using (IDestroyDataStore destroy = conn.CreateCommand(CommandType.CommandType_DestroyDataStore) as IDestroyDataStore)
                {
                    IDataStorePropertyDictionary dict = destroy.DataStoreProperties;
                    string[] names = dict.PropertyNames;
                    foreach (string n in names)
                    {
                        DictionaryProperty p = null;
                        if (dict.IsPropertyEnumerable(n))
                        {
                            EnumerableDictionaryProperty ep = new EnumerableDictionaryProperty();
                            ep.Values = dict.EnumeratePropertyValues(n);
                            p = ep;
                        }
                        else
                        {
                            p = new DictionaryProperty();
                        }
                        p.IsFile = dict.IsPropertyFileName(n);
                        p.IsPath = dict.IsPropertyFilePath(n);
                        p.DefaultValue = dict.GetPropertyDefault(n);
                        p.LocalizedName = dict.GetLocalizedName(n);
                        p.Name = n;
                        p.Required = dict.IsPropertyRequired(n);
                        p.Protected = dict.IsPropertyProtected(n);
                        _createDataStoreProperties[provider].Add(p);
                    }
                    dict.Dispose();
                }
                conn.Dispose();
                return _destroyDataStoreProperties[provider];
            }
            return null;
        }

        private IConnection _conn;

        private FgfGeometryFactory _GeomFactory;

        /// <summary>
        /// Gets the FDO Geometry factory instance
        /// </summary>
        public FgfGeometryFactory GeometryFactory
        {
            get { return _GeomFactory; }
        }

        /// <summary>
        /// Constructor. The passed connection must already be open.
        /// </summary>
        /// <param name="conn"></param>
        public FdoFeatureService(IConnection conn)
        {
            if (conn.ConnectionState != ConnectionState.ConnectionState_Open)
                throw new FeatureServiceException(Res.GetString("ERR_CONNECTION_NOT_OPEN"));
            _conn = conn;
            _GeomFactory = new FgfGeometryFactory();
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~FdoFeatureService()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _GeomFactory.Dispose();
            }
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            if (fs == null)
                throw new ArgumentNullException("fs");

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

        public static ClassDefinition CloneClass(ClassDefinition cd)
        {
            ClassDefinition classDef = null;
            switch (cd.ClassType)
            {
                case ClassType.ClassType_Class:
                    {
                        Class c = new Class(cd.Name, cd.Description);
                        CopyProperties(cd.Properties, c.Properties);
                        CopyIdentityProperties(cd.IdentityProperties, c.IdentityProperties);
                        CopyClassAttributes(cd.Attributes, c.Attributes);
                        classDef = c;
                    }
                    break;
                case ClassType.ClassType_FeatureClass:
                    {
                        FeatureClass sfc = (FeatureClass)cd;
                        FeatureClass fc = new FeatureClass(cd.Name, cd.Description);
                        CopyProperties(cd.Properties, fc.Properties);
                        CopyIdentityProperties(cd.IdentityProperties, fc.IdentityProperties);
                        if (sfc.GeometryProperty != null)
                        {
                            string geomName = sfc.GeometryProperty.Name;
                            fc.GeometryProperty = fc.Properties[fc.Properties.IndexOf(geomName)] as GeometricPropertyDefinition;
                        }
                        classDef = fc;
                    }
                    break;
                default:
                    throw new UnsupportedException(Res.GetStringFormatted("ERR_UNSUPPORTED_CLONE_CLASS_TYPE", cd.ClassType));
            }
            return classDef;
        }

        private static void CopyClassAttributes(SchemaAttributeDictionary srcAttributes, SchemaAttributeDictionary targetAttributes)
        {
            foreach (string attr in srcAttributes.AttributeNames)
            {
                targetAttributes.SetAttributeValue(attr, srcAttributes.GetAttributeValue(attr));
            }
        }

        private static void CopyIdentityProperties(DataPropertyDefinitionCollection srcProperties, DataPropertyDefinitionCollection targetProperties)
        {
            foreach (PropertyDefinition propDef in srcProperties)
            {
                targetProperties.Add(CloneProperty(propDef) as DataPropertyDefinition);
            }
        }

        private static void CopyProperties(PropertyDefinitionCollection srcProperties, PropertyDefinitionCollection targetProperties)
        {
            foreach (PropertyDefinition propDef in srcProperties)
            {
                targetProperties.Add(CloneProperty(propDef));
            }
        }

        /// <summary>
        /// Gets the names of all schemas in this connection
        /// </summary>
        /// <returns></returns>
        public List<string> GetSchemaNames()
        {
            List<string> schemaNames = new List<string>();
            if (SupportsPartialSchemaDiscovery())
            {
                using (IGetSchemaNames getnames = Connection.CreateCommand(CommandType.CommandType_GetSchemaNames) as IGetSchemaNames)
                {
                    OSGeo.FDO.Common.StringCollection names = getnames.Execute();
                    foreach (OSGeo.FDO.Common.StringElement sn in names)
                    {
                        schemaNames.Add(sn.String);
                    }
                }
            }
            else
            {
                FeatureSchemaCollection schemas = DescribeSchema();
                foreach (FeatureSchema fs in schemas)
                {
                    schemaNames.Add(fs.Name);
                }
            }
            return schemaNames;
        }

        /// <summary>
        /// Gets the names of all classes for a given schema
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public List<string> GetClassNames(string schemaName)
        {
            List<string> classNames = new List<string>();
            if (SupportsPartialSchemaDiscovery())
            {
                using (IGetClassNames getnames = Connection.CreateCommand(CommandType.CommandType_GetClassNames) as IGetClassNames)
                {
                    getnames.SchemaName = schemaName;
                    OSGeo.FDO.Common.StringCollection names = getnames.Execute();
                    foreach (OSGeo.FDO.Common.StringElement sn in names)
                    {
                        classNames.Add(sn.String);
                    }
                }
            }
            else
            {
                FeatureSchema schema = GetSchemaByName(schemaName);
                if (schema != null)
                {
                    foreach (ClassDefinition cd in schema.Classes)
                    {
                        classNames.Add(cd.Name);
                    }
                }
            }
            return classNames;
        }

        /// <summary>
        /// Clones a given property definition
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public static PropertyDefinition CloneProperty(PropertyDefinition pd)
        {
            PropertyDefinition propDef = null;
            switch (pd.PropertyType)
            {
                case PropertyType.PropertyType_DataProperty:
                    {
                        DataPropertyDefinition srcData = pd as DataPropertyDefinition;
                        DataPropertyDefinition dataDef = new DataPropertyDefinition(srcData.Name, srcData.Description);
                        dataDef.DataType = srcData.DataType;
                        dataDef.DefaultValue = srcData.DefaultValue;
                        dataDef.IsAutoGenerated = srcData.IsAutoGenerated;
                        dataDef.IsSystem = srcData.IsSystem;
                        dataDef.Length = srcData.Length;
                        dataDef.Nullable = srcData.Nullable;
                        dataDef.Precision = srcData.Precision;
                        dataDef.ReadOnly = srcData.ReadOnly;
                        //Copy constraints
                        if (srcData.ValueConstraint != null)
                        {
                            if (srcData.ValueConstraint.ConstraintType == PropertyValueConstraintType.PropertyValueConstraintType_Range)
                            {
                                PropertyValueConstraintRange range = (PropertyValueConstraintRange)srcData.ValueConstraint;
                                PropertyValueConstraintRange constraint = new PropertyValueConstraintRange(range.MinValue, range.MaxValue);
                                constraint.MaxInclusive = range.MaxInclusive;
                                constraint.MinInclusive = range.MinInclusive;
                                dataDef.ValueConstraint = constraint;
                            }
                            else if (srcData.ValueConstraint.ConstraintType == PropertyValueConstraintType.PropertyValueConstraintType_List)
                            {
                                PropertyValueConstraintList list = (PropertyValueConstraintList)srcData.ValueConstraint;
                                PropertyValueConstraintList constraint = new PropertyValueConstraintList();
                                foreach (DataValue dval in list.ConstraintList)
                                {
                                    constraint.ConstraintList.Add(dval);
                                }
                                dataDef.ValueConstraint = constraint;
                            }
                        }
                        CopyPropertyAttributes(srcData, dataDef);
                        propDef = dataDef;
                    }
                    break;
                case PropertyType.PropertyType_GeometricProperty:
                    {
                        GeometricPropertyDefinition srcData = pd as GeometricPropertyDefinition;
                        GeometricPropertyDefinition geomDef = new GeometricPropertyDefinition(srcData.Name, srcData.Description);
                        geomDef.GeometryTypes = srcData.GeometryTypes;
                        geomDef.HasElevation = srcData.HasElevation;
                        geomDef.HasMeasure = srcData.HasMeasure;
                        geomDef.IsSystem = srcData.IsSystem;
                        geomDef.ReadOnly = srcData.ReadOnly;
                        geomDef.SpatialContextAssociation = srcData.SpatialContextAssociation;
                        CopyPropertyAttributes(srcData, geomDef);
                        propDef = geomDef;
                    }
                    break;
                case PropertyType.PropertyType_RasterProperty:
                    {
                        RasterPropertyDefinition srcData = pd as RasterPropertyDefinition;
                        RasterPropertyDefinition rastDef = new RasterPropertyDefinition(srcData.Name, srcData.Description);
                        if (srcData.DefaultDataModel != null)
                        {
                            rastDef.DefaultDataModel = new OSGeo.FDO.Raster.RasterDataModel();
                            rastDef.DefaultDataModel.BitsPerPixel = srcData.DefaultDataModel.BitsPerPixel;
                            rastDef.DefaultDataModel.DataModelType = srcData.DefaultDataModel.DataModelType;
                            rastDef.DefaultDataModel.DataType = srcData.DefaultDataModel.DataType;
                            rastDef.DefaultDataModel.Organization = srcData.DefaultDataModel.Organization;
                            rastDef.DefaultDataModel.TileSizeX = srcData.DefaultDataModel.TileSizeX;
                            rastDef.DefaultDataModel.TileSizeY = srcData.DefaultDataModel.TileSizeY;
                        }
                        rastDef.DefaultImageXSize = srcData.DefaultImageXSize;
                        rastDef.DefaultImageYSize = srcData.DefaultImageYSize;
                        rastDef.IsSystem = srcData.IsSystem;
                        rastDef.Nullable = srcData.Nullable;
                        rastDef.ReadOnly = srcData.ReadOnly;
                        rastDef.SpatialContextAssociation = srcData.SpatialContextAssociation;
                        CopyPropertyAttributes(srcData, rastDef);
                        propDef = rastDef;
                    }
                    break;
                case PropertyType.PropertyType_AssociationProperty:
                    throw new UnsupportedException(Res.GetString("ERR_UNSUPPORTED_CLONE_ASSOCIATION"));
                case PropertyType.PropertyType_ObjectProperty:
                    throw new UnsupportedException(Res.GetString("ERR_UNSUPPORTED_CLONE_OBJECT"));
                    
            }
            return propDef;
        }

        private static void CopyPropertyAttributes(PropertyDefinition srcData, PropertyDefinition target)
        {
            //Copy attributes
            if (srcData.Attributes != null)
            {
                foreach (string attr in srcData.Attributes.AttributeNames)
                {
                    target.Attributes.SetAttributeValue(attr, srcData.Attributes.GetAttributeValue(attr));
                }
            }
        }

        /// <summary>
        /// Gets the number of features in a given class definition
        /// </summary>
        /// <param name="classDef"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public long GetFeatureCount(ClassDefinition classDef, string filter)
        {
            long count = 0;
            string className = classDef.Name;
            string property = "FEATURECOUNT";
            if (SupportsCommand(CommandType.CommandType_SQLCommand))
            {
                using (ISQLCommand cmd = CreateCommand<ISQLCommand>(CommandType.CommandType_SQLCommand))
                {
                    string sql = string.Empty;
                    sql = string.Format("SELECT COUNT(*) AS {0} FROM {1}", property, className);
                    if (!string.IsNullOrEmpty(filter))
                        sql += " WHERE " + filter;

                    cmd.SQLStatement = sql;

                    using (ISQLDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.ReadNext())
                        {
                            count = reader.GetInt64(property);
                        }
                        reader.Close();
                    }
                }
            }
            else if (SupportsCommand(CommandType.CommandType_SelectAggregates))
            {
                using (ISelectAggregates select = CreateCommand<ISelectAggregates>(CommandType.CommandType_SelectAggregates))
                {
                    select.SetFeatureClassName(className);
                    if (!string.IsNullOrEmpty(filter))
                        select.Filter = Filter.Parse(filter);
                    select.PropertyNames.Add(new ComputedIdentifier(property, Expression.Parse("COUNT(" + classDef.IdentityProperties[0].Name + ")")));

                    using (IDataReader reader = select.Execute())
                    {
                        if (reader.ReadNext() && !reader.IsNull(property))
                        {
                            count = reader.GetInt64(property);
                        }
                        reader.Close();
                    }
                }
            }
            return count;
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
                schema.WriteXml(xmlFile);
            }
            else
            {
                throw new FeatureServiceException(Res.GetStringFormatted("ERR_SCHEMA_NOT_FOUND", schemaName));
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

            if (SupportsPartialSchemaDiscovery())
            {
                FeatureSchemaCollection schemas = null;
                using (IDescribeSchema describe = Connection.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    describe.SchemaName = schemaName;
                    schemas = describe.Execute();
                }
                if (schemas != null && schemas.Count == 1)
                    return schemas[0];
            }
            else
            {
                FeatureSchemaCollection schemas = DescribeSchema();

                foreach (FeatureSchema schema in schemas)
                {
                    if (schema.Name == schemaName)
                        return schema;
                }
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

            if (SupportsPartialSchemaDiscovery())
            {
                ClassDefinition classDef = null;
                using (IDescribeSchema describe = Connection.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    describe.SchemaName = schemaName;
                    describe.ClassNames.Add(new OSGeo.FDO.Common.StringElement(className));
                    FeatureSchemaCollection schemas = describe.Execute();
                    if (schemas != null)
                        classDef = schemas[0].Classes[0];
                }
            }
            else
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
        public ReadOnlyCollection<SpatialContextInfo> GetSpatialContexts()
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
            return contexts.AsReadOnly();
        }

        /// <summary>
        /// Gets a spatial context by name
        /// </summary>
        /// <param name="name">The name of the spatial context</param>
        /// <returns>The spatial context information if found. null if otherwise</returns>
        public SpatialContextInfo GetSpatialContext(string name)
        {
            ReadOnlyCollection<SpatialContextInfo> contexts = GetSpatialContexts();
            foreach (SpatialContextInfo info in contexts)
            {
                if (info.Name == name)
                    return info;
            }
            return null;
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
                if (create.ExtentType == SpatialContextExtentType.SpatialContextExtentType_Static || !string.IsNullOrEmpty(ctx.ExtentGeometryText))
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
        public ReadOnlyCollection<DataStoreInfo> ListDataStores(bool onlyFdoEnabled)
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
            return new ReadOnlyCollection<DataStoreInfo>(stores);
        }

        /// <summary>
        /// Computes the minimum envelope (bounding box) for the given list
        /// of feature classes
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        public IEnvelope ComputeEnvelope(IEnumerable<ClassDefinition> classes)
        {
            double? maxx = null;
            double? maxy = null;
            double? minx = null;
            double? miny = null;

            IEnvelope computedEnvelope = null;

            //Use brute-force instead of SpatialExtents() as there
            //is no guarantee that every provider will implement that
            //expression function
            foreach (ClassDefinition classDef in classes)
            {
                if (classDef.ClassType == ClassType.ClassType_FeatureClass)
                {
                    using (ISelect select = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select) as ISelect)
                    {
                        string propertyName = ((FeatureClass)classDef).GeometryProperty.Name;
                        select.SetFeatureClassName(classDef.Name);
                        select.PropertyNames.Clear();
                        select.PropertyNames.Add((Identifier)Identifier.Parse(propertyName));
                        using (IFeatureReader reader = select.Execute())
                        {
                            while (reader.ReadNext())
                            {
                                if (!reader.IsNull(propertyName))
                                {
                                    byte[] bGeom = reader.GetGeometry(propertyName);
                                    IGeometry geom = _GeomFactory.CreateGeometryFromFgf(bGeom);
                                    IEnvelope env = geom.Envelope;
                                    if (!maxx.HasValue || env.MaxX > maxx)
                                        maxx = env.MaxX;
                                    if (!maxy.HasValue || env.MaxY > maxy)
                                        maxy = env.MaxY;
                                    if (!minx.HasValue || env.MinX < minx)
                                        minx = env.MinX;
                                    if (!miny.HasValue || env.MinY < miny)
                                        miny = env.MinY;
                                    env.Dispose();
                                    geom.Dispose();
                                }
                            }
                        }
                    }
                }
            }

            if ((maxx.HasValue) && (maxy.HasValue) && (minx.HasValue) && (miny.HasValue))
            {
                computedEnvelope = _GeomFactory.CreateEnvelopeXY(minx.Value, miny.Value, maxx.Value, maxy.Value);
            }

            return computedEnvelope;
        }

        /// <summary>
        /// Returns true if this connection supports batch insertion. Returns false if otherwise.
        /// </summary>
        /// <returns></returns>
        public bool SupportsBatchInsertion()
        {
            //This bombs on PostGIS, must be something to do with the refcounting.
            bool supported = false;
            using (IInsert insert = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert) as IInsert)
            {
                supported = (insert.BatchParameterValues != null);
            }
            return supported;
        }

        /// <summary>
        /// Creates a FDO command 
        /// </summary>
        /// <typeparam name="T">The FDO command reference to create. This must match the command type specified by the <paramref name="commandType"/> parameter</typeparam>
        /// <param name="commandType">The type of FDO commadn to create</param>
        /// <returns></returns>
        public T CreateCommand<T>(OSGeo.FDO.Commands.CommandType commandType) where T : class, OSGeo.FDO.Commands.ICommand
        {
            return _conn.CreateCommand(commandType) as T;
        }

        public FeatureSchema AlterSchema(FeatureSchema schema, IncompatibleSchema incompatibleSchema)
        {
            //Clone the incoming schema so we can work with the cloned copy
            FeatureSchema altSchema = CloneSchema(schema);
            
            //Process each incompatible class
            foreach (IncompatibleClass incClass in incompatibleSchema.Classes)
            {
                int cidx = altSchema.Classes.IndexOf(incClass.Name);
                if (cidx >= 0)
                {
                    ClassDefinition classDef = altSchema.Classes[cidx];
                    //Process each incompatible property
                    foreach (IncompatibleProperty incProp in incClass.Properties)
                    {
                        int pidx = classDef.Properties.IndexOf(incProp.Name);
                        if (pidx >= 0)
                        {
                            PropertyDefinition prop = classDef.Properties[pidx];
                            AlterProperty(ref classDef, ref prop, incProp.ReasonCodes);
                        }
                        else
                        {
                            throw new FeatureServiceException(Res.GetStringFormatted("ERR_INCOMPATIBLE_PROPERTY_NOT_FOUND", incProp.Name, incClass.Name));
                        }
                    }
                    AlterClass(ref classDef, incClass.ReasonCodes);
                    //Finally make sure the data properties lie within the limits of this
                    //connection
                    FixDataProperties(ref classDef);
                }
                else
                {
                    throw new FeatureServiceException(Res.GetStringFormatted("ERR_INCOMPATIBLE_CLASS_NOT_FOUND", incClass.Name));
                }
            }

            return altSchema;
        }

        private void AlterClass(ref ClassDefinition classDef, ISet<IncompatibleClassReason> reasons)
        {
            if (reasons.Count == 0)
                return;

            foreach (IncompatibleClassReason reason in reasons)
            {
                switch (reason)
                {
                    case IncompatibleClassReason.UnsupportedAutoProperties:
                        {
                            //AlterProperty() should have dealt with this
                        }
                        break;
                    case IncompatibleClassReason.UnsupportedClassType:
                        {
                            throw new FeatureServiceException(Res.GetString("ERR_UNABLE_TO_CONVERT_CLASS"));
                        }
                    case IncompatibleClassReason.UnsupportedCompositeKeys:
                        {
                            //Remove identity properties and replace with an auto-generated property
                            DataPropertyDefinition id = new DataPropertyDefinition("Autogenerated_ID", "");
                            DataType[] idTypes = new DataType[this.Connection.SchemaCapabilities.SupportedAutoGeneratedTypes.Length];
                            Array.Copy(this.Connection.SchemaCapabilities.SupportedAutoGeneratedTypes, idTypes, idTypes.Length);
                            Array.Sort<DataType>(idTypes, new DataTypeComparer());
                            id.DataType = idTypes[idTypes.Length - 1];
                            id.IsAutoGenerated = true;

                            classDef.IdentityProperties.Clear();
                            classDef.Properties.Add(id);
                            classDef.IdentityProperties.Add(id);
                        }
                        break;
                    case IncompatibleClassReason.UnsupportedInheritance:
                        {
                            //Move base class properties to derived, prefix properties with BASE_
                            ClassDefinition baseClass = classDef.BaseClass;
                            List<PropertyDefinition> properties = new List<PropertyDefinition>();
                            List<DataPropertyDefinition> ids = new List<DataPropertyDefinition>();
                            foreach (PropertyDefinition propDef in baseClass.Properties)
                            {
                                DataPropertyDefinition dp = propDef as DataPropertyDefinition;
                                PropertyDefinition pd = CloneProperty(propDef);
                                pd.Name = "BASE_" + pd.Name;
                                properties.Add(pd);
                                if (dp != null && baseClass.Properties.Contains(dp))
                                    ids.Add(pd as DataPropertyDefinition);
                            }
                            classDef.BaseClass = null;
                            foreach (PropertyDefinition pd in properties)
                            {
                                classDef.Properties.Add(pd);
                            }
                            foreach (DataPropertyDefinition id in ids)
                            {
                                classDef.IdentityProperties.Add(id);
                            }
                        }
                        break;
                }
            }
        }

        private void AlterProperty(ref ClassDefinition classDef, ref PropertyDefinition prop, ISet<IncompatiblePropertyReason> reasons)
        {
            if (reasons.Count == 0)
                return;

            DataPropertyDefinition dp = prop as DataPropertyDefinition;
            foreach (IncompatiblePropertyReason reason in reasons)
            {
                switch (reason)
                { 
                    case IncompatiblePropertyReason.UnsupportedAssociationProperties:
                        //Can't do anything here
                        throw new FeatureServiceException(Res.GetString("ERR_CANNOT_ALTER_ASSOCIATION"));
                    case IncompatiblePropertyReason.UnsupportedDataType:
                        {
                            //Try to promote data type
                            DataType dt = GetPromotedDataType(dp.DataType, this.Connection.SchemaCapabilities.DataTypes);
                            dp.DataType = dt;
                            if (dp.DataType == DataType.DataType_BLOB ||
                                dp.DataType == DataType.DataType_CLOB ||
                                dp.DataType == DataType.DataType_String)
                            {
                                dp.Length = (int)this.Connection.SchemaCapabilities.get_MaximumDataValueLength(dp.DataType);
                            }
                            if (dp.DataType == DataType.DataType_Decimal)
                            {
                                dp.Scale = this.Connection.SchemaCapabilities.MaximumDecimalScale;
                                dp.Precision = this.Connection.SchemaCapabilities.MaximumDecimalPrecision;
                            }
                        }
                        break;
                    case IncompatiblePropertyReason.UnsupportedAutoGeneratedType:
                        {
                            //Try to promote data type
                            DataType dt = GetPromotedDataType(dp.DataType, this.Connection.SchemaCapabilities.DataTypes);
                            dp.DataType = dt;
                            if (dp.DataType == DataType.DataType_BLOB ||
                                dp.DataType == DataType.DataType_CLOB ||
                                dp.DataType == DataType.DataType_String)
                            {
                                dp.Length = (int)this.Connection.SchemaCapabilities.get_MaximumDataValueLength(dp.DataType);
                            }
                            if (dp.DataType == DataType.DataType_Decimal)
                            {
                                dp.Scale = this.Connection.SchemaCapabilities.MaximumDecimalScale;
                                dp.Precision = this.Connection.SchemaCapabilities.MaximumDecimalPrecision;
                            }
                        }
                        break;
                    case IncompatiblePropertyReason.UnsupportedDefaultValues:
                        {
                            //Remove default value
                            dp.DefaultValue = string.Empty;
                        }
                        break;
                    case IncompatiblePropertyReason.UnsupportedExclusiveValueRangeConstraints:
                        {
                            //Remove constraint
                            dp.ValueConstraint = null;
                        }
                        break;
                    case IncompatiblePropertyReason.UnsupportedIdentityProperty:
                        {
                            //Remove from identity property list
                            classDef.IdentityProperties.Remove(dp);   
                        }
                        break;
                    case IncompatiblePropertyReason.UnsupportedIdentityPropertyType:
                        {
                            //Try to promote data type
                            DataType dt = GetPromotedDataType(dp.DataType, this.Connection.SchemaCapabilities.SupportedIdentityPropertyTypes);
                            dp.DataType = dt;
                            if (dp.DataType == DataType.DataType_BLOB ||
                                dp.DataType == DataType.DataType_CLOB ||
                                dp.DataType == DataType.DataType_String)
                            {
                                dp.Length = (int)this.Connection.SchemaCapabilities.get_MaximumDataValueLength(dp.DataType);
                            }
                            if (dp.DataType == DataType.DataType_Decimal)
                            {
                                dp.Scale = this.Connection.SchemaCapabilities.MaximumDecimalScale;
                                dp.Precision = this.Connection.SchemaCapabilities.MaximumDecimalPrecision;
                            }
                        }
                        break;
                    case IncompatiblePropertyReason.UnsupportedInclusiveValueRangeConstraints:
                        {
                            //Remove constraint
                            dp.ValueConstraint = null;
                        }
                        break;
                    case IncompatiblePropertyReason.UnsupportedNullValueConstraints:
                        {
                            //Make nullable = false
                            dp.Nullable = false;
                        }
                        break;
                    case IncompatiblePropertyReason.UnsupportedObjectProperties:
                        //Can't do anything here
                        throw new FeatureServiceException(Res.GetString("ERR_CANNOT_ALTER_OBJECT"));
                    case IncompatiblePropertyReason.UnsupportedUniqueValueConstraints:
                        {
                            //Remove constraint
                            dp.ValueConstraint = null;
                        }
                        break;
                    case IncompatiblePropertyReason.UnsupportedValueListConstraints:
                        {
                            //Remove constraint
                            dp.ValueConstraint = null;
                        }
                        break;
                }
            }
        }

        public static DataType GetPromotedDataType(DataType dataType, DataType [] dataTypes)
        {
            DataType? dt = null;
            switch (dataType)
            {
                case DataType.DataType_BLOB:
                    throw new FeatureServiceException(Res.GetString("ERR_CANNOT_PROMOTE_BLOB"));
                case DataType.DataType_Boolean:
                    {
                        if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Byte) >= 0)
                            dt = DataType.DataType_Byte;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Int16) >= 0)
                            dt = DataType.DataType_Int16;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Int32) >= 0)
                            dt = DataType.DataType_Int32;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Int64) >= 0)
                            dt = DataType.DataType_Int64;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_String) >= 0)
                            dt = DataType.DataType_String;
                    }
                    break;
                case DataType.DataType_Byte:
                    {
                        if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Int16) >= 0)
                            dt = DataType.DataType_Int16;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Int32) >= 0)
                            dt = DataType.DataType_Int32;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Int64) >= 0)
                            dt = DataType.DataType_Int64;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_String) >= 0)
                            dt = DataType.DataType_String;
                    }
                    break;
                case DataType.DataType_CLOB:
                    throw new FeatureServiceException(Res.GetString("ERR_CANNOT_PROMOTE_CLOB"));
                case DataType.DataType_DateTime:
                    {
                        if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_String) >= 0)
                            dt = DataType.DataType_String;
                    }
                    break;
                case DataType.DataType_Decimal:
                    {
                        if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Double) >= 0)
                            dt = DataType.DataType_Double;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_String) >= 0)
                            dt = DataType.DataType_String;
                    }
                    break;
                case DataType.DataType_Double:
                    throw new FeatureServiceException(Res.GetString("ERR_CANNOT_PROMOTE_DOUBLE"));
                case DataType.DataType_Int16:
                    {
                        if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Int32) >= 0)
                            dt = DataType.DataType_Int32;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Int64) >= 0)
                            dt = DataType.DataType_Int64;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_String) >= 0)
                            dt = DataType.DataType_String;
                    }
                    break;
                case DataType.DataType_Int32:
                    {
                        if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Int64) >= 0)
                            dt = DataType.DataType_Int64;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_String) >= 0)
                            dt = DataType.DataType_String;
                    }
                    break;
                case DataType.DataType_Int64:
                    {
                        if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_String) >= 0)
                            dt = DataType.DataType_String;
                    }
                    break;
                case DataType.DataType_Single:
                    {
                        if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_Double) >= 0)
                            dt = DataType.DataType_Double;
                        else if (Array.IndexOf<DataType>(dataTypes, DataType.DataType_String) >= 0)
                            dt = DataType.DataType_String;
                    }
                    break;
                case DataType.DataType_String:
                    throw new FeatureServiceException(Res.GetString("ERR_CANNOT_PROMOTE_STRINGS"));
            }

            if (!dt.HasValue)
                throw new FeatureServiceException(Res.GetStringFormatted("ERR_UNSUITABLE_DATA_TYPE_REPLACEMENT", dataType));

            return dt.Value;
        }

        /// <summary>
        /// Returns true if the given schema can be applied to this connection
        /// Returns false if otherwise.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public bool CanApplySchema(FeatureSchema schema, out IncompatibleSchema incSchema)
        {
            incSchema = null;
            ISchemaCapabilities capabilities = _conn.SchemaCapabilities;
            foreach (ClassDefinition classDef in schema.Classes)
            {
                string className = classDef.Name;
                ClassType ctype = classDef.ClassType;
                IncompatibleClass cls = null;
                if (Array.IndexOf<ClassType>(capabilities.ClassTypes, ctype) < 0)
                {
                    string classReason = "Un-supported class type";
                    AddIncompatibleClass(className, ref cls, classReason, IncompatibleClassReason.UnsupportedClassType);
                }
                if (!capabilities.SupportsCompositeId && classDef.IdentityProperties.Count > 1)
                {
                    string classReason = "Multiple identity properties (composite id) not supported"; 
                    AddIncompatibleClass(className, ref cls, classReason, IncompatibleClassReason.UnsupportedCompositeKeys);
                }
                if (!capabilities.SupportsInheritance && classDef.BaseClass != null)
                {
                    string classReason = "Class inherits from a base class (inheritance not supported)";
                    AddIncompatibleClass(className, ref cls, classReason, IncompatibleClassReason.UnsupportedInheritance);
                }
                foreach (PropertyDefinition propDef in classDef.Properties)
                {
                    string propName = propDef.Name;
                    DataPropertyDefinition dataDef = propDef as DataPropertyDefinition;
                    //AssociationPropertyDefinition assocDef = propDef as AssociationPropertyDefinition;
                    //GeometricPropertyDefinition geomDef = propDef as GeometricPropertyDefinition;
                    //RasterPropertyDefinition rasterDef = propDef as RasterPropertyDefinition;
                    ObjectPropertyDefinition objDef = propDef as ObjectPropertyDefinition;
                    if (!capabilities.SupportsAutoIdGeneration)
                    {
                        if (dataDef != null && dataDef.IsAutoGenerated)
                        {
                            string classReason = "Class has unsupported auto-generated properties";
                            string propReason = "Unsupported auto-generated id";

                            AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedIdentityProperty);
                        }
                    }
                    else
                    {
                        if (dataDef != null && dataDef.IsAutoGenerated)
                        {
                            if (Array.IndexOf<DataType>(capabilities.SupportedAutoGeneratedTypes, dataDef.DataType) < 0)
                            {
                                string classReason = "Class has unsupported auto-generated data type";
                                string propReason = "Unsupported auto-generated data type: " + dataDef.DataType;
                                AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedAutoGeneratedType);
                            }
                        }
                    }
                    if (dataDef != null && classDef.IdentityProperties.Contains(dataDef))
                    {
                        if (Array.IndexOf<DataType>(capabilities.SupportedIdentityPropertyTypes, dataDef.DataType) < 0)
                        {
                            string classReason = "Class has unsupported identity property data type";
                            string propReason = "Unsupported identity property data type";
                            AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedIdentityPropertyType);
                        }
                    }
                    if (!capabilities.SupportsAssociationProperties)
                    {
                        if (propDef.PropertyType == PropertyType.PropertyType_AssociationProperty)
                        {
                            string classReason = "Class has unsupported association properties";
                            string propReason = "Unsupported association property type";
                            
                            AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedAssociationProperties);
                        }
                    }
                    if (!capabilities.SupportsDefaultValue)
                    {
                        if (dataDef != null && !string.IsNullOrEmpty(dataDef.DefaultValue))
                        {
                            string classReason = "Class has properties with unsupported default values";
                            string propReason = "Default values not supported";
                            
                            AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedDefaultValues);
                        }
                    }
                    if (!capabilities.SupportsExclusiveValueRangeConstraints)
                    {
                        if (dataDef != null && dataDef.ValueConstraint != null && dataDef.ValueConstraint.ConstraintType == PropertyValueConstraintType.PropertyValueConstraintType_Range)
                        {
                            PropertyValueConstraintRange range = dataDef.ValueConstraint as PropertyValueConstraintRange;
                            if (!range.MaxInclusive && !range.MinInclusive)
                            {
                                string classReason = "Class has properties with unsupported exclusive range constraints";
                                string propReason = "Exclusive range constraint not supported";

                                AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedExclusiveValueRangeConstraints);
                            }
                        }
                    }
                    if (!capabilities.SupportsInclusiveValueRangeConstraints)
                    {
                        if (dataDef != null && dataDef.ValueConstraint != null && dataDef.ValueConstraint.ConstraintType == PropertyValueConstraintType.PropertyValueConstraintType_Range)
                        {
                            PropertyValueConstraintRange range = dataDef.ValueConstraint as PropertyValueConstraintRange;
                            if (range.MaxInclusive && range.MinInclusive)
                            {
                                string classReason = "Class has properties with unsupported inclusive range constraints";
                                string propReason = "Inclusive range constraint not supported";

                                AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedInclusiveValueRangeConstraints);
                            }
                        }
                    }
                    if (!capabilities.SupportsNullValueConstraints)
                    {
                        if (dataDef != null && dataDef.Nullable)
                        {
                            string classReason = "Class has unsupported nullable properties";
                            string propReason = "Null value constraints not supported";

                            AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedNullValueConstraints);
                        }
                    }
                    if (!capabilities.SupportsObjectProperties)
                    {
                        if (objDef != null)
                        {
                            string classReason = "Class has unsupported object properties";
                            string propReason = "Object properties not supported";

                            AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedObjectProperties);
                        }
                    }
                    if (!capabilities.SupportsUniqueValueConstraints)
                    {
                        //
                    }
                    if (!capabilities.SupportsValueConstraintsList)
                    {
                        if (dataDef != null && dataDef.ValueConstraint != null && dataDef.ValueConstraint.ConstraintType == PropertyValueConstraintType.PropertyValueConstraintType_List)
                        {
                            string classReason = "Class has properties with unsupported value list constraints";
                            string propReason = "value list constraints not supported";

                            AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedValueListConstraints);
                        }
                    }

                    if (dataDef != null)
                    {
                        if (Array.IndexOf<DataType>(capabilities.DataTypes, dataDef.DataType) < 0)
                        {
                            string classReason = "Class has properties with unsupported data type: " + dataDef.DataType;
                            string propReason = "Unsupported data type: " + dataDef.DataType;

                            AddIncompatibleProperty(className, ref cls, propName, classReason, propReason, IncompatiblePropertyReason.UnsupportedDataType);
                        }
                    }
                }
                
                if (cls != null)
                {
                    if (incSchema == null)
                        incSchema = new IncompatibleSchema(schema.Name);

                    incSchema.AddClass(cls);
                }
            }

            return (incSchema == null);
        }

        private static void AddIncompatibleClass(string className, ref IncompatibleClass cls, string classReason, IncompatibleClassReason rcode)
        {
            if (cls == null)
                cls = new IncompatibleClass(className, classReason);
            else
                cls.AddReason(classReason);

            cls.ReasonCodes.Add(rcode);
        }

        private static void AddIncompatibleProperty(string className, ref IncompatibleClass cls, string propName, string classReason, string propReason, IncompatiblePropertyReason rcode)
        {
            if (cls == null)
                cls = new IncompatibleClass(className, classReason);
            else
                cls.AddReason(classReason);

            IncompatibleProperty prop = cls.FindProperty(propName);
            if (prop == null)
            {
                prop = new IncompatibleProperty(propName, propReason);
                prop.ReasonCodes.Add(rcode);
                cls.AddProperty(prop);
            }
            else
            {
                prop.AddReason(propReason);
                prop.ReasonCodes.Add(rcode);
            }
        }

        /// <summary>
        /// Selects features from this connection according to the criteria set in the FeatureQueryOptions argument
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public FdoFeatureReader SelectFeatures(FeatureQueryOptions options)
        {
            return SelectFeatures(options, -1);
        }

        /// <summary>
        /// Selects features from this connection according to the criteria set in the FeatureQueryOptions argument
        /// </summary>
        /// <param name="options"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public FdoFeatureReader SelectFeatures(FeatureQueryOptions options, int limit)
        {
            IFeatureReader reader = null;
            ISelect select = CreateCommand<ISelect>(OSGeo.FDO.Commands.CommandType.CommandType_Select);
            using (select)
            {
                SetSelectOptions(options, select);
                reader = select.Execute();
            }
            if(limit > 0)
                return new FdoFeatureReader(reader, limit);
            else
                return new FdoFeatureReader(reader);
        }

        /// <summary>
        /// Selects groups of features from this connection and applies filters to each of the groups 
        /// according to the criteria set in the FeatureAggregateOptions argument
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public FdoDataReader SelectAggregates(FeatureAggregateOptions options)
        {
            if (!SupportsCommand(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates))
                throw new FeatureServiceException(Res.GetString("ERR_UNSUPPORTED_SELECT_AGGREGATE"));

            IDataReader reader = null;
            ISelectAggregates select = CreateCommand<ISelectAggregates>(OSGeo.FDO.Commands.CommandType.CommandType_SelectAggregates);
            using (select)
            {
                SetSelectOptions(options, select);
                select.Distinct = options.Distinct;
                if(!string.IsNullOrEmpty(options.GroupFilter))
                    select.GroupingFilter = Filter.Parse(options.GroupFilter);
                foreach (string propName in options.GroupByProperties)
                {
                    select.Grouping.Add((Identifier)Identifier.Parse(propName));
                }
                reader = select.Execute();
            }
            return new FdoDataReader(reader);
        }

        private static void SetSelectOptions(FeatureQueryOptions options, IBaseSelect select)
        {
            select.SetFeatureClassName(options.ClassName);

            if (options.IsFilterSet)
                select.Filter = Filter.Parse(options.Filter);

            if (options.PropertyList.Count > 0)
            {
                select.PropertyNames.Clear();
                foreach (string propName in options.PropertyList)
                {
                    select.PropertyNames.Add((Identifier)Identifier.Parse(propName));
                }
            }

            if (options.ComputedProperties.Count > 0)
            {
                foreach (string alias in options.ComputedProperties.Keys)
                {
                    select.PropertyNames.Add(new ComputedIdentifier(alias, options.ComputedProperties[alias]));
                }
            }

            if (options.OrderBy.Count > 0)
            {
                foreach (string propertyName in options.OrderBy)
                {
                    select.Ordering.Add((Identifier)Identifier.Parse(propertyName));
                }
                select.OrderingOption = options.OrderOption;
            }
        }
        
        /// <summary>
        /// Executes the SQL SELECT statement on this connection.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public FdoSqlReader ExecuteSQLQuery(string sql)
        {
            if (!SupportsCommand(CommandType.CommandType_SQLCommand))
                throw new FeatureServiceException(Res.GetString("ERR_UNSUPPORTED_SQL"));

            ISQLDataReader reader = null;
            ISQLCommand cmd = CreateCommand<ISQLCommand>(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand);
            using (cmd)
            {
                cmd.SQLStatement = sql;
                reader = cmd.ExecuteReader();
            }
            return new FdoSqlReader(reader);
        }

        /// <summary>
        /// Executes SQL statements NOT including SELECT statements. 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteSQLNonQuery(string sql)
        {
            if (!SupportsCommand(CommandType.CommandType_SQLCommand))
                throw new FeatureServiceException(Res.GetString("ERR_UNSUPPORTED_SQL"));

            int result = default(int);
            ISQLCommand cmd = CreateCommand<ISQLCommand>(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand);
            using (cmd)
            {
                cmd.SQLStatement = sql;
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }

        /// <summary>
        /// Inserts a new feature into the given feature class
        /// </summary>
        /// <param name="className"></param>
        /// <param name="propertyValues"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public int InsertFeature(string className, PropertyValueCollection propertyValues, bool useTransaction)
        {
            if (!SupportsCommand(CommandType.CommandType_Insert))
                throw new FeatureServiceException(Res.GetString("ERR_UNSUPPORTED_INSERT"));

            bool useTrans = (useTransaction && this.Connection.ConnectionCapabilities.SupportsTransactions());
            int inserted = 0;

            IInsert insert = CreateCommand<IInsert>(CommandType.CommandType_Insert);
            using (insert)
            {
                insert.SetFeatureClassName(className);
                foreach (PropertyValue pv in propertyValues)
                {
                    insert.PropertyValues.Add(pv);
                }
                if (useTrans)
                {
                    ITransaction trans = this.Connection.BeginTransaction();
                    using (trans)
                    {
                        try
                        {
                            using (IFeatureReader reader = insert.Execute())
                            {
                                while (reader.ReadNext()) { inserted++; }
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw new FeatureServiceException(Res.GetString("ERR_INSERT"), ex);
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (IFeatureReader reader = insert.Execute())
                        {
                            while (reader.ReadNext()) { inserted++; }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new FeatureServiceException(Res.GetString("ERR_INSERT"), ex);
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Inserts a new feature into the given feature class
        /// </summary>
        /// <param name="className"></param>
        /// <param name="values"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public int InsertFeature(string className, Dictionary<string, ValueExpression> values, bool useTransaction)
        {
            if (!SupportsCommand(CommandType.CommandType_Insert))
                throw new FeatureServiceException(Res.GetString("ERR_UNSUPPORTED_INSERT"));

            bool useTrans = (useTransaction && this.Connection.ConnectionCapabilities.SupportsTransactions());
            int inserted = 0;

            IInsert insert = CreateCommand<IInsert>(CommandType.CommandType_Insert);
            using(insert)
            {
                insert.SetFeatureClassName(className);
                foreach(string propName in values.Keys)
                {
                    insert.PropertyValues.Add(new PropertyValue(propName, values[propName]));
                }
                if (useTrans)
                {
                    ITransaction trans = this.Connection.BeginTransaction();
                    using (trans)
                    {
                        try
                        {
                            using (IFeatureReader reader = insert.Execute())
                            {
                                while (reader.ReadNext()) { inserted++; }
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw new FeatureServiceException(Res.GetString("ERR_INSERT"), ex);
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (IFeatureReader reader = insert.Execute())
                        {
                            while (reader.ReadNext()) { inserted++; }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new FeatureServiceException(Res.GetString("ERR_INSERT"), ex);
                    }
                }
            }
            return inserted;
        }

        /// <summary>
        /// Checks and modifies the lengths of any [blob/clob/string/decimal] data 
        /// properties inside the class definition so that it lies within the limits 
        /// defined in this connection.
        /// </summary>
        /// <param name="classDef"></param>
        /// <returns></returns>
        internal void FixDataProperties(ref ClassDefinition classDef)
        {
            ISchemaCapabilities caps = this.Connection.SchemaCapabilities;
            foreach (PropertyDefinition propDef in classDef.Properties)
            {
                DataPropertyDefinition dp = propDef as DataPropertyDefinition;
                if (dp != null)
                {
                    switch (dp.DataType)
                    {
                        case DataType.DataType_BLOB:
                            {
                                int length = (int)caps.get_MaximumDataValueLength(DataType.DataType_BLOB);
                                if (dp.Length > length && length > 0)
                                    dp.Length = length;
                            }
                            break;
                        case DataType.DataType_CLOB:
                            {
                                int length = (int)caps.get_MaximumDataValueLength(DataType.DataType_CLOB);
                                if (dp.Length > length && length > 0)
                                    dp.Length = length;
                            }
                            break;
                        case DataType.DataType_String:
                            {
                                int length = (int)caps.get_MaximumDataValueLength(DataType.DataType_String);
                                if (dp.Length > length && length > 0)
                                    dp.Length = length;
                            }
                            break;
                        case DataType.DataType_Decimal:
                            {
                                if (dp.Precision > caps.MaximumDecimalPrecision)
                                    dp.Precision = caps.MaximumDecimalPrecision;

                                if (dp.Scale > caps.MaximumDecimalScale)
                                    dp.Scale = caps.MaximumDecimalScale;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a data store
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="dataStoreProperties"></param>
        /// <param name="connProperties"></param>
        public static void CreateDataStore(string providerName, NameValueCollection dataStoreProperties, NameValueCollection connProperties)
        {
            if (connProperties == null)
                throw new ArgumentNullException("connProperties");

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection(providerName);
            using (conn)
            {
                FdoException fex = null;
                try
                {
                    if (connProperties != null && connProperties.Count > 0)
                    {
                        foreach (string key in connProperties.Keys)
                        {
                            if (!string.IsNullOrEmpty(connProperties[key]))
                                conn.ConnectionInfo.ConnectionProperties.SetProperty(key, connProperties[key]);
                        }
                        conn.Open();
                    }

                    using (ICreateDataStore cmd = conn.CreateCommand(CommandType.CommandType_CreateDataStore) as ICreateDataStore)
                    {
                        foreach (string key in dataStoreProperties.Keys)
                        {
                            if (!string.IsNullOrEmpty(dataStoreProperties[key]))
                                cmd.DataStoreProperties.SetProperty(key, dataStoreProperties[key]);
                        }
                        cmd.Execute();
                    }
                }
                catch (OSGeo.FDO.Common.Exception ex)
                {
                    fex = new FdoException(ex);
                }
                finally
                {
                    if (conn.ConnectionState == ConnectionState.ConnectionState_Open)
                        conn.Close();
                }

                if (fex != null)
                    throw fex;
            }
        }

        /// <summary>
        /// Returns true if this connection supports partial schema discovery.
        /// ie. It supports IGetClassNames and IGetSchemaNames and enhanced IDescribeSchema
        /// </summary>
        /// <returns></returns>
        public bool SupportsPartialSchemaDiscovery()
        {
            int[] cmds = this.Connection.CommandCapabilities.Commands;
            return (Array.IndexOf<int>(cmds, (int)CommandType.CommandType_GetClassNames) >= 0
                 && Array.IndexOf<int>(cmds, (int)CommandType.CommandType_GetSchemaNames) >= 0);
        }

        /// <summary>
        /// Removes an FDO provider from the provider registry
        /// </summary>
        /// <param name="prov"></param>
        public static void UnregisterProvider(string prov)
        {
            FeatureAccessManager.GetProviderRegistry().UnregisterProvider(prov);
            _providerInfo = null; //Invalidate
        }

        /// <summary>
        /// Adds a new FDO provider to the provider registry
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <param name="description"></param>
        /// <param name="version"></param>
        /// <param name="fdoVersion"></param>
        /// <param name="libraryPath"></param>
        /// <param name="managed"></param>
        public static void RegisterProvider(string name, string displayName, string description, string version, string fdoVersion, string libraryPath, bool managed)
        {
            FeatureAccessManager.GetProviderRegistry().RegisterProvider(name, displayName, description, version, fdoVersion, libraryPath, managed);
            _providerInfo = null; //Invalidate
        }

        /// <summary>
        /// Applies the given feature schemas to the current connection
        /// </summary>
        /// <param name="schemas"></param>
        public void ApplySchemas(FeatureSchemaCollection schemas)
        {
            foreach (FeatureSchema s in schemas)
            {
                this.ApplySchema(s);
            }
        }

        /// <summary>
        /// Gets the active spatial context in this connection
        /// </summary>
        /// <returns></returns>
        public SpatialContextInfo GetActiveSpatialContext()
        {
            using (IGetSpatialContexts get = _conn.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_GetSpatialContexts) as IGetSpatialContexts)
            {
                get.ActiveOnly = true;
                using (ISpatialContextReader reader = get.Execute())
                {
                    if (reader.ReadNext())
                    {
                        SpatialContextInfo info = new SpatialContextInfo(reader);
                        return info;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a feature schema containing the given sub-set of classes
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="classes"></param>
        /// <returns></returns>
        public FeatureSchema PartialDescribeSchema(string schemaName, List<string> classes)
        {
            if (SupportsPartialSchemaDiscovery())
            {
                //Use new API
                using (IDescribeSchema describe = Connection.CreateCommand(CommandType.CommandType_DescribeSchema) as IDescribeSchema)
                {
                    describe.SchemaName = schemaName;
                    foreach (string cls in classes)
                    {
                        describe.ClassNames.Add(new OSGeo.FDO.Common.StringElement(cls));
                    }
                    FeatureSchemaCollection schemas = describe.Execute();
                    if (schemas != null)
                        return schemas[0];
                }
            }
            else
            {
                //Use old approach, full schema and rip out classes not in
                //the list
                FeatureSchema schema = GetSchemaByName(schemaName);
                List<string> clsRemove = new List<string>();
                foreach (ClassDefinition cd in schema.Classes)
                {
                    if (!classes.Contains(cd.Name))
                        clsRemove.Add(cd.Name);
                }
                foreach (string cls in clsRemove)
                {
                    schema.Classes.RemoveAt(schema.Classes.IndexOf(cls));
                }
                return schema;
            }
            return null;
        }
    }
}
