using System;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace NewsWebsite
{
    public class UploadImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";
            
            try
            {
                // Check if request has files
                if (context.Request.Files == null || context.Request.Files.Count == 0)
                {
                    context.Response.Write("{\"success\": false, \"message\": \"Không có file được upload.\"}");
                    return;
                }
                
                var file = context.Request.Files[0];
                
                if (file == null || file.ContentLength == 0)
                {
                    context.Response.Write("{\"success\": false, \"message\": \"File rỗng.\"}");
                    return;
                }
                
                // Validate file size (5MB max)
                if (file.ContentLength > 5 * 1024 * 1024)
                {
                    context.Response.Write("{\"success\": false, \"message\": \"File vượt quá 5MB.\"}");
                    return;
                }
                
                // Validate file type
                string fileExt = Path.GetExtension(file.FileName).ToLower();
                if (fileExt != ".jpg" && fileExt != ".jpeg" && fileExt != ".png" && fileExt != ".gif")
                {
                    context.Response.Write("{\"success\": false, \"message\": \"Chỉ chấp nhận file ảnh (JPG, PNG, GIF).\"}");
                    return;
                }
                
                // Create upload directory if not exists
                string uploadDir = HostingEnvironment.MapPath("~/Images/Content/");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }
                
                // Generate unique filename
                string uniqueFileName = Guid.NewGuid().ToString("N") + fileExt;
                string filePath = Path.Combine(uploadDir, uniqueFileName);
                
                // Save file
                file.SaveAs(filePath);
                
                // Return success with image URL
                string imageUrl = "~/Images/Content/" + uniqueFileName;
                string fileName = file.FileName ?? "image" + fileExt;
                
                // Escape JSON string properly
                fileName = fileName.Replace("\\", "\\\\").Replace("\"", "\\\"");
                
                context.Response.Write(string.Format("{{\"success\": true, \"url\": \"{0}\", \"fileName\": \"{1}\"}}", 
                    imageUrl, fileName));
            }
            catch (System.Web.HttpRequestValidationException ex)
            {
                // Handle validation exception
                context.Response.Write(string.Format("{{\"success\": false, \"message\": \"Lỗi validation: {0}\"}}", 
                    ex.Message.Replace("\\", "\\\\").Replace("\"", "\\\"")));
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                System.Diagnostics.Debug.WriteLine("UploadImageHandler Error: " + ex.ToString());
                
                string errorMsg = ex.Message.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");
                context.Response.Write(string.Format("{{\"success\": false, \"message\": \"Lỗi: {0}\"}}", errorMsg));
            }
        }
        
        public bool IsReusable
        {
            get { return false; }
        }
    }
}

