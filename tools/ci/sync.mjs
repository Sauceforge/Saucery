import {exec,policy} from './lib.mjs';
const solution='Saucery.slnx';
for(const args of policy.syncOrder)exec('saucery-nuget',['--solution',solution,...args]);
exec('saucery-nuget',['--solution',solution,'--exclude-projects','Saucery.NuGet','Saucery.Core','Saucery.TUnit','Saucery.XUnit3','Saucery.XUnit','Saucery','Saucery.Playwright.NUnit']);
exec('saucery-nuget',['--solution',solution,'--scan-unregistered']);
