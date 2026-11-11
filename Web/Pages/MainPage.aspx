<%@ Page Language="C#" MasterPageFile="~/Site.Master"
    Inherits="Es.Udc.DotNet.PracticaMAD.Web.HTTP.Session.SpecificCulturePage"
    Culture="es-ES" UICulture="es" meta:resourcekey="PageResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4 py-md-5">
        <div class="card border-0 shadow-sm rounded-4">
            <div class="card-body p-4 p-md-5">
                <h1 class="h3 fw-bold"><asp:Localize ID="lclTitle" runat="server" meta:resourcekey="lclTitle" /></h1>
                <asp:LoginView ID="loginView" runat="server">
                    <AnonymousTemplate>
                        <p class="mb-0"><asp:Localize ID="lclAnonymous" runat="server" meta:resourcekey="lclAnonymous" /></p>
                    </AnonymousTemplate>
                    <LoggedInTemplate>
                        <p class="mb-0" role="status"><asp:Localize ID="lclAuthenticated" runat="server" meta:resourcekey="lclAuthenticated" /></p>
                    </LoggedInTemplate>
                </asp:LoginView>
                <asp:HyperLink ID="lnkFindProducts" runat="server" NavigateUrl="~/Pages/Product/FindProducts.aspx"
                    CssClass="btn btn-primary mt-3" meta:resourcekey="lnkFindProducts" />
            </div>
        </div>
    </div>
</asp:Content>
