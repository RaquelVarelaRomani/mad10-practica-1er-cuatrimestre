using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Session;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.View.ApplicationObjects;
using System;
using System.Globalization;
using System.Web.Security;

namespace Es.Udc.DotNet.PracticaMAD.Web.Pages.User
{
    public partial class UpdateProfile : SpecificCulturePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!EnsureAuthenticated())
            {
                return;
            }

            lnkChangePassword.Visible = !SessionManager.GetUserSession(Context).IsExternal;
            pnlUpdated.Visible = !IsPostBack && Request.QueryString["updated"] == "1";

            if (!IsPostBack)
            {
                UserProfileDetails details = SessionManager.FindUserProfileDetails(Context);

                txtFirstname.Text = details.FirstName;
                txtSurname.Text = details.LastName;
                txtEmail.Text = details.Email;

                string language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                ddlLanguage.DataSource = Languages.GetLanguages(language);
                ddlLanguage.DataTextField = "Text";
                ddlLanguage.DataValueField = "Value";
                ddlLanguage.DataBind();
                ddlLanguage.SelectedValue = details.Language;

                ddlCountry.DataSource = Countries.GetCountries(language);
                ddlCountry.DataTextField = "Text";
                ddlCountry.DataValueField = "Value";
                ddlCountry.DataBind();
                ddlCountry.SelectedValue = details.Country;
            }
        }

        protected void BtnUpdateClick(object sender, EventArgs e)
        {
            if (!EnsureAuthenticated())
            {
                return;
            }

            Page.Validate("UpdateProfile");

            if (!Page.IsValid)
            {
                return;
            }

            UserProfileDetails details = new UserProfileDetails(
                txtFirstname.Text.Trim(),
                txtSurname.Text.Trim(),
                txtEmail.Text.Trim(),
                ddlLanguage.SelectedValue,
                ddlCountry.SelectedValue);

            SessionManager.UpdateUserProfileDetails(Context, details);

            // Reload the saved profile and apply the updated culture without resubmitting.
            Response.Redirect(Response.ApplyAppPathModifier(
                "~/Pages/User/UpdateProfile.aspx?updated=1"), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private bool EnsureAuthenticated()
        {
            if (SessionManager.IsUserAuthenticated(Context))
            {
                return true;
            }

            // A forms ticket can outlive the session. A redirect alone does not stop postback events.
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }
    }
}
