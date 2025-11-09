using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewsWebsite.App_Code;

namespace NewsWebsite
{
    public partial class MyAccount : Page
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
            
            if (!isLoggedIn)
            {
                return;
            }

            if (!IsPostBack)
            {
                LoadUserInfo();
            }
        }

        private void LoadUserInfo()
        {
            var email = CurrentUsername;
            var user = UserManager.GetByEmail(email);
            
            if (user != null)
            {
                lblInfo.Text = string.Format("Quản lý tài khoản của bạn - {0} ({1})", user.FullName ?? email, CurrentRole);
                
                txtFullName.Text = user.FullName ?? string.Empty;
                lblEmail.Text = user.Email ?? string.Empty;
                lblRole.Text = user.Role ?? "Viewer";
                lblCreatedAt.Text = user.CreatedAt != default(DateTime) 
                    ? user.CreatedAt.ToString("dd/MM/yyyy HH:mm") 
                    : "N/A";
                
                // Show personal info panel only if not Admin
                pnlPersonalInfo.Visible = CurrentRole != "Admin";
                
                if (CurrentRole != "Admin")
                {
                    // Load Gender
                    if (ddlGender != null)
                    {
                        if (!string.IsNullOrEmpty(user.Gender))
                        {
                            ddlGender.SelectedValue = user.Gender;
                        }
                        else
                        {
                            ddlGender.SelectedValue = string.Empty;
                        }
                    }
                    
                    // Load DateOfBirth
                    if (txtDateOfBirth != null)
                    {
                        if (user.DateOfBirth.HasValue)
                        {
                            // Convert to local date and format for HTML5 date input (YYYY-MM-DD)
                            txtDateOfBirth.Text = user.DateOfBirth.Value.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            txtDateOfBirth.Text = string.Empty;
                        }
                    }
                }
            }
            else
            {
                lblMsg.Text = "Không tìm thấy thông tin tài khoản.";
                lblMsg.CssClass = "text-danger";
            }
        }

        protected void btnUpdateInfo_Click(object sender, EventArgs e)
        {
            Page.Validate("UpdateAccount");
            if (!Page.IsValid)
            {
                return;
            }

            var email = CurrentUsername;
            var user = UserManager.GetByEmail(email);
            
            if (user == null)
            {
                lblMsg.Text = "Không tìm thấy tài khoản.";
                lblMsg.CssClass = "text-danger";
                return;
            }

            // Update full name only (email and role cannot be changed by user)
            user.FullName = txtFullName.Text.Trim();
            
            // Update Gender and DateOfBirth only if not Admin
            if (CurrentRole != "Admin")
            {
                if (ddlGender != null)
                {
                    user.Gender = ddlGender.SelectedValue;
                }
                
                if (txtDateOfBirth != null && !string.IsNullOrEmpty(txtDateOfBirth.Text))
                {
                    DateTime dob;
                    if (DateTime.TryParse(txtDateOfBirth.Text, out dob))
                    {
                        user.DateOfBirth = dob;
                    }
                    else
                    {
                        user.DateOfBirth = null;
                    }
                }
                else
                {
                    user.DateOfBirth = null;
                }
            }
            
            if (UserManager.UpdateUser(user))
            {
                Session["FullName"] = user.FullName;
                lblMsg.Text = "Đã cập nhật thông tin thành công.";
                lblMsg.CssClass = "text-success";
                LoadUserInfo();
            }
            else
            {
                lblMsg.Text = "Lỗi khi cập nhật thông tin.";
                lblMsg.CssClass = "text-danger";
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            Page.Validate("ChangePassword");
            if (!Page.IsValid)
            {
                return;
            }

            var email = CurrentUsername;
            var user = UserManager.GetByEmail(email);
            
            if (user == null)
            {
                lblMsg.Text = "Không tìm thấy tài khoản.";
                lblMsg.CssClass = "text-danger";
                return;
            }

            // Verify current password
            if (user.Password != txtCurrentPassword.Text)
            {
                lblMsg.Text = "Mật khẩu hiện tại không đúng.";
                lblMsg.CssClass = "text-danger";
                return;
            }

            // Update password
            user.Password = txtNewPassword.Text;
            
            if (UserManager.UpdateUser(user))
            {
                lblMsg.Text = "Đã đổi mật khẩu thành công.";
                lblMsg.CssClass = "text-success";
                
                // Clear password fields
                txtCurrentPassword.Text = string.Empty;
                txtNewPassword.Text = string.Empty;
                txtConfirmPassword.Text = string.Empty;
            }
            else
            {
                lblMsg.Text = "Lỗi khi đổi mật khẩu.";
                lblMsg.CssClass = "text-danger";
            }
        }
    }
}

