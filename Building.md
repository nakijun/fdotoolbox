## Important ##

As of [r1040](https://code.google.com/p/fdotoolbox/source/detail?r=1040), you will now need the .net Framework 3.5 or higher to build FDO Toolbox.

## Check out source from SVN ##

Follow the instructions in the "Source" tab of this project site.

## Requirements ##

In order to build the User and API documentation, you will need the following tools installed:

  * [HTML Help Workshop](http://www.microsoft.com/downloads/details.aspx?familyid=00535334-c8a6-452f-9aa0-d597d16580cc&displaylang=en)
  * [Python](http://www.python.org)
  * [Sphinx](http://sphinx.pocoo.org)
  * [Sandcastle May 2008 edition](http://sandcastle.codeplex.com)
  * [Sandcastle Help File Builder](http://shfb.codeplex.com)

## Building ##

To build FDO Toolbox run build.bat, the following options are available

```
build.bat [-h]
          [-t]
          [-v]
          [-c=BuildType]
          [-a=Action]

Help:          -h[elp]
Test:          -t[est]
Verbose:       -v
BuildType:     -c[onfig]=Release(default), Debug
Action:        -a[ction]=build(default), clean
```

When running build.bat with the -t switch, the unit tests will be built and run instead of the main source code.

To run the unit tests, do any of the following:

  * type: build.bat -t
  * type: nunit-console.exe TestLibrary.dll
  * load and run TestLibrary.dll in the NUnit GUI application.
  * run the TestRunner.exe application which will execute the unit tests.

Additionally you can set the TestRunner project as the startup project to allow for debugging of unit tests.

## A special note when building in Visual Studio ##

If you switch platforms (eg. x86 to x64) make sure you **clean** the solution first before debugging or building, otherwise you may have thirdparty libraries built for the wrong platform being copied to your output directory.