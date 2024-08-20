namespace CleanArch.Api.Filter
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class TokenValidator
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenValidator(string secretKey, string issuer, string audience)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetTokenValidationParameters();

            try
            {
                // Validate token and return the principal
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                // Handle the case when the token is expired
                Console.WriteLine($"Token expired: {ex.Message}");
                return null;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                // Handle the case when the token signature is invalid
                Console.WriteLine($"Invalid token signature: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Handle other possible exceptions
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero, // Adjust if necessary

                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey))
            };
        }
    }
}