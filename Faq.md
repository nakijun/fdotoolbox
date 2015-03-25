### Q: What version of FDO does FDO Toolbox use? ###

A: 3.4.0. FDO Toolbox v0.6.0 and older releases use FDO 3.3.x

### Q: How do I supply the ConnectionString parameter for the ODBC Provider? ###

A: You must surround the value in double quotes. Alternatively, use the specialised ODBC connection dialog.

### Q: Where are my preferences stored at? ###

A: %APPDATA%\FDO Toolbox (ie. C:\Documents and Settings\YOURUSERNAME\Application Data\FDO Toolbox)

### Q: I get errors bulk copying between different providers (when it attempts to apply the schema) ###

A: As of version 0.5.8, you will get a detailed explanation of why the source schema can not be applied to the target connection. With this information you can work around this error by re-creating the source schema on the target connection with the incompatible parts changed to parts supported by the target connection.

### Q: I can't see schemas/classes in Oracle (using OSGeo.KingOracle) ###

A: When connecting to Oracle, leave the OracleSchema and KingFdoClass properties blank.

### Q: Does FDO Toolbox support creating Value Constraints for Data Properties? ###

A: Yes

### Q: Does FDO Toolbox support Association Properties? ###

A: Yes

### Q: Does FDO Toolbox support Object Properties? ###

A: Yes

### Q: Does FDO Toolbox support Raster Properties? ###

A: Not at this point in time

### Q: Does FDO Toolbox support class inheritance? ###

A: Not at this point in time

### Q: Does FDO Toolbox support creation of Schema Mappings? ###

A: Not at this point in time

### Q: Can I run FDO Toolbox in Linux (via Mono)? ###

A: No, as the FDO managed wrapper libraries are mixed-mode assemblies and Mono will most likely [never support the capability](http://www.jprl.com/Blog/archive/development/mono/2008/Jan-27.html). However certain parts of FDO Toolbox (such as command-line utilities) are on the roadmap to be ported to linux. To get a full Linux port of FDO Toolbox requires a rewrite of the application (at worst), or a rethinking of how the FDO managed API is created (at best).

### Q: I get crashes on the Map Data Preview for some data sources ###

~~A: FDO stores geometries as FGF (FDO Geometry Format) which is a superset of WKB, the SharpMap rendering library used currently only supports rendering WKB geometries. Thus it will break down on incompatible/unsupported geometries.~~

A: The first release after 0.8.8 will have support for flattening geometries (stripping Z and M ordinates). The side-benefit of this is that we can now render FGF geometries that aren't downward compatible with WKB by stripping out the FGF-specific components (ie. the Z and/or M ordinates) making them WKB-compatible geometries. Long story short: You can now preview **all** data sources.

### Q: Where can I get add-ins for FDO Toolbox? ###

A: [The add-in project page](http://fdotoolbox-addins.googlecode.com). Unfortunatelty it is quite empty at the moment.

### Q: I have an idea for a new feature ###

A: File an enhacement issue.

### Q: Are the APIs stable enough to develop extension modules against? ###

A: Until 1.0 is released, there is no guarantee that the current APIs will remain the same.