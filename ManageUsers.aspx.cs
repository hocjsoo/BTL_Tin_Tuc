using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewsWebsite.App_Code;

namespace NewsWebsite
{
    public partial class ManageUsers : Page
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
            var isAdmin = CurrentRole == "Admin";
            
            pnlAuth.Visible = !isLoggedIn || !isAdmin;
            pnlMain.Visible = isLoggedIn && isAdmin;
            
            if (!isLoggedIn || !isAdmin)
            {
                if (!isLoggedIn)
                    lblInfo.Text = "Vui lòng đăng nhập để truy cập.";
                    else
                        lblInfo.Text = "Chỉ Admin mới có quyền quản lý người dùng.";
                return;
            }

            lblInfo.Text = string.Format("Quản lý người dùng - Đăng nhập: {0} ({1})", CurrentUsername, CurrentRole);
            
            if (!IsPostBack)
            {
                BindGrid();
                // Hide form by default
                pnlAddUserForm.Visible = false;
                ViewState["ShowAddForm"] = false;
            }
            else
            {
                // Restore form visibility from ViewState
                var showForm = ViewState["ShowAddForm"] as bool?;
                if (showForm.HasValue)
                {
                    pnlAddUserForm.Visible = showForm.Value;
                }
            }
        }

        private void BindGrid()
        {
            gvUsers.DataSource = UserManager.GetAll();
            gvUsers.DataBind();
        }

        protected void gvUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvUsers.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var userId = gvUsers.DataKeys[e.Row.RowIndex].Value.ToString();
                var user = UserManager.GetById(userId);
                
                // Prevent deleting yourself or admin accounts
                foreach (Control ctl in e.Row.Cells[e.Row.Cells.Count - 1].Controls)
                {
                    var btn = ctl as LinkButton;
                    if (btn != null && btn.CommandName == "Delete")
                    {
                        if (user != null)
                        {
                            // Don't allow deleting yourself, the default admin account, or other admins
                            if (string.Equals(user.Email, CurrentUsername, StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(userId, "admin", StringComparison.OrdinalIgnoreCase) ||
                                user.Role == "Admin")
                            {
                                btn.Visible = false;
                            }
                            else
                            {
                                btn.OnClientClick = "return confirm('Bạn có chắc muốn xóa tài khoản này?');";
                            }
                        }
                    }
                }
                
                // Set role dropdown value in edit mode and disable for admin account
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    var ddlRole = e.Row.FindControl("ddlRole") as DropDownList;
                    if (ddlRole != null && user != null && !string.IsNullOrEmpty(user.Role))
                    {
                        ddlRole.SelectedValue = user.Role;
                        // Disable role dropdown for admin account
                        if (string.Equals(userId, "admin", StringComparison.OrdinalIgnoreCase))
                        {
                            ddlRole.Enabled = false;
                            ddlRole.ToolTip = "Tài khoản admin mặc định không thể thay đổi vai trò";
                        }
                    }
                }
            }
        }

        protected void gvUsers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvUsers.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvUsers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var userId = gvUsers.DataKeys[e.RowIndex].Value.ToString();
            var user = UserManager.GetById(userId);
            
            if (user == null)
            {
                lblMsg.Text = "Không tìm thấy tài khoản.";
                lblMsg.CssClass = "text-danger";
                return;
            }
            
            // Prevent changing your own role or admin account
            if (string.Equals(user.Email, CurrentUsername, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(userId, "admin", StringComparison.OrdinalIgnoreCase))
            {
                lblMsg.Text = "Bạn không thể thay đổi quyền của chính mình hoặc tài khoản admin mặc định.";
                lblMsg.CssClass = "text-danger";
                gvUsers.EditIndex = -1;
                BindGrid();
                return;
            }
            
            var ddlRole = gvUsers.Rows[e.RowIndex].FindControl("ddlRole") as DropDownList;
            if (ddlRole != null)
            {
                user.Role = ddlRole.SelectedValue;
                if (UserManager.UpdateUser(user))
                {
                    lblMsg.Text = "Đã cập nhật quyền thành công.";
                    lblMsg.CssClass = "text-success";
                }
                else
                {
                    lblMsg.Text = "Lỗi khi cập nhật quyền.";
                    lblMsg.CssClass = "text-danger";
                }
            }
            
            gvUsers.EditIndex = -1;
            BindGrid();
        }

        protected void gvUsers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvUsers.EditIndex = -1;
            BindGrid();
        }

        protected void gvUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var userId = gvUsers.DataKeys[e.RowIndex].Value.ToString();
            var user = UserManager.GetById(userId);
            
            if (user == null)
            {
                lblMsg.Text = "Không tìm thấy tài khoản.";
                lblMsg.CssClass = "text-danger";
                return;
            }
            
            // Prevent deleting yourself, admin account, or other admins
            if (string.Equals(user.Email, CurrentUsername, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(userId, "admin", StringComparison.OrdinalIgnoreCase) ||
                user.Role == "Admin")
            {
                lblMsg.Text = "Bạn không thể xóa tài khoản này.";
                lblMsg.CssClass = "text-danger";
                return;
            }
            
            if (UserManager.DeleteUser(userId))
            {
                lblMsg.Text = "Đã xóa tài khoản thành công.";
                lblMsg.CssClass = "text-success";
            }
            else
            {
                lblMsg.Text = "Lỗi khi xóa tài khoản.";
                lblMsg.CssClass = "text-danger";
            }
            
            BindGrid();
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            Page.Validate("AddUser");
            if (!Page.IsValid)
            {
                return;
            }

            // Check if email already exists
            var existingUser = UserManager.GetByEmail(txtNewEmail.Text.Trim());
            if (existingUser != null)
            {
                lblMsg.Text = "Email này đã được sử dụng. Vui lòng chọn email khác.";
                lblMsg.CssClass = "text-danger";
                return;
            }

            // Validate password length
            if (txtNewPassword.Text.Length < 6)
            {
                lblMsg.Text = "Mật khẩu phải có ít nhất 6 ký tự.";
                lblMsg.CssClass = "text-danger";
                return;
            }

            // Create new user
            var newUser = new UserManager.User
            {
                FullName = txtNewFullName.Text.Trim(),
                Email = txtNewEmail.Text.Trim(),
                Password = txtNewPassword.Text,
                Role = ddlNewRole.SelectedValue,
                Gender = ddlNewGender.SelectedValue,
                CreatedAt = DateTime.UtcNow
            };

            // Parse DateOfBirth if provided
            if (!string.IsNullOrEmpty(txtNewDateOfBirth.Text))
            {
                DateTime dob;
                if (DateTime.TryParse(txtNewDateOfBirth.Text, out dob))
                {
                    newUser.DateOfBirth = dob;
                }
            }

            if (UserManager.Register(newUser))
            {
                lblMsg.Text = string.Format("Đã thêm người dùng '{0}' thành công.", newUser.FullName);
                lblMsg.CssClass = "text-success";
                
                // Clear form
                txtNewFullName.Text = string.Empty;
                txtNewEmail.Text = string.Empty;
                txtNewPassword.Text = string.Empty;
                ddlNewRole.SelectedValue = "Viewer";
                ddlNewGender.SelectedValue = string.Empty;
                txtNewDateOfBirth.Text = string.Empty;
                
                // Hide form after successful add
                pnlAddUserForm.Visible = false;
                ViewState["ShowAddForm"] = false;
                
                // Refresh grid
                BindGrid();
            }
            else
            {
                lblMsg.Text = "Lỗi khi thêm người dùng. Vui lòng thử lại.";
                lblMsg.CssClass = "text-danger";
            }
        }

        protected void btnCancelAdd_Click(object sender, EventArgs e)
        {
            // Clear form
            txtNewFullName.Text = string.Empty;
            txtNewEmail.Text = string.Empty;
            txtNewPassword.Text = string.Empty;
            ddlNewRole.SelectedValue = "Viewer";
            ddlNewGender.SelectedValue = string.Empty;
            txtNewDateOfBirth.Text = string.Empty;
            
            // Hide form
            pnlAddUserForm.Visible = false;
            ViewState["ShowAddForm"] = false;
            lblMsg.Text = string.Empty;
        }

        protected void btnShowAddForm_Click(object sender, EventArgs e)
        {
            // Show form and clear any previous messages
            pnlAddUserForm.Visible = true;
            ViewState["ShowAddForm"] = true;
            lblMsg.Text = string.Empty;
            
            // Clear form fields
            txtNewFullName.Text = string.Empty;
            txtNewEmail.Text = string.Empty;
            txtNewPassword.Text = string.Empty;
            ddlNewRole.SelectedValue = "Viewer";
            ddlNewGender.SelectedValue = string.Empty;
            txtNewDateOfBirth.Text = string.Empty;
        }
    }
}

