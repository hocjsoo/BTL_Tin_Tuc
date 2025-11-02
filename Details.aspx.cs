using System;
using System.Web.UI;
using NewsManagement.App_Code;

namespace NewsManagement
{
    public partial class Details : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                {
                    id = System.Web.HttpUtility.UrlDecode(id);
                }
                var item = NewsManager.GetById(id);
                if (item == null)
                {
                    pnlDetails.Visible = false;
                    pnlNotFound.Visible = true;
                    return;
                }
                lblTitle.Text = item.Title;
                lblAuthor.Text = item.Author;
                lblDate.Text = item.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                lblSummary.Text = item.Summary;
                litContent.Text = Server.HtmlEncode(item.Content).Replace("\n", "<br/>");
                
                var categoryCtrl = FindControl("detailCategory") as System.Web.UI.HtmlControls.HtmlGenericControl;
                if (categoryCtrl != null) categoryCtrl.InnerText = item.Category ?? "News";
                
                // Display image
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    litImage.Text = string.Format(@"<div class=""mb-4"" style=""background-image: url('{0}'); background-size: cover; background-position: center; min-height: 400px; border-radius: 10px;""></div>", ResolveUrl(item.ImageUrl));
                }
                else
                {
                    litImage.Text = @"<div class=""mb-4"" style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 400px; display: flex; align-items: center; justify-content: center; border-radius: 10px;""><div style=""text-align: center; color: white;""><div style=""font-size: 5rem;"">🔵</div><p style=""margin-top: 20px; font-size: 1.2rem;"">Featured Image</p></div></div>";
                }
            }
        }
    }
}


