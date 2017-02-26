namespace SimpleTokenApplication.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using SimpleTokenApplication.Models;

    public class TokenService : ITokenService
    {
        private readonly AuthTokenOptions options;

        public TokenService(IOptions<AuthTokenOptions> optionsAccessor)
        {
            this.options = optionsAccessor.Value;
        }

        public object CreateTicketAsync(ApplicationUser user)
        {
            var now = DateTime.UtcNow;

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.options.TokenKey));

            // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64)
            };

            // Create the JWT and write it to a string
            // The Audience (aud) claim for the generated tokens
            // The Issuer (iss) claim for generated tokens
            var jwt = new JwtSecurityToken(
                issuer: this.options.TokenIssuer,
                audience: this.options.TokenAudience,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)TimeSpan.FromMinutes(5).TotalSeconds
            };

            return response;
        }

        private static long ToUnixEpochDate(DateTime date) => new DateTimeOffset(date).ToUniversalTime().ToUnixTimeSeconds();
    }
}
