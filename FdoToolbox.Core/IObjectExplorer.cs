using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Core
{
    public interface IObjectExplorer : IFormWrapper
    {
        //Connections

        /// <summary>
        /// If a connection node is selected, returns the
        /// underlying connection object
        /// </summary>
        /// <returns></returns>
        ConnectionInfo GetSelectedConnection();

        //Tasks

        /// <summary>
        /// If a task node is selected, returns the
        /// underlying task object
        /// </summary>
        /// <returns></returns>
        ITask GetSelectedTask();

        //Modules

        /// <summary>
        /// If a module node is selected, returns the
        /// underlying module object
        /// </summary>
        /// <returns></returns>
        IModule GetSelectedModule();

        /// <summary>
        /// Refreshes the selected connection's child nodes
        /// </summary>
        /// <param name="name"></param>
        void RefreshConnection(string name);
    }
}
