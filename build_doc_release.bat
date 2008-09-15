pushd %CD%\Doc
..\Thirdparty\NDoc\NDocConsole.exe -documenter=MSDN-CHM -project=FdoToolbox.release.ndoc
copy msdn-chm\FdoToolbox.chm ..\out\Release\
popd