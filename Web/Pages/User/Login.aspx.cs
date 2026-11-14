using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Session;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Security;
using System;
using System.Configuration;
using System.Web;
using System.Web.Security;

namespace Es.Udc.DotNet.PracticaMAD.Web.Pages.User
{
    public partial class Login : SpecificCulturePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblLoginError.Visible = false;
            lblPasswordError.Visible = false;
            btnGoogleLogin.Enabled = GoogleOAuthConfiguration.IsConfigured();
            lblGoogleError.Visible = false;

            if (!btnGoogleLogin.Enabled) ShowGoogleError("unavailable");
            else if (!IsPostBack) ShowGoogleError(Request.QueryString["oauthError"]);

            if (!IsPostBack && SessionManager.IsUserAuthenticated(Context))
            {
                RedirectAfterLogin();
            }
        }

        protected void BtnLoginClick(object sender, EventArgs e)
        {
            Page.Validate("Login");

            if (!Page.IsValid)
            {
                return;
            }

            try
            {
                SessionManager.Login(Context, txtLogin.Text.Trim(),
                    txtPassword.Text, cbRememberPassword.Checked);

                RedirectAfterLogin();
            }
            catch (InstanceNotFoundException)
            {
                lblLoginError.Visible = true;
                txtLogin.Focus();
            }
            catch (IncorrectPasswordException)
            {
                lblPasswordError.Visible = true;
                txtPassword.Focus();
            }
        }

        protected void BtnGoogleLoginClick(object sender, EventArgs e)
        {
            if (SessionManager.IsUserAuthenticated(Context)) { RedirectAfterLogin(); return; }
            try
            {
                var configuration = GoogleOAuthConfiguration.Load();
                var request = GoogleLoginState.Begin(new HttpSessionStateWrapper(Session),
                    Request.QueryString["ReturnUrl"], Request.ApplicationPath);
                Response.Redirect(request.GetAuthorizationUrl(configuration), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (ConfigurationErrorsException) { ShowGoogleError("unavailable"); }
        }

        private void ShowGoogleError(string error)
        {
            if (error != "cancelled" && error != "expired" && error != "failed" && error != "unavailable") return;
            lblGoogleError.Text = (string)GetLocalResourceObject("GoogleError." + error);
            lblGoogleError.Visible = true;
        }

        private void RedirectAfterLogin()
        {
            // Forms authentication validates ReturnUrl and otherwise uses defaultUrl.
            string url = FormsAuthentication.GetRedirectUrl(
                txtLogin.Text.Trim(), cbRememberPassword.Checked);

            Response.Redirect(Response.ApplyAppPathModifier(url), false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
