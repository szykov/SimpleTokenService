namespace SimpleTokenApplication.Controllers.Api
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    using SimpleTokenApplication.Models;
    using SimpleTokenApplication.Services;

    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ITokenService tokenService;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AuthController(
            ITokenService tokenService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            this.tokenService = tokenService;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        // POST api/token
        [HttpPost, Produces("application/json")]
        public async Task<IActionResult> Exchange(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && this.HttpContext.Request.HasFormContentType)
            {
                var user = await this.userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return this.BadRequest(new { Error = "The username/password couple is invalid." });
                }

                // Ensure the user is allowed to sign in.
                if (!await this.signInManager.CanSignInAsync(user))
                {
                    return this.BadRequest(new { Error = "The specified user is not allowed to sign in." });
                }

                // Reject the token request if two-factor authentication has been enabled by the user.
                if (this.userManager.SupportsUserTwoFactor && await this.userManager.GetTwoFactorEnabledAsync(user))
                {
                    return this.BadRequest(new { Error = "The specified user is not allowed to sign in." });
                }

                // Ensure the user is not already locked out.
                if (this.userManager.SupportsUserLockout && await this.userManager.IsLockedOutAsync(user))
                {
                    return this.BadRequest(new { Error = "The username/password couple is invalid." });
                }

                // Ensure the password is valid.
                if (!await this.userManager.CheckPasswordAsync(user, password))
                {
                    if (this.userManager.SupportsUserLockout)
                    {
                        await this.userManager.AccessFailedAsync(user);
                    }

                    return this.BadRequest(new { Error = "The username/password couple is invalid." });
                }

                if (this.userManager.SupportsUserLockout)
                {
                    await this.userManager.ResetAccessFailedCountAsync(user);
                }

                // Object with token data
                var response = this.tokenService.CreateTicketAsync(user);

                var serializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };

                // Serialize and return the response
                this.Response.ContentType = "application/json";
                return this.Ok(JsonConvert.SerializeObject(response, serializerSettings));
            }

            return this.BadRequest(new { Error = "The specified grant type is not supported." });
        }
    }
}
