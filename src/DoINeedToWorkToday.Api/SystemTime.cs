using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoINeedToWork.Api
{
    public static class SystemTime
    {
        public static Func<DateTime> LocalNow = () => DateTime.Now;
        public static Func<DateTime> UtcNow = () => DateTime.UtcNow;
    }
}
