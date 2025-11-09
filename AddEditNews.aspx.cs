using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Hosting;
using System.Text.RegularExpressions;
using NewsWebsite.App_Code;

namespace NewsWebsite
{
    public partial class AddEditNews : Page
    {
        private string CurrentUsername
        {
            get { return (string)Session["Username"]; }
        }
        private string CurrentRole
        {
            get { return (string)Session["Role"]; }
        }

        public override void ProcessRequest(System.Web.HttpContext context)
        {
            // Override to handle validation exception early
            // Note: This may not catch all cases as validation can happen before ProcessRequest
            // The Web.config settings and Base64 encoding should handle most cases
            try
            {
                base.ProcessRequest(context);
            }
            catch (System.Web.HttpRequestValidationException ex)
            {
                // Validation exception caught - clear error and try to continue
                // This allows HTML content to be submitted if validation is disabled
                context.Server.ClearError();
                // Log the exception for debugging (optional)
                System.Diagnostics.Debug.WriteLine("HttpRequestValidationException caught: " + ex.Message);
                // Try to process request again - this should work if ValidateRequest="false" is set
                try
                {
                    base.ProcessRequest(context);
                }
                catch
                {
                    // If it still fails, show error page
                    context.Response.Write("<html><body><h1>Lỗi xử lý nội dung</h1><p>Vui lòng thử lại hoặc liên hệ quản trị viên.</p></body></html>");
                    context.Response.End();
                }
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only Editor and Admin can access
            var isLoggedIn = !string.IsNullOrEmpty(CurrentUsername);
            var canEdit = (CurrentRole == "Admin" || CurrentRole == "Editor");
            
            pnlAuth.Visible = !isLoggedIn || !canEdit;
            pnlMain.Visible = isLoggedIn && canEdit;
            
            if (!isLoggedIn || !canEdit)
            {
                return;
            }

            if (!IsPostBack)
            {
                // Load categories
                LoadCategories();
                
                // Check if editing existing article
                var id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                {
                    LoadArticle(id);
                }
                else
                {
                    // New article - page title is already set in aspx
                    UpdateStatusDisplay(null);
                }
            }
        }

        private void LoadCategories()
        {
            if (ddlCategory == null) return;
            
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new System.Web.UI.WebControls.ListItem("-- Chọn chuyên mục --", ""));
            
            var categories = CategoryManager.GetAll();
            foreach (var cat in categories)
                {
                ddlCategory.Items.Add(new System.Web.UI.WebControls.ListItem(cat.Name, cat.Name));
            }
        }

        private void LoadArticle(string id)
        {
            var item = NewsManager.GetById(id);
            if (item == null)
            {
                lblMsg.Text = "Không tìm thấy bài viết.";
                lblMsg.CssClass = "text-danger";
                return;
            }

            // Check permission - only Admin or article author can edit
            if (CurrentRole != "Admin" && !string.Equals(item.Author, CurrentUsername, StringComparison.OrdinalIgnoreCase))
            {
                lblMsg.Text = "Bạn không có quyền sửa bài viết này.";
                lblMsg.CssClass = "text-danger";
                return;
            }

            // Update page title (it's in ContentPlaceHolder, need to access via MasterPage or use literal)
            System.Web.UI.HtmlControls.HtmlGenericControl pageTitleCtrl = null;
            if (Master != null)
            {
                var mainContent = Master.FindControl("MainContent");
                if (mainContent != null)
                {
                    pageTitleCtrl = mainContent.FindControl("pageTitle") as System.Web.UI.HtmlControls.HtmlGenericControl;
                }
            }
            if (pageTitleCtrl != null) pageTitleCtrl.InnerText = "Sửa bài viết";
            hfId.Value = item.Id;
            txtTitle.Text = item.Title;
            txtSummary.Text = item.Summary;
            // Content may contain image tags, keep them as is for editing
            // TinyMCE will convert them to HTML when initialized
            txtContent.Text = item.Content ?? string.Empty;
            txtContent.Attributes["data-server-encoded"] = "false";
            if (ddlCategory != null)
            {
                ddlCategory.SelectedValue = item.Category ?? string.Empty;
            }
            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                pnlCurrentImage.Visible = true;
                imgCurrent.Src = item.ImageUrl;
            }
            else
            {
                pnlCurrentImage.Visible = false;
            }
            
            // Display status
            UpdateStatusDisplay(item);
        }
        
