using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.TestStatus;

public class SauceLabsEmulatedStatusNotifier() : SauceLabsStatusNotifier(SauceryConstants.SAUCE_REST_BASE)
{
    public void NotifyEmulatedStatus(string jobId, bool isPassed) {
        base.NotifyStatus(string.Format(SauceryConstants.JOB_REQUEST, UserName, jobId), isPassed);
    }
}