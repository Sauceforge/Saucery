<h1 align="center">

<img src="/Saucery.Core/Images/Saucery.Core.png" alt="Saucery" width="200"/>
<br/>
Saucery
</h1>

<div align="center">
    
<b>Automated testing made more awesome</b>

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/0ba3cb69efe14366af8c84e485e80077)](https://app.codacy.com/gh/Sauceforge/Saucery/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
![Codacy coverage](https://img.shields.io/codacy/coverage/0ba3cb69efe14366af8c84e485e80077)
![Created](https://img.shields.io/github/created-at/sauceforge/saucery)

![GitHub Repo stars](https://img.shields.io/github/stars/Sauceforge/Saucery) 
[![GitHub Sponsors](https://img.shields.io/github/sponsors/Sauceforge)](https://github.com/sponsors/Sauceforge)
![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/Sauceforge/Saucery/pipeline.yml) 
![Commits](https://img.shields.io/github/commit-activity/t/sauceforge/saucery)
![GitHub last commit (master)](https://img.shields.io/github/last-commit/Sauceforge/Saucery/master)
![Website](https://img.shields.io/website?url=https%3A%2F%2Fsauceforge.github.io)
![License](https://img.shields.io/github/license/Sauceforge/Saucery) 

</div>

Saucery handles all the plumbing required to integrate with SauceLabs, making writing tests a breeze. so you only need to tell Saucery *what* you want. Saucery takes care of the *how*.

Saucery comes in multiple flavors supporting popular test frameworks. Targetting all real and emulated [platforms](https://saucelabs.com/products/platform-configurator) for Appium and Selenium 4+.

The templates below include example tests.

### Packages

| Package | Badges |
| --- | --- |
| Saucery | [![nuget](https://img.shields.io/nuget/v/Saucery.svg)](https://www.nuget.org/packages/Saucery/)  [![NuGet Downloads](https://img.shields.io/nuget/dt/Saucery)](https://www.nuget.org/packages/Saucery/) |
| Saucery.XUnit | [![nuget](https://img.shields.io/nuget/v/Saucery.XUnit.svg)](https://www.nuget.org/packages/Saucery.XUnit/)  [![NuGet Downloads](https://img.shields.io/nuget/dt/Saucery.XUnit)](https://www.nuget.org/packages/Saucery.XUnit/) |
| Saucery.TUnit | [![nuget](https://img.shields.io/nuget/v/Saucery.TUnit.svg)](https://www.nuget.org/packages/Saucery.TUnit/)  [![NuGet Downloads](https://img.shields.io/nuget/dt/Saucery.TUnit)](https://www.nuget.org/packages/Saucery.TUnit/) |
| Saucery.XUnit.v3 | [![nuget](https://img.shields.io/nuget/v/Saucery.XUnit.v3.svg)](https://www.nuget.org/packages/Saucery.XUnit.v3/)  [![NuGet Downloads](https://img.shields.io/nuget/dt/Saucery.XUnit.v3)](https://www.nuget.org/packages/Saucery.XUnit.v3/) |

### Sponsoring
Saucery has been developed as an open-source project for over 10 years. If you find it valuable for your projects and team work, please consider supporting it and becoming a  [![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub&color=%23fe8e86)](https://github.com/sponsors/Sauceforge)

### Dog food Status

We test Saucery itself on SauceLabs!

[![Build Status](https://app.saucelabs.com/buildstatus/saucefauge?saucy)](https://app.saucelabs.com/buildstatus/saucefauge?saucy)

### Initial Setup

These steps apply to all flavors:

1. You'll need a SauceLabs account. You can get a free trial account [here](https://saucelabs.com/sign-up).
1. If you want to run your tests locally you need to set 2 environment variables, SAUCE_USER_NAME and SAUCE_API_KEY
1. To run your test suite from your GitHub Actions pipeline you need to set two secrets SAUCE_USER_NAME and SAUCE_API_KEY. Instructions on how to set Github Secrets are [here](https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions#creating-secrets-for-a-repository).

### NUnit

<img src="/Saucery/Images/Saucery.NUnit.png" alt="Saucery" width="100"/>

#### 🏁 Quick Start

#### Using the Project Template (Recommended)
```bash
cd Templates
dotnet new install .\NUnit
dotnet new saucery-nunit -n "MyTestProject"
```

### XUnit

<img src="/Saucery.XUnit/Images/Saucery.XUnit.png" alt="Saucery.XUnit" width="100"/>

#### 🏁 Quick Start

#### Using the Project Template (Recommended)
```bash
cd Templates
dotnet new install .\XUnit
dotnet new saucery-xunit -n "MyTestProject"
```

### TUnit

<img src="/Saucery.TUnit/Images/Saucery.TUnit.png" alt="Saucery.XUnit" width="100"/>

#### 🏁 Quick Start

#### Using the Project Template (Recommended)
```bash
cd Templates
dotnet new install .\TUnit
dotnet new saucery-tunit -n "MyTestProject"
```

### XUnit-v3

<img src="/Saucery.XUnit/Images/Saucery.XUnit.png" alt="Saucery.XUnit" width="100"/>

#### 🏁 Quick Start

#### Using the Project Template (Recommended)
```bash
cd Templates
dotnet new install .\XUnit3
dotnet new saucery-xunit3 -n "MyTestProject"
```

## Platform Range Expansion

Platform range expansion is a feature unique to Saucery. Say you wanted to test on a range of browser versions but you didn't want to specify each individually. That's fine. Saucery supports specifying ranges.

```csharp
new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "100->119")
```

This will test on Windows 11 Chrome all available versions from 100 to 119 inclusive.

## Real Devices

Yes, Saucery supports Real Devices!

### Trends

[Nuget downloads](https://nugettrends.com/packages?months=24&ids=Saucery&ids=Saucery.XUnit&ids=Saucery.TUnit&ids=Saucery.Core&ids=Saucery.XUnit.v3)  
[GitHub stars](https://star-history.com/#sauceforge/Saucery)

## Contact

Author: Andrew Gray  
Twitter: [@agrayz](https://twitter.com/agrayz)