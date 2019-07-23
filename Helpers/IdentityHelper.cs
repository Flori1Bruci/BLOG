using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Helpers
{
    public static class IdentityHelper
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static int GetWorkerId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return Convert.ToInt32(principal.FindFirst(AppClaims.WORKER_ID)?.Value);
        }
    }
}
