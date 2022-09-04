using Backend.Options;

namespace Backend.Security.Authentication {
    public class JwtTokenAuthenticationOptions : OptionsFromConfiguration {
        public override string Position => "Authentication";

        public string RefreshTokenExpirationTimeInHours { get; set; }
        public string AccessTokenExpirationTimeInMinutes { get; set; }
        public string DebugAccessToken { get; set; }
    }
}