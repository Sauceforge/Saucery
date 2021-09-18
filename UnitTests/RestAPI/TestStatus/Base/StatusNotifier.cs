namespace UnitTests.RestAPI.TestStatus.Base {
    public abstract class StatusNotifier : RestBase {
        public abstract void NotifyStatus(string jobId, bool isPassed);
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 29th July 2014
 * 
 */