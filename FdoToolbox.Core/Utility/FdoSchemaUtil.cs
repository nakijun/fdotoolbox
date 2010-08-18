#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Connections.Capabilities;

namespace FdoToolbox.Core.Utility
{
    public static class FdoSchemaUtil
    {
        public static int SetDefaultSpatialContextAssociation(FeatureSchemaCollection fsc, string name)
        {
            int modified = 0;
            foreach (FeatureSchema fs in fsc)
            {
                modified += SetDefaultSpatialContextAssociation(fs, name);
            }
            return modified;
        }

        public static int SetDefaultSpatialContextAssociation(FeatureSchema fs, string name)
        {
            int modified = 0;
            foreach (ClassDefinition cls in fs.Classes)
            {
                foreach (PropertyDefinition prop in cls.Properties)
                {
                    if (prop.PropertyType == PropertyType.PropertyType_GeometricProperty)
                    {
                        GeometricPropertyDefinition geom = (GeometricPropertyDefinition)prop;
                        if (!geom.SpatialContextAssociation.Equals(name))
                        {
                            geom.SpatialContextAssociation = name;
                            modified++;
                        }
                    }
                }
            }
            return modified;
        }

        internal static PropertyDefinition CreatePropertyFromExpressionType(string exprText, FunctionDefinitionCollection functionDefs, string defaultSpatialContextName)
        {
            string name = string.Empty;
            using (var expr = Expression.Parse(exprText))
            {
                var et = expr.GetType();
                if (typeof(ComputedIdentifier).IsAssignableFrom(et))
                {
                    var subExpr = ((ComputedIdentifier)expr).Expression;
                    name = ((ComputedIdentifier)expr).Name;
                }
                else if (typeof(Function).IsAssignableFrom(et))
                {
                    var func = (Function)expr;
                    if (functionDefs != null)
                    {
                        var fidx = functionDefs.IndexOf(func.Name);
                        if (fidx >= 0)
                        {
                            var funcDef = functionDefs[fidx];
                            switch (funcDef.ReturnPropertyType)
                            {
                                case PropertyType.PropertyType_DataProperty:
                                    {
                                        var dp = new DataPropertyDefinition(name, "");
                                        dp.DataType = funcDef.ReturnType;
                                        dp.Nullable = true;
                                        if (dp.DataType == DataType.DataType_String)
                                            dp.Length = 255;

                                        return dp;
                                    }
                                    break;
                                case PropertyType.PropertyType_GeometricProperty:
                                    {
                                        var geom = new GeometricPropertyDefinition(name, "");
                                        geom.SpatialContextAssociation = defaultSpatialContextName;
                                        geom.GeometryTypes = (int)GeometricType.GeometricType_All;

                                        return geom;
                                    }
                                    break;

                            }
                        }
                    }
                }
                else if (typeof(BinaryExpression).IsAssignableFrom(et))
                {
                    var dp = new DataPropertyDefinition(name, "");
                    dp.DataType = DataType.DataType_Boolean;
                    dp.Nullable = true;

                    return dp;
                }
                else if (typeof(DataValue).IsAssignableFrom(et))
                {
                    var dv = (DataValue)expr;
                    var dp = new DataPropertyDefinition(name, "");
                    dp.DataType = dv.DataType;
                    if (dp.DataType == DataType.DataType_String)
                        dp.Length = 255;

                    dp.Nullable = true;
                    return dp;
                }
                else if (typeof(GeometryValue).IsAssignableFrom(et))
                {
                    var geom = new GeometricPropertyDefinition(name, "");
                    geom.GeometryTypes = (int)GeometricType.GeometricType_All;
                    
                    return geom;
                }
            }
            return null;
        }
    }
}
