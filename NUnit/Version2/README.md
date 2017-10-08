# [Saucery 2](http://www.nuget.org/packages/saucery2) (for NUnit 2)

In a Jenkins job, execute your test project in a Windows Batch Command step like this:

    "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console.exe" <workspace\relative\path\to\my\test.dll> /xml=nunit-selenium-testsuite.xml
    exit %%ERRORLEVEL%%

Publish test results in Jenkins with a Post Build Publish NUnit test result report step specifying nunit-selenium-testsuite.xml (or whatever filename you specified in the command above).
