namespace Saucery2.RestAPI.FlowControl.Base {
    internal abstract class FlowController : RestBase {
        protected abstract bool TooManyTests();
        public abstract void ControlFlow();
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */