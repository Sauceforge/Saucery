using Saucery.Core.OnDemand.Base;
using System.Text;

namespace Saucery.Core.Util;

public static class Extensions {
    public static string ToCSV(this List<SaucePlatform> platforms) {
        StringBuilder sb = new();

        foreach(var platform in platforms) {
            sb.AppendLine($"{platform.LongName} {platform.LongVersion} {platform.Browser} {platform.BrowserVersion} {platform.Os} {platform.ScreenResolution} {platform.DeviceOrientation}");
        }

        return sb.ToString();
    }
}
