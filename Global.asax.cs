using System;
using System.Web;

namespace NewsWebsite
{
    public partial class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Application["TotalVisits"] = 0;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            try
            {
                var current = (int)(Application["TotalVisits"] ?? 0);
                Application["TotalVisits"] = current + 1;
            }
            finally
            {
                Application.UnLock();
            }
        }
    }
}


