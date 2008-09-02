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
rd /S /Q TestLibrary\bin
rd /S /Q TestLibrary\obj
echo "Cleaning FdoToolbox"
rd /S /Q FdoToolbox\bin
rd /S /Q FdoToolbox\obj
echo "Cleaning FdoToolbox.Core"
rd /S /Q FdoToolbox.Core\bin
rd /S /Q FdoToolbox.Core\obj
echo "Cleaning FdoToolbox.UI"
rd /S /Q FdoToolbox.UI\bin
rd /S /Q FdoToolbox.UI\obj
echo "Cleaning TestModule"
rd /S /Q TestModule\bin
rd /S /Q TestModule\obj
echo "Cleaning MGModule"
rd /S /Q MGModule\bin
rd /S /Q MGModule\obj