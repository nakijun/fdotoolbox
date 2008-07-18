SET PATH=%PATH%;%systemroot%\Microsoft.NET\Framework\v2.0.50727
msbuild.exe /p:Configuration=Release FdoToolbox.sln
copy_thirdparty_release.bat