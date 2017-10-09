# [Saucery 3](http://www.nuget.org/packages/saucery3) (for NUnit 3)

Previously released NuGet packages are held in a private repository.  This version is driving forward to integrate with NUnit 3.8.1 and later.

As Saucery is highly dependent on NUnit3, bugs in NUnit3 can impact Saucery.  

Saucery NuGet packages will only be released when Selenium testing against SauceLabs show that Saucery is functioning as expected.

In a Jenkins job, execute your test project in a Windows Batch Command step like this:

    "C:\Program Files (x86)\NUnit.org\nunit-console\nunit3-console.exe" <workspace\relative\path\to\my\test.dll> --result:junit-selenium-testsuite.xml;transform=nunit3-junit.xslt
    exit %%ERRORLEVEL%%

nunit3-junit.xslt can be acquired from <a href="https://github.com/nunit/nunit-transforms/tree/master/nunit3-junit" target="_blank">here</a>.
	
Publish test results in Jenkins with a Post Build "Publish JUnit test result report" step specifying junit-*-testsuite.xml (or whatever filename you specified in the command above).  

Adding the "Embed Sauce Labs reports" additional test report feature and the "Run Sauce Labs Test Publisher" Post build step is also recommended.