        private void UpdateStatusDisplay(NewsManager.NewsItem item)
        {
            if (item != null)
            {
                string statusText = "Nháp";
                string statusClass = "bg-secondary";
                
                if (item.Status == "Published")
                {
                    statusText = "Đã xuất bản";
                    statusClass = "bg-success";
                    if (item.PublishedAt.HasValue)
                    {
                        statusText += " (" + item.PublishedAt.Value.ToString("dd/MM/yyyy HH:mm") + ")";
                    }
                }
                else if (item.Status == "Scheduled")
                {
                    statusText = "Đã lên lịch";
                    statusClass = "bg-warning";
                    if (item.ScheduledAt.HasValue)
                    {
                        statusText += " (" + item.ScheduledAt.Value.ToString("dd/MM/yyyy HH:mm") + ")";
                    }
                }
                else
                {
                    statusText = "Nháp";
                }
                
                lblStatus.Text = statusText;
                lblStatus.CssClass = "badge " + statusClass + " ms-2";
            }
        }
        
        private string HandleImageUpload()
        {
            string imageUrl = string.Empty;
            
            if (fileUploadImage.HasFile)
            {
                try
                {
                    string uploadDir = HostingEnvironment.MapPath("~/Images/");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }
                    
                    string fileName = Path.GetFileName(fileUploadImage.FileName);
                    string fileExt = Path.GetExtension(fileName).ToLower();
                    
                    // Validate image type
                    if (fileExt == ".jpg" || fileExt == ".jpeg" || fileExt == ".png" || fileExt == ".gif")
                    {
                        string uniqueFileName = Guid.NewGuid().ToString("N") + fileExt;
                        string filePath = Path.Combine(uploadDir, uniqueFileName);
                        fileUploadImage.SaveAs(filePath);
                        imageUrl = "~/Images/" + uniqueFileName;
                    }
                    else
                    {
                        throw new Exception("Chỉ chấp nhận file ảnh (JPG, PNG, GIF).");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            
            return imageUrl;
        }
        
        private string HandleContentImageUpload()
        {
            string imageUrl = string.Empty;
            
            if (fileContentImageUpload.HasFile)
            {
                try
                {
                    string uploadDir = HostingEnvironment.MapPath("~/Images/Content/");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }
                    
                    string fileName = Path.GetFileName(fileContentImageUpload.FileName);
                    string fileExt = Path.GetExtension(fileName).ToLower();
                    
                    // Validate file size (5MB max)
                    if (fileContentImageUpload.PostedFile.ContentLength > 5 * 1024 * 1024)
                    {
                        throw new Exception("File ảnh vượt quá 5MB.");
                    }
                    
                    // Validate image type
                    if (fileExt == ".jpg" || fileExt == ".jpeg" || fileExt == ".png" || fileExt == ".gif")
                    {
                        string uniqueFileName = Guid.NewGuid().ToString("N") + fileExt;
                        string filePath = Path.Combine(uploadDir, uniqueFileName);
                        fileContentImageUpload.SaveAs(filePath);
                        imageUrl = "~/Images/Content/" + uniqueFileName;
                    }
                    else
                    {
                        throw new Exception("Chỉ chấp nhận file ảnh (JPG, PNG, GIF).");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            
            return imageUrl;
        }
        
        private NewsManager.NewsItem GetCurrentItem()
        {
            var id = hfId.Value;
            if (!string.IsNullOrEmpty(id))
            {
                return NewsManager.GetById(id);
            }
            return null;
        }
        
        private void SaveArticle(string status, DateTime? publishedAt, DateTime? scheduledAt)
        {
            if (CurrentRole != "Admin" && CurrentRole != "Editor")
            {
                lblMsg.Text = "Bạn không có quyền thêm/sửa bài viết.";
                lblMsg.CssClass = "text-danger";
                return;
            }
            
            string imageUrl = string.Empty;
            string contentImageUrl = string.Empty;
            
            try
            {
                imageUrl = HandleImageUpload();
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.CssClass = "text-danger";
                return;
            }
            
            try
            {
                contentImageUrl = HandleContentImageUpload();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Lỗi upload ảnh nội dung: " + ex.Message;
                lblMsg.CssClass = "text-danger";
                return;
            }
            
            var id = hfId.Value;
            NewsManager.NewsItem item;
            
            if (string.IsNullOrEmpty(id))
            {
                // Create new article
                // Get content from unvalidated form to bypass validation
                string content = Request.Unvalidated.Form[txtContent.UniqueID] ?? txtContent.Text ?? string.Empty;
                // Decode base64 if content is encoded
                content = DecodeContent(content);
                
                // If content image was uploaded, insert it into content
                if (!string.IsNullOrEmpty(contentImageUrl))
                {
                    string imageId = "img_" + Guid.NewGuid().ToString("N");
                    string fileName = Path.GetFileName(fileContentImageUpload.FileName);
                    string imageTag = "\n[IMAGE:" + imageId + ":" + contentImageUrl + ":" + fileName + "]\n";
                    content = content + imageTag;
                }
                
                item = new NewsManager.NewsItem
                {
                    Title = txtTitle.Text.Trim(),
                    Summary = txtSummary.Text.Trim(),
                    Content = SanitizeContent(content.Trim()),
                    Author = CurrentUsername,
                    Role = CurrentRole,
                    Category = ddlCategory.SelectedValue ?? string.Empty,
                    ImageUrl = imageUrl,
                    CreatedAt = DateTime.UtcNow,
                    Status = status,
                    PublishedAt = publishedAt,
                    ScheduledAt = scheduledAt
                };
                NewsManager.Create(item);
                hfId.Value = item.Id;
                lblMsg.Text = "Đã lưu bài viết thành công.";
                lblMsg.CssClass = "text-success";
                
                // Reload article to show updated content with images
                LoadArticle(item.Id);
            }
            else
            {
                // Update existing article
                item = NewsManager.GetById(id);
                if (item == null)
                {
                    lblMsg.Text = "Không tìm thấy bài viết.";
                    lblMsg.CssClass = "text-danger";
                    return;
                }
                
                // Check permission
                if (CurrentRole != "Admin" && !string.Equals(item.Author, CurrentUsername, StringComparison.OrdinalIgnoreCase))
                {
                    lblMsg.Text = "Bạn không có quyền sửa bài viết này.";
                    lblMsg.CssClass = "text-danger";
                    return;
                }
                
                item.Title = txtTitle.Text.Trim();
                item.Summary = txtSummary.Text.Trim();
                // Content is synced from editor to textarea
                // It contains image tags in format [IMAGE:id:url:name] or HTML
                // Get content from unvalidated form to bypass validation
                string content = Request.Unvalidated.Form[txtContent.UniqueID] ?? txtContent.Text ?? string.Empty;
                // Decode base64 if content is encoded
                content = DecodeContent(content);
                
                // If content image was uploaded, insert it into content
                if (!string.IsNullOrEmpty(contentImageUrl))
                {
                    string imageId = "img_" + Guid.NewGuid().ToString("N");
                    string fileName = Path.GetFileName(fileContentImageUpload.FileName);
                    string imageTag = "\n[IMAGE:" + imageId + ":" + contentImageUrl + ":" + fileName + "]\n";
                    content = content + imageTag;
                }
                
                // Sanitize content to remove dangerous scripts
                item.Content = SanitizeContent(content.Trim());
                item.Category = ddlCategory.SelectedValue ?? string.Empty;
                item.Status = status;
                
                // Only update PublishedAt if explicitly provided (not null)
                if (publishedAt.HasValue)
                {
                    item.PublishedAt = publishedAt;
                }
                // Only update ScheduledAt if explicitly provided (not null) or clearing it
                if (scheduledAt.HasValue)
                {
                    item.ScheduledAt = scheduledAt;
                }
                else if (status != "Scheduled")
                {
                    // Clear scheduled time if status is not Scheduled
                    item.ScheduledAt = null;
                }
                // If status is Scheduled but scheduledAt is null, keep existing scheduledAt
                
                // Only update image if new image uploaded, otherwise keep existing
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    item.ImageUrl = imageUrl;
                }
                
                NewsManager.Update(item);
                lblMsg.Text = "Đã cập nhật bài viết thành công.";
                lblMsg.CssClass = "text-success";
                
                // Reload article to show updated content with images
                LoadArticle(item.Id);
            }
            
            UpdateStatusDisplay(item);
        }
        
        protected void btnPreview_Click(object sender, EventArgs e)
        {
            Page.Validate("SaveNews");
            if (!Page.IsValid)
            {
                return;
            }
            
            // Save current content first (as draft if not published, or keep current status if published)
            var currentItem = GetCurrentItem();
            string currentStatus = currentItem != null ? currentItem.Status : "Draft";
            
            // Always save current content before preview
            // If already published, keep status as Published, otherwise save as Draft
            if (currentStatus == "Published")
            {
                // Keep published status but update content
                SaveArticle("Published", currentItem.PublishedAt, currentItem.ScheduledAt);
            }
            else
            {
                // Save as draft
                SaveArticle("Draft", null, null);
            }
            
            // Redirect to preview page - after save, id should be available
            var id = hfId.Value;
            if (string.IsNullOrEmpty(id))
            {
                lblMsg.Text = "Lỗi: Không thể lưu bài viết. Vui lòng thử lại.";
                lblMsg.CssClass = "text-danger";
                return;
            }
            
            Response.Redirect(string.Format("Preview.aspx?id={0}", id));
        }
        
        protected void btnPublishNow_Click(object sender, EventArgs e)
        {
            Page.Validate("SaveNews");
            if (!Page.IsValid)
            {
                return;
            }
            
            SaveArticle("Published", DateTime.UtcNow, null);
        }
        
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageNews.aspx");
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            if (string.IsNullOrEmpty(hfId.Value))
            {
                // Only clear if it's a new article
                txtTitle.Text = string.Empty;
                txtSummary.Text = string.Empty;
                txtContent.Text = string.Empty;
                if (ddlCategory != null) ddlCategory.SelectedIndex = 0;
                pnlCurrentImage.Visible = false;
                if (fileUploadImage != null) fileUploadImage.Attributes.Clear();
            }
        }
        
        /// <summary>
        /// Decode base64 encoded content
        /// </summary>
        private string DecodeContent(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            
            var originalContent = content.Trim();
            
            if (originalContent.IndexOf("[IMAGE:", StringComparison.OrdinalIgnoreCase) >= 0 ||
                (originalContent.IndexOf('<') >= 0 && originalContent.IndexOf('>') > originalContent.IndexOf('<')))
            {
                return originalContent;
            }
            
            // Convert spaces back to plus characters (form submissions often turn '+' into ' ')
            var candidate = originalContent.Replace(' ', '+');
            
            // Remove other whitespace characters
            candidate = Regex.Replace(candidate, @"\s+", string.Empty);
            
            if (candidate.Length <= 20)
            {
                return originalContent;
            }
            
            if (!Regex.IsMatch(candidate, @"^[A-Za-z0-9+/=]+$"))
            {
                return originalContent;
            }
            
            if (candidate.Length % 4 != 0)
            {
                int padding = 4 - (candidate.Length % 4);
                if (padding < 4)
                {
                    candidate = candidate.PadRight(candidate.Length + padding, '=');
                }
            }
            
            try
            {
                byte[] data = Convert.FromBase64String(candidate);
                string decoded = System.Text.Encoding.UTF8.GetString(data);
                
                if (!string.IsNullOrEmpty(decoded) &&
                    (decoded.IndexOf("[IMAGE:", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     decoded.IndexOf('<') >= 0 ||
                     decoded.IndexOf('&') >= 0 ||
                     decoded.IndexOf('"') >= 0 ||
                     decoded.IndexOf("\n", StringComparison.Ordinal) >= 0))
                {
                    return decoded;
                }
            }
            catch (FormatException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (Exception)
            {
            }
            
            return originalContent;
        }
        
        /// <summary>
        /// Sanitize content to remove dangerous scripts while keeping safe HTML
        /// </summary>
        private string SanitizeContent(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            
            // First, clean up Word-specific HTML that might cause issues
            // Remove Word-specific style attributes and meta tags
            content = Regex.Replace(content, @"\s*mso-[^:;""'\s]+:[^;""'\s]*;?", string.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\s*font-family:[^;""'\s]*;?", string.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\s*panose-[^:;""'\s]+:[^;""'\s]*;?", string.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\s*xml:lang=""[^""]*""", string.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\s*lang=""[^""]*""", string.Empty, RegexOptions.IgnoreCase);
            
            // Remove Word-specific comments and meta tags
            content = Regex.Replace(content, @"<!--\[if[^\]]*\]>.*?<!\[endif\]-->", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            content = Regex.Replace(content, @"<meta[^>]*>", string.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"<style[^>]*>.*?</style>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            // Remove script tags and event handlers
            content = Regex.Replace(content, @"<script[^>]*>.*?</script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            content = Regex.Replace(content, @"<script[^>]*>", string.Empty, RegexOptions.IgnoreCase);
            
            // Remove dangerous event handlers
            content = Regex.Replace(content, @"\s*on\w+\s*=\s*[""'][^""']*[""']", string.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"\s*on\w+\s*=\s*[^\s>]*", string.Empty, RegexOptions.IgnoreCase);
            
            // Remove javascript: protocol
            content = Regex.Replace(content, @"javascript\s*:", string.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"vbscript\s*:", string.Empty, RegexOptions.IgnoreCase);
            
            // Remove iframe tags
            content = Regex.Replace(content, @"<iframe[^>]*>.*?</iframe>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            content = Regex.Replace(content, @"<iframe[^>]*>", string.Empty, RegexOptions.IgnoreCase);
            
            // Remove object and embed tags
            content = Regex.Replace(content, @"<object[^>]*>.*?</object>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            content = Regex.Replace(content, @"<embed[^>]*>.*?</embed>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            // Clean up empty style attributes
            content = Regex.Replace(content, @"\s*style\s*=\s*[""']\s*[""']", string.Empty, RegexOptions.IgnoreCase);
            
            // Clean up multiple whitespace (but preserve line breaks in text content)
            content = Regex.Replace(content, @"[ \t]+", " ", RegexOptions.Multiline);
            content = Regex.Replace(content, @"\r\n\s*\r\n", "\r\n", RegexOptions.Multiline);
            
            return content;
        }
    }
}

