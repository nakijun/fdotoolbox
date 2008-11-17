@echo off

SET TYPEACTION=build
SET TYPEBUILD=Release

SET FDOTOOLBOX_OUTDIR=%CD%\out\%TYPEBUILD%
SET DOCPATH=%CD%\Doc
SET THIRDPARTY=%CD%\Thirdparty
SET INSTALL=%CD%\Install
SET FDOINFO=%CD%\FdoInfo
SET FDOUTIL=%CD%\FdoUtil
SET TESTAPP=%CD%\TestApp
SET TESTLIBRARY=%CD%\TestLibrary
SET TESTDATA=%CD%\UnitTestData
SET TESTPATH=%CD%\out\Test
SET FDOTOOLBOX=%CD%\FdoToolbox
SET FDOTOOLBOXCORE=%CD%\FdoToolbox.Core
SET FDOTOOLBOXBASE=%CD%\FdoToolbox.Base
SET FDOTOOLBOXADDINMGR=%CD%\FdoToolbox.AddInManager
SET FDOTOOLBOXADODB=%CD%\FdoToolbox.AdoDb
SET FDOTOOLBOXEXPRESS=%CD%\FdoToolbox.Express
SET TESTMODULE=%CD%\TestModule
SET MGMODULE=%CD%\MGModule

SET PATH=%PATH%;%systemroot%\Microsoft.NET\Framework\v2.0.50727;%THIRDPARTY%\NDoc;%THIRDPARTY%\NSIS
SET VERBOSITY=/v:q

:study_params
if (%1)==() goto start_build

if "%1"=="-help"    goto help_show
if "%1"=="-h"       goto help_show

if "%1"=="-c"       goto get_conf
if "%1"=="-config"  goto get_conf

if "%1"=="-a"       goto get_action
if "%1"=="-action"  goto get_action

if "%1"=="-v"       goto get_verbose
if "%1"=="-v"        goto get_verbose

if "%1"=="-t"       goto test
if "%1"=="-test"     goto test

goto custom_error

:next_param
shift
shift
goto study_params

:get_verbose
SET VERBOSITY=/v:n
goto next_param

:get_conf
SET TYPEBUILD=%2
SET FDOTOOLBOX_OUTDIR=%CD%\out\%TYPEBUILD%
if "%2"=="release" goto next_param
if "%2"=="Release" goto next_param
if "%2"=="debug" goto next_param
if "%2"=="Debug" goto next_param
goto custom_error

:get_action
SET TYPEACTION=%2
if "%2"=="build" goto next_param
if "%2"=="clean" goto next_param
goto custom_error

:start_build
if "%TYPEACTION%"=="build" goto build
if "%TYPEACTION%"=="clean" goto clean

:build
echo Configuration is: %TYPEBUILD%

echo Building FdoToolbox
msbuild.exe /p:Configuration=%TYPEBUILD% %VERBOSITY% FdoToolbox.sln

echo Building API Documentation
pushd %DOCPATH%
NDocConsole.exe -documenter=MSDN-CHM -project=FdoToolbox.%TYPEBUILD%.ndoc
copy "msdn-chm\FDO Toolbox Core API.chm" %FDOTOOLBOX_OUTDIR%
popd

echo Building User Documentation
pushd %DOCPATH%\userdoc_tmphhp
call build_userdoc.bat
copy userdoc.chm %FDOTOOLBOX_OUTDIR%
popd

IF NOT EXIST %FDOTOOLBOX_OUTDIR%\FDO xcopy /S /Y /I %THIRDPARTY%\Fdo\*.* %FDOTOOLBOX_OUTDIR%\FDO

echo Creating installer
pushd %INSTALL%
makensis /DSLN_CONFIG=%TYPEBUILD% FdoToolbox.nsi
popd
goto quit

:clean
echo Cleaning Temp doc directories
rd /S /Q %DOCPATH%\doc
rd /S /Q %DOCPATH%\msdn-chm
echo Cleaning Output Directory
rd /S /Q out
echo Cleaning FdoInfo
rd /S /Q %FDOINFO%\bin
rd /S /Q %FDOINFO%\obj
echo Cleaning FdoUtil
rd /S /Q %FDOUTIL%\bin
rd /S /Q %FDOUTIL%\obj
echo Cleaning TestApp
rd /S /Q %TESTLIBRARY%\bin
rd /S /Q %TESTLIBRARY%\obj
echo Cleaning FdoToolbox
rd /S /Q %FDOTOOLBOX%\bin
rd /S /Q %FDOTOOLBOX%\obj
echo Cleaning FdoToolbox.Core
rd /S /Q %FDOTOOLBOXCORE%\bin
rd /S /Q %FDOTOOLBOXCORE%\obj
echo Cleaning FdoToolbox.Base
rd /S /Q %FDOTOOLBOXBASE%\bin
rd /S /Q %FDOTOOLBOXBASE%\obj
echo Cleaning TestModule
rd /S /Q %TESTMODULE%\bin
rd /S /Q %TESTMODULE%\obj
echo Cleaning MGModule
rd /S /Q %MGMODULE%\bin
rd /S /Q %MGMODULE%\obj
echo Cleaning FdoToolbox.AddInManager
rd /S /Q %FDOTOOLBOXADDINMGR%\bin
rd /S /Q %FDOTOOLBOXADDINMGR%\obj
echo Cleaning FdoToolbox.AdoDb
rd /S /Q %FDOTOOLBOXADODB%\bin
rd /S /Q %FDOTOOLBOXADODB%\obj
echo Cleaning FdoToolbox.Express
rd /S /Q %FDOTOOLBOXEXPRESS%\bin
rd /S /Q %FDOTOOLBOXEXPRESS%\obj
goto quit

:test
echo Building unit tests
msbuild.exe /p:Configuration=Test %VERBOSITY% %TESTLIBRARY%\TestLibrary.csproj
xcopy /S /Y /I %TESTDATA%\*.* %TESTPATH%
xcopy /S /Y /I %THIRDPARTY%\Fdo\*.* %TESTPATH%
xcopy /S /Y /I %THIRDPARTY%\NUnit\bin\*.* %TESTPATH%
echo Running unit tests
%TESTPATH%\nunit-console.exe /labels /wait /exclude=Stress,RawFdo %TESTPATH%\TestLibrary.dll
goto quit

:custom_error
echo The command is not recognized.
echo Please use the format:
:help_show
echo ************************************************************************
echo build.bat [-h]
echo           [-t]
echo           [-v]
echo           [-c=BuildType]
echo           [-a=Action]
echo Help:                  -h[elp]
echo Test:                  -t[est]
echo Verbose:               -v
echo BuildType:             -c[onfig]=Release(default), Debug
echo Action:                -a[ction]=build(default),
echo                                  clean
echo ************************************************************************
:quit
