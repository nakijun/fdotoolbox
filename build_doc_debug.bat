pushd %CD%\Doc
..\Thirdparty\NDoc\NDocConsole.exe -documenter=MSDN-CHM -project=FdoToolbox.debug.ndoc
copy msdn-chm\FdoToolbox.chm ..\out\Debug\
popd