using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtExercise.Extensions
{
    public static class JwtInformation
    {
        //发行者
        public static string Issure = "jwtExercise";
        //接受者
        public static string Audience = "jwtExercise";
        //秘钥
        public static string SecurityKey = "1234567890123456";
    }
}
