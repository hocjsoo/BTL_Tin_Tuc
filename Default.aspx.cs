using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewsManagement.App_Code;

namespace NewsManagement
{
    public partial class _Default : Page
    {
        private const int PageSize = 6;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindPaged();
            }
        }
        
        protected void btnSearchHome_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchHome.Text.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                Response.Redirect("Search.aspx?q=" + System.Web.HttpUtility.UrlEncode(keyword));
            }
        }

        private void BindPaged()
        {
            var all = NewsManager.GetAll();
            // Filter to only articles with valid ID (must exist in admin)
            var allList = System.Linq.Enumerable.ToList(all).Where(a => !string.IsNullOrEmpty(a.Id)).ToList();
            
            // Get hero article index from query string (default to 0)
            int heroIndex = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["hero"]))
            {
                int.TryParse(Request.QueryString["hero"], out heroIndex);
            }
            if (heroIndex < 0) heroIndex = 0;
            if (heroIndex >= allList.Count) heroIndex = allList.Count > 0 ? allList.Count - 1 : 0;
            
            // Featured article (based on hero index)
            var featuredCard = FindControl("featuredArticle") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (allList.Count > 0)
            {
                var featured = allList[heroIndex];
                
                // Show banner
                if (featuredCard != null) featuredCard.Visible = true;
                
                // Set article data - find controls and update with real data
                System.Web.UI.Control mainContent = null;
                if (this.Master != null)
                {
                    mainContent = this.Master.FindControl("MainContent");
                }
                
                var titleCtrl = mainContent != null ? mainContent.FindControl("featuredTitle") as System.Web.UI.HtmlControls.HtmlGenericControl : null;
                if (titleCtrl == null) titleCtrl = FindControl("featuredTitle") as System.Web.UI.HtmlControls.HtmlGenericControl;
                
                var summaryCtrl = mainContent != null ? mainContent.FindControl("featuredSummary") as System.Web.UI.HtmlControls.HtmlGenericControl : null;
                if (summaryCtrl == null) summaryCtrl = FindControl("featuredSummary") as System.Web.UI.HtmlControls.HtmlGenericControl;
                
                var categoryCtrl = mainContent != null ? mainContent.FindControl("featuredCategory") as System.Web.UI.HtmlControls.HtmlGenericControl : null;
                if (categoryCtrl == null) categoryCtrl = FindControl("featuredCategory") as System.Web.UI.HtmlControls.HtmlGenericControl;
                
                // Force update with actual data from database - this will override any HTML placeholder text
                if (titleCtrl != null && featured != null)
                {
                    titleCtrl.InnerText = featured.Title ?? "";
                    titleCtrl.Visible = !string.IsNullOrEmpty(featured.Title);
                }
                if (summaryCtrl != null && featured != null)
                {
                    summaryCtrl.InnerText = featured.Summary ?? "";
                    summaryCtrl.Visible = !string.IsNullOrEmpty(featured.Summary);
                }
                if (categoryCtrl != null && featured != null)
                {
                    categoryCtrl.InnerText = featured.Category ?? "Tin tức";
                    categoryCtrl.Visible = true;
                }
                
                // Find link control through Master Page if needed
                var linkCtrl = mainContent != null ? mainContent.FindControl("featuredLink") as System.Web.UI.HtmlControls.HtmlAnchor : null;
                if (linkCtrl == null) linkCtrl = FindControl("featuredLink") as System.Web.UI.HtmlControls.HtmlAnchor;
                
                if (linkCtrl != null && featured != null)
                {
                    if (!string.IsNullOrEmpty(featured.Id))
                    {
                        linkCtrl.HRef = ResolveUrl("~/Details.aspx?id=" + featured.Id);
                        linkCtrl.InnerText = "Đọc bài viết";
                        linkCtrl.Visible = true;
                    }
                    else
                    {
                        linkCtrl.Visible = false;
                    }
                }
                
                // Featured image - find controls through Master Page if needed
                var heroImageContainer = mainContent != null ? mainContent.FindControl("heroImageContainer") as System.Web.UI.HtmlControls.HtmlGenericControl : null;
                if (heroImageContainer == null) heroImageContainer = FindControl("heroImageContainer") as System.Web.UI.HtmlControls.HtmlGenericControl;
                
                var heroPlaceholder = mainContent != null ? mainContent.FindControl("heroPlaceholder") as System.Web.UI.HtmlControls.HtmlGenericControl : null;
                if (heroPlaceholder == null) heroPlaceholder = FindControl("heroPlaceholder") as System.Web.UI.HtmlControls.HtmlGenericControl;
                
                if (heroImageContainer != null && featured != null)
                {
                    // Trim and check ImageUrl
                    string imageUrlValue = (featured.ImageUrl ?? "").Trim();
                    if (!string.IsNullOrEmpty(imageUrlValue))
                    {
                        // Show real image as background - ensure proper URL resolution
                        string imageUrl = ResolveUrl(imageUrlValue);
                        // Remove any leading slashes or tildes if needed
                        if (imageUrl.StartsWith("~")) imageUrl = ResolveUrl(imageUrl);
                        
                        heroImageContainer.Style["background-image"] = string.Format("url('{0}')", imageUrl);
                        heroImageContainer.Style["background-size"] = "cover";
                        heroImageContainer.Style["background-position"] = "center";
                        heroImageContainer.Style["background-repeat"] = "no-repeat";
                        heroImageContainer.Style.Remove("background"); // Remove gradient if exists
                        // Hide placeholder when image exists
                        if (heroPlaceholder != null) heroPlaceholder.Visible = false;
                    }
                    else
                    {
                        // Show placeholder when no image
                        heroImageContainer.Style["background"] = "linear-gradient(135deg, #667eea 0%, #764ba2 100%)";
                        heroImageContainer.Style["background-image"] = "none";
                        heroImageContainer.Style.Remove("background-size");
                        heroImageContainer.Style.Remove("background-position");
                        if (heroPlaceholder != null) heroPlaceholder.Visible = true;
                    }
                }
                
                // Remove old literal approach
                var litFeaturedImg = FindControl("litFeaturedImage") as System.Web.UI.WebControls.Literal;
                if (litFeaturedImg != null) litFeaturedImg.Visible = false;
                
                // Hero indicator
                var heroIndicator = FindControl("heroIndicator") as System.Web.UI.HtmlControls.HtmlGenericControl;
                if (heroIndicator != null && allList.Count > 1)
                {
                    heroIndicator.InnerText = string.Format("Bài viết {0}/{1}", heroIndex + 1, allList.Count);
                }
                
                // Navigation buttons visibility
                var btnPrev = FindControl("btnHeroPrev") as System.Web.UI.HtmlControls.HtmlButton;
                var btnNext = FindControl("btnHeroNext") as System.Web.UI.HtmlControls.HtmlButton;
                if (btnPrev != null) btnPrev.Visible = allList.Count > 1;
                if (btnNext != null) btnNext.Visible = allList.Count > 1;
            }
            else
            {
                // Hide banner when no articles
                if (featuredCard != null) featuredCard.Visible = false;
            }
            
            // Display ALL articles in grid (no pagination, no exclusion of hero article)
            rptNews.DataSource = allList;
            rptNews.DataBind();

            // Hide pagination since we show all articles
            lnkPrev.Visible = false;
            lnkNext.Visible = false;
            lblPageInfo.Visible = false;
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

        protected void btnHeroPrev_Click(object sender, EventArgs e)
        {
            var all = NewsManager.GetAll();
            var allList = System.Linq.Enumerable.ToList(all);
            if (allList.Count == 0) return;
            
            int heroIndex = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["hero"]))
            {
                int.TryParse(Request.QueryString["hero"], out heroIndex);
            }
            
            heroIndex = (heroIndex - 1 + allList.Count) % allList.Count;
            // Preserve page parameter if exists
            string pageParam = Request.QueryString["page"];
            string redirectUrl = "Default.aspx?hero=" + heroIndex;
            if (!string.IsNullOrEmpty(pageParam))
            {
                redirectUrl += "&page=" + pageParam;
            }
            Response.Redirect(redirectUrl);
        }

        protected void btnHeroNext_Click(object sender, EventArgs e)
        {
            var all = NewsManager.GetAll();
            var allList = System.Linq.Enumerable.ToList(all);
            if (allList.Count == 0) return;
            
            int heroIndex = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["hero"]))
            {
                int.TryParse(Request.QueryString["hero"], out heroIndex);
            }
            
            heroIndex = (heroIndex + 1) % allList.Count;
            // Preserve page parameter if exists
            string pageParam = Request.QueryString["page"];
            string redirectUrl = "Default.aspx?hero=" + heroIndex;
            if (!string.IsNullOrEmpty(pageParam))
            {
                redirectUrl += "&page=" + pageParam;
            }
            Response.Redirect(redirectUrl);
        }
    }
}


