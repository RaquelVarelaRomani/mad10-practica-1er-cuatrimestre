using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Session;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.View.ApplicationObjects;
using System;

namespace Es.Udc.DotNet.PracticaMAD.Web.Pages.User
{
    public partial class Register : SpecificCulturePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblLoginError.Visible = false;

            if (!IsPostBack)
            {
                if (Request.QueryString["registered"] == "1" &&
                    SessionManager.IsUserAuthenticated(Context))
                {
                    pnlRegistration.Visible = false;
                    pnlRegistered.Visible = true;
                    return;
                }

                ddlLanguage.DataSource = Languages.GetLanguages("es");
                ddlLanguage.DataTextField = "Text";
                ddlLanguage.DataValueField = "Value";
                ddlLanguage.DataBind();
                ddlLanguage.SelectedValue = "es";

                ddlCountry.DataSource = Countries.GetCountries("es");
                ddlCountry.DataTextField = "Text";
                ddlCountry.DataValueField = "Value";
                ddlCountry.DataBind();
                ddlCountry.SelectedValue = "ES";
            }
        }

        protected void BtnRegisterClick(object sender, EventArgs e)
        {
            Page.Validate("Register");

            if (!Page.IsValid)
            {
                return;
            }

            try
            {
                UserProfileDetails details = new UserProfileDetails(
                    txtFirstname.Text.Trim(),
                    txtSurname.Text.Trim(),
                    txtEmail.Text.Trim(),
                    ddlLanguage.SelectedValue,
                    ddlCountry.SelectedValue);

                SessionManager.RegisterUser(Context, txtLogin.Text.Trim(),
                    txtPassword.Text, details);

                // Redirect to the confirmation to avoid registering again on refresh.
                Response.Redirect(Response.ApplyAppPathModifier(
                    "~/Pages/User/Register.aspx?registered=1"), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (DuplicateInstanceException)
            {
                lblLoginError.Visible = true;
                txtLogin.Focus();
            }
        }
    }
}
