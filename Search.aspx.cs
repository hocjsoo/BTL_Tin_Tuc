using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewsManagement.App_Code;

namespace NewsManagement
{
    public partial class Search : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if there's a search query in URL
                string query = Request.QueryString["q"];
                if (!string.IsNullOrEmpty(query))
                {
                    txtSearch.Text = query;
                    PerformSearch(query);
                }
                else
                {
                    // Show all latest articles by default
                    LoadAllArticles();
                }
            }
        }
        
        private void LoadAllArticles()
        {
            var allArticles = NewsManager.GetAll().ToList();
            if (allArticles.Count > 0)
            {
                rptSearchResults.DataSource = allArticles;
                rptSearchResults.DataBind();
                pnlNoResults.Visible = false;
                lblMessage.Text = string.Format("Hiển thị {0} bài viết mới nhất", allArticles.Count);
                lblMessage.Visible = true;
            }
            else
            {
                rptSearchResults.DataSource = null;
                rptSearchResults.DataBind();
                pnlNoResults.Visible = true;
                lblMessage.Visible = false;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                PerformSearch(keyword);
            }
            else
            {
                // If search box is empty, show all articles
                LoadAllArticles();
            }
        }
        
        // Method for AJAX search (called by JavaScript)
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                PerformSearch(keyword);
            }
            else
            {
                LoadAllArticles();
            }
        }

        private void PerformSearch(string keyword)
        {
            var results = NewsManager.Search(keyword).ToList();
            
            if (results.Count > 0)
            {
                rptSearchResults.DataSource = results;
                rptSearchResults.DataBind();
                pnlNoResults.Visible = false;
                lblMessage.Text = string.Format("Tìm thấy {0} bài viết cho từ khóa \"{1}\"", results.Count, keyword);
                lblMessage.Visible = true;
            }
            else
            {
                rptSearchResults.DataSource = null;
                rptSearchResults.DataBind();
                pnlNoResults.Visible = true;
                lblMessage.Text = string.Format("Không tìm thấy bài viết nào cho từ khóa \"{0}\"", keyword);
                lblMessage.Visible = true;
            }
        }

        protected void rptSearchResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
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


