Provider-specific Notes
=======================

.. _shp-constraints:

SHP Provider
------------

 * Geometric Properties can only support one particular Geometric Type. You cannot combine multiple Geometric Types. Eg. You can only have Point, Curve or Surface. You cannot have combinations of the 3.
 * SHP supports a limited set of data types. Take note of this when copying data to SHP.

OGR Provider
------------

[TODO]

SQLite Provider
---------------

[TODO]

PostGIS Provider
----------------

[TODO]

SQL Server Spatial (2008) Provider
----------------------------------

 * This provider is extremely sensitive to geometry validity and will reject any input of invalid geometries, FDO itself does not provide any form of geometry validation. Take note of this when bulk copying to sql server.
 * Bulk copying to this provider is quite slow. Use alternate means of copying data if possible.