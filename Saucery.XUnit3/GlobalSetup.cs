using System.Runtime.CompilerServices;

namespace Saucery.XUnit3;

public static class GlobalSetup
{
    [ModuleInitializer]
    public static void Setup() { } 
        //XunitContext.EnableExceptionCapture();
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 31st May 2025
* 
*/