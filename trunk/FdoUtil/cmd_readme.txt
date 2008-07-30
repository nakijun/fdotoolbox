FdoToolbox Command-line Utilities readme
----------------------------------------

The following command-line utilities are included with FdoToolbox:

- FdoUtil.exe

The general invocation is as follows:

FdoUtil.exe -cmd:<command name> [-quiet] [-test] <command parameters>

The valid list of commands include:
- ApplySchema
- CreateDataStore
- Destroy
- DumpSchema
- ExpressBCP
- MakeSdf
- RunTask

Where applicable, the -test switch performs a FDO capability check to determine if 
execution can go ahead.

Where applicable, the -quiet switch will suppress all console output. This is not 
entirely the case however for debug builds of the command-line utilities

FdoUtil.exe will return 0 for successful execution and will return a non-zero value
otherwise. Consult CommandStatus.cs for the list of return codes.

For commands that require a connection string parameter (see below) the connection 
string is of the following format:

[Name1]=[Value1];[Name2]=[Value2];...

Some examples:

SDF: -connection:File=C:\Test\Test.sdf
SHP: -connection:DefaultFileLocation=C:\Test\Test.shp
MySQL: -connection:Username=root;Password=1234;Service=localhost:3306;DataStore=mydatastore

Each command is described in further detail below:

ApplySchema
-----------

Description: Applies a schema definition xml file to a FDO data source

Usage: FdoUtil.exe -cmd:ApplySchema -file:<schema definition file> -provider:<provider> -connection:<connection string> [-quiet] [-test]

Notes: This will only work on providers that implement the IApplySchema command. 

Destroy
-------

Description: Destroys a datastore in an FDO data source

Usage: FdoUtil.exe -cmd:Destroy -provider:<provider> -properties:<data store properties> [-connection:<connection string>] [-quiet]

Notes: 

The -properties parameter is a the same format as the connection string
eg. For OSGeo.MySQL: -properties:DataStore=mydatastore

The -connection parameter is only required for rdbms-based providers. Usually rdbms-based 
providers require a DataStore parameter as part of the connection. This is not required
in this case.

DumpSchema
----------

Description: Writes a schema in a FDO data store to an XML file

Usage: FdoUtil.exe -cmd:DumpSchema -file:<schema file> -provider:<provider> -connection:<connection string> -schema:<selected schema> [-quiet]

Notes: n/a

ExpressBCP
----------

Description: Copies the contents of a feature schema (or optionally, a list of classes within) into a 
flat-file data store.

Usage: FdoUtil.exe -cmd:ExpressBCP -src_provider:<provider name> -src_conn:<connection
string> -dest_provider:<provider name> -dest_file:<file> -schema:<source
schema name> [-classes:<comma-separated list of class names>]
[-copy_srs:<source spatial context name>] [-quiet]

Notes:

If the -classes parameter is omitted, all classes under the given schema
will be copied

Any class names not found will be ignored during the copy process.

For this initial version, no attribute or any spatial filtering will be
performed.

The dest_provider parameter can only be the following (at the moment):

- OSGeo.SDF


MakeSdf
-------

Description: Create a new SDF with the option of applying a schema to it.

Usage: FdoUtil.exe -cmd:MakeSdf -path:<path to sdf file> [-schema:<path to schema file>] [-quiet]

Notes: If the -schema parameter points to an non-existent file or is not valid, schema
application will not take place.

CreateDataStore
---------------

Description: Create a new FDO data store

Usage: FdoUtil.exe -cmd:CreateDataStore -provider:<provider> -properties:<data store properties> [-connection:<connection string>] [-quiet] [-test]

Notes:

The -properties parameter is a the same format as the connection string
eg. For OSGeo.MySQL: -properties:DataStore=mydatastore

The -connection parameter is only required for rdbms-based providers. Usually rdbms-based 
providers require a DataStore parameter as part of the connection. This is not required
in this case.

RunTask
-------

Description: Loads and executes a Task Definition

Usage: FdoUtil.exe -cmd:RunTask -file:<task definition file> [-log:<log file>] [-quiet]

Notes: 

For bulk copy tasks, both the source and target connections must be valid 
connections. Otherwise execution will fail.