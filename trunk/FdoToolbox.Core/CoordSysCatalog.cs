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
    /// <summary>
    /// A simple data access object to the Coordinate System Catalog which resides
    /// in a SQLite database.
    /// </summary>
    public class CoordSysCatalog : ICoordinateSystemCatalog
    {
        const string DB_FILE = "cscatalog.sqlite";

        private string _ConnectionString;

        public CoordSysCatalog()
        {
            _ConnectionString = string.Format("Data Source={0};Version=3;Compress=True", DB_FILE);
        }

        private BindingList<CoordinateSystem> _Projections;
        
        /// <summary>
        /// Adds a new coordinate system to the database
        /// </summary>
        /// <param name="cs"></param>
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
                        AppConsole.WriteLine("Coordinate System {0} added to database", cs.Name);
                    }
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Updates an existing coordinate system in the database
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="oldName"></param>
        /// <returns></returns>
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
                        AppConsole.WriteLine("Coordinate System {0} updated in database", oldName);
                        return true;
                    }
                }
                conn.Close();
            }
            return false;
        }

        /// <summary>
        /// Deletes a coordinate system from the database
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
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
                        AppConsole.WriteLine("Coordinate System {0} deleted from database", cs.Name);
                        _Projections.Remove(cs);
                        return true;
                    }
                }
                conn.Close();
            }
            return false;
        }

        /// <summary>
        /// Gets all the coordinate systems in the database
        /// </summary>
        /// <returns></returns>
        public BindingList<CoordinateSystem> GetAllProjections()
        {
            if (_Projections != null)
                return _Projections;

            _Projections = new BindingList<CoordinateSystem>();
            SQLiteConnection conn = new SQLiteConnection(_ConnectionString);
            using (conn)
            {
                AppConsole.WriteLine("Loading all Coordinate Systems from {0}", DB_FILE);
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

        /// <summary>
        /// Checks if a coordinate system exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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
