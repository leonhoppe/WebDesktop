namespace Backend.Security {
    public interface ITokenContext {
        bool IsAuthenticated { get; }
        Guid UserId { get; }
        Guid AccessTokenId { get; }
        Guid RefreshTokenId { get; }
        string[] Permissions { get; }
    }
}