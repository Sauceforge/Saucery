﻿using System.Runtime.CompilerServices;

namespace Saucery.XUnit;

public static class GlobalSetup
{
    [ModuleInitializer]
    public static void Setup() => XunitContext.EnableExceptionCapture();
}