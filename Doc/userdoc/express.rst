The Express Add-In
==================

[TODO]

.. index::
   single: Connecting to Data; Express

.. _connect-express:

Creating connections
--------------------

The Express Add-In offers a faster method of creating connections to spatial data sources is through the Connection Dialogs in the Express Add-In. These dialogs
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
 
Creating Flat Files
-------------------

The Express Add-In also offers a faster method of creating data stores. For example, creating a SDF data store is as simple as opening a save dialog and entering
the name of the SDF file to create. The following providers, have support for creating data stores:

 * SDF
 * SHP
 * SQLite
 
When creating a data store, the following dialog will be presented:

[Image]

Fill in the fields and click :guilabel:`OK` to create the data store.

For SHP, the feature schema **is required**. Creating a SHP file will fail if a feature schema definition is not specified.

Express Bulk Copy
-----------------

The Express Add-In also offers a simple bulk copy from one file-based data store to another. This is a very useful tool for doing conversions from one spatial data
format to another. The Express Bulk Copy supports the following providers:

 * SDF
 * SHP
 * SQLite
 
Please take note of the following when using the Express Bulk Copy:

 * When copying from source to target, the target is always overwritten.
 * Copying to SHP may fail if certain conditions are not met. See :ref:`shp-constraints`