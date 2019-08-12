using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Domain;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Models;
using PipelineSpace.Presentation.Models.AccountViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static PipelineSpace.Domain.DomainConstants;
using DomainModels = PipelineSpace.Domain.Models;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Route("{api}/users")]
    public class UserController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<DomainModels.User> _signInManager;

        public UserController(UserManager<User> userManager,
                              SignInManager<DomainModels.User> signInManager,
                              IConfiguration configuration,
                              IEmailSender emailSender,
                              IDomainManagerService domainManagerService): base(domainManagerService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterPostRp resource)
        {
            if (Boolean.Parse(_configuration["RegisterEnabled"]))
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                var user = DomainModels.User.Factory.Create(resource.FirstName, resource.LastName, resource.Email);
                
                var result = await _userManager.CreateAsync(user, resource.Password);
                if (result.Succeeded)
                {
                    //add global admin role
                    await _userManager.AddToRoleAsync(user, DomainConstants.Roles.GlobalAdmin);

                    //add account owner claim
                    await _userManager.AddClaimAsync(user, new Claim("owneridentifier", user.Id));
                    await _userManager.AddClaimAsync(user, new Claim("firstName", user.FirstName));
                    await _userManager.AddClaimAsync(user, new Claim("lastName", user.LastName));
                    await _userManager.AddClaimAsync(user, new Claim("fullname", $"{user.FirstName} {user.LastName}"));

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var keyValues = new List<KeyValuePair<string, string>>();
                    keyValues.Add(new KeyValuePair<string, string>("grant_type", "password"));
                    keyValues.Add(new KeyValuePair<string, string>("client_id", _configuration["AdminClient:ClientId"]));
                    keyValues.Add(new KeyValuePair<string, string>("client_secret", _configuration["AdminClient:ClientSecret"]));
                    keyValues.Add(new KeyValuePair<string, string>("username", resource.Email));
                    keyValues.Add(new KeyValuePair<string, string>("password", resource.Password));
                    keyValues.Add(new KeyValuePair<string, string>("scope", _configuration["AdminClient:Scope"]));

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.PostAsync($"{_configuration["AdminClient:Authority"]}/connect/token", new FormUrlEncodedContent(keyValues));

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonData = await response.Content.ReadAsStringAsync();
                            return this.Ok(Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData));
                        }

                        return this.Conflict(new { Reason = "InvalidLoginAttempt" });
                    }
                }

                return Conflict(result.Errors);
            }
            else
            {
                return ServiceUnavailable(new { Message = "Register is not available for the moment." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]UserLoginPostRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(resource.Email, resource.Password, resource.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var keyValues = new List<KeyValuePair<string, string>>();
                keyValues.Add(new KeyValuePair<string, string>("grant_type", "password"));
                keyValues.Add(new KeyValuePair<string, string>("client_id", _configuration["AdminClient:ClientId"]));
                keyValues.Add(new KeyValuePair<string, string>("client_secret", _configuration["AdminClient:ClientSecret"]));
                keyValues.Add(new KeyValuePair<string, string>("username", resource.Email));
                keyValues.Add(new KeyValuePair<string, string>("password", resource.Password));
                keyValues.Add(new KeyValuePair<string, string>("scope", _configuration["AdminClient:Scope"]));

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync($"{_configuration["AdminClient:Authority"]}/connect/token", new FormUrlEncodedContent(keyValues));

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        return this.Ok(Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData));
                    }

                    return this.Conflict(new { Reason = "InvalidLoginAttempt" });
                }
            }
            if (result.RequiresTwoFactor)
            {
                return this.Conflict( new { Reason = "RequiresTwoFactor" });
            }
            if (result.IsLockedOut)
            {
                return this.Conflict(new { Reason = "IsLockedOut" });
            }
            else
            {
                return this.Conflict(new { Reason = "InvalidLoginAttempt" });
            }
        }

        [HttpGet()]
        [Route("externalCredential")]
        public async Task<IActionResult> GetUserExternalCredential([FromQuery(Name = "provider")]string provider)
        {
            var user = await this._userManager.GetUserAsync(User);

            var accessToken = await this._userManager.GetAuthenticationTokenAsync(user, provider, "access_token");
            var accessEmail = await this._userManager.GetAuthenticationTokenAsync(user, provider, "access_account");

            var model = new
            {
                userName = accessEmail,
                email = accessEmail,
                accessToken = accessToken
            };

            return this.Ok(model);
        }

        [HttpPost()]
        [AllowAnonymous]
        [Route("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return BadRequest(new { status = false });
                }
                
                return Ok(new { status = true });
            }

            // If we got this far, something failed, redisplay form
            return BadRequest(ModelState);
        }

    }
}
