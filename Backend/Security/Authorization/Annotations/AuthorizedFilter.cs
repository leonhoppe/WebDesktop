using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Security.Authorization {
    public class AuthorizedFilter : IAuthorizationFilter {
        private readonly string[] _permissions;

        public AuthorizedFilter(params string[] permissions) {
            _permissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context) {
            if (EndpointHasAllowAnonymousFilter(context)) {
                return;
            }

            if (!IsAuthenticated(context)) {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!ContainsRequiredRole(context)) {
                context.Result = new ForbidResult();
                return;
            }
        }

        private static bool EndpointHasAllowAnonymousFilter(AuthorizationFilterContext context) {
            return context.Filters.Any(item => item is IAllowAnonymousFilter);
        }

        private bool IsAuthenticated(AuthorizationFilterContext context) {
            return context.HttpContext.User.Identity.IsAuthenticated;
        }

        private bool ContainsRequiredRole(AuthorizationFilterContext context) {
            if (context.HttpContext.User.HasClaim(CustomClaimTypes.Permission, "*"))
                return true;

            var perms = context.HttpContext.User.Claims
                .Where(c => c.Type == CustomClaimTypes.Permission)
                .Select(c => c.Value).ToArray();

            if (context.RouteData.Values.ContainsKey("userId")) {
                var accessedUser = context.RouteData.Values["userId"] as string;

                if (accessedUser == context.HttpContext.User.GetUserId()) {
                    var selfPerms = _permissions.Where(p => p.StartsWith("self.")).ToArray();
                    
                    if (!selfPerms.Any())
                        return true;
                    
                    if (CheckPermission(selfPerms, perms))
                        return true;
                }
            }

            if (CheckPermission(_permissions, perms.Where(p => !p.StartsWith("self.")).ToArray()))
                return true;

            return false;

            bool CheckPermission(string[] permissions, string[] permission) {
                if (permissions.Length == 0)
                    return true;

                if (permission.Contains("*"))
                    return true;
                
                foreach (var perm in permissions) {
                    if (permission.Contains(perm))
                        return true;
                    
                    string[] splice = perm.Split(".");
                    string cache = "";
                    foreach (var s in splice) {
                        cache += s + ".";
                        if (permission.Contains(cache + "*"))
                            return true;
                    }
                }

                return false;
            }
        }
    }
}