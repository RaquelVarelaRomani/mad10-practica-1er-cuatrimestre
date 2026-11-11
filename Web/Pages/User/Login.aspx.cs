using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Session;
using System;
using System.Web.Security;

namespace Es.Udc.DotNet.PracticaMAD.Web.Pages.User
{
    public partial class Login : SpecificCulturePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblLoginError.Visible = false;
            lblPasswordError.Visible = false;

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
