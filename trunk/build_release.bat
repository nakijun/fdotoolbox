SET PATH=%PATH%;%systemroot%\Microsoft.NET\Framework\v2.0.50727
msbuild.exe /p:Configuration=Release FdoToolbox.sln
call copy_thirdparty_release.bat
call build_doc_release.bat