using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewsWebsite.App_Code;

namespace NewsWebsite
{
    public partial class Category : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var category = Request.QueryString["cat"] ?? string.Empty;
                lblCategoryName.Text = string.IsNullOrEmpty(category) ? "Tất cả" : category;
                
                var items = string.IsNullOrEmpty(category) 
                    ? NewsManager.GetPublished() 
                    : NewsManager.GetByCategory(category);
                
                var itemsList = items.ToList();
                if (itemsList.Count > 0)
                {
                    rptNews.DataSource = itemsList;
                    rptNews.DataBind();
                    pnlEmpty.Visible = false;
                }
                else
                {
                    pnlEmpty.Visible = true;
                }
            }
        }

        protected void rptNews_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as NewsManager.NewsItem;
                if (item == null) return;
                
                var litImage = e.Item.FindControl("litImage") as Literal;
                if (litImage != null)
                {
                    if (string.IsNullOrEmpty(item.ImageUrl))
                    {
                        litImage.Text = @"<div style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); height: 200px; display: flex; align-items: center; justify-content: center; color: white;""><span style=""font-size: 3rem;"">📰</span></div>";
                    }
                    else
                    {
                        litImage.Text = string.Format(@"<img src=""{0}"" alt="""" class=""card-img-top"" style=""height: 200px; object-fit: cover;"" />", ResolveUrl(item.ImageUrl));
                    }
                }
            }
        }
    }
}

