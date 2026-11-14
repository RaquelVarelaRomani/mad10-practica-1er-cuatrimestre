using System;
using System.Configuration;

namespace Es.Udc.DotNet.PracticaMAD.Web.HTTP.Security
{
    public sealed class GoogleOAuthConfiguration
    {
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
        public string RedirectUri { get; private set; }

        public GoogleOAuthConfiguration(string clientId, string clientSecret, string redirectUri)
        {
            Uri uri;
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) ||
                !Uri.TryCreate(redirectUri, UriKind.Absolute, out uri) ||
                !(uri.Scheme == Uri.UriSchemeHttps || (uri.IsLoopback && uri.Scheme == Uri.UriSchemeHttp)) ||
                !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query) ||
                !string.IsNullOrEmpty(uri.Fragment))
            {
                throw new ConfigurationErrorsException("La configuración de OAuth de Google está incompleta o no es válida.");
            }
            ClientId = clientId.Trim();
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
        }

        public static GoogleOAuthConfiguration Load()
        {
            return new GoogleOAuthConfiguration(
                ReadSetting("GOOGLE_CLIENT_ID", "OAuth.Google.ClientId"),
                ReadSetting("GOOGLE_CLIENT_SECRET", "OAuth.Google.ClientSecret"),
                ReadSetting("GOOGLE_REDIRECT_URI", "OAuth.Google.RedirectUri"));
        }

        public static bool IsConfigured()
        {
            try { Load(); return true; }
            catch (ConfigurationErrorsException) { return false; }
        }

        private static string ReadSetting(string variable, string key)
        {
            return Environment.GetEnvironmentVariable(variable) ?? ConfigurationManager.AppSettings[key];
        }
    }
}
