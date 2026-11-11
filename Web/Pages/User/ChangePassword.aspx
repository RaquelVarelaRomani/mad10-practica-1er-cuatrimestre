<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs"
    Inherits="Es.Udc.DotNet.PracticaMAD.Web.Pages.User.ChangePassword"
    Culture="es-ES" UICulture="es" UnobtrusiveValidationMode="None"
    meta:resourcekey="PageResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container login-shell py-4 py-md-5">
        <div class="card border-0 shadow-sm rounded-4">
            <div class="card-body p-4 p-md-5">
                <h1 class="h3 fw-bold text-center mb-4"><asp:Localize ID="lclTitle" runat="server" meta:resourcekey="lclTitle" /></h1>

                <asp:Panel ID="pnlChanged" runat="server" Visible="false" CssClass="alert alert-success" role="status">
                    <asp:Localize ID="lclChanged" runat="server" meta:resourcekey="lclChanged" />
                </asp:Panel>

                <asp:Panel ID="pnlPassword" runat="server" CssClass="login-form row g-3" DefaultButton="btnChangePassword">
                    <div class="col-12">
                        <asp:Label ID="lclCurrentPassword" runat="server" CssClass="form-label fw-semibold"
                            AssociatedControlID="txtCurrentPassword" meta:resourcekey="lclCurrentPassword" />
                        <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control"
                            TextMode="Password" autocomplete="current-password" />
                        <asp:RequiredFieldValidator ID="rfvCurrentPassword" runat="server"
                            ControlToValidate="txtCurrentPassword" ValidationGroup="ChangePassword"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvCurrentPassword" />
                        <asp:Label ID="lblCurrentPasswordError" runat="server" Visible="false"
                            CssClass="text-danger small" role="alert" meta:resourcekey="lblCurrentPasswordError" />
                    </div>

                    <div class="col-12">
                        <asp:Label ID="lclNewPassword" runat="server" CssClass="form-label fw-semibold"
                            AssociatedControlID="txtNewPassword" meta:resourcekey="lclNewPassword" />
                        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control"
                            TextMode="Password" autocomplete="new-password" />
                        <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server"
                            ControlToValidate="txtNewPassword" ValidationGroup="ChangePassword"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvNewPassword" />
                        <asp:CompareValidator ID="cvNewPassword" runat="server"
                            ControlToValidate="txtNewPassword" ControlToCompare="txtCurrentPassword" Operator="NotEqual" Type="String"
                            ValidationGroup="ChangePassword" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="cvNewPassword" />
                    </div>

                    <div class="col-12">
                        <asp:Label ID="lclRetypeNewPassword" runat="server" CssClass="form-label fw-semibold"
                            AssociatedControlID="txtRetypeNewPassword" meta:resourcekey="lclRetypeNewPassword" />
                        <asp:TextBox ID="txtRetypeNewPassword" runat="server" CssClass="form-control"
                            TextMode="Password" autocomplete="new-password" />
                        <asp:RequiredFieldValidator ID="rfvRetypeNewPassword" runat="server"
                            ControlToValidate="txtRetypeNewPassword" ValidationGroup="ChangePassword"
                            CssClass="text-danger small" Display="Dynamic" meta:resourcekey="rfvRetypeNewPassword" />
                        <asp:CompareValidator ID="cvRetypeNewPassword" runat="server"
                            ControlToValidate="txtRetypeNewPassword" ControlToCompare="txtNewPassword" Operator="Equal" Type="String"
                            ValidationGroup="ChangePassword" CssClass="text-danger small" Display="Dynamic"
                            meta:resourcekey="cvRetypeNewPassword" />
                    </div>

                    <div class="col-12 mt-4 d-flex flex-wrap gap-2">
                        <asp:Button ID="btnChangePassword" runat="server" CssClass="btn btn-primary"
                            ValidationGroup="ChangePassword" OnClick="BtnChangePasswordClick" meta:resourcekey="btnChangePassword" />
                        <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-outline-secondary"
                            NavigateUrl="~/Pages/User/UpdateProfile.aspx" meta:resourcekey="lnkCancel" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
