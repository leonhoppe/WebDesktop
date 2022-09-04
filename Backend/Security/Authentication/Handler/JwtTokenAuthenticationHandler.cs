using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;
using Backend.Entitys;
using Backend.Repositorys;
using Backend.Security.Authorization;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Backend.Security.Authentication {
    public class JwtTokenAuthenticationHandler : AuthenticationHandler<JwtTokenAuthenticationHandlerOptions> {
        private readonly TokenRepository _tokens;
        private readonly GroupRepository _groups;
        private readonly JwtTokenAuthenticationOptions _options;

        public JwtTokenAuthenticationHandler(
            IOptionsMonitor<JwtTokenAuthenticationHandlerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptions<JwtTokenAuthenticationOptions> tokenOptions,
            TokenRepository tokens,
            GroupRepository groups)
            : base(options, logger, encoder, clock) {
            _options = tokenOptions.Value;
            _tokens = tokens;
            _groups = groups;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
            if (Request.Headers["Authorization"].Equals(_options.DebugAccessToken))
                return AuthenticateResult.Success(GetAuthenticationTicket(null, null, "*"));
            
            var accessToken = GetAccessToken();
            if (accessToken == null) return AuthenticateResult.Fail("Access Token invalid");
            var refreshToken = _tokens.GetRefreshToken(accessToken.RefreshTokenId);
            if (refreshToken == null) return AuthenticateResult.Fail("Refresh Token invalid");
            if (!_tokens.ValidateRefreshToken(refreshToken.Id)) return AuthenticateResult.Fail("Refresh Token invalid");
            bool valid = _tokens.ValidateAccessToken(accessToken.Id);
            return valid
                ? AuthenticateResult.Success(GetAuthenticationTicket(accessToken, refreshToken))
                : AuthenticateResult.Fail("Access Token invalid");
        }

        private AuthenticationTicket GetAuthenticationTicket(AccessToken accessToken, RefreshToken refreshToken, params string[] customPerms) {
            List<Claim> claims = GenerateClaims(accessToken, refreshToken, customPerms);
            ClaimsPrincipal principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(claims, JwtTokenAuthentication.Scheme));
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
            return ticket;
        }

        private List<Claim> GenerateClaims(AccessToken accessToken, RefreshToken refreshToken, params string[] customPerms) {
            List<Claim> claims = new List<Claim>();
            if (accessToken is not null && refreshToken is not null) {
                claims.AddRange(new List<Claim> {
                    new(CustomClaimTypes.AccessTokenId, accessToken.Id.ToString()),
                    new(CustomClaimTypes.RefreshTokenId, refreshToken.Id.ToString()),
                    new(CustomClaimTypes.UserId, refreshToken.UserId.ToString()),
                });
                
                string[] permissions = _groups.GetUserPermissions(refreshToken.UserId).Select(perm => perm.PermissionKey).ToArray();
                claims.AddRange(permissions
                    .Select(permission => new Claim(CustomClaimTypes.Permission, permission)));
            }

            claims.AddRange(customPerms.Select(perm => new Claim(CustomClaimTypes.Permission, perm)));

            return claims;
        }

        private AccessToken GetAccessToken() {
            string key = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(key)) {
                key = Request.Query["token"];
            }

            if (string.IsNullOrEmpty(key))
                return null;

            AccessToken token = _tokens.GetAccessToken(Guid.Parse(key));
            return token;
        }
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously