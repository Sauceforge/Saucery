﻿using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.OnDemand.Base;
using System.Collections;
using System.Collections.Generic;

namespace Saucery.DataSources;

public class SauceryTestData : IEnumerable {
    protected static List<BrowserVersion> BrowserVersions { get; set; }

    public IEnumerator GetEnumerator() => BrowserVersions?.GetEnumerator();

    protected static void SetPlatforms(List<SaucePlatform> platforms)
    {
        PlatformConfigurator platformConfigurator = new(PlatformFilter.ALL);
        PlatformExpander expander = new(platformConfigurator, platforms);
        List<SaucePlatform> expandedPlatforms = expander.Expand();
        BrowserVersions = platformConfigurator.FilterAll(expandedPlatforms);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/