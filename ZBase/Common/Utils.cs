using System;

namespace ZBase.Common {
    public static class Utils {
        public static DateTime GetUnixEpoch() {
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}