using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Es.Udc.DotNet.PracticaMAD.Web.HTTP.Security
{
    public sealed class GoogleOAuthClient
    {
        private static readonly HttpClient Client = new HttpClient(
            new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false })
            { Timeout = TimeSpan.FromSeconds(15) };

        public async Task<GoogleJsonWebSignature.Payload> AuthenticateAsync(string code,
            GoogleLoginState request, GoogleOAuthConfiguration configuration)
        {
            var values = new Dictionary<string, string>
            {
                { "code", code }, { "client_id", configuration.ClientId },
                { "client_secret", configuration.ClientSecret }, { "redirect_uri", configuration.RedirectUri },
                { "grant_type", "authorization_code" }, { "code_verifier", request.CodeVerifier }
            };
            using (var content = new FormUrlEncodedContent(values))
            using (var response = await Client.PostAsync("https://oauth2.googleapis.com/token", content))
            {
                if (!response.IsSuccessStatusCode)
                    throw new InvalidJwtException("Google no ha aceptado el código de autorización.");
                var token = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
                if (token == null || string.IsNullOrWhiteSpace(token.IdToken))
                    throw new InvalidJwtException("No se ha recibido un ID token.");
                var payload = await GoogleJsonWebSignature.ValidateAsync(token.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { configuration.ClientId } });
                ValidateIdentity(payload, request.Nonce);
                return payload;
            }
        }

        public static void ValidateIdentity(GoogleJsonWebSignature.Payload payload, string expectedNonce)
        {
            if (payload == null || string.IsNullOrEmpty(expectedNonce) ||
                !string.Equals(payload.Nonce, expectedNonce, StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(payload.Subject) || payload.Subject.Length > 255 ||
                !payload.EmailVerified || string.IsNullOrWhiteSpace(payload.Email) || payload.Email.Length > 60)
                throw new InvalidJwtException("La identidad de Google no es válida para esta solicitud.");
        }

        public static UserProfileDetails GetProfileDetails(GoogleJsonWebSignature.Payload payload)
        {
            string language = (payload.Locale ?? "es").Split('-', '_')[0].ToLowerInvariant();
            if (language != "es" && language != "gl" && language != "en") language = "es";
            string firstName = payload.GivenName;
            if (string.IsNullOrWhiteSpace(firstName)) firstName = payload.Email.Split('@')[0];
            return new UserProfileDetails(Truncate(firstName, 30), Truncate(payload.FamilyName ?? "", 40),
                payload.Email, language, "ES");
        }

        private static string Truncate(string value, int maximum)
        {
            return value.Length <= maximum ? value : value.Substring(0, maximum);
        }
    }
}
