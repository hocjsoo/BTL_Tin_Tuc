using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Hosting;
using NewsManagement.App_Code;

namespace NewsManagement
{
    public partial class Admin : Page
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
            var isLoggedIn = !string.IsNullOrEmpty(CurrentUsername);
            pnlAuth.Visible = !isLoggedIn;
            pnlMain.Visible = isLoggedIn;
            if (!isLoggedIn) return;

            lblInfo.Text = string.Format("Đăng nhập: {0} ({1})", CurrentUsername, CurrentRole);
            if (!IsPostBack)
            {
                BindGrid();
                pnlForm.Visible = false;
                
                // Check if "add" parameter exists in query string
                if (!string.IsNullOrEmpty(Request.QueryString["add"]) && Request.QueryString["add"] == "1")
                {
                    ShowAddForm();
                }
            }
        }

        private void BindGrid()
        {
            gvNews.DataSource = NewsManager.GetAll();
            gvNews.DataBind();
        }

        protected void gvNews_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvNews.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvNews_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // add confirm to delete button in CommandField
                foreach (Control ctl in e.Row.Cells[e.Row.Cells.Count - 1].Controls)
                {
                    var btn = ctl as System.Web.UI.WebControls.LinkButton;
                    if (btn != null && btn.CommandName == "Delete")
                    {
                        btn.OnClientClick = "return confirm('Bạn có chắc muốn xóa bài viết này?');";
                    }
                }
            }
        }

        protected void gvNews_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = gvNews.DataKeys[e.RowIndex].Value.ToString();
            var item = NewsManager.GetById(id);
            if (item == null) return;
            if (CurrentRole == "Admin" || string.Equals(item.Author, CurrentUsername, StringComparison.OrdinalIgnoreCase))
            {
                NewsManager.Delete(id);
                BindGrid();
                lblMsg.CssClass = "ms-3 text-success";
                lblMsg.Text = "Đã xóa.";
            }
            else
            {
                lblMsg.CssClass = "ms-3 text-danger";
                lblMsg.Text = "Bạn không có quyền xóa bài viết này.";
            }
        }

        protected void gvNews_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = gvNews.SelectedDataKey.Value.ToString();
            var item = NewsManager.GetById(id);
            if (item == null) return;
            
            // Show form and load article data
            pnlForm.Visible = true;
            var formTitleCtrl = pnlForm.FindControl("formTitle") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (formTitleCtrl != null) formTitleCtrl.InnerText = "Sửa bài viết";
            
            hfId.Value = item.Id;
            txtTitle.Text = item.Title;
            txtSummary.Text = item.Summary;
            txtContent.Text = item.Content;
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
        }
        
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            ShowAddForm();
        }
        
        private void ShowAddForm()
        {
            pnlForm.Visible = true;
            var formTitleCtrl = pnlForm.FindControl("formTitle") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (formTitleCtrl != null) formTitleCtrl.InnerText = "Thêm bài viết mới";
            ClearForm();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string imageUrl = string.Empty;
            
            // Handle image upload
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
                        lblMsg.CssClass = "ms-3 text-danger";
                        lblMsg.Text = "Chỉ chấp nhận file ảnh (JPG, PNG, GIF).";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.CssClass = "ms-3 text-danger";
                    lblMsg.Text = "Lỗi upload ảnh: " + ex.Message;
                    return;
                }
            }
            
            var id = hfId.Value;
            if (string.IsNullOrEmpty(id))
            {
                var created = new NewsManager.NewsItem
                {
                    Title = txtTitle.Text.Trim(),
                    Summary = txtSummary.Text.Trim(),
                    Content = txtContent.Text.Trim(),
                    Author = CurrentUsername,
                    Role = CurrentRole,
                    Category = ddlCategory.SelectedValue ?? string.Empty,
                    ImageUrl = imageUrl,
                    CreatedAt = DateTime.UtcNow
                };
                NewsManager.Create(created);
                lblMsg.CssClass = "ms-3 text-success";
                lblMsg.Text = "Đã thêm.";
            }
            else
            {
                var item = NewsManager.GetById(id);
                if (item == null) return;
                if (CurrentRole != "Admin" && !string.Equals(item.Author, CurrentUsername, StringComparison.OrdinalIgnoreCase))
                {
                    lblMsg.CssClass = "ms-3 text-danger";
                    lblMsg.Text = "Bạn không có quyền sửa bài viết này.";
                    return;
                }
                item.Title = txtTitle.Text.Trim();
                item.Summary = txtSummary.Text.Trim();
                item.Content = txtContent.Text.Trim();
                item.Category = ddlCategory.SelectedValue ?? string.Empty;
                // Only update image if new image uploaded, otherwise keep existing
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    item.ImageUrl = imageUrl;
                }
                // If no new image and existing item has no imageUrl, keep it empty
                NewsManager.Update(item);
                lblMsg.CssClass = "ms-3 text-success";
                lblMsg.Text = "Đã lưu thay đổi.";
            }
            ClearForm();
            pnlForm.Visible = false; // Hide form after saving
            BindGrid();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            hfId.Value = string.Empty;
            txtTitle.Text = string.Empty;
            txtSummary.Text = string.Empty;
            txtContent.Text = string.Empty;
            if (ddlCategory != null) ddlCategory.SelectedIndex = 0;
            pnlCurrentImage.Visible = false;
            if (fileUploadImage != null) fileUploadImage.Attributes.Clear();
        }
    }
}


