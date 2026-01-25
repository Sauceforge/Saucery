using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.TestStatus;

public class SauceLabsRealDeviceStatusNotifier() : SauceLabsStatusNotifier(SauceryConstants.SAUCE_REAL_DEVICE_REST_BASE)
{
    public async void NotifyRealDeviceStatus(string jobId, bool isPassed) {
        await NotifyStatus(string.Format(SauceryConstants.RD_JOB_REQUEST, jobId), isPassed);
    }
}