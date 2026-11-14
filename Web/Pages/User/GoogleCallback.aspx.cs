using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Security;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Session;
using Google.Apis.Auth;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Es.Udc.DotNet.PracticaMAD.Web.Pages.User
{
    public partial class GoogleCallback : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Headers["Referrer-Policy"] = "no-referrer";
            RegisterAsyncTask(new PageAsyncTask(CompleteLoginAsync));
        }

        private async Task CompleteLoginAsync()
        {
            var request = GoogleLoginState.Consume(new HttpSessionStateWrapper(Session), Request.QueryString["state"]);
            if (request == null) { RedirectToLogin("expired", null); return; }
            if (!string.IsNullOrEmpty(Request.QueryString["error"]))
            {
                RedirectToLogin(Request.QueryString["error"] == "access_denied" ? "cancelled" : "failed", request);
                return;
            }
            string code = Request.QueryString["code"];
            if (string.IsNullOrWhiteSpace(code)) { RedirectToLogin("failed", request); return; }
            try
            {
                var configuration = GoogleOAuthConfiguration.Load();
                var payload = await new GoogleOAuthClient().AuthenticateAsync(code, request, configuration);
                SessionManager.LoginWithGoogle(Context, payload.Subject, GoogleOAuthClient.GetProfileDetails(payload));
            }
            catch (ConfigurationErrorsException) { RedirectToLogin("unavailable", request); return; }
            catch (HttpRequestException) { RedirectToLogin("failed", request); return; }
            catch (TaskCanceledException) { RedirectToLogin("failed", request); return; }
            catch (InvalidJwtException) { RedirectToLogin("failed", request); return; }
            catch (JsonException) { RedirectToLogin("failed", request); return; }
            catch (DuplicateInstanceException) { RedirectToLogin("failed", request); return; }
            Response.Redirect(request.ReturnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void RedirectToLogin(string error, GoogleLoginState request)
        {
            string url = "~/Pages/User/Login.aspx?oauthError=" + error;
            if (request != null) url += "&ReturnUrl=" + HttpUtility.UrlEncode(request.ReturnUrl);
            Response.Redirect(ResolveUrl(url), false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
