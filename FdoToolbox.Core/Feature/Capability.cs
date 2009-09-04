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
using OSGeo.FDO.Connections.Capabilities;

namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// Generic provider capability interface
    /// </summary>
    public interface ICapability : IDisposable
    {
        /// <summary>
        /// Gets the boolean capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        bool GetBooleanCapability(CapabilityType cap);
        /// <summary>
        /// Gets the int32 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        int GetInt32Capability(CapabilityType cap);
        /// <summary>
        /// Gets the int64 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        long GetInt64Capability(CapabilityType cap);
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
        /// <summary>
        /// Determines if an array capability contains the specified value
        /// </summary>
        /// <param name="capabilityType">The capability (must be an array capability)</param>
        /// <param name="value">The value to check for</param>
        /// <returns>True of the value exists; false otherwise</returns>
        bool HasArrayCapability(CapabilityType capabilityType, int value);
    }

    /// <summary>
    /// Allows querying of FDO provider capabilities in a generic fashion.
    /// </summary>
    public class Capability : ICapability
    {
        private ICommandCapabilities commandCaps;
        private IConnectionCapabilities connCaps;
        private IExpressionCapabilities exprCaps;
        private IGeometryCapabilities geomCaps;
        private IFilterCapabilities filterCaps;
        private IRasterCapabilities rasterCaps;
        private ISchemaCapabilities schemaCaps;
        private ITopologyCapabilities topoCaps;

        internal Capability(FdoConnection conn)
        {
            IConnection internalConn = conn.InternalConnection;
            commandCaps = internalConn.CommandCapabilities;
            connCaps = internalConn.ConnectionCapabilities;
            exprCaps = internalConn.ExpressionCapabilities;
            filterCaps = internalConn.FilterCapabilities;
            geomCaps = internalConn.GeometryCapabilities;
            rasterCaps = internalConn.RasterCapabilities;
            schemaCaps = internalConn.SchemaCapabilities;
            topoCaps = internalConn.TopologyCapabilities;
        }

        public void Dispose()
        {
            commandCaps.Dispose();
            connCaps.Dispose();
            exprCaps.Dispose();
            geomCaps.Dispose();
            filterCaps.Dispose();
            rasterCaps.Dispose();
            schemaCaps.Dispose();
            topoCaps.Dispose();

            commandCaps = null;
            connCaps = null;
            exprCaps = null;
            geomCaps = null;
            filterCaps = null;
            rasterCaps = null;
            schemaCaps = null;
            topoCaps = null;
        }

        /// <summary>
        /// Gets the boolean capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public bool GetBooleanCapability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_SupportsAssociationProperties:
                    return schemaCaps.SupportsAssociationProperties;
                case CapabilityType.FdoCapabilityType_SupportsAutoIdGeneration:
                    return schemaCaps.SupportsAutoIdGeneration;
                //case CapabilityType.FdoCapabilityType_SupportsCalculatedProperties:
                case CapabilityType.FdoCapabilityType_SupportsCompositeId:
                    return schemaCaps.SupportsCompositeId;
                case CapabilityType.FdoCapabilityType_SupportsCompositeUniqueValueConstraints:
                    return schemaCaps.SupportsCompositeUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsConfiguration:
                    return connCaps.SupportsConfiguration();
                case CapabilityType.FdoCapabilityType_SupportsCSysWKTFromCSysName:
                    return connCaps.SupportsCSysWKTFromCSysName();
                //case CapabilityType.FdoCapabilityType_SupportsDataModel:
                    //return rasterCaps.SupportsDataModel(OSGeo.FDO.Raster.RasterDataModel;
                case CapabilityType.FdoCapabilityType_SupportsDataStoreScopeUniqueIdGeneration:
                    return schemaCaps.SupportsDataStoreScopeUniqueIdGeneration;
                case CapabilityType.FdoCapabilityType_SupportsDefaultValue:
                    return schemaCaps.SupportsDefaultValue;
                case CapabilityType.FdoCapabilityType_SupportsExclusiveValueRangeConstraints:
                    return schemaCaps.SupportsExclusiveValueRangeConstraints;
                case  CapabilityType.FdoCapabilityType_SupportsFlush:
                    return connCaps.SupportsFlush();
                //case CapabilityType.FdoCapabilityType_SupportsGeodesicDistance:
                case CapabilityType.FdoCapabilityType_SupportsInclusiveValueRangeConstraints:
                    return schemaCaps.SupportsInclusiveValueRangeConstraints;
                case CapabilityType.FdoCapabilityType_SupportsInheritance:
                    return schemaCaps.SupportsInheritance;
                case CapabilityType.FdoCapabilityType_SupportsLocking:
                    return connCaps.SupportsLocking();
                case CapabilityType.FdoCapabilityType_SupportsLongTransactions:
                    return connCaps.SupportsLongTransactions();
                case CapabilityType.FdoCapabilityType_SupportsMultipleSchemas:
                    return schemaCaps.SupportsMultipleSchemas;
                case CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts:
                    return connCaps.SupportsMultipleSpatialContexts();
                //case CapabilityType.FdoCapabilityType_SupportsMultiUserWrite:
                case CapabilityType.FdoCapabilityType_SupportsNetworkModel:
                    return schemaCaps.SupportsNetworkModel;
                //case CapabilityType.FdoCapabilityType_SupportsNonLiteralGeometricOperations:
                case CapabilityType.FdoCapabilityType_SupportsNullValueConstraints:
                    return schemaCaps.SupportsNullValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsObjectProperties:
                    return schemaCaps.SupportsObjectProperties;
                case CapabilityType.FdoCapabilityType_SupportsParameters:
                    return commandCaps.SupportsParameters();
                case CapabilityType.FdoCapabilityType_SupportsRaster:
                    return rasterCaps.SupportsRaster();
                case CapabilityType.FdoCapabilityType_SupportsSchemaModification:
                    return schemaCaps.SupportsSchemaModification;
                case CapabilityType.FdoCapabilityType_SupportsSchemaOverrides:
                    return schemaCaps.SupportsSchemaOverrides;
                case CapabilityType.FdoCapabilityType_SupportsSelectDistinct:
                    return commandCaps.SupportsSelectDistinct();
                case CapabilityType.FdoCapabilityType_SupportsSelectExpressions:
                    return commandCaps.SupportsSelectExpressions();
                case CapabilityType.FdoCapabilityType_SupportsSelectFunctions:
                    return commandCaps.SupportsSelectFunctions();
                case CapabilityType.FdoCapabilityType_SupportsSelectGrouping:
                    return commandCaps.SupportsSelectGrouping();
                case CapabilityType.FdoCapabilityType_SupportsSelectOrdering:
                    return commandCaps.SupportsSelectOrdering();
                case CapabilityType.FdoCapabilityType_SupportsSQL:
                    return connCaps.SupportsSQL();
                case CapabilityType.FdoCapabilityType_SupportsStitching:
                    return rasterCaps.SupportsStitching();
                case CapabilityType.FdoCapabilityType_SupportsSubsampling:
                    return rasterCaps.SupportsSubsampling();
                case CapabilityType.FdoCapabilityType_SupportsCommandTimeout:
                    return commandCaps.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsConnectionTimeout:
                    return connCaps.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsTransactions:
                    return connCaps.SupportsTransactions();
                case CapabilityType.FdoCapabilityType_SupportsUniqueValueConstraints:
                    return schemaCaps.SupportsUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsValueConstraintsList:
                    return schemaCaps.SupportsValueConstraintsList;
                //case CapabilityType.FdoCapabilityType_SupportsWritableIdentityProperties:
                //case CapabilityType.FdoCapabilityType_SupportsWrite:
                default:
                    throw new ArgumentException(cap.ToString());
            }
        }

        /// <summary>
        /// Gets the int32 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public int GetInt32Capability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_MaximumDecimalPrecision:
                    return schemaCaps.MaximumDecimalPrecision;
                case CapabilityType.FdoCapabilityType_MaximumDecimalScale:
                    return schemaCaps.MaximumDecimalScale;
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Class:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Class);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Datastore:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Datastore);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Description:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Description);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Property:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Property);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Schema:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Schema);
                case CapabilityType.FdoCapabilityType_Dimensionalities:
                    return geomCaps.Dimensionalities;
                default:
                    throw new ArgumentException(cap.ToString());
            }
        }

        /// <summary>
        /// Gets the int64 capability.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <returns></returns>
        public long GetInt64Capability(CapabilityType cap)
        {
            switch (cap)
            {
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_BLOB:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_BLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Boolean:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Boolean);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Byte:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Byte);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_CLOB:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_CLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_DateTime:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_DateTime);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Decimal:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Decimal);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Double:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Double);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int16:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int16);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int32:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int32);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int64:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int64);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Single:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Single);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_String:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_String);

                case CapabilityType.FdoCapabilityType_MaximumDecimalPrecision:
                    return schemaCaps.MaximumDecimalPrecision;
                case CapabilityType.FdoCapabilityType_MaximumDecimalScale:
                    return schemaCaps.MaximumDecimalScale;
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Class:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Class);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Datastore:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Datastore);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Description:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Description);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Property:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Property);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Schema:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Schema);
             
                case CapabilityType.FdoCapabilityType_Dimensionalities:
                    return geomCaps.Dimensionalities;

                default:
                    throw new ArgumentException(cap.ToString());
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
                    return schemaCaps.ReservedCharactersForName;
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
                    return Array.ConvertAll<ClassType, int>(schemaCaps.ClassTypes, delegate(ClassType ct) { return (int)ct; });
                case CapabilityType.FdoCapabilityType_CommandList:
                    return commandCaps.Commands;
                case CapabilityType.FdoCapabilityType_ConditionTypes:
                    return Array.ConvertAll<ConditionType, int>(filterCaps.ConditionTypes, delegate(ConditionType ct) { return (int)ct; });
                case CapabilityType.FdoCapabilityType_DataTypes:
                    return Array.ConvertAll<DataType, int>(schemaCaps.DataTypes, delegate(DataType dt) { return (int)dt; });
                case CapabilityType.FdoCapabilityType_DistanceOperations:
                    return Array.ConvertAll<DistanceOperations, int>(filterCaps.DistanceOperations, delegate(DistanceOperations d) { return (int)d; });
                case CapabilityType.FdoCapabilityType_ExpressionTypes:
                    return Array.ConvertAll<ExpressionType, int>(exprCaps.ExpressionTypes, delegate(ExpressionType e) { return (int)e; });
                case CapabilityType.FdoCapabilityType_GeometryComponentTypes:
                    return Array.ConvertAll<GeometryComponentType, int>(geomCaps.GeometryComponentTypes, delegate(GeometryComponentType g) { return (int)g; });
                case CapabilityType.FdoCapabilityType_GeometryTypes:
                    return Array.ConvertAll<GeometryType, int>(geomCaps.GeometryTypes, delegate(GeometryType g) { return (int)g; });
                case CapabilityType.FdoCapabilityType_LockTypes:
                    return Array.ConvertAll<LockType, int>(connCaps.LockTypes, delegate(LockType l) { return (int)l; });
                case CapabilityType.FdoCapabilityType_SpatialContextTypes:
                    return Array.ConvertAll<SpatialContextExtentType, int>(connCaps.SpatialContextTypes, delegate(SpatialContextExtentType s) { return (int)s; });
                case CapabilityType.FdoCapabilityType_SpatialOperations:
                    return Array.ConvertAll<SpatialOperations, int>(filterCaps.SpatialOperations, delegate(SpatialOperations s) { return (int)s; });
                case CapabilityType.FdoCapabilityType_SupportedAutoGeneratedTypes:
                    return Array.ConvertAll<DataType, int>(schemaCaps.SupportedAutoGeneratedTypes, delegate(DataType dt) { return (int)dt; });
                case CapabilityType.FdoCapabilityType_SupportedIdentityPropertyTypes:
                    return Array.ConvertAll<DataType, int>(schemaCaps.SupportedIdentityPropertyTypes, delegate(DataType dt) { return (int)dt; }); 
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
                case CapabilityType.FdoCapabilityType_SupportedAutoGeneratedTypes:
                    return schemaCaps.SupportedAutoGeneratedTypes;
                case CapabilityType.FdoCapabilityType_SupportedIdentityPropertyTypes:
                    return schemaCaps.SupportedIdentityPropertyTypes;
                case CapabilityType.FdoCapabilityType_SupportsAssociationProperties:
                    return schemaCaps.SupportsAssociationProperties;
                case CapabilityType.FdoCapabilityType_SupportsAutoIdGeneration:
                    return schemaCaps.SupportsAutoIdGeneration;
                //case CapabilityType.FdoCapabilityType_SupportsCalculatedProperties:
                case CapabilityType.FdoCapabilityType_SupportsCompositeId:
                    return schemaCaps.SupportsCompositeId;
                case CapabilityType.FdoCapabilityType_SupportsCompositeUniqueValueConstraints:
                    return schemaCaps.SupportsCompositeUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsConfiguration:
                    return connCaps.SupportsConfiguration();
                case CapabilityType.FdoCapabilityType_SupportsCSysWKTFromCSysName:
                    return connCaps.SupportsCSysWKTFromCSysName();
                //case CapabilityType.FdoCapabilityType_SupportsDataModel:
                //    return rasterCaps.SupportsDataModel();
                case CapabilityType.FdoCapabilityType_SupportsDataStoreScopeUniqueIdGeneration:
                    return schemaCaps.SupportsDataStoreScopeUniqueIdGeneration;
                case CapabilityType.FdoCapabilityType_SupportsDefaultValue:
                    return schemaCaps.SupportsDefaultValue;
                case CapabilityType.FdoCapabilityType_SupportsExclusiveValueRangeConstraints:
                    return schemaCaps.SupportsExclusiveValueRangeConstraints;
                case CapabilityType.FdoCapabilityType_SupportsFlush:
                    return connCaps.SupportsFlush();
                //case CapabilityType.FdoCapabilityType_SupportsGeodesicDistance:
                case CapabilityType.FdoCapabilityType_SupportsInclusiveValueRangeConstraints:
                    return schemaCaps.SupportsInclusiveValueRangeConstraints;
                case CapabilityType.FdoCapabilityType_SupportsInheritance:
                    return schemaCaps.SupportsInheritance;
                case CapabilityType.FdoCapabilityType_SupportsLocking:
                    return connCaps.SupportsLocking();
                case CapabilityType.FdoCapabilityType_SupportsLongTransactions:
                    return connCaps.SupportsLongTransactions();
                case CapabilityType.FdoCapabilityType_SupportsMultipleSchemas:
                    return schemaCaps.SupportsMultipleSchemas;
                case CapabilityType.FdoCapabilityType_SupportsMultipleSpatialContexts:
                    return connCaps.SupportsMultipleSpatialContexts();
                //case CapabilityType.FdoCapabilityType_SupportsMultiUserWrite:
                case CapabilityType.FdoCapabilityType_SupportsNetworkModel:
                    return schemaCaps.SupportsNetworkModel;
                //case CapabilityType.FdoCapabilityType_SupportsNonLiteralGeometricOperations:
                case CapabilityType.FdoCapabilityType_SupportsNullValueConstraints:
                    return schemaCaps.SupportsNullValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsObjectProperties:
                    return schemaCaps.SupportsObjectProperties;
                case CapabilityType.FdoCapabilityType_SupportsParameters:
                    return commandCaps.SupportsParameters();
                case CapabilityType.FdoCapabilityType_SupportsRaster:
                    return rasterCaps.SupportsRaster();
                case CapabilityType.FdoCapabilityType_SupportsSchemaModification:
                    return schemaCaps.SupportsSchemaModification;
                case CapabilityType.FdoCapabilityType_SupportsSchemaOverrides:
                    return schemaCaps.SupportsSchemaOverrides;
                case CapabilityType.FdoCapabilityType_SupportsSelectDistinct:
                    return commandCaps.SupportsSelectDistinct();
                case CapabilityType.FdoCapabilityType_SupportsSelectExpressions:
                    return commandCaps.SupportsSelectExpressions();
                case CapabilityType.FdoCapabilityType_SupportsSelectFunctions:
                    return commandCaps.SupportsSelectFunctions();
                case CapabilityType.FdoCapabilityType_SupportsSelectGrouping:
                    return commandCaps.SupportsSelectGrouping();
                case CapabilityType.FdoCapabilityType_SupportsSelectOrdering:
                    return commandCaps.SupportsSelectOrdering();
                case CapabilityType.FdoCapabilityType_SupportsSQL:
                    return connCaps.SupportsSQL();
                case CapabilityType.FdoCapabilityType_SupportsStitching:
                    return rasterCaps.SupportsStitching();
                case CapabilityType.FdoCapabilityType_SupportsSubsampling:
                    return rasterCaps.SupportsSubsampling();
                case CapabilityType.FdoCapabilityType_SupportsCommandTimeout:
                    return commandCaps.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsConnectionTimeout:
                    return connCaps.SupportsTimeout();
                case CapabilityType.FdoCapabilityType_SupportsTransactions:
                    return connCaps.SupportsTransactions();
                case CapabilityType.FdoCapabilityType_SupportsUniqueValueConstraints:
                    return schemaCaps.SupportsUniqueValueConstraints;
                case CapabilityType.FdoCapabilityType_SupportsValueConstraintsList:
                    return schemaCaps.SupportsValueConstraintsList;
                //case CapabilityType.FdoCapabilityType_SupportsWritableIdentityProperties:
                //case CapabilityType.FdoCapabilityType_SupportsWrite:
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_BLOB:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_BLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Boolean:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Boolean);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Byte:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Byte);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_CLOB:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_CLOB);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_DateTime:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_DateTime);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Decimal:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Decimal);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Double:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Double);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int16:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int16);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int32:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int32);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Int64:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Int64);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_Single:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_Single);
                case CapabilityType.FdoCapabilityType_MaximumDataValueLength_String:
                    return schemaCaps.get_MaximumDataValueLength(OSGeo.FDO.Schema.DataType.DataType_String);
                case CapabilityType.FdoCapabilityType_ExpressionFunctions:
                    return exprCaps.Functions;

                case CapabilityType.FdoCapabilityType_MaximumDecimalPrecision:
                    return schemaCaps.MaximumDecimalPrecision;
                case CapabilityType.FdoCapabilityType_MaximumDecimalScale:
                    return schemaCaps.MaximumDecimalScale;
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Class:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Class);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Datastore:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Datastore);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Description:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Description);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Property:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Property);
                case CapabilityType.FdoCapabilityType_NameSizeLimit_Schema:
                    return schemaCaps.get_NameSizeLimit(OSGeo.FDO.Connections.Capabilities.SchemaElementNameType.SchemaElementNameType_Schema);

                case CapabilityType.FdoCapabilityType_Dimensionalities:
                    return geomCaps.Dimensionalities;
                case CapabilityType.FdoCapabilityType_ReservedCharactersForName:
                    return schemaCaps.ReservedCharactersForName;
                case CapabilityType.FdoCapabilityType_ClassTypes:
                    return schemaCaps.ClassTypes;
                case CapabilityType.FdoCapabilityType_CommandList:
                    return commandCaps.Commands;
                case CapabilityType.FdoCapabilityType_ConditionTypes:
                    return filterCaps.ConditionTypes;
                case CapabilityType.FdoCapabilityType_DataTypes:
                    return schemaCaps.DataTypes;
                case CapabilityType.FdoCapabilityType_DistanceOperations:
                    return filterCaps.DistanceOperations;
                case CapabilityType.FdoCapabilityType_ExpressionTypes:
                    return exprCaps.ExpressionTypes;
                case CapabilityType.FdoCapabilityType_GeometryComponentTypes:
                    return geomCaps.GeometryComponentTypes;
                case CapabilityType.FdoCapabilityType_GeometryTypes:
                    return geomCaps.GeometryTypes;
                case CapabilityType.FdoCapabilityType_LockTypes:
                    return connCaps.LockTypes;
                case CapabilityType.FdoCapabilityType_SpatialContextTypes:
                    return connCaps.SpatialContextTypes;
                case CapabilityType.FdoCapabilityType_SpatialOperations:
                    return filterCaps.SpatialOperations;
                case CapabilityType.FdoCapabilityType_ThreadCapability:
                    return connCaps.ThreadCapability;

                default:
                    return null;
            }
        }


        /// <summary>
        /// Determines if an array capability contains the specified value
        /// </summary>
        /// <param name="capabilityType">The capability (must be an array capability)</param>
        /// <param name="value">The value to check for</param>
        /// <returns>
        /// True of the value exists; false otherwise
        /// </returns>
        public bool HasArrayCapability(CapabilityType capabilityType, int value)
        {
            int [] values = this.GetArrayCapability(capabilityType);
            if (values != null)
                return Array.IndexOf<int>(values, value) >= 0;
            return false;
        }
    }
}
