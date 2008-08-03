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

namespace FdoToolbox.Core
{
    public enum CommandStatus : int
    {
        E_OK = 0,
        E_FAIL_SDF_CREATE = 1,
        E_FAIL_APPLY_SCHEMA = 2,
        E_FAIL_DESTROY_DATASTORE = 3,
        E_FAIL_CONNECT = 4,
        E_FAIL_SERIALIZE_SCHEMA_XML = 5,
        E_FAIL_CREATE_DATASTORE = 6,
        E_FAIL_BULK_COPY = 7,
        E_FAIL_TASK_VALIDATION = 8,
        E_FAIL_CREATE_CONNECTION = 9,
        E_FAIL_SCHEMA_NOT_FOUND = 10,
        E_FAIL_CLASS_NOT_FOUND = 11,
        E_FAIL_UNSUPPORTED_CAPABILITY = 12
    }
}
