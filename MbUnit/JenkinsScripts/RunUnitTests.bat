SET NUNIT="C:\Program Files (x86)\NUnit 2.6.4\bin\nunit-console.exe"
SET "NUNITSWITCHES=/nologo /noshadow /framework=net-4.5"
SET "OUTPUTDIR=TestReports"

IF NOT EXIST "%WORKSPACE%\%OUTPUTDIR%" (
    MKDIR %OUTPUTDIR%
)

REM CONTROLLER TESTS
%NUNIT% NUnit\Version3\UnitTests\bin\Release\UnitTests.dll %NUNITSWITCHES% /xml=%OUTPUTDIR%\nunit-saucery3-testsuite.xml

exit %%ERRORLEVEL%%