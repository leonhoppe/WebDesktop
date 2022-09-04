using System.Linq;
using System.Security.Claims;

namespace Backend.Security.Authorization {
    public static class ClaimsPrincipalExtensions {
        public static string GetAccessTokenId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(CustomClaimTypes.AccessTokenId);

        public static string GetRefreshTokenId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(CustomClaimTypes.RefreshTokenId);

        public static string GetUserId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(CustomClaimTypes.UserId);

        public static string[] GetPermissions(this ClaimsPrincipal principal) => principal.Claims
            .Where(claim => claim.Type.Equals(CustomClaimTypes.Permission))
            .Select(claim => claim.Value)
            .ToArray();
    }
}