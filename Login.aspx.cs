using System;
using System.Web.UI;
using NewsWebsite.App_Code;

namespace NewsWebsite
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure button is properly initialized
            if (!IsPostBack)
            {
                // Make sure login form is visible
                if (btnLogin != null)
                {
                    btnLogin.UseSubmitBehavior = true;
                    btnLogin.CausesValidation = true;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear previous messages
                lblMsg.Text = "";
                lblMsg.Visible = true;

                var email = txtUsername.Text.Trim();
                var password = txtPassword.Text;

                // Validate
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    lblMsg.Text = "Vui lòng nhập đầy đủ email và mật khẩu.";
                    lblMsg.CssClass = "text-danger mt-2 d-block text-center";
                    return;
                }

                // First, try hardcoded admin account (always work, always Admin role)
                if ((email.Equals("admin@example.com", StringComparison.OrdinalIgnoreCase) || email.Equals("admin", StringComparison.OrdinalIgnoreCase)) && password == "password")
                {
                    Session["Username"] = "admin@example.com";
                    Session["Role"] = "Admin"; // Always Admin role for admin account
                    Session["FullName"] = "Administrator";
                    Response.Redirect("Admin.aspx");
                    return;
                }
                if ((email.Equals("editor@example.com", StringComparison.OrdinalIgnoreCase) || email.Equals("editor", StringComparison.OrdinalIgnoreCase)) && password == "password")
                {
                    Session["Username"] = "editor@example.com";
                    Session["Role"] = "Editor";
                    Session["FullName"] = "Editor";
                    Response.Redirect("Admin.aspx");
                    return;
                }

                // Then try to authenticate using UserManager (for registered users)
                try
                {
                    var user = UserManager.Authenticate(email, password);
                    if (user != null)
                    {
                        Session["Username"] = user.Email;
                        Session["Role"] = user.Role;
                        Session["FullName"] = user.FullName ?? user.Email;
                        Response.Redirect("Admin.aspx");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // If UserManager fails (file doesn't exist, etc), just log and continue
                    System.Diagnostics.Debug.WriteLine("UserManager error: " + ex.Message);
                }

                lblMsg.Text = "Sai tài khoản hoặc mật khẩu.";
                lblMsg.CssClass = "text-danger mt-2 d-block text-center";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Lỗi đăng nhập: " + ex.Message;
                lblMsg.CssClass = "text-danger mt-2 d-block text-center";
                lblMsg.Visible = true;
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // Validate all fields for RegisterGroup
            Page.Validate("RegisterGroup");
            if (Page.IsValid)
            {
                var fullName = txtRegFullName.Text.Trim();
                var email = txtRegEmail.Text.Trim();
                var password = txtRegPassword.Text;

                // Check if passwords match
                if (password != txtRegConfirmPassword.Text)
                {
                    lblRegMsg.Text = "Mật khẩu xác nhận không khớp.";
                    lblRegMsg.CssClass = "text-danger mt-2 d-block text-center";
                    return;
                }

                // Check if email already exists
                var existingUser = UserManager.GetByEmail(email);
                if (existingUser != null)
                {
                    lblRegMsg.Text = "Email này đã được sử dụng. Vui lòng chọn email khác.";
                    lblRegMsg.CssClass = "text-danger mt-2 d-block text-center";
                    return;
                }

                // Create new user
                var newUser = new UserManager.User
                {
                    FullName = fullName,
                    Email = email,
                    Password = password,
                    Role = "Viewer", // Default role for new users (requires admin to grant Editor role)
                    CreatedAt = DateTime.UtcNow
                };

                if (UserManager.Register(newUser))
                {
                    lblRegMsg.Text = "Đăng ký thành công! Vui lòng chuyển sang tab Đăng nhập để đăng nhập.";
                    lblRegMsg.CssClass = "text-success mt-2 d-block text-center";
                    
                    // Auto fill login form
                    txtUsername.Text = email;
                }
                else
                {
                    lblRegMsg.Text = "Đăng ký thất bại. Vui lòng thử lại.";
                    lblRegMsg.CssClass = "text-danger mt-2 d-block text-center";
                }
            }
        }
    }
}


