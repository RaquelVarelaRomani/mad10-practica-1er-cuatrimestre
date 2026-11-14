using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Es.Udc.DotNet.PracticaMAD.Web.HTTP.Security
{
    [Serializable]
    public sealed class GoogleLoginState
    {
        private const string SessionKey = "googleOAuthRequest";
        public string State { get; private set; }
        public string Nonce { get; private set; }
        public string CodeVerifier { get; private set; }
        public string ReturnUrl { get; private set; }
        public DateTime CreatedUtc { get; private set; }

        private GoogleLoginState() { }

        public static GoogleLoginState Begin(HttpSessionStateBase session, string returnUrl, string applicationPath)
        {
            var request = new GoogleLoginState
            {
                State = RandomToken(), Nonce = RandomToken(), CodeVerifier = RandomToken(),
                ReturnUrl = GetLocalReturnUrl(returnUrl, applicationPath), CreatedUtc = DateTime.UtcNow
            };
            session[SessionKey] = request;
            return request;
        }

        public static GoogleLoginState Consume(HttpSessionStateBase session, string returnedState)
        {
            var request = session[SessionKey] as GoogleLoginState;
            session.Remove(SessionKey);
            return request != null && request.IsValid(returnedState, DateTime.UtcNow) ? request : null;
        }

        public bool IsValid(string returnedState, DateTime utcNow)
        {
            return !string.IsNullOrEmpty(returnedState) &&
                string.Equals(State, returnedState, StringComparison.Ordinal) &&
                utcNow >= CreatedUtc && utcNow - CreatedUtc <= TimeSpan.FromMinutes(10);
        }

        public string GetAuthorizationUrl(GoogleOAuthConfiguration configuration)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["response_type"] = "code";
            query["client_id"] = configuration.ClientId;
            query["redirect_uri"] = configuration.RedirectUri;
            query["scope"] = "openid email profile";
            query["state"] = State;
            query["nonce"] = Nonce;
            query["code_challenge_method"] = "S256";
            using (var sha = SHA256.Create())
                query["code_challenge"] = Base64Url(sha.ComputeHash(Encoding.ASCII.GetBytes(CodeVerifier)));
            return "https://accounts.google.com/o/oauth2/v2/auth?" + query;
        }

        public static string GetLocalReturnUrl(string returnUrl, string applicationPath)
        {
            string root = applicationPath == "/" ? "" : applicationPath.TrimEnd('/');
            string fallback = root + "/Pages/MainPage.aspx";
            if (string.IsNullOrWhiteSpace(returnUrl)) return fallback;
            if (returnUrl.StartsWith("~/", StringComparison.Ordinal)) returnUrl = root + returnUrl.Substring(1);
            string decoded;
            try { decoded = Uri.UnescapeDataString(returnUrl); }
            catch (UriFormatException) { return fallback; }
            if (!decoded.StartsWith("/", StringComparison.Ordinal) ||
                decoded.StartsWith("//", StringComparison.Ordinal) || decoded.IndexOf('\\') >= 0)
                return fallback;
            foreach (char character in decoded)
                if (char.IsControl(character)) return fallback;
            Uri target, decodedTarget;
            var origin = new Uri("https://local.invalid");
            if (!Uri.TryCreate(origin, returnUrl, out target) || target.Host != origin.Host ||
                !Uri.TryCreate(origin, decoded, out decodedTarget) || decodedTarget.Host != origin.Host ||
                target.AbsolutePath.StartsWith("//", StringComparison.Ordinal) ||
                decodedTarget.AbsolutePath.StartsWith("//", StringComparison.Ordinal) ||
                !target.AbsolutePath.StartsWith(root + "/", StringComparison.OrdinalIgnoreCase) ||
                !decodedTarget.AbsolutePath.StartsWith(root + "/", StringComparison.OrdinalIgnoreCase))
                return fallback;
            return target.PathAndQuery + target.Fragment;
        }

        private static string RandomToken()
        {
            var bytes = new byte[32];
            using (var random = RandomNumberGenerator.Create()) random.GetBytes(bytes);
            return Base64Url(bytes);
        }

        private static string Base64Url(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
    }
}
