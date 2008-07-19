@echo off
echo "Cleaning Output Directory"
rd /S /Q out
echo "Cleaning FdoToolbox"
rd /S /Q FdoToolbox\bin
rd /S /Q FdoToolbox\obj
echo "Cleaning FdoToolbox.Core"
rd /S /Q FdoToolbox.Core\bin
rd /S /Q FdoToolbox.Core\obj
echo "Cleaning TestModule"
rd /S /Q TestModule\bin
rd /S /Q TestModule\obj