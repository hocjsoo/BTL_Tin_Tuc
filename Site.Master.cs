using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using NewsWebsite.App_Code;

namespace NewsWebsite
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
            var lnkMyAccount = FindControl("lnkMyAccount") as HtmlAnchor;

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
                // Only show Admin link for Admin and Editor roles, not for Viewer
                var canAccessAdmin = !string.IsNullOrEmpty(username) && 
                    (role == "Admin" || role == "Editor");
                lnkAdmin.Visible = canAccessAdmin;
                lnkAdmin.Attributes["class"] = lnkAdmin.Visible ? "btn-login" : "btn-login d-none";
            }
            if (lnkMyAccount != null)
            {
                lnkMyAccount.Visible = !string.IsNullOrEmpty(username);
                lnkMyAccount.Attributes["class"] = lnkMyAccount.Visible ? "btn-login" : "btn-login d-none";
            }
            
            // Mobile menu links
            var lnkMyAccountMobile = FindControl("lnkMyAccountMobile") as HtmlAnchor;
            var lnkAdminMobile = FindControl("lnkAdminMobile") as HtmlAnchor;
            var lnkLoginMobile = FindControl("lnkLoginMobile") as HtmlAnchor;
            var lnkLogoutMobile = FindControl("lnkLogoutMobile") as HtmlAnchor;
            
            if (lnkMyAccountMobile != null)
            {
                lnkMyAccountMobile.Visible = !string.IsNullOrEmpty(username);
                lnkMyAccountMobile.Attributes["class"] = lnkMyAccountMobile.Visible ? "" : "d-none";
            }
            if (lnkAdminMobile != null)
            {
                var canAccessAdmin = !string.IsNullOrEmpty(username) && 
                    (role == "Admin" || role == "Editor");
                lnkAdminMobile.Visible = canAccessAdmin;
                lnkAdminMobile.Attributes["class"] = lnkAdminMobile.Visible ? "" : "d-none";
            }
            if (lnkLoginMobile != null)
            {
                lnkLoginMobile.Visible = string.IsNullOrEmpty(username);
                lnkLoginMobile.Attributes["class"] = lnkLoginMobile.Visible ? "" : "d-none";
            }
            if (lnkLogoutMobile != null)
            {
                lnkLogoutMobile.Visible = !string.IsNullOrEmpty(username);
                lnkLogoutMobile.Attributes["class"] = lnkLogoutMobile.Visible ? "" : "d-none";
            }
            
            // Load categories for desktop dropdown menu
            var categoriesDropdown = FindControl("categoriesDropdown") as HtmlGenericControl;
            if (categoriesDropdown != null)
            {
                var categories = CategoryManager.GetAll().OrderBy(c => c.Name).ToList();
                if (categories.Count > 0)
                {
                    categoriesDropdown.InnerHtml = "";
                    foreach (var cat in categories)
                    {
                        var link = new HtmlAnchor
                        {
                            HRef = string.Format("Category.aspx?cat={0}", System.Web.HttpUtility.UrlEncode(cat.Name)),
                            InnerText = cat.Name
                        };
                        link.Attributes.Add("onclick", "event.stopPropagation();");
                        categoriesDropdown.Controls.Add(link);
                    }
                }
            }
            
            // Load categories for mobile menu
            var mobileMenuCategories = FindControl("mobileMenuCategories") as HtmlGenericControl;
            if (mobileMenuCategories != null)
            {
                var categories = CategoryManager.GetAll().OrderBy(c => c.Name).ToList();
                if (categories.Count > 0)
                {
                    mobileMenuCategories.InnerHtml = "";
                    foreach (var cat in categories)
                    {
                        var link = new HtmlAnchor
                        {
                            HRef = string.Format("Category.aspx?cat={0}", System.Web.HttpUtility.UrlEncode(cat.Name)),
                            InnerText = cat.Name
                        };
                        link.Attributes.Add("onclick", "toggleMobileMenu();");
                        link.Attributes.Add("class", "mobile-menu-link");
                        mobileMenuCategories.Controls.Add(link);
                    }
                }
            }
            
            // Load categories for footer
            var footerCategories = FindControl("footerCategories") as HtmlGenericControl;
            if (footerCategories != null)
            {
                var categories = CategoryManager.GetAll().OrderBy(c => c.Name).ToList();
                if (categories.Count > 0)
                {
                    footerCategories.InnerHtml = "";
                    foreach (var cat in categories)
                    {
                        var link = new HtmlAnchor
                        {
                            HRef = string.Format("Category.aspx?cat={0}", System.Web.HttpUtility.UrlEncode(cat.Name)),
                            InnerText = cat.Name
                        };
                        footerCategories.Controls.Add(link);
                    }
                }
            }
            
            // Handle footer admin link visibility
            var lnkAdminFooter = FindControl("lnkAdminFooter") as HtmlAnchor;
            if (lnkAdminFooter != null)
            {
                var canAccessAdmin = !string.IsNullOrEmpty(username) && 
                    (role == "Admin" || role == "Editor");
                lnkAdminFooter.Visible = canAccessAdmin;
                // Remove d-none class if visible, otherwise it won't render anyway
                if (lnkAdminFooter.Visible)
                {
                    var currentClass = lnkAdminFooter.Attributes["class"] ?? "";
                    if (currentClass.Contains("d-none"))
                    {
                        lnkAdminFooter.Attributes["class"] = currentClass.Replace("d-none", "").Trim();
                    }
                }
            }
        }
    }
}


