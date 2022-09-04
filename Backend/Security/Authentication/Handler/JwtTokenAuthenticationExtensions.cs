using Backend.Options;
using Microsoft.AspNetCore.Authentication;

namespace Backend.Security.Authentication {
    public static class JwtTokenAuthenticationExtensions {
        public static AuthenticationBuilder AddJwtTokenAuthentication(this AuthenticationBuilder builder,
            IConfiguration configuration) {
            builder.Services.AddOptionsFromConfiguration<JwtTokenAuthenticationOptions>(configuration);

            return builder.AddScheme<JwtTokenAuthenticationHandlerOptions, JwtTokenAuthenticationHandler>(
                JwtTokenAuthentication.Scheme,
                _ => { });
        }
    }
}