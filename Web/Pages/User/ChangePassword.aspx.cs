using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Session;
using System;
using System.Web.Security;

namespace Es.Udc.DotNet.PracticaMAD.Web.Pages.User
{
    public partial class ChangePassword : SpecificCulturePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!EnsureAuthenticated())
            {
                return;
            }

            lblCurrentPasswordError.Visible = false;
            pnlChanged.Visible = !IsPostBack && Request.QueryString["changed"] == "1";
        }

        protected void BtnChangePasswordClick(object sender, EventArgs e)
        {
            if (!EnsureAuthenticated())
            {
                return;
            }

            Page.Validate("ChangePassword");

            if (!Page.IsValid)
            {
                return;
            }

            try
            {
                SessionManager.ChangePassword(Context, txtCurrentPassword.Text,
                    txtNewPassword.Text);

                // Reload the form without password values or a repeated submission on refresh.
                Response.Redirect(Response.ApplyAppPathModifier(
                    "~/Pages/User/ChangePassword.aspx?changed=1"), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (IncorrectPasswordException)
            {
                lblCurrentPasswordError.Visible = true;
                txtCurrentPassword.Focus();
            }
        }

        private bool EnsureAuthenticated()
        {
            if (SessionManager.IsUserAuthenticated(Context))
            {
                return true;
            }

            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }
    }
}
