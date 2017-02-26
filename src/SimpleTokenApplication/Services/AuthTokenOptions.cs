namespace SimpleTokenApplication.Services
{
    public class AuthTokenOptions
    {
        // Secret Key
        public string TokenKey { get; set; }

        // The Issuer (iss) claim for generated tokens.
        public string TokenIssuer { get; set; }

        // The Audience (aud) claim for the generated tokens.
        public string TokenAudience { get; set; }
    }
}
