using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.TestStatus;

public class SauceLabsEmulatedStatusNotifier() : SauceLabsStatusNotifier(SauceryConstants.SAUCE_REST_BASE)
{
    public async void NotifyEmulatedStatus(string jobId, bool isPassed) {
        await NotifyStatus(string.Format(SauceryConstants.JOB_REQUEST, UserName, jobId), isPassed);
    }
}