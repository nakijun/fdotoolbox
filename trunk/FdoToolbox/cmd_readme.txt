FdoToolbox Command-line Utilities readme
----------------------------------------

The following command-line utilities are included with FdoToolbox:

- ApplySchema.exe
- Destroy.exe
- DumpSchema.exe
- ExpressBCP.exe
- MakeSdf.exe
- Mkdstore.exe
- TaskRun.exe

For utilities that require a connection string parameter (see below) the connection 
string is of the following format:

[Name1]=[Value1];[Name2]=[Value2];...

Some examples:

SDF: -connection:File=C:\Test\Test.sdf
SHP: -connection:DefaultFileLocation=C:\Test\Test.shp
MySQL: -connection:Username=root;Password=1234;Service=localhost:3306;DataStore=mydatastore

Each utility is described in further detail below:

ApplySchema.exe
---------------

Description: Applies a schema definition xml file to a FDO data source

Usage: ApplySchema.exe -file:<schema definition file> -provider:<provider> -connection:<connection string>

Notes: This will only work on providers that implement the IApplySchema command. 

Destroy.exe
-----------

Description: Destroys a datastore in an FDO data source

Usage: Destroy.exe -provider:<provider> -properties:<data store properties> [-connection:<connection string>]

Notes: 

The -properties parameter is a the same format as the connection string
eg. For OSGeo.MySQL: -properties:DataStore=mydatastore

The -connection parameter is only required for rdbms-based providers. Usually rdbms-based 
providers require a DataStore parameter as part of the connection. This is not required
in this case.

DumpSchema.exe
--------------

Description: Writes a schema in a FDO data store to an XML file

Usage: DumpSchema.exe -file:<schema file> -provider:<provider> -connection:<connection string> -schema:<selected schema>

Notes: n/a

ExpressBCP.exe
--------------

Description: Copies the contents of a feature schema (or optionally, a list of classes within) into a 
flat-file data store.

Usage: ExpressBCP.exe -src_provider:<provider name> -src_conn:<connection
string> -dest_provider:<provider name> -dest_file:<file> -schema:<source
schema name> [-classes:<comma-separated list of class names>]
[-copy_srs:<source spatial context name>]

Notes:

If the -classes parameter is omitted, all classes under the given schema
will be copied

Any class names not found will be ignored during the copy process.

For this initial version, no attribute or any spatial filtering will be
performed.

The dest_provider parameter can only be the following (at the moment):

- OSGeo.SDF


MakeSdf.exe
-----------

Description: Create a new SDF with the option of applying a schema to it.

Usage: MakeSdf.exe -path:<path to sdf file> [-schema:<path to schema file>]

Notes: If the -schema parameter points to an non-existent file or is not valid, schema
application will not take place.

Mkdstore.exe
------------

Description: Create a new FDO data store

Usage: Mkdstore.exe -provider:<provider> -properties:<data store properties> [-connection:<connection string>]

Notes:

The -properties parameter is a the same format as the connection string
eg. For OSGeo.MySQL: -properties:DataStore=mydatastore

The -connection parameter is only required for rdbms-based providers. Usually rdbms-based 
providers require a DataStore parameter as part of the connection. This is not required
in this case.

TaskRun.exe
-----------

Description: Loads and executes a Task Definition

Usage: TaskRun.exe -file:<task definition file> [-log:<log file>]

Notes: 

For bulk copy tasks, both the source and target connections must be valid 
connections. Otherwise execution will fail.