set OUTDIR=%CD%\out\Debug
set PATH=%PATH%;%CD%\Thirdparty\NDoc

pushd %CD%\Doc
NDocConsole.exe -documenter=MSDN-CHM -project=FdoToolbox.debug.ndoc
copy msdn-chm\FdoToolbox.chm %OUTDIR%
popd

pushd %CD%\Doc\userdoc_tmphhp
call build_userdoc.bat
copy userdoc.chm %OUTDIR%
popd