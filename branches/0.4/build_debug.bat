SET PATH=%PATH%;%systemroot%\Microsoft.NET\Framework\v2.0.50727
msbuild.exe /p:Configuration=Debug FdoToolbox.sln
copy_thirdparty_debug.bat