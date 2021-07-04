using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IdentityServices.Models.Response
{
    public class LoginResponse
    {
        public LoginResponse(string token, string userName, string email, List<string> roles, string refreshToken)
        {
            Token = token;
            UserName = userName;
            Email = email;
            Roles = roles;
            RefreshToken = refreshToken;
        }

        public string Id { get; }
        public string Token { get; }

        public string UserName { get; }

        public string Email { get; }

        public List<string> Roles { get; }

        [JsonIgnore]
        public string RefreshToken { get; }
    }
}
