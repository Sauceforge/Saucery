namespace Saucery.Util {
    internal static class UserChecker {
        internal static bool ItIsMe() {
            return SauceryConstants.SAUCE_USER_NAME.ToLower().Equals(SauceryConstants.MY_USERNAME);
        }
    }
}
