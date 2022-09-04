using System.Text;
using Backend.Entitys;
using Backend.Security.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;

namespace Backend.Repositorys;

public class TokenRepository {
    private readonly JwtTokenAuthenticationOptions _options;
    private readonly DatabaseContext _context;

    public TokenRepository(IOptions<JwtTokenAuthenticationOptions> options, DatabaseContext context) {
        _options = options.Value;
        _context = context;
    }

    public RefreshToken GetRefreshToken(Guid refreshTokenId) {
        if (string.IsNullOrEmpty(refreshTokenId.ToString())) return null;
        return _context.RefreshTokens.SingleOrDefault(token => token.Id == refreshTokenId);
    }

    public AccessToken GetAccessToken(Guid accessTokenId) {
        if (string.IsNullOrEmpty(accessTokenId.ToString())) return null;
        return _context.AccessTokens.SingleOrDefault(token => token.Id == accessTokenId);
    }

    public IEnumerable<AccessToken> GetAccessTokens(Guid refreshTokenId) {
        if (string.IsNullOrEmpty(refreshTokenId.ToString())) return ArraySegment<AccessToken>.Empty;
        return _context.AccessTokens.Where(token => token.RefreshTokenId == refreshTokenId);
    }

    public bool ValidateAccessToken(Guid accessTokenId) {
        AccessToken token = GetAccessToken(accessTokenId);
        if (token == null) return false;
        TimeSpan span = token.ExpirationDate - DateTime.Now;
        return span.TotalMilliseconds > 0;
    }

    public bool ValidateRefreshToken(Guid refreshTokenId) {
        RefreshToken token = GetRefreshToken(refreshTokenId);
        if (token == null) return false;
        TimeSpan span = token.ExpirationDate - DateTime.Now;
        return span.TotalMilliseconds > 0;
    }

    public RefreshToken CreateRefreshToken(Guid userId) {
        RefreshToken token = new RefreshToken {
            UserId = userId, Id = Guid.NewGuid(),
            ExpirationDate = DateTime.Now.Add(new TimeSpan(int.Parse(_options.RefreshTokenExpirationTimeInHours), 0, 0))
        };
        _context.RefreshTokens.Add(token);
        _context.SaveChanges();
        return token;
    }

    public AccessToken CreateAccessToken(Guid refreshTokenId) {
        AccessToken token = new AccessToken {
            RefreshTokenId = refreshTokenId, Id = Guid.NewGuid(),
            ExpirationDate = DateTime.Now
                .Add(new TimeSpan(0, int.Parse(_options.AccessTokenExpirationTimeInMinutes), 0))
        };
        _context.AccessTokens.Add(token);
        _context.SaveChanges();
        return token;
    }

    public void DeleteUserTokens(Guid userId) {
        List<RefreshToken> refreshTokens = _context.RefreshTokens.Where(token => token.UserId == userId).ToList();
        refreshTokens.ForEach(token => DeleteRefreshToken(token.Id));
        _context.SaveChanges();
    }

    public void DeleteRefreshToken(Guid refreshTokenId) {
        _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(token => token.Id == refreshTokenId));
        _context.AccessTokens.RemoveRange(_context.AccessTokens.Where(token => token.RefreshTokenId == refreshTokenId));
    }
    
    public static string Hash128(string plainText, string salt) {
        try {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: plainText,
                salt: Encoding.Default.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            ));

            return hashed;
        } catch (Exception) { return ""; }
    }
}