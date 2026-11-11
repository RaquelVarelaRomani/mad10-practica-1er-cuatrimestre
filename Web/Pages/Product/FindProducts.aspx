<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="FindProducts.aspx.cs" Inherits="Es.Udc.DotNet.PracticaMAD.Web.Pages.Product.FindProducts"
    Culture="es-ES" UICulture="es" UnobtrusiveValidationMode="None" meta:resourcekey="PageResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4 py-md-5">
        <h1 class="h3 fw-bold mb-4"><asp:Localize ID="lclTitle" runat="server" meta:resourcekey="lclTitle" /></h1>
        <asp:Panel ID="pnlSearch" runat="server" CssClass="card border-0 shadow-sm rounded-4 mb-4" DefaultButton="btnSearch">
            <div class="card-body p-4">
                <asp:Label ID="lclKeyword" runat="server" CssClass="form-label fw-semibold"
                    AssociatedControlID="txtKeyword" meta:resourcekey="lclKeyword" />
                <div class="d-flex flex-wrap gap-2">
                    <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control catalog-keyword" MaxLength="150" />
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-primary" ValidationGroup="Search"
                        OnClick="BtnSearchClick" meta:resourcekey="btnSearch" />
                </div>
                <p class="form-text mb-0"><asp:Localize ID="lclHint" runat="server" meta:resourcekey="lclHint" /></p>
                <asp:RequiredFieldValidator ID="rfvKeyword" runat="server" ControlToValidate="txtKeyword"
                    ValidationGroup="Search" Display="None" />
                <asp:RegularExpressionValidator ID="revKeyword" runat="server" ControlToValidate="txtKeyword"
                    ValidationExpression="\S{1,150}" ValidationGroup="Search" Display="None" />
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlResults" runat="server" Visible="false">
            <div class="table-responsive">
                <asp:GridView ID="productList" runat="server" AutoGenerateColumns="false" GridLines="None"
                    CssClass="table table-striped align-middle" UseAccessibleHeader="true" meta:resourcekey="productList">
                    <Columns>
                        <asp:BoundField DataField="Name" HtmlEncode="true" meta:resourcekey="colName" />
                        <asp:BoundField DataField="CategoryName" HtmlEncode="true" meta:resourcekey="colCategory" />
                        <asp:BoundField DataField="CreationDate" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="true" meta:resourcekey="colDate" />
                        <asp:BoundField DataField="SellerName" HtmlEncode="true" meta:resourcekey="colSeller" />
                    </Columns>
                </asp:GridView>
            </div>
            <div class="d-flex justify-content-between gap-3 mt-3">
                <asp:HyperLink ID="lnkPrevious" runat="server" CssClass="btn btn-outline-primary" Visible="false" meta:resourcekey="lnkPrevious" />
                <asp:HyperLink ID="lnkNext" runat="server" CssClass="btn btn-outline-primary ms-auto" Visible="false" meta:resourcekey="lnkNext" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>
