using System;

namespace Backend.Entitys; 

public class Tokens {
    public AccessToken AccessToken { get; set; }
    public RefreshToken RefreshToken { get; set; }
}

public class RefreshToken {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpirationDate { get; set; }
}

public class AccessToken {
    public Guid Id { get; set; }
    public Guid RefreshTokenId { get; set; }
    public DateTime ExpirationDate { get; set; }
}