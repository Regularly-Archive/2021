using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Kafka.Learning.Consumer
{
    static class UserContext
    {
        private static AsyncLocal<UserInfo> _localUserInfo = new AsyncLocal<UserInfo>();

        public static void SetUserInfo(UserInfo userInfo) => _localUserInfo.Value = userInfo;

        public static UserInfo GetUserInfo() => _localUserInfo.Value;
    }
}
