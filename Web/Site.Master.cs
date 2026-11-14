using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Session;
using System;
using System.Web.UI;

namespace Es.Udc.DotNet.PracticaMAD.Web
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            UserSession userSession = SessionManager.GetUserSession(Context);
            anonymousLinks.Visible = userSession == null;
            authenticatedLinks.Visible = userSession != null;

            if (userSession != null)
            {
                lnkChangePassword.Visible = !userSession.IsExternal;
                litFirstName.Text = userSession.FirstName;
            }
        }

        protected void BtnLogoutClick(object sender, EventArgs e)
        {
            SessionManager.Logout(Context);
            Response.Redirect(Response.ApplyAppPathModifier("~/Pages/MainPage.aspx"), false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
