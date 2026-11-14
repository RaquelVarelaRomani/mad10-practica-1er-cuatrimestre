<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="Es.Udc.DotNet.PracticaMAD.Web.Pages.User.Login"
    Culture="es-ES" UICulture="es" UnobtrusiveValidationMode="None"
    meta:resourcekey="PageResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container login-shell py-4 py-md-5">
        <div class="card border-0 shadow-sm rounded-4">
            <div class="card-body p-4 p-md-5">
                <h1 class="h3 fw-bold text-center mb-4"><asp:Localize ID="lclTitle" runat="server" meta:resourcekey="lclTitle" /></h1>

                <asp:Panel ID="pnlLogin" runat="server" CssClass="login-form row g-3" DefaultButton="btnLogin">
                    <div class="col-12">
                        <asp:Label ID="lclLogin" runat="server" CssClass="form-label fw-semibold"
                            AssociatedControlID="txtLogin" meta:resourcekey="lclLogin" />
                        <asp:TextBox ID="txtLogin" runat="server" CssClass="form-control" MaxLength="30" autocomplete="username" />
                        <asp:RequiredFieldValidator ID="rfvLogin" runat="server"
                            ControlToValidate="txtLogin" ValidationGroup="Login"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvLogin" />
                        <asp:RegularExpressionValidator ID="revLoginLength" runat="server"
                            ControlToValidate="txtLogin" ValidationExpression="[\s\S]{1,30}" ValidationGroup="Login"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="revLoginLength" />
                        <asp:Label ID="lblLoginError" runat="server" Visible="false"
                            CssClass="text-danger small" role="alert" meta:resourcekey="lblLoginError" />
                    </div>

                    <div class="col-12">
                        <asp:Label ID="lclPassword" runat="server" CssClass="form-label fw-semibold"
                            AssociatedControlID="txtPassword" meta:resourcekey="lclPassword" />
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control"
                            TextMode="Password" autocomplete="current-password" />
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                            ControlToValidate="txtPassword" ValidationGroup="Login"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvPassword" />
                        <asp:Label ID="lblPasswordError" runat="server" Visible="false"
                            CssClass="text-danger small" role="alert" meta:resourcekey="lblPasswordError" />
                    </div>

                    <div class="col-12">
                        <asp:CheckBox ID="cbRememberPassword" runat="server" CssClass="remember-password"
                            meta:resourcekey="cbRememberPassword" />
                    </div>

                    <div class="col-12 mt-4">
                        <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary btn-lg w-100"
                            ValidationGroup="Login" OnClick="BtnLoginClick" meta:resourcekey="btnLogin" />
                    </div>
                </asp:Panel>

                <div class="mt-3">
                    <asp:Button ID="btnGoogleLogin" runat="server" CssClass="btn btn-outline-dark btn-lg w-100"
                        CausesValidation="false" OnClick="BtnGoogleLoginClick" meta:resourcekey="btnGoogleLogin" />
                    <asp:Label ID="lblGoogleError" runat="server" Visible="false"
                        CssClass="text-danger small d-block mt-2" role="alert" />
                </div>

                <p class="text-center mt-4 mb-0">
                    <asp:HyperLink ID="lnkRegister" runat="server" NavigateUrl="~/Pages/User/Register.aspx"
                        meta:resourcekey="lnkRegister" />
                </p>
            </div>
        </div>
    </div>
</asp:Content>
