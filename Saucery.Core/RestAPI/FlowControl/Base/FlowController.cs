namespace Saucery.Core.RestAPI.FlowControl.Base; 

public abstract class FlowController : RestBase {
    protected abstract bool TooManyTests(bool realDevices);
    public abstract void ControlFlow(bool realDevices);
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/