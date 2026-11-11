using System;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;


namespace Es.Udc.DotNet.PracticaMAD.Web.HTTP.Util
{
    public class CookiesManager
    {
        private const string LOGIN_NAME_COOKIE = "loginName";
        private const string ENCRYPTED_PASSWORD_COOKIE = "encryptedPassword";

        private const int REMEMBER_MY_PASSWORD_AGE = 30 * 24 * 3600;
        private const string PASSWORD_PROTECTION_PURPOSE = "PracticaMAD.RememberPassword";

        public static void LeaveCookies(HttpContext context, String loginName,
                String encryptedPassword)
        {
            int timeToLive = REMEMBER_MY_PASSWORD_AGE;

            /* Create the loginName cookie. */
            HttpCookie loginNameCookie =
                new HttpCookie(LOGIN_NAME_COOKIE, loginName);

            /* Create the encryptedPassword cookie. */
            HttpCookie encryptedPasswordCookie =
                new HttpCookie(ENCRYPTED_PASSWORD_COOKIE, Convert.ToBase64String(
                    MachineKey.Protect(Encoding.UTF8.GetBytes(encryptedPassword),
                        PASSWORD_PROTECTION_PURPOSE, loginName)));


            /* Set maximum age to cookies. */
            loginNameCookie.Expires = DateTime.Now.AddSeconds(timeToLive);
            encryptedPasswordCookie.Expires = DateTime.Now.AddSeconds(timeToLive);

            ConfigureCookie(context, loginNameCookie);
            ConfigureCookie(context, encryptedPasswordCookie);

            /* Add cookies to response (These values will be used in TouchSession method to create
               the authCookie when the session starts. */
            context.Response.Cookies.Add(loginNameCookie);
            context.Response.Cookies.Add(encryptedPasswordCookie);           

        }

        public static void RemoveCookies(HttpContext context)
        {
            /* Create the loginName cookie. */
            HttpCookie loginNameCookie =
                new HttpCookie(LOGIN_NAME_COOKIE, "");

            /* Create the encryptedPassword cookie. */
            HttpCookie encryptedPasswordCookie =
                new HttpCookie(ENCRYPTED_PASSWORD_COOKIE, "");

            /* Set maximum age to cookies. */
            loginNameCookie.Expires = DateTime.UtcNow.AddDays(-1);
            encryptedPasswordCookie.Expires = DateTime.UtcNow.AddDays(-1);

            ConfigureCookie(context, loginNameCookie);
            ConfigureCookie(context, encryptedPasswordCookie);

            /* Add cookies to response. */
            context.Response.Cookies.Add(loginNameCookie);
            context.Response.Cookies.Add(encryptedPasswordCookie);
            
        }

        public static String GetLoginName(HttpContext context)
        {
            HttpCookie loginNameCookie =
                context.Request.Cookies.Get(LOGIN_NAME_COOKIE);

            if (loginNameCookie != null)
                return loginNameCookie.Value;
            else
                return null;

        }

        public static String GetEncryptedPassword(HttpContext context)
        {
            HttpCookie encryptedPasswordCookie =
                context.Request.Cookies.Get(ENCRYPTED_PASSWORD_COOKIE);

            string loginName = GetLoginName(context);

            if (encryptedPasswordCookie == null || String.IsNullOrEmpty(loginName))
            {
                return null;
            }

            try
            {
                byte[] password = MachineKey.Unprotect(
                    Convert.FromBase64String(encryptedPasswordCookie.Value),
                    PASSWORD_PROTECTION_PURPOSE, loginName);

                return password == null ? null : Encoding.UTF8.GetString(password);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        private static void ConfigureCookie(HttpContext context, HttpCookie cookie)
        {
            cookie.HttpOnly = true;
            cookie.Secure = context.Request.IsSecureConnection;
            cookie.SameSite = SameSiteMode.Lax;
            cookie.Path = String.IsNullOrEmpty(context.Request.ApplicationPath)
                ? "/" : context.Request.ApplicationPath;
        }

    }
}
