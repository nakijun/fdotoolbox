Connecting to Data
==================

.. index::
   single: Generic Connection Method

Using the Generic Dialog
------------------------

The generic dialog is a "one size fits all" approach to connecting to a data source. The generic dialog can connect to any FDO provider.

To connect, enter a name for this connection and select a provider from the **Provider** combo box.

[Image]

When selected, the property grid will be initialized with the required parameters. Enter the required parameters and click **Connect** to
create a new connection. You can click **Test** beforehand to ensure the connection is a valid one.

[Image]

When the connection is created, a new connection object is visible in the **Object Explorer**

.. index::
   single: Express Connection Method

Using the Express Connection Dialogs
------------------------------------

Another (faster) method of creating connections to spatial data sources is through the Connection Dialogs in the Express Add-In. These dialogs
have been optimised for specific providers. For example, creating a SDF connection is as simple as browsing for the SDF file.

The following providers, have express connection support:

 * SDF
 * SHP
 * SQLite
 * ODBC
  
   * Jet (Microsoft Access)
   * SQL Server
   * Microsoft Excel
   * Text
 
 * OGR
  
   * MapInfo
   * CSV
   * ESRI Personal GeoDB
   
 * PostGIS
 * MySQL

.. index::
   single: Save Connection; Load Connection

Saving/Loading connections
--------------------------

Connections can be saved and loaded between sessions, which is really useful when working with specific data sources very frequently. To save 
a connection, right click the connection object in the **Object Explorer** and choose **Save Connection**

Connections can be loaded by right clicking the **FDO Data Sources** node in the **Object Explorer** and choosing **Load Connection**

When you shut down FDO Toolbox, any open connections are automatically saved and are recreated when you next run FDO Toolbox.

Connection Notes
----------------

[TODO]