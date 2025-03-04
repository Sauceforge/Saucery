using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.TestStatus;

public class SauceLabsRealDeviceStatusNotifier() : SauceLabsStatusNotifier(SauceryConstants.SAUCE_REAL_DEVICE_REST_BASE)
{
    private readonly Lock _notifyStatusLock = new();

    public void NotifyRealDeviceStatus(string jobId, bool isPassed) {
        lock (_notifyStatusLock)
        {
            NotifyStatus(string.Format(SauceryConstants.RD_JOB_REQUEST, jobId), isPassed);
        }
    }
}