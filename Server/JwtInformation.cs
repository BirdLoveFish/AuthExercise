using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public static class JwtInformation
    {
        //发行者
        public static string Issure = "zhang";
        //接受者
        public static string Audience = Issure;
        //秘钥
        public static string SecurityKey = "1234567890123456";
    }
}
