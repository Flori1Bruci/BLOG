using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Helpers
{
    public static class AppRoles
    {
        public const string ADMINISTRATOR = "Administrator";
        public const string WORKER = "Worker";
    }

    public static class AppClaims
    {
        public const string WORKER_ID = "WORKER_ID";
    }
}
