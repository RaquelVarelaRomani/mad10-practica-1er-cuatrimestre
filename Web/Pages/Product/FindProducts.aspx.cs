using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Es.Udc.DotNet.ModelUtil.IoC;
using Es.Udc.DotNet.PracticaMAD.Web.HTTP.Session;
using System;
using System.Globalization;
using System.Web;

namespace Es.Udc.DotNet.PracticaMAD.Web.Pages.Product
{
    public partial class FindProducts : SpecificCulturePage
    {
        private const int PageSize = 10;

        private string Keyword
        {
            get { return (Request.QueryString["keywords"] ?? String.Empty).Trim(); }
        }

        private int StartIndex
        {
            get
            {
                int value;
                return Int32.TryParse(Request.QueryString["startIndex"], NumberStyles.None,
                    CultureInfo.InvariantCulture, out value) && value <= Int32.MaxValue - PageSize
                    ? value - value % PageSize : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["keywords"] != null)
            {
                txtKeyword.Text = Keyword;
                Page.Validate("Search");
                if (!Page.IsValid)
                    return;

                IIoCManager manager = (IIoCManager)Application["managerIoC"];
                Block<ProductVO> products = manager.Resolve<IProductService>()
                    .FindProductByName(Keyword, StartIndex, PageSize);
                productList.DataSource = products.Items;
                productList.DataBind();
                pnlResults.Visible = true;

                lnkPrevious.Visible = StartIndex > 0;
                lnkPrevious.NavigateUrl = SearchUrl(Math.Max(0, StartIndex - PageSize));
                lnkNext.Visible = products.ExistsMoreItems;
                lnkNext.NavigateUrl = SearchUrl(StartIndex + PageSize);
            }
        }

        protected void BtnSearchClick(object sender, EventArgs e)
        {
            txtKeyword.Text = txtKeyword.Text.Trim();
            Page.Validate("Search");
            pnlResults.Visible = false;
            if (Page.IsValid)
            {
                Response.Redirect(Response.ApplyAppPathModifier(
                    "~/Pages/Product/FindProducts.aspx?keywords="
                    + HttpUtility.UrlEncode(txtKeyword.Text) + "&startIndex=0"), false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private string SearchUrl(int startIndex)
        {
            return "~/Pages/Product/FindProducts.aspx?keywords=" + HttpUtility.UrlEncode(Keyword)
                + "&startIndex=" + startIndex.ToString(CultureInfo.InvariantCulture);
        }
    }
}
