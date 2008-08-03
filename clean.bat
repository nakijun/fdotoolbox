@echo off
echo "Cleaning Output Directory"
rd /S /Q out
echo "Cleaning FdoInfo"
rd /S /Q FdoInfo\bin
rd /S /Q FdoInfo\obj
echo "Cleaning FdoUtil"
rd /S /Q FdoUtil\bin
rd /S /Q FdoUtil\obj
echo "Cleaning TestApp"
rd /S /Q TestApp\bin
rd /S /Q TestApp\obj
echo "Cleaning FdoToolbox"
rd /S /Q FdoToolbox\bin
rd /S /Q FdoToolbox\obj
echo "Cleaning FdoToolbox.Core"
rd /S /Q FdoToolbox.Core\bin
rd /S /Q FdoToolbox.Core\obj
echo "Cleaning TestModule"
rd /S /Q TestModule\bin
rd /S /Q TestModule\obj