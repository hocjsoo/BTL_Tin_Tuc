using System;
using System.Web.UI;

namespace NewsWebsite
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

            lblInfo.Text = string.Format("Chào mừng, {0} ({1}) - Quản trị hệ thống", CurrentUsername, CurrentRole);
            
            // Show admin card only for Admin
            pnlAdminCard.Visible = CurrentRole == "Admin";
        }
    }
}


