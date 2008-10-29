SET PATH=%PATH%;%systemroot%\Microsoft.NET\Framework\v2.0.50727
msbuild.exe /p:Configuration=Debug FdoToolbox.sln
call copy_thirdparty_debug.bat
call build_doc_debug.bat