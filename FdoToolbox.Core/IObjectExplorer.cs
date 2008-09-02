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
using System.Windows.Forms;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Modules;
using FdoToolbox.Core.Common;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Object Explorer interface
    /// </summary>
    public interface IObjectExplorer : IFormWrapper
    {
        /// <summary>
        /// Initialize the object explorer menus
        /// </summary>
        /// <param name="menuMapFile"></param>
        void InitializeMenus(string menuMapFile);
        
        /// <summary>
        /// If a spatial connection node (or child) is selected, returns the
        /// underlying connection object
        /// </summary>
        /// <returns></returns>
        FdoConnectionInfo GetSelectedSpatialConnection();

        /// <summary>
        /// If a schema node is selected, returns the name of the
        /// schema
        /// </summary>
        /// <returns></returns>
        string GetSelectedSchema();

        /// <summary>
        /// If a class node is selected, returns the name of the
        /// class
        /// </summary>
        /// <returns></returns>
        string GetSelectedClass();

        /// <summary>
        /// If a task node is selected, returns the
        /// underlying task object
        /// </summary>
        /// <returns></returns>
        ITask GetSelectedTask();

        /// <summary>
        /// If a module node is selected, returns the
        /// underlying module object
        /// </summary>
        /// <returns></returns>
        IModule GetSelectedModule();

        /// <summary>
        /// Refreshes the selected spatial connection's child nodes
        /// </summary>
        /// <param name="name"></param>
        void RefreshSpatialConnection(string name);

        /// <summary>
        /// Extend the Object Explorer UI
        /// </summary>
        /// <param name="objExplorerExt">The Module's UI Extension file</param>
        void ExtendUI(string uiExtensionFile);

        void UnHide();

        /// <summary>
        /// Refreshes the selected database connection's child nodes
        /// </summary>
        /// <param name="name"></param>
        void RefreshDatabaseConnection(string name);

        /// <summary>
        /// If a database connection node (or child) is selected, returns the
        /// underlying connection object
        /// </summary>
        /// <returns></returns>
        DbConnectionInfo GetSelectedDatabaseConnection();

        /// <summary>
        /// If the database node is selected, gets the name of the selected database
        /// </summary>
        /// <returns></returns>
        string GetSelectedDatabase();

        /// <summary>
        /// If the table node is selected, gets the name of the selected table
        /// </summary>
        /// <returns></returns>
        string GetSelectedTable();

        /// <summary>
        /// Register a new root node that will be accessible to the whole application. The
        /// node to be registered must have a ContextMenuStrip attached.
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="node"></param>
        void RegisterRootNode(string nodeName, TreeNode node);

        /// <summary>
        /// Gets a registered root node
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        TreeNode GetRootNode(string nodeName);

        /// <summary>
        /// Registers a context menu
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="contextMenu"></param>
        void RegisterContextMenu(string nodeName, ContextMenuStrip contextMenu);

        /// <summary>
        /// Gets a registered context menu
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        ContextMenuStrip GetContextMenu(string nodeName);

        /// <summary>
        /// Gets the currently selected node in the Object Explorer
        /// </summary>
        /// <returns></returns>
        TreeNode GetSelectedNode();

        /// <summary>
        /// Registers a image that can be used for node icons in the Object Explorer
        /// </summary>
        /// <param name="key"></param>
        /// <param name="image"></param>
        void RegisterImage(string key, System.Drawing.Image image);
    }
}
