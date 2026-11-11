<%@ Page Language="C#" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Redirect(Response.ApplyAppPathModifier("~/Pages/MainPage.aspx"), false);
        Context.ApplicationInstance.CompleteRequest();
    }
</script>
