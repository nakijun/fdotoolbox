### OSGeo.PostGIS ###

  * The provider is very unstable, any operations with a PostGIS connection may throw anything from win32 exceptions to System.AccessViolationException

  * Changes made after applying a schema to a PostGIS connection are not visible until you remove and re-open the connection

### OSGeo.OGR ###

  * Can get VC++ "pure virtual function calls" at random. This mostly happens in the generic connect dialog. If you are connecting to an OGR format via methods supported by the Express Module, you can safely bypass this problem.

  * Using FDO 3.4.0. There are still occasional instability with this provider.

### OSGeo.KingOracle / King.Oracle ###

  * The providers.xml entry is incorrect, since internally the provider is named King.Oracle. This will cause issues when saving/loading connections. Releases after 0.8.8 will have the proper provider name in providers.xml