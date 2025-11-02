using System;
using System.Web.UI;

namespace NewsManagement
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            // New email-based login
            if ((username.Equals("admin@example.com", StringComparison.OrdinalIgnoreCase) || username.Equals("admin", StringComparison.OrdinalIgnoreCase)) && password == "password")
            {
                Session["Username"] = "admin";
                Session["Role"] = "Admin";
                Response.Redirect("Admin.aspx");
                return;
            }
            if ((username.Equals("editor@example.com", StringComparison.OrdinalIgnoreCase) || username.Equals("editor", StringComparison.OrdinalIgnoreCase)) && password == "password")
            {
                Session["Username"] = "editor";
                Session["Role"] = "Editor";
                Response.Redirect("Admin.aspx");
                return;
            }
            // Legacy support
            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase) && password == "admin123")
            {
                Session["Username"] = username;
                Session["Role"] = "Admin";
                Response.Redirect("Admin.aspx");
                return;
            }
            if (username.Equals("editor", StringComparison.OrdinalIgnoreCase) && password == "editor123")
            {
                Session["Username"] = username;
                Session["Role"] = "Editor";
                Response.Redirect("Admin.aspx");
                return;
            }
            lblMsg.Text = "Sai tài khoản hoặc mật khẩu.";
        }
    }
}


