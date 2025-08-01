﻿# Saucery.XUnit

Saucery handles all the plumbing required to integrate with SauceLabs, making writing XUnit tests a breeze, so you only need to tell Saucery *what* you want. Saucery takes care of the *how*.

Note: The tests specified below are provided as examples only. Your tests, of course, will be specific to your System Under Test.

## Sponsoring
Saucery has been developed as an open-source project for over 10 years. If you find it valuable for your projects and team work, please consider supporting it and becoming a  [![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub&color=%23fe8e86)](https://github.com/sponsors/Sauceforge)

## Initial Setup

1. You'll need a SauceLabs account. You can get a free trial account [here](https://saucelabs.com/sign-up).
1. If you want to run your tests locally you need to set 2 environment variables, SAUCE_USER_NAME and SAUCE_API_KEY
1. To run your test suite from your GitHub Actions pipeline you need to set two secrets SAUCE_USER_NAME and SAUCE_API_KEY. Instructions on how to set Github Secrets are [here](https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions#creating-secrets-for-a-repository).

## XUnit3

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