using Backend.Security.Authorization;

namespace Backend.Security {
    internal class TokenContext : ITokenContext {
        private readonly IHttpContextAccessor _accessor;

        public TokenContext(IHttpContextAccessor accessor) {
            _accessor = accessor;
        }

        public bool IsAuthenticated => _accessor.HttpContext?.User.Identity?.IsAuthenticated == true;

        public Guid UserId => CreateGuild(_accessor.HttpContext?.User.GetUserId());

        public Guid AccessTokenId => CreateGuild(_accessor.HttpContext?.User.GetAccessTokenId());

        public Guid RefreshTokenId => CreateGuild(_accessor.HttpContext?.User.GetRefreshTokenId());

        public string[] Permissions => _accessor.HttpContext?.User.GetPermissions();

        private static Guid CreateGuild(string id) {
            if (string.IsNullOrEmpty(id)) return Guid.Empty;
            return Guid.Parse(id);
        }
    }
}