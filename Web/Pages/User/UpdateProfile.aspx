<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UpdateProfile.aspx.cs"
    Inherits="Es.Udc.DotNet.PracticaMAD.Web.Pages.User.UpdateProfile"
    Culture="es-ES" UICulture="es" UnobtrusiveValidationMode="None"
    meta:resourcekey="PageResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container registration-shell py-4 py-md-5">
        <div class="card border-0 shadow-sm rounded-4">
            <div class="card-body p-4 p-md-5">
                <h1 class="h3 fw-bold text-center mb-4"><asp:Localize ID="lclTitle" runat="server" meta:resourcekey="lclTitle" /></h1>

                <asp:Panel ID="pnlUpdated" runat="server" Visible="false" CssClass="alert alert-success" role="status">
                    <asp:Localize ID="lclUpdated" runat="server" meta:resourcekey="lclUpdated" />
                </asp:Panel>

                <asp:Panel ID="pnlProfile" runat="server" CssClass="registration-form row g-3" DefaultButton="btnUpdate">
                    <div class="col-12 col-md-6">
                        <asp:Label ID="lclFirstname" runat="server" CssClass="form-label fw-semibold"
                            AssociatedControlID="txtFirstname" meta:resourcekey="lclFirstname" />
                        <asp:TextBox ID="txtFirstname" runat="server" CssClass="form-control" MaxLength="30" autocomplete="given-name" />
                        <asp:RequiredFieldValidator ID="rfvFirstname" runat="server"
                            ControlToValidate="txtFirstname" ValidationGroup="UpdateProfile"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvFirstname" />
                        <asp:RegularExpressionValidator ID="revFirstnameLength" runat="server"
                            ControlToValidate="txtFirstname" ValidationExpression="[\s\S]{1,30}"
                            ValidationGroup="UpdateProfile" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="revFirstnameLength" />
                    </div>

                    <div class="col-12 col-md-6">
                        <asp:Label ID="lclSurname" runat="server" CssClass="form-label fw-semibold"
                            AssociatedControlID="txtSurname" meta:resourcekey="lclSurname" />
                        <asp:TextBox ID="txtSurname" runat="server" CssClass="form-control" MaxLength="40" autocomplete="family-name" />
                        <asp:RequiredFieldValidator ID="rfvSurname" runat="server"
                            ControlToValidate="txtSurname" ValidationGroup="UpdateProfile"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvSurname" />
                        <asp:RegularExpressionValidator ID="revSurnameLength" runat="server"
                            ControlToValidate="txtSurname" ValidationExpression="[\s\S]{1,40}"
                            ValidationGroup="UpdateProfile" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="revSurnameLength" />
                    </div>

                    <div class="col-12">
                        <asp:Label ID="lclEmail" runat="server" CssClass="form-label fw-semibold"
                            AssociatedControlID="txtEmail" meta:resourcekey="lclEmail" />
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="60" autocomplete="email" />
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                            ControlToValidate="txtEmail" ValidationGroup="UpdateProfile"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvEmail" />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server"
                            ControlToValidate="txtEmail" ValidationExpression="[^\s@]+@[^\s@]+\.[^\s@]+"
                            ValidationGroup="UpdateProfile" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="revEmail" />
                        <asp:RegularExpressionValidator ID="revEmailLength" runat="server"
                            ControlToValidate="txtEmail" ValidationExpression="[\s\S]{1,60}"
                            ValidationGroup="UpdateProfile" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="revEmailLength" />
                    </div>

                    <div class="col-12 col-md-6">
                        <asp:Label ID="lclLanguage" runat="server" CssClass="form-label fw-semibold"
                            AssociatedControlID="ddlLanguage" meta:resourcekey="lclLanguage" />
                        <asp:DropDownList ID="ddlLanguage" runat="server" CssClass="form-select" />
                        <asp:RequiredFieldValidator ID="rfvLanguage" runat="server"
                            ControlToValidate="ddlLanguage" ValidationGroup="UpdateProfile"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvLanguage" />
                    </div>

                    <div class="col-12 col-md-6">
                        <asp:Label ID="lclCountry" runat="server" CssClass="form-label fw-semibold"
                            AssociatedControlID="ddlCountry" meta:resourcekey="lclCountry" />
                        <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-select" autocomplete="country" />
                        <asp:RequiredFieldValidator ID="rfvCountry" runat="server"
                            ControlToValidate="ddlCountry" ValidationGroup="UpdateProfile"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvCountry" />
                    </div>

                    <div class="col-12 mt-4 d-flex flex-wrap gap-2">
                        <asp:Button ID="btnUpdate" runat="server" CssClass="btn btn-primary"
                            ValidationGroup="UpdateProfile" OnClick="BtnUpdateClick" meta:resourcekey="btnUpdate" />
                        <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-outline-secondary"
                            NavigateUrl="~/Pages/MainPage.aspx" meta:resourcekey="lnkCancel" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
