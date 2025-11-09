using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;
using NewsWebsite.App_Code;

namespace NewsWebsite
{
    public partial class Preview : Page
    {
        private string CurrentUsername
        {
            get { return (string)Session["Username"]; }
        }
        private string CurrentRole
        {
            get { return (string)Session["Role"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            if (string.IsNullOrEmpty(id))
            {
                pnlNotFound.Visible = true;
                return;
            }

            var item = NewsManager.GetById(id);
            if (item == null)
            {
                pnlNotFound.Visible = true;
                return;
            }

            // Check permission - only author, Editor, or Admin can preview
            if (CurrentRole != "Admin" && CurrentRole != "Editor" && 
                !string.Equals(item.Author, CurrentUsername, StringComparison.OrdinalIgnoreCase))
            {
                pnlNotFound.Visible = true;
                return;
            }

            // Display preview
            pnlPreview.Visible = true;
            previewTitle.InnerText = item.Title ?? string.Empty;
            previewSummary.InnerText = item.Summary ?? string.Empty;
            previewContent.InnerHtml = ProcessContentWithImages(item.Content);
            previewAuthor.InnerText = "Tác giả: " + (item.Author ?? "Không xác định");
            previewCategory.InnerText = item.Category ?? "Chưa phân loại";
            previewDate.InnerText = "Ngày tạo: " + item.CreatedAt.ToString("dd/MM/yyyy HH:mm");

            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                previewImageContainer.Visible = true;
                previewImage.Src = item.ImageUrl;
            }
            else
            {
                previewImageContainer.Visible = false;
            }

            // Update back link to include article ID
            System.Web.UI.HtmlControls.HtmlAnchor backLink = null;
            var mainContent = Master != null ? Master.FindControl("MainContent") : null;
            if (mainContent != null)
            {
                backLink = mainContent.FindControl("backLink") as System.Web.UI.HtmlControls.HtmlAnchor;
            }
            if (backLink == null)
            {
                backLink = FindControl("backLink") as System.Web.UI.HtmlControls.HtmlAnchor;
            }
            if (backLink != null)
            {
                backLink.HRef = string.Format("AddEditNews.aspx?id={0}", id);
            }
        }
        private string ProcessContentWithImages(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            // Handle [IMAGE:id:url:name] format
            // For data URLs, url can contain many colons, so we need to parse manually
            var imageReplacements = new System.Collections.Generic.Dictionary<string, string>();
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
    <img src=""{0}"" alt=""{1}"" class=""img-fluid"" style=""max-width: 100%; height: auto; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);""/>
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
            processed = Server.HtmlEncode(processed);
            processed = processed.Replace("\n", "<br/>");

            foreach (var replacement in imageReplacements)
            {
                processed = processed.Replace(Server.HtmlEncode(replacement.Key), replacement.Value);
            }

            return processed;
        }
    }
}

