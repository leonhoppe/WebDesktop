using Microsoft.AspNetCore.Mvc;

namespace Backend.Security.Authorization {
    public sealed class AuthorizedAttribute : TypeFilterAttribute {
        public AuthorizedAttribute(params string[] permission) : base(typeof(AuthorizedFilter)) {
            Arguments = new object[] { permission };
        }
    }
}