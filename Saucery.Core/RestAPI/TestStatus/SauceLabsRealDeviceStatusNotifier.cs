using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.TestStatus;

public class SauceLabsRealDeviceStatusNotifier() : SauceLabsStatusNotifier(SauceryConstants.SAUCE_REAL_DEVICE_REST_BASE)
{
    public void NotifyRealDeviceStatus(string jobId, bool isPassed) {
        NotifyStatus(string.Format(SauceryConstants.RD_JOB_REQUEST, jobId), isPassed);
    }
}