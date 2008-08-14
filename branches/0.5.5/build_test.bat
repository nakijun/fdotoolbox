SET PATH=%PATH%;%systemroot%\Microsoft.NET\Framework\v2.0.50727
msbuild.exe /p:Configuration=Debug TestLibrary\TestLibrary.csproj
xcopy /S /Y /I UnitTestData\*.* out\Test
copy_thirdparty_test.bat