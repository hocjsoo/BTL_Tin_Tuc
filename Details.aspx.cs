using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewsWebsite.App_Code;

namespace NewsWebsite
{
    public partial class Details : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                {
                    id = System.Web.HttpUtility.UrlDecode(id);
                }
                var item = NewsManager.GetById(id);
                if (item == null)
                {
                    pnlDetails.Visible = false;
                    pnlNotFound.Visible = true;
                    return;
                }
                lblTitle.Text = item.Title;
                lblAuthor.Text = item.Author;
                lblDate.Text = item.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                lblSummary.Text = item.Summary;
                
                // Process content with images
                string processedContent = ProcessContentWithImages(item.Content);
                litContent.Text = processedContent;
                
                var categoryCtrl = FindControl("detailCategory") as System.Web.UI.HtmlControls.HtmlGenericControl;
                if (categoryCtrl != null) categoryCtrl.InnerText = item.Category ?? "News";
                
                // Display image
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    litImage.Text = string.Format(@"<div class=""mb-4"" style=""background-image: url('{0}'); background-size: cover; background-position: center; min-height: 400px; border-radius: 10px;""></div>", ResolveUrl(item.ImageUrl));
                }
                else
                {
                    litImage.Text = @"<div class=""mb-4"" style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 400px; display: flex; align-items: center; justify-content: center; border-radius: 10px;""><div style=""text-align: center; color: white;""><div style=""font-size: 5rem;"">🔵</div><p style=""margin-top: 20px; font-size: 1.2rem;"">Featured Image</p></div></div>";
                }
                
                // Load related articles
                LoadRelatedArticles(item);
            }
        }
        
        private void LoadRelatedArticles(NewsManager.NewsItem currentItem)
        {
            if (currentItem == null || string.IsNullOrEmpty(currentItem.Id))
            {
                rptRelatedArticles.Visible = false;
                lblNoRelatedArticles.Visible = true;
                return;
            }
            
            // Get related articles: same category first, then recent articles
            var allPublished = NewsManager.GetPublished().ToList();
            
            // Filter out current article (null-safe comparison)
            var relatedArticles = allPublished
                .Where(a => a != null && 
                           !string.IsNullOrEmpty(a.Id) && 
                           a.Id != currentItem.Id)
                .ToList();
            
            if (relatedArticles.Count == 0)
            {
                rptRelatedArticles.Visible = false;
                lblNoRelatedArticles.Visible = true;
                return;
            }
            
            // Try to get articles from same category first
            var sameCategoryArticles = new List<NewsManager.NewsItem>();
            
            if (!string.IsNullOrEmpty(currentItem.Category))
            {
                sameCategoryArticles = relatedArticles
                    .Where(a => !string.IsNullOrEmpty(a.Category) &&
                               string.Equals(a.Category, currentItem.Category, StringComparison.OrdinalIgnoreCase))
                    .Take(3)
                    .ToList();
            }
            
            // If we don't have enough articles from same category, fill with recent articles
            if (sameCategoryArticles.Count < 3)
            {
                // Get IDs of articles already in sameCategoryArticles
                var existingIds = new HashSet<string>(sameCategoryArticles.Where(s => s != null && !string.IsNullOrEmpty(s.Id)).Select(s => s.Id));
                
                var additionalArticles = relatedArticles
                    .Where(a => a != null && !string.IsNullOrEmpty(a.Id) && !existingIds.Contains(a.Id))
                    .OrderByDescending(a => a.PublishedAt ?? a.ScheduledAt ?? a.CreatedAt)
                    .Take(3 - sameCategoryArticles.Count)
                    .ToList();
                
                sameCategoryArticles.AddRange(additionalArticles);
            }
            
            // Limit to 3 articles and ensure all have valid IDs
            var finalRelatedArticles = sameCategoryArticles
                .Where(a => a != null && !string.IsNullOrEmpty(a.Id))
                .Take(3)
                .ToList();
            
            if (finalRelatedArticles.Count > 0)
            {
                rptRelatedArticles.DataSource = finalRelatedArticles;
                rptRelatedArticles.DataBind();
                rptRelatedArticles.Visible = true;
                lblNoRelatedArticles.Visible = false;
            }
            else
            {
                rptRelatedArticles.Visible = false;
                lblNoRelatedArticles.Visible = true;
            }
        }
        
        protected void rptRelatedArticles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as NewsManager.NewsItem;
                if (item == null || string.IsNullOrEmpty(item.Id)) return;
                
                var litImage = e.Item.FindControl("litRelatedImage") as Literal;
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
                
                // Ensure link has valid ID and URL encoding
                var linkCtrl = e.Item.FindControl("lnkRelatedArticle") as HyperLink;
                if (linkCtrl != null && !string.IsNullOrEmpty(item.Id))
                {
                    linkCtrl.NavigateUrl = ResolveUrl("~/Details.aspx?id=" + System.Web.HttpUtility.UrlEncode(item.Id));
                }
            }
        }
        
        private string ProcessContentWithImages(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            
            // Handle [IMAGE:id:url:name] format
            // For data URLs, url can contain many colons, so we need to parse manually
            var imageReplacements = new Dictionary<string, string>();
            var replacementIndex = 0;
            var result = new System.Text.StringBuilder();
            var lastIndex = 0;
            var startTag = "[IMAGE:";

            while (true)
            {
                var startPos = content.IndexOf(startTag, lastIndex);
                if (startPos == -1)
                {
                    // No more image tags
                    result.Append(content.Substring(lastIndex));
                    break;
                }

                // Add text before tag
                result.Append(content.Substring(lastIndex, startPos - lastIndex));

                // Find the end of the tag: ] after the name
                var tagStart = startPos + startTag.Length;
                var idEnd = content.IndexOf(':', tagStart);
                if (idEnd == -1) break;

                var id = content.Substring(tagStart, idEnd - tagStart);
                var urlStart = idEnd + 1;

                // Find the last colon before the closing bracket
                var bracketPos = content.IndexOf(']', urlStart);
                if (bracketPos == -1) break;

                // Work backwards from ] to find the last :
                var urlEnd = -1;
                for (int i = bracketPos - 1; i >= urlStart; i--)
                {
                    if (content[i] == ':')
                    {
                        urlEnd = i;
                        break;
                    }
                }

                if (urlEnd == -1)
                {
                    // Malformed tag, skip it
                    lastIndex = startPos + startTag.Length;
                    continue;
                }

                var url = content.Substring(urlStart, urlEnd - urlStart).Trim();
                var fileName = content.Substring(urlEnd + 1, bracketPos - urlEnd - 1).Trim();
                
                // If url is a base64 data URL, use it directly
                // Otherwise, treat it as a relative URL
                string imgSrc;
                if (!string.IsNullOrEmpty(url) && url.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
                {
                    imgSrc = Regex.Replace(url, @"\s+", string.Empty);
                }
                else
                {
                    imgSrc = ResolveUrl(url);
                }
                
                var placeholder = string.Format("__IMAGE_PLACEHOLDER_{0}__", replacementIndex);
                var htmlImage = string.Format(
                    @"<div class=""article-image mb-4"" style=""text-align: center;"">
                        <img src=""{0}"" alt=""{1}"" class=""img-fluid"" style=""max-width: 100%; height: auto; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);"" />
                        <p class=""text-muted mt-2 small"">{1}</p>
                    </div>",
                    imgSrc,
                    Server.HtmlEncode(fileName)
                );
                
                imageReplacements[placeholder] = htmlImage;
                result.Append(placeholder);
                replacementIndex++;

                lastIndex = bracketPos + 1;
            }
            
            var processed = result.ToString();
            // Now encode the rest of the content
            processed = Server.HtmlEncode(processed);
            processed = processed.Replace("\n", "<br/>");
            
            // Replace placeholders back with HTML images
            foreach (var replacement in imageReplacements)
            {
                processed = processed.Replace(Server.HtmlEncode(replacement.Key), replacement.Value);
            }
            
            return processed;
        }
    }
}


