using IdentityServices.Jwt;
using IdentityServices.Models;
using IdentityServices.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SerilogMongoDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SerilogMongoDb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtTokenCreator _jwtCreator;

        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            JwtTokenCreator jwtCreator,
        ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtCreator = jwtCreator;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpGet("AddRole/{roleName}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddRole([FromRoute] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError("rolename", "The role name is not null");
                return BadRequest(ModelState);
            }

            var result = await _roleManager.CreateAsync(new ApplicationRole
            {
                Name = roleName
            });
            if (result.Succeeded)
            {
                _logger.LogInformation("RoleName: {0}", roleName);
                return Ok("Role is create success.");
            }

            return Ok("Role is create fail.");
        }


        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                string errorMesssage = string.Join(",", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return BadRequest();
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                await _userManager.AddToRoleAsync(user, "User").ConfigureAwait(false);
                await _userManager.AddClaimAsync(user, new Claim("Office", user.PhoneNumber, ClaimValueTypes.Integer)).ConfigureAwait(false);
                
                //var token = _jwtCreator.Generate(user.Email, new Guid().ToString());
                var responeData = new LoginResponse("", user.UserName, user.Email, new List<string> { "User" }, "");

                _logger.LogInformation(3, "User created a new account with password.");

                return Ok(new
                {
                    message = "Success! User created a new account with password.",
                    data = responeData
                });
            }
            return Ok(new
            {
                message = $"Fail! {string.Join("; ", result.Errors?.Select(p => p.Description))}",
                data = new object()
            });
        }


    }
}
