using AspNetCore.Identity.MongoDbCore.Models;
using IdentityServices.Jwt;
using IdentityServices.Models;
using IdentityServices.Models.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SerilogMongoDb.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SerilogMongoDb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtTokenCreator _jwtCreator;
        private readonly JwtSettings _jwtSettings;

        private readonly ILogger _logger;
        public IdentityController(
            JwtTokenCreator jwtCreator,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            JwtSettings jwtSettings,
            ILoggerFactory loggerFactory)
        {
            _jwtCreator = jwtCreator;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings;
            _logger = loggerFactory.CreateLogger<IdentityController>();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(request.UserName).ConfigureAwait(false);

            if (user == null)
            {
                // Return bad request if the user doesn't exist
                return StatusCode((int)HttpStatusCode.Unauthorized, $"Bad Credentials! The user({request.UserName}) doesn't exist");
            }

            // Check that the user can sign in and is not locked out.
            // If two-factor authentication is supported, it would also be appropriate to check that 2FA is enabled for the user
            if (!await _signInManager.CanSignInAsync(user).ConfigureAwait(false) || (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user).ConfigureAwait(false)))
            {
                // Return bad request is the user can't sign in
                return StatusCode((int)HttpStatusCode.Unauthorized, $"Bad Credentials! The user({request.UserName}) can't sign in");
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe, lockoutOnFailure: false).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var token = _jwtCreator.Generate(user.Email, user.Id.ToString());
                var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

                // await _userManager.RemoveAuthenticationTokenAsync(user, "SerilogMongoDb", "RefreshToken");
                // var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, "SerilogMongoDb", "RefreshToken");
                // await _userManager.SetAuthenticationTokenAsync(user, "SerilogMongoDb", "RefreshToken", newRefreshToken);

                //user.To
                user.Tokens = new List<Token>
                {
                    new Token
                    {
                        LoginProvider = JwtBearerDefaults.AuthenticationScheme,
                        Name = "AccessToken",
                        Value = token
                    }
                };

                await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
                var dateSingin = DateTime.UtcNow;
                var dateExpries = dateSingin + _jwtSettings.Expire;

                var rootData = new
                {
                    token_type = JwtBearerDefaults.AuthenticationScheme,
                    access_token = token,
                    expires_in = (dateExpries - dateSingin).TotalSeconds
                };

                Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
                Response.Cookies.Append("X-Username", user.UserName, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
                ///Response.Cookies.Append("X-Refresh-Token", user.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
                _logger.LogInformation(1, $"User({user.UserName}) logged in.");
                return Ok(rootData);
            }
            return StatusCode((int)HttpStatusCode.Unauthorized, "Invalid username or password");
        }

        //
        // POST: /Account/LogOff
        [HttpPost("LogOff")]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            _logger.LogInformation(4, "User logged out.");
            return Ok("User logged out.");
        }
    }
}
