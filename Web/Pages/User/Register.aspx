<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs"
    Inherits="Es.Udc.DotNet.PracticaMAD.Web.Pages.User.Register"
    Culture="es-ES" UICulture="es" UnobtrusiveValidationMode="None"
    meta:resourcekey="PageResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container registration-shell py-4 py-md-5">
        <div class="card border-0 shadow-sm rounded-4">
            <div class="card-body p-4 p-md-5">
                <h1 class="h3 fw-bold text-center mb-4"><asp:Localize ID="lclTitle" runat="server" meta:resourcekey="lclTitle" /></h1>

                <asp:Panel ID="pnlRegistration" runat="server" CssClass="registration-form row g-3"
                    DefaultButton="btnRegister">
                    <div class="col-12">
                        <asp:Label ID="lclUserName" runat="server" CssClass="form-label fw-semibold" AssociatedControlID="txtLogin"
                            meta:resourcekey="lclUserName" />
                        <asp:TextBox ID="txtLogin" runat="server" CssClass="form-control" MaxLength="30" autocomplete="username" />
                        <asp:RequiredFieldValidator ID="rfvUserName" runat="server"
                            ControlToValidate="txtLogin" ValidationGroup="Register"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvUserName" />
                        <asp:RegularExpressionValidator ID="revLoginLength" runat="server"
                            ControlToValidate="txtLogin" ValidationExpression="[\s\S]{1,30}"
                            ValidationGroup="Register" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="revLoginLength" />
                        <asp:Label ID="lblLoginError" runat="server" Visible="false"
                            CssClass="text-danger small" role="alert" meta:resourcekey="lblLoginError" />
                    </div>

                    <div class="col-12 col-md-6">
                        <asp:Label ID="lclPassword" runat="server" CssClass="form-label fw-semibold" AssociatedControlID="txtPassword"
                            meta:resourcekey="lclPassword" />
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" autocomplete="new-password" />
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                            ControlToValidate="txtPassword" ValidationGroup="Register"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvPassword" />
                    </div>

                    <div class="col-12 col-md-6">
                        <asp:Label ID="lclRetypePassword" runat="server" CssClass="form-label fw-semibold" AssociatedControlID="txtRetypePassword"
                            meta:resourcekey="lclRetypePassword" />
                        <asp:TextBox ID="txtRetypePassword" runat="server" CssClass="form-control" TextMode="Password" autocomplete="new-password" />
                        <asp:RequiredFieldValidator ID="rfvRetypePassword" runat="server"
                            ControlToValidate="txtRetypePassword" ValidationGroup="Register"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvRetypePassword" />
                        <asp:CompareValidator ID="cvPasswordCheck" runat="server"
                            ControlToCompare="txtPassword" ControlToValidate="txtRetypePassword"
                            ValidationGroup="Register" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="cvPasswordCheck" />
                    </div>

                    <div class="col-12 col-md-6">
                        <asp:Label ID="lclFirstname" runat="server" CssClass="form-label fw-semibold" AssociatedControlID="txtFirstname"
                            meta:resourcekey="lclFirstname" />
                        <asp:TextBox ID="txtFirstname" runat="server" CssClass="form-control" MaxLength="30" autocomplete="given-name" />
                        <asp:RequiredFieldValidator ID="rfvFirstname" runat="server"
                            ControlToValidate="txtFirstname" ValidationGroup="Register"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvFirstname" />
                        <asp:RegularExpressionValidator ID="revFirstnameLength" runat="server"
                            ControlToValidate="txtFirstname" ValidationExpression="[\s\S]{1,30}"
                            ValidationGroup="Register" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="revFirstnameLength" />
                    </div>

                    <div class="col-12 col-md-6">
                        <asp:Label ID="lclSurname" runat="server" CssClass="form-label fw-semibold" AssociatedControlID="txtSurname"
                            meta:resourcekey="lclSurname" />
                        <asp:TextBox ID="txtSurname" runat="server" CssClass="form-control" MaxLength="40" autocomplete="family-name" />
                        <asp:RequiredFieldValidator ID="rfvSurname" runat="server"
                            ControlToValidate="txtSurname" ValidationGroup="Register"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvSurname" />
                        <asp:RegularExpressionValidator ID="revSurnameLength" runat="server"
                            ControlToValidate="txtSurname" ValidationExpression="[\s\S]{1,40}"
                            ValidationGroup="Register" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="revSurnameLength" />
                    </div>

                    <div class="col-12">
                        <asp:Label ID="lclEmail" runat="server" CssClass="form-label fw-semibold" AssociatedControlID="txtEmail"
                            meta:resourcekey="lclEmail" />
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="60" autocomplete="email" />
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                            ControlToValidate="txtEmail" ValidationGroup="Register"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvEmail" />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server"
                            ControlToValidate="txtEmail" ValidationExpression="[^\s@]+@[^\s@]+\.[^\s@]+"
                            ValidationGroup="Register" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="revEmail" />
                        <asp:RegularExpressionValidator ID="revEmailLength" runat="server"
                            ControlToValidate="txtEmail" ValidationExpression="[\s\S]{1,60}"
                            ValidationGroup="Register" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="revEmailLength" />
                    </div>

                    <div class="col-12 col-md-6">
                        <asp:Label ID="lclLanguage" runat="server" CssClass="form-label fw-semibold" AssociatedControlID="ddlLanguage"
                            meta:resourcekey="lclLanguage" />
                        <asp:DropDownList ID="ddlLanguage" runat="server" CssClass="form-select" />
                        <asp:RequiredFieldValidator ID="rfvLanguage" runat="server"
                            ControlToValidate="ddlLanguage" ValidationGroup="Register"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvLanguage" />
                    </div>

                    <div class="col-12 col-md-6">
                        <asp:Label ID="lclCountry" runat="server" CssClass="form-label fw-semibold" AssociatedControlID="ddlCountry"
                            meta:resourcekey="lclCountry" />
                        <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-select" autocomplete="country" />
                        <asp:RequiredFieldValidator ID="rfvCountry" runat="server"
                            ControlToValidate="ddlCountry" ValidationGroup="Register"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvCountry" />
                    </div>

                    <div class="col-12 mt-4">
                        <asp:Button ID="btnRegister" runat="server" CssClass="btn btn-primary btn-lg w-100" ValidationGroup="Register"
                            meta:resourcekey="btnRegister" OnClick="BtnRegisterClick" />
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlRegistered" runat="server" Visible="false" CssClass="alert alert-success mb-0" role="status">
                    <p class="mb-0"><asp:Localize ID="lclRegistered" runat="server" meta:resourcekey="lclRegistered" /></p>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
