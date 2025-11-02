using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace NewsManagement
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var username = (string)Session["Username"];
            var role = (string)Session["Role"];

            var userInfo = FindControl("userInfo") as HtmlGenericControl;
            var lnkLogin = FindControl("lnkLogin") as HtmlAnchor;
            var lnkLogout = FindControl("lnkLogout") as HtmlAnchor;
            var lnkAdmin = FindControl("lnkAdmin") as HtmlAnchor;

            if (userInfo != null)
            {
                userInfo.InnerText = string.IsNullOrEmpty(username) ? "Khách" : string.Format("{0} ({1})", username, role);
            }
            if (lnkLogin != null) 
            {
                lnkLogin.Visible = string.IsNullOrEmpty(username);
                lnkLogin.Attributes["class"] = lnkLogin.Visible ? "btn-login" : "btn-login d-none";
            }
            if (lnkLogout != null) 
            {
                lnkLogout.Visible = !string.IsNullOrEmpty(username);
                lnkLogout.Attributes["class"] = lnkLogout.Visible ? "btn-login" : "btn-login d-none";
            }
            if (lnkAdmin != null)
            {
                lnkAdmin.Visible = !string.IsNullOrEmpty(username);
                lnkAdmin.Attributes["class"] = lnkAdmin.Visible ? "btn-login" : "btn-login d-none";
            }
        }
    }
}


