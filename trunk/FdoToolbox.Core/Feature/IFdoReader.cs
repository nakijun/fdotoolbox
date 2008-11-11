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
namespace FdoToolbox.Core.Feature
{
    /// <summary>
    /// A "bridge" interface that all fdo reader wrapper classes implement
    /// </summary>
    public interface IFdoReader : System.Data.IDataReader
    {
        bool GetBoolean(string name);
        byte GetByte(string name);
        DateTime GetDateTime(string name);
        FdoPropertyType GetFdoPropertyType(string name);
        double GetDouble(string name);
        byte[] GetGeometry(string name);
        short GetInt16(string name);
        int GetInt32(string name);
        long GetInt64(string name);
        OSGeo.FDO.Expression.LOBValue GetLOB(string name);
        OSGeo.FDO.Common.IStreamReader GetLOBStreamReader(string name);
        float GetSingle(string name);
        string GetString(string name);
        bool IsNull(string name);
        bool ReadNext();
        string[] GeometryProperties { get; }
        string DefaultGeometryProperty { get; }
    }
}
