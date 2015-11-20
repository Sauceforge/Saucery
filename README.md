# Saucery
Documentation and examples for Saucery2 and Saucery3 NuGet packages and JUnit 4 implementation.

## Saucery 2 (for NUnit 2)
[NuGet](http://www.nuget.org/packages/saucery2)

In a windows batch command execute your test project like this:
"C:\Program Files (x86)\NUnit 2.6.4\bin\nunit-console.exe" <workspace\relative\path\to\my\test.dll> /xml=nunit-selenium-testsuite.xml
exit %%ERRORLEVEL%%

Publish test results in Jenkins with a Post Build Publish NUnit test result report step specifying nunit-selenium-testsuite.xml

## Saucery 3 (for NUnit 3)
[NuGet](http://www.nuget.org/packages/saucery3)

In a windows batch command execute your test project like this:
"C:\Program Files (x86)\NUnit.org\nunit-console\nunit3-console.exe" <workspace\relative\path\to\my\test.dll> --result:nunit-selenium-testsuite.xml;format=nunit2
exit %%ERRORLEVEL%%

Publish test results in Jenkins with a Post Build Publish NUnit test result report step specifying nunit-selenium-testsuite.xml

SauceryForJUnit is for JUnit 4.12
