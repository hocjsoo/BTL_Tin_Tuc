using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewsWebsite.App_Code;

namespace NewsWebsite
{
    public partial class ManageNews : Page
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

            lblInfo.Text = string.Format("Quản lý tin tức - Đăng nhập: {0} ({1})", CurrentUsername, CurrentRole);
            
            // Show edit buttons only for Admin and Editor
            var canEdit = CurrentRole == "Admin" || CurrentRole == "Editor";
            pnlEditButtons.Visible = canEdit;
            pnlCategoryButtons.Visible = canEdit;
            
            if (!IsPostBack)
            {
                BindGrid();
                BindCategoriesGrid();
            }
        }

        private void BindGrid()
        {
            gvNews.DataSource = NewsManager.GetAll();
            gvNews.DataBind();
        }
        
        private void BindCategoriesGrid()
        {
            gvCategories.DataSource = CategoryManager.GetAll();
            gvCategories.DataBind();
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
                // Display status
                var lblStatus = e.Row.FindControl("lblStatus") as Label;
                if (lblStatus != null)
                {
                    var item = NewsManager.GetById(gvNews.DataKeys[e.Row.RowIndex].Value.ToString());
                    if (item != null)
                    {
                        string statusText = "Nháp";
                        string statusClass = "badge bg-secondary";
                        
                        if (item.Status == "Published")
                        {
                            statusText = "Đã xuất bản";
                            statusClass = "badge bg-success";
                        }
                        else if (item.Status == "Scheduled")
                        {
                            statusText = "Đã lên lịch";
                            statusClass = "badge bg-warning";
                        }
                        else
                        {
                            statusText = "Nháp";
                            statusClass = "badge bg-secondary";
                        }
                        
                        lblStatus.Text = statusText;
                        lblStatus.CssClass = statusClass;
                    }
                }
                
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
                lblMsg.CssClass = "text-success";
                lblMsg.Text = "Đã xóa bài viết thành công.";
            }
            else
            {
                lblMsg.CssClass = "text-danger";
                lblMsg.Text = "Bạn không có quyền xóa bài viết này.";
            }
        }

        protected void gvNews_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only Editor and Admin can edit
            if (CurrentRole != "Admin" && CurrentRole != "Editor")
            {
                lblMsg.CssClass = "text-danger";
                lblMsg.Text = "Bạn không có quyền sửa bài viết. Chỉ Editor và Admin mới có thể sửa bài viết.";
                return;
            }
            
            var id = gvNews.SelectedDataKey.Value.ToString();
            // Redirect to AddEditNews page with id parameter
            Response.Redirect(string.Format("AddEditNews.aspx?id={0}", id));
        }
        
        // Categories management
        protected void gvCategories_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategories.PageIndex = e.NewPageIndex;
            BindCategoriesGrid();
        }
        
        protected void gvCategories_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Add confirm to delete button
                foreach (Control ctl in e.Row.Cells[e.Row.Cells.Count - 1].Controls)
                {
                    var btn = ctl as System.Web.UI.WebControls.LinkButton;
                    if (btn != null && btn.CommandName == "Delete")
                    {
                        btn.OnClientClick = "return confirm('Bạn có chắc muốn xóa chuyên mục này?');";
                    }
                }
            }
        }
        
        protected void gvCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = gvCategories.DataKeys[e.RowIndex].Value.ToString();
            
            if (CurrentRole == "Admin" || CurrentRole == "Editor")
            {
                try
                {
                    CategoryManager.Delete(id);
                    BindCategoriesGrid();
                    lblMsg.CssClass = "text-success";
                    lblMsg.Text = "Đã xóa chuyên mục thành công.";
                    
                    // Switch to categories tab
                    string script = @"
                        var categoriesTab = document.getElementById('tab-categories');
                        if (categoriesTab) {
                            var tab = new bootstrap.Tab(categoriesTab);
                            tab.show();
                        }
                    ";
                    ClientScript.RegisterStartupScript(this.GetType(), "SwitchToCategoriesTab", script, true);
                }
                catch (Exception ex)
                {
                    lblMsg.CssClass = "text-danger";
                    lblMsg.Text = "Lỗi khi xóa chuyên mục: " + ex.Message;
                }
            }
            else
            {
                lblMsg.CssClass = "text-danger";
                lblMsg.Text = "Bạn không có quyền xóa chuyên mục.";
            }
        }
        
        protected void gvCategories_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var id = gvCategories.DataKeys[e.NewEditIndex].Value.ToString();
            var category = CategoryManager.GetById(id);
            if (category != null)
            {
                // Use JavaScript to open modal with category data
                string categoryId = category.Id.Replace("'", "\\'").Replace("\"", "\\\"");
                string categoryName = (category.Name ?? "").Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");
                string categoryDesc = (category.Description ?? "").Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");
                string script = string.Format(
                    "editCategory('{0}', '{1}', '{2}');",
                    categoryId,
                    categoryName,
                    categoryDesc
                );
                ClientScript.RegisterStartupScript(this.GetType(), "EditCategory", script, true);
            }
        }
        
        protected void btnSaveCategory_Click(object sender, EventArgs e)
        {
            if (CurrentRole != "Admin" && CurrentRole != "Editor")
            {
                lblMsg.CssClass = "text-danger";
                lblMsg.Text = "Bạn không có quyền thêm/sửa chuyên mục.";
                return;
            }
            
            var id = hfCategoryId.Value.Trim();
            var name = txtCategoryName.Text.Trim();
            var description = txtCategoryDescription.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                lblMsg.CssClass = "text-danger";
                lblMsg.Text = "Tên chuyên mục không được để trống.";
                return;
            }
            
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    // Create new
                    var newCategory = new CategoryManager.CategoryItem
                    {
                        Name = name,
                        Description = description
                    };
                    CategoryManager.Create(newCategory);
                    lblMsg.CssClass = "text-success";
                    lblMsg.Text = "Đã thêm chuyên mục thành công.";
                }
                else
                {
                    // Update existing
                    var category = CategoryManager.GetById(id);
                    if (category != null)
                    {
                        category.Name = name;
                        category.Description = description;
                        if (CategoryManager.Update(category))
                        {
                            lblMsg.CssClass = "text-success";
                            lblMsg.Text = "Đã cập nhật chuyên mục thành công.";
                        }
                        else
                        {
                            lblMsg.CssClass = "text-danger";
                            lblMsg.Text = "Lỗi: Tên chuyên mục đã tồn tại hoặc không thể cập nhật.";
                        }
                    }
                    else
                    {
                        lblMsg.CssClass = "text-danger";
                        lblMsg.Text = "Không tìm thấy chuyên mục để cập nhật.";
                    }
                }
                
                BindCategoriesGrid();
                
                // Close modal and switch to categories tab
                string script = @"
                    var modal = bootstrap.Modal.getInstance(document.getElementById('categoryModal'));
                    if (modal) modal.hide();
                    // Switch to categories tab
                    var categoriesTab = document.getElementById('tab-categories');
                    if (categoriesTab) {
                        var tab = new bootstrap.Tab(categoriesTab);
                        tab.show();
                    }
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "CloseModalAndSwitchTab", script, true);
            }
            catch (Exception ex)
            {
                lblMsg.CssClass = "text-danger";
                lblMsg.Text = "Lỗi: " + ex.Message;
            }
        }
    }
}

