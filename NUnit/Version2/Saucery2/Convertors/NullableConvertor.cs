using System;

namespace Saucery2.Convertors {
    internal static class NullableConvertor {
        public static T GetValue<T>(this object source) {
            var value = source;

            var t = typeof (T);
            t = Nullable.GetUnderlyingType(t) ?? t;

            return (value == null || DBNull.Value.Equals(value))
                ? default(T)
                : (T) Convert.ChangeType(value, t);
        }
    }
}