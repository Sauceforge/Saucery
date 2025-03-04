using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.TestStatus;

public class SauceLabsEmulatedStatusNotifier() : SauceLabsStatusNotifier(SauceryConstants.SAUCE_REST_BASE)
{
    private readonly Lock _notifyStatusLock = new();

    public void NotifyEmulatedStatus(string jobId, bool isPassed) {
        lock (_notifyStatusLock)
        {
            NotifyStatus(string.Format(SauceryConstants.JOB_REQUEST, UserName, jobId), isPassed);
        }
    }
}