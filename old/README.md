# Saucery
Everything you need to integrate [Jenkins](https://jenkins-ci.org) with [SauceLabs](https://saucelabs.com).

Implementation for [NUnit 2.6.3](http://nunit.org), [NUnit 3](http://nunit.org) and [JUnit 4](http://junit.org).  

Experimental implementation against XUnit.

## General Notes
Saucery is compatible with any CI server that the [SauceOnDemand](https://github.com/jenkinsci/sauce-ondemand-plugin) plugin supports.

If you have Resharper installed the Saucery classes will appear in red.  However the solution will still build perfectly well.  This is a Resharper bug over which we have no control.

You should add Saucery NuGet packages to an empty class library, not a Unit Test project.

| Platform you wish to target | Write your tests with | Base class you should use |
| --------------------------- | --------------------- | ------------------------- | 
| Desktop browser             | Selenium              | SauceryBase               |
| Android device              | Appium                | SauceryAndroidBase        |
| IOS device                  | Appium                | SauceryIOSBase            |