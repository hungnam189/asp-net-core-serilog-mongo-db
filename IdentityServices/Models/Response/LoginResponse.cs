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
        }

        public string Id { get; private set; }
        public string Token { get; private set; }

        public string UserName { get; private set; }

        public string Email { get; private set; }

        public List<string> Roles { get; private set; }

        [JsonIgnore]
        public string RefreshToken { get; private set; }

    }
}
