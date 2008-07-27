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
using System.Data.SQLite;
using System.ComponentModel;

namespace FdoToolbox.Core
{
    public class CoordSysCatalog : ICoordinateSystemCatalog
    {
        const string DB_FILE = "cscatalog.sqlite";

        private string _ConnectionString;

        public CoordSysCatalog()
        {
            _ConnectionString = string.Format("Data Source={0};Version=3;Compress=True", DB_FILE);
        }

        private BindingList<CoordinateSystem> _Projections;

        public void AddProjection(CoordinateSystem cs)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Projections(Name, Description, WKT) VALUES(@a, @b, @c)", conn))
                {
                    cmd.Parameters.AddWithValue("@a", cs.Name);
                    cmd.Parameters.AddWithValue("@b", cs.Description);
                    cmd.Parameters.AddWithValue("@c", cs.Wkt);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        _Projections.Add(cs);
                    }
                }
                conn.Close();
            }
        }

        public bool UpdateProjection(CoordinateSystem cs, string oldName)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand update = new SQLiteCommand("UPDATE Projections SET Name = @a, Description = @b, WKT = @c WHERE Name = @d", conn))
                {
                    update.Parameters.AddWithValue("@a", cs.Name);
                    update.Parameters.AddWithValue("@b", cs.Description);
                    update.Parameters.AddWithValue("@c", cs.Wkt);
                    update.Parameters.AddWithValue("@d", oldName);
                    if (update.ExecuteNonQuery() == 1)
                    {
                        return true;
                    }
                }
                conn.Close();
            }
            return false;
        }

        public bool DeleteProjection(CoordinateSystem cs)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand delete = new SQLiteCommand("DELETE FROM Projections WHERE Name = @a", conn))
                {
                    delete.Parameters.AddWithValue("@a", cs.Name);
                    if (delete.ExecuteNonQuery() == 1)
                    {
                        _Projections.Remove(cs);
                        return true;
                    }
                }
                conn.Close();
            }
            return false;
        }

        public BindingList<CoordinateSystem> GetAllProjections()
        {
            if (_Projections != null)
                return _Projections;

            _Projections = new BindingList<CoordinateSystem>();
            SQLiteConnection conn = new SQLiteConnection(_ConnectionString);
            using (conn)
            {
                conn.Open();
                string name = string.Empty;
                string desc = string.Empty;
                string wkt = string.Empty;
                SQLiteCommand cmd = new SQLiteCommand("SELECT Name, Description, WKT FROM Projections", conn);
                using (cmd)
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        name = reader.GetString(reader.GetOrdinal("Name"));
                        desc = reader.GetString(reader.GetOrdinal("Description"));
                        wkt = reader.GetString(reader.GetOrdinal("WKT"));
                        _Projections.Add(new CoordinateSystem(name, desc, wkt));
                    }
                }
                conn.Close();
            }

            return _Projections;
        }

        public bool ProjectionExists(string name)
        {
            if (_Projections == null)
                GetAllProjections();

            foreach (CoordinateSystem cs in _Projections)
            {
                if (cs.Name == name)
                    return true;
            }
            return false;
        }
    }
}
