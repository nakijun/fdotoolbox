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
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Filter;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Common;
using OSGeo.FDO.Commands.Locking;
using OSGeo.FDO.Commands.SpatialContext;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Generic provider capability interface
    /// </summary>
    public interface ICapability
    {
        /// <summary>
        /// Gets the boolean capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        bool? GetBooleanCapability(CapabilityType cap);
        /// <summary>
        /// Gets the int32 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        int? GetInt32Capability(CapabilityType cap);
        /// <summary>
        /// Gets the int64 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        long? GetInt64Capability(CapabilityType cap);
        /// <summary>
        /// Gets the string capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        string GetStringCapability(CapabilityType cap);
        /// <summary>
        /// Gets the array capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        int[] GetArrayCapability(CapabilityType cap);
        /// <summary>
        /// Gets the object capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        object GetObjectCapability(CapabilityType cap);
    }

    /// <summary>
    /// Allows querying of FDO provider capabilities in a generic fashion.
    /// </summary>
    public class Capability : ICapability
    {
        private IConnection _conn;

        internal Capability(FdoConnection conn)
        {
            _conn = conn.InternalConnection;
        }

        /// <summary>
        /// Gets the boolean capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public bool? GetBooleanCapability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_SupportsAssociationProperties:
                    return _conn.SchemaCapabilities.SupportsAssociationProperties;
                case CapabilityType.FdoCapabilityType_SupportsAutoIdGeneration:
                    return _conn.SchemaCapabilities.SupportsAutoIdGeneration;
                //case CapabilityType.FdoCapabilityType_SupportsCalculatedProperties:
                case CapabilityType.FdoCapabilityType_SupportsCompositeId:
                    return _conn.SchemaCapabilities.SupportsCompositeId;
                case CapabilityType.FdoCapabilityType_SupportsCompositeUniqueValueConstraints:
                    return _conn.SchemaCapabilities.SupportsCompositeUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsConfiguration:
                    return _conn.ConnectionCapabilities.SupportsConfiguration();
                case CapabilityType.FdoCapabilityType_SupportsCSysWKTFromCSysName:
                    return _conn.ConnectionCapabilities.SupportsCSysWKTFromCSysName();
                //case CapabilityType.FdoCapabilityType_SupportsDataModel:
                    //return _conn.RasterCapabilities.SupportsDataModel(OSGeo.FDO.Raster.RasterDataModel;
                case CapabilityType.FdoCapabilityType_SupportsDataStoreScopeUniqueIdGeneration:
                    return _conn.SchemaCapabilities.SupportsDataStoreScopeUniqueIdGeneration;
                case CapabilityType.FdoCapabilityType_SupportsDefaultValue:
                    return _conn.SchemaCapabilities.SupportsDefaultValue;
                case CapabilityType.FdoCapabilityType_SupportsExclusiveValueRangeConstraints:
                    return _conn.SchemaCapabilities.SupportsExclusiveValueRangeConstraints;
                case  CapabilityType.FdoCapabilityType_SupportsFlush:
                    return _conn.ConnectionCapabilities.SupportsFlush();
                //case CapabilityType.FdoCapabilityType_SupportsGeodesicDistance:
                case CapabilityType.FdoCapabilityType_SupportsInclusiveValueRangeConstraints:
                    return _conn.SchemaCapabilities.SupportsInclusiveValueRangeConstraints;
                case CapabilityType.FdoCapabilityType_SupportsInheritance:
                    return _conn.SchemaCapabilities.SupportsInheritance;
                case CapabilityType.FdoCapabilityType_SupportsLocking:
                    return _conn.ConnectionCapabilities.SupportsLocking();
                case CapabilityType.FdoCapabilityType_SupportsLongTransactions:
                    return _conn.ConnectionCapabilities.SupportsLongTransactions();
                case CapabilityType.FdoCapabilityType_SupportsMultipleSchemas:
                    return _conn.SchemaCapabilities.SupportsMultipleSchemas;
                case CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts:
                    return _conn.ConnectionCapabilities.SupportsMultipleSpatialContexts();
                //case CapabilityType.FdoCapabilityType_SupportsMultiUserWrite:
                case CapabilityType.FdoCapabilityType_SupportsNetworkModel:
                    return _conn.SchemaCapabilities.SupportsNetworkModel;
                //case CapabilityType.FdoCapabilityType_SupportsNonLiteralGeometricOperations:
                case CapabilityType.FdoCapabilityType_SupportsNullValueConstraints:
                    return _conn.SchemaCapabilities.SupportsNullValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsObjectProperties:
                    return _conn.SchemaCapabilities.SupportsObjectProperties;
                case CapabilityType.FdoCapabilityType_SupportsParameters:
                    return _conn.CommandCapabilities.SupportsParameters();
                case CapabilityType.FdoCapabilityType_SupportsRaster:
                    return _conn.RasterCapabilities.SupportsRaster();
                case CapabilityType.FdoCapabilityType_SupportsSchemaModification:
                    return _conn.SchemaCapabilities.SupportsSchemaModification;
                case CapabilityType.FdoCapabilityType_SupportsSchemaOverrides:
                    return _conn.SchemaCapabilities.SupportsSchemaOverrides;
                case CapabilityType.FdoCapabilityType_SupportsSelectDistinct:
                    return _conn.CommandCapabilities.SupportsSelectDistinct();
                case CapabilityType.FdoCapabilityType_SupportsSelectExpressions:
                    return _conn.CommandCapabilities.SupportsSelectExpressions();
                case CapabilityType.FdoCapabilityType_SupportsSelectFunctions:
                    return _conn.CommandCapabilities.SupportsSelectFunctions();
                case CapabilityType.FdoCapabilityType_SupportsSelectGrouping:
                    return _conn.CommandCapabilities.SupportsSelectGrouping();
                case CapabilityType.FdoCapabilityType_SupportsSelectOrdering:
                    return _conn.CommandCapabilities.SupportsSelectOrdering();
                case CapabilityType.FdoCapabilityType_SupportsSQL:
                    return _conn.ConnectionCapabilities.SupportsSQL();
                case CapabilityType.FdoCapabilityType_SupportsStitching:
                    return _conn.RasterCapabilities.SupportsStitching();
                case CapabilityType.FdoCapabilityType_SupportsSubsampling:
                    return _conn.RasterCapabilities.SupportsSubsampling();
                case CapabilityType.FdoCapabilityType_SupportsCommandTimeout:
                    return _conn.CommandCapabilities.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsConnectionTimeout:
                    return _conn.ConnectionCapabilities.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsTransactions:
                    return _conn.ConnectionCapabilities.SupportsTransactions();
                case CapabilityType.FdoCapabilityType_SupportsUniqueValueConstraints:
                    return _conn.SchemaCapabilities.SupportsUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsValueConstraintsList:
                    return _conn.SchemaCapabilities.SupportsValueConstraintsList;
                //case CapabilityType.FdoCapabilityType_SupportsWritableIdentityProperties:
                //case CapabilityType.FdoCapabilityType_SupportsWrite:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the int32 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public int? GetInt32Capability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_MaximumDecimalPrecision:
                    return _conn.SchemaCapabilities.MaximumDecimalPrecision;
                case CapabilityType.FdoCapabilityType_MaximumDecimalScale:
                    return _conn.SchemaCapabilities.MaximumDecimalScale;
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Class:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Class);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Datastore:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Datastore);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Description:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Description);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Property:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Property);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Schema:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Schema);
                case CapabilityType.FdoCapabilityType_Dimensionalities:
                    return _conn.GeometryCapabilities.Dimensionalities;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the int64 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public long? GetInt64Capability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_BLOB:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_BLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Boolean:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Boolean);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Byte:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Byte);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_CLOB:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_CLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_DateTime:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_DateTime);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Decimal:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Decimal);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Double:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Double);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int16:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int16);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int32:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int32);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int64:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int64);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Single:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Single);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_String:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_String);

                case CapabilityType.FdoCapabilityType_MaximumDecimalPrecision:
                    return _conn.SchemaCapabilities.MaximumDecimalPrecision;
                case CapabilityType.FdoCapabilityType_MaximumDecimalScale:
                    return _conn.SchemaCapabilities.MaximumDecimalScale;
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Class:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Class);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Datastore:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Datastore);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Description:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Description);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Property:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Property);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Schema:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Schema);
             
                case CapabilityType.FdoCapabilityType_Dimensionalities:
                    return _conn.GeometryCapabilities.Dimensionalities;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the string capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public string GetStringCapability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_ReservedCharactersForName:
                    return _conn.SchemaCapabilities.ReservedCharactersForName;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the array capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public int[] GetArrayCapability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_ClassTypes:
                    return Array.ConvertAll<ClassType, int>(_conn.SchemaCapabilities.ClassTypes, delegate(ClassType ct) { return (int)ct; });
                case CapabilityType.FdoCapabilityType_CommandList:
                    return _conn.CommandCapabilities.Commands;
                case CapabilityType.FdoCapabilityType_ConditionTypes:
                    return Array.ConvertAll<ConditionType, int>(_conn.FilterCapabilities.ConditionTypes, delegate(ConditionType ct) { return (int)ct; });
                case CapabilityType.FdoCapabilityType_DataTypes:
                    return Array.ConvertAll<DataType, int>(_conn.SchemaCapabilities.DataTypes, delegate(DataType dt) { return (int)dt; });
                case CapabilityType.FdoCapabilityType_DistanceOperations:
                    return Array.ConvertAll<DistanceOperations, int>(_conn.FilterCapabilities.DistanceOperations, delegate(DistanceOperations d) { return (int)d; });
                case CapabilityType.FdoCapabilityType_ExpressionTypes:
                    return Array.ConvertAll<ExpressionType, int>(_conn.ExpressionCapabilities.ExpressionTypes, delegate(ExpressionType e) { return (int)e; });
                case CapabilityType.FdoCapabilityType_GeometryComponentTypes:
                    return Array.ConvertAll<GeometryComponentType, int>(_conn.GeometryCapabilities.GeometryComponentTypes, delegate(GeometryComponentType g) { return (int)g; });
                case CapabilityType.FdoCapabilityType_GeometryTypes:
                    return Array.ConvertAll<GeometryType, int>(_conn.GeometryCapabilities.GeometryTypes, delegate(GeometryType g) { return (int)g; });
                case CapabilityType.FdoCapabilityType_LockTypes:
                    return Array.ConvertAll<LockType, int>(_conn.ConnectionCapabilities.LockTypes, delegate(LockType l) { return (int)l; });
                case CapabilityType.FdoCapabilityType_SpatialContextTypes:
                    return Array.ConvertAll<SpatialContextExtentType, int>(_conn.ConnectionCapabilities.SpatialContextTypes, delegate(SpatialContextExtentType s) { return (int)s; });
                case CapabilityType.FdoCapabilityType_SpatialOperations:
                    return Array.ConvertAll<SpatialOperations, int>(_conn.FilterCapabilities.SpatialOperations, delegate(SpatialOperations s) { return (int)s; });
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the object capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public object GetObjectCapability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_SupportsAssociationProperties:
                    return _conn.SchemaCapabilities.SupportsAssociationProperties;
                case CapabilityType.FdoCapabilityType_SupportsAutoIdGeneration:
                    return _conn.SchemaCapabilities.SupportsAutoIdGeneration;
                //case CapabilityType.FdoCapabilityType_SupportsCalculatedProperties:
                case CapabilityType.FdoCapabilityType_SupportsCompositeId:
                    return _conn.SchemaCapabilities.SupportsCompositeId;
                case CapabilityType.FdoCapabilityType_SupportsCompositeUniqueValueConstraints:
                    return _conn.SchemaCapabilities.SupportsCompositeUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsConfiguration:
                    return _conn.ConnectionCapabilities.SupportsConfiguration();
                case CapabilityType.FdoCapabilityType_SupportsCSysWKTFromCSysName:
                    return _conn.ConnectionCapabilities.SupportsCSysWKTFromCSysName();
                //case CapabilityType.FdoCapabilityType_SupportsDataModel:
                //    return _conn.RasterCapabilities.SupportsDataModel();
                case CapabilityType.FdoCapabilityType_SupportsDataStoreScopeUniqueIdGeneration:
                    return _conn.SchemaCapabilities.SupportsDataStoreScopeUniqueIdGeneration;
                case CapabilityType.FdoCapabilityType_SupportsDefaultValue:
                    return _conn.SchemaCapabilities.SupportsDefaultValue;
                case CapabilityType.FdoCapabilityType_SupportsExclusiveValueRangeConstraints:
                    return _conn.SchemaCapabilities.SupportsExclusiveValueRangeConstraints;
                case CapabilityType.FdoCapabilityType_SupportsFlush:
                    return _conn.ConnectionCapabilities.SupportsFlush();
                //case CapabilityType.FdoCapabilityType_SupportsGeodesicDistance:
                case CapabilityType.FdoCapabilityType_SupportsInclusiveValueRangeConstraints:
                    return _conn.SchemaCapabilities.SupportsInclusiveValueRangeConstraints;
                case CapabilityType.FdoCapabilityType_SupportsInheritance:
                    return _conn.SchemaCapabilities.SupportsInheritance;
                case CapabilityType.FdoCapabilityType_SupportsLocking:
                    return _conn.ConnectionCapabilities.SupportsLocking();
                case CapabilityType.FdoCapabilityType_SupportsLongTransactions:
                    return _conn.ConnectionCapabilities.SupportsLongTransactions();
                case CapabilityType.FdoCapabilityType_SupportsMultipleSchemas:
                    return _conn.SchemaCapabilities.SupportsMultipleSchemas;
                case CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts:
                    return _conn.ConnectionCapabilities.SupportsMultipleSpatialContexts();
                //case CapabilityType.FdoCapabilityType_SupportsMultiUserWrite:
                case CapabilityType.FdoCapabilityType_SupportsNetworkModel:
                    return _conn.SchemaCapabilities.SupportsNetworkModel;
                //case CapabilityType.FdoCapabilityType_SupportsNonLiteralGeometricOperations:
                case CapabilityType.FdoCapabilityType_SupportsNullValueConstraints:
                    return _conn.SchemaCapabilities.SupportsNullValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsObjectProperties:
                    return _conn.SchemaCapabilities.SupportsObjectProperties;
                case CapabilityType.FdoCapabilityType_SupportsParameters:
                    return _conn.CommandCapabilities.SupportsParameters();
                case CapabilityType.FdoCapabilityType_SupportsRaster:
                    return _conn.RasterCapabilities.SupportsRaster();
                case CapabilityType.FdoCapabilityType_SupportsSchemaModification:
                    return _conn.SchemaCapabilities.SupportsSchemaModification;
                case CapabilityType.FdoCapabilityType_SupportsSchemaOverrides:
                    return _conn.SchemaCapabilities.SupportsSchemaOverrides;
                case CapabilityType.FdoCapabilityType_SupportsSelectDistinct:
                    return _conn.CommandCapabilities.SupportsSelectDistinct();
                case CapabilityType.FdoCapabilityType_SupportsSelectExpressions:
                    return _conn.CommandCapabilities.SupportsSelectExpressions();
                case CapabilityType.FdoCapabilityType_SupportsSelectFunctions:
                    return _conn.CommandCapabilities.SupportsSelectFunctions();
                case CapabilityType.FdoCapabilityType_SupportsSelectGrouping:
                    return _conn.CommandCapabilities.SupportsSelectGrouping();
                case CapabilityType.FdoCapabilityType_SupportsSelectOrdering:
                    return _conn.CommandCapabilities.SupportsSelectOrdering();
                case CapabilityType.FdoCapabilityType_SupportsSQL:
                    return _conn.ConnectionCapabilities.SupportsSQL();
                case CapabilityType.FdoCapabilityType_SupportsStitching:
                    return _conn.RasterCapabilities.SupportsStitching();
                case CapabilityType.FdoCapabilityType_SupportsSubsampling:
                    return _conn.RasterCapabilities.SupportsSubsampling();
                case CapabilityType.FdoCapabilityType_SupportsCommandTimeout:
                    return _conn.CommandCapabilities.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsConnectionTimeout:
                    return _conn.ConnectionCapabilities.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsTransactions:
                    return _conn.ConnectionCapabilities.SupportsTransactions();
                case CapabilityType.FdoCapabilityType_SupportsUniqueValueConstraints:
                    return _conn.SchemaCapabilities.SupportsUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsValueConstraintsList:
                    return _conn.SchemaCapabilities.SupportsValueConstraintsList;
                //case CapabilityType.FdoCapabilityType_SupportsWritableIdentityProperties:
                //case CapabilityType.FdoCapabilityType_SupportsWrite:
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_BLOB:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_BLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Boolean:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Boolean);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Byte:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Byte);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_CLOB:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_CLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_DateTime:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_DateTime);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Decimal:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Decimal);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Double:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Double);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int16:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int16);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int32:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int32);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int64:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int64);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Single:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Single);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_String:
                    return _conn.SchemaCapabilities.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_String);
                case CapabilityType.FdoCapabilityType_ExpressionFunctions:
                    return _conn.ExpressionCapabilities.Functions;

                case CapabilityType.FdoCapabilityType_MaximumDecimalPrecision:
                    return _conn.SchemaCapabilities.MaximumDecimalPrecision;
                case CapabilityType.FdoCapabilityType_MaximumDecimalScale:
                    return _conn.SchemaCapabilities.MaximumDecimalScale;
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Class:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Class);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Datastore:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Datastore);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Description:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Description);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Property:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Property);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Schema:
                    return _conn.SchemaCapabilities.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Schema);

                case CapabilityType.FdoCapabilityType_Dimensionalities:
                    return _conn.GeometryCapabilities.Dimensionalities;
                case CapabilityType.FdoCapabilityType_ReservedCharactersForName:
                    return _conn.SchemaCapabilities.ReservedCharactersForName;
                case CapabilityType.FdoCapabilityType_ClassTypes:
                    return _conn.SchemaCapabilities.ClassTypes;
                case CapabilityType.FdoCapabilityType_CommandList:
                    return _conn.CommandCapabilities.Commands;
                case CapabilityType.FdoCapabilityType_ConditionTypes:
                    return _conn.FilterCapabilities.ConditionTypes;
                case CapabilityType.FdoCapabilityType_DataTypes:
                    return _conn.SchemaCapabilities.DataTypes;
                case CapabilityType.FdoCapabilityType_DistanceOperations:
                    return _conn.FilterCapabilities.DistanceOperations;
                case CapabilityType.FdoCapabilityType_ExpressionTypes:
                    return _conn.ExpressionCapabilities.ExpressionTypes;
                case CapabilityType.FdoCapabilityType_GeometryComponentTypes:
                    return _conn.GeometryCapabilities.GeometryComponentTypes;
                case CapabilityType.FdoCapabilityType_GeometryTypes:
                    return _conn.GeometryCapabilities.GeometryTypes;
                case CapabilityType.FdoCapabilityType_LockTypes:
                    return _conn.ConnectionCapabilities.LockTypes;
                case CapabilityType.FdoCapabilityType_SpatialContextTypes:
                    return _conn.ConnectionCapabilities.SpatialContextTypes;
                case CapabilityType.FdoCapabilityType_SpatialOperations:
                    return _conn.FilterCapabilities.SpatialOperations;
                case CapabilityType.FdoCapabilityType_ThreadCapability:
                    return _conn.ConnectionCapabilities.ThreadCapability;

                default:
                    return null;
            }
        }
    }
}
