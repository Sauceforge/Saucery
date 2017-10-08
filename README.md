# Saucery
Everything you need to integrate [Jenkins](https://jenkins-ci.org) with [SauceLabs](https://saucelabs.com).

Implementation for [NUnit 2.6.3](http://nunit.org), [NUnit 3](http://nunit.org) and [JUnit 4](http://junit.org).  

Experimental implementations for MbUnit and XUnit.

## General Notes
Saucery is compatible with any CI server that the [SauceOnDemand](https://github.com/jenkinsci/sauce-ondemand-plugin) plugin supports.

If you have Resharper installed the Saucery classes will appear in red.  However the solution will still build perfectly well.  This is a Resharper bug over which we have no control.

You should add Saucery NuGet packages to an empty class library, not a Unit Test project.

| Platform you wish to target | Write your tests with | Base class you should use |
| --------------------------- | --------------------- | ------------------------- | 
| Desktop browser             | Selenium              | SauceryBase               |
| Android device              | Appium                | SauceryAndroidBase        |
| IOS device                  | Appium                | SauceryIOSBase            |

## [Saucery 2](http://www.nuget.org/packages/saucery2) (for NUnit 2)

In a Jenkins job, execute your test project in a Windows Batch Command step like this:

    "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console.exe" <workspace\relative\path\to\my\test.dll> /xml=nunit-selenium-testsuite.xml
    exit %%ERRORLEVEL%%

Publish test results in Jenkins with a Post Build Publish NUnit test result report step specifying nunit-selenium-testsuite.xml (or whatever filename you specified in the command above).

## Saucery (for JUnit 4)

Can be deployed to bintray in future.