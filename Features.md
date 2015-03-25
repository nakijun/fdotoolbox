## Data Management ##

### Connect to a wide-variety of geospatial data sources ###

See [[FDO project page](http://fdo.osgeo.org/OSProviderOverviews.html)] for a list of supported geospatial data sources.

### Create geospatial data stores ###

See [[FDO project page](http://fdo.osgeo.org/OSFeatureMatrix.html)] for a list of providers which support data store creation.

### Manage geospatial data ###

  * Create and edit feature schemas <sup>1</sup>
  * Create and edit feature classes and its properties <sup>1</sup>
  * Create and edit spatial contexts <sup>1</sup>
  * Full CRUD (Create, Read, Update, Delete) support <sup>1</sup>

### Save/Load schema configurations ###

  * Save schema to XML
  * Load and apply a schema from XML <sup>1</sup>
  * Apply schema to new SDF file

## Data Preview ##

  * Preview geospatial data using a variety of methods
    * Standard FDO query
    * Standard FDO aggregate query <sup>1</sup>
    * SQL query <sup>1</sup>

## Batching/Automation ##

  * Command-line utilites to automate common geo-processing tasks.

## Bulk Copy ##

  * Copy any number of Feature Classes and its properties from one source to another <sup>1</sup>
  * Save/Load bulk copy tasks.
  * Convert properties of different types.
  * Map FDO expressions to properties.

## Spatial/Non-Spatial Data Integration ##

  * Join/Merge Spatial/Non-Spatial data into a new joined spatial data source.

## Extensibility ##

All menus in FDO Toolbox are XML driven, you can edit these files to customise the user interface.

FDO Toolbox can be extended through Add-Ins. With Add-Ins you can add new commands for custom functionality and include new menu entries to expose them in the user interface.

FDO Toolbox also includes an IronPython scripting engine, allowing you to script/extend FDO Toolbox using Python.




<sup>1</sup> Certain FDO providers do not support such a feature.