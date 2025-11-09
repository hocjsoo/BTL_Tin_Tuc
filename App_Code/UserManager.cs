using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;

namespace NewsWebsite.App_Code
{
    public class UserManager
    {
        private static readonly string XmlPath = HostingEnvironment.MapPath("~/App_Data/Users.xml");

        public class User
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public string Gender { get; set; } // Male, Female, Other
            public DateTime? DateOfBirth { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private static XDocument LoadOrCreate()
        {
            if (string.IsNullOrEmpty(XmlPath)) throw new InvalidOperationException("Cannot resolve App_Data/Users.xml path.");
            if (!File.Exists(XmlPath))
            {
                var docNew = new XDocument(
                    new XElement("Users",
                        // Add default admin user (always protected, ID must be "admin")
                        new XElement("User",
                            new XAttribute("ID", "admin"),
                            new XElement("FullName", "Administrator"),
                            new XElement("Email", "admin@example.com"),
                            new XElement("Password", "password"),
                            new XElement("Role", "Admin"),
                            new XElement("CreatedAt", DateTime.UtcNow.ToString("o"))
                        ),
                        new XElement("User",
                            new XAttribute("ID", "editor"),
                            new XElement("FullName", "Editor"),
                            new XElement("Email", "editor@example.com"),
                            new XElement("Password", "password"),
                            new XElement("Role", "Editor"),
                            new XElement("CreatedAt", DateTime.UtcNow.ToString("o"))
                        )
                    )
                );
                Directory.CreateDirectory(Path.GetDirectoryName(XmlPath));
                docNew.Save(XmlPath);
                return docNew;
            }
            
            // Ensure admin account exists and is correct
            var doc = XDocument.Load(XmlPath);
            var adminUser = doc.Root.Elements("User").FirstOrDefault(e => 
                string.Equals((string)e.Attribute("ID"), "admin", StringComparison.OrdinalIgnoreCase));
            
            if (adminUser == null)
            {
                // Admin account missing, add it
                doc.Root.AddFirst(new XElement("User",
                    new XAttribute("ID", "admin"),
                    new XElement("FullName", "Administrator"),
                    new XElement("Email", "admin@example.com"),
                    new XElement("Password", "password"),
                    new XElement("Role", "Admin"),
                    new XElement("CreatedAt", DateTime.UtcNow.ToString("o"))
                ));
                Save(doc);
            }
            else
            {
                // Ensure admin account has correct ID and Role
                if (!string.Equals((string)adminUser.Attribute("ID"), "admin", StringComparison.OrdinalIgnoreCase))
                {
                    adminUser.Attribute("ID").Value = "admin";
                }
                var roleEl = adminUser.Element("Role");
                if (roleEl == null || roleEl.Value != "Admin")
                {
                    if (roleEl == null)
                        adminUser.Add(new XElement("Role", "Admin"));
                    else
                        roleEl.Value = "Admin";
                    Save(doc);
                }
            }
            
            return doc;
        }

        private static void Save(XDocument doc)
        {
            doc.Save(XmlPath);
        }

        public static User GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("User").FirstOrDefault(e => 
                string.Equals((string)e.Element("Email"), email, StringComparison.OrdinalIgnoreCase));
            return el == null ? null : ToUser(el);
        }

        public static User Authenticate(string email, string password)
        {
            var user = GetByEmail(email);
            if (user != null && user.Password == password)
            {
                return user;
            }
            return null;
        }

        public static bool Register(User user)
        {
            if (user == null) return false;
            var doc = LoadOrCreate();
            
            // Check if email already exists
            if (GetByEmail(user.Email) != null)
            {
                return false; // Email already exists
            }

            var id = Guid.NewGuid().ToString("N");
            user.Id = id;
            if (user.CreatedAt == default(DateTime)) user.CreatedAt = DateTime.UtcNow;
            if (string.IsNullOrEmpty(user.Role)) user.Role = "Viewer"; // Default role for new registrations

            var el = new XElement("User",
                new XAttribute("ID", id),
                new XElement("FullName", user.FullName ?? string.Empty),
                new XElement("Email", user.Email ?? string.Empty),
                new XElement("Password", user.Password ?? string.Empty),
                new XElement("Role", user.Role),
                new XElement("Gender", user.Gender ?? string.Empty),
                new XElement("DateOfBirth", user.DateOfBirth.HasValue ? user.DateOfBirth.Value.ToString("o") : string.Empty),
                new XElement("CreatedAt", user.CreatedAt.ToString("o"))
            );
            doc.Root.Add(el);
            Save(doc);
            return true;
        }

        public static List<User> GetAll()
        {
            var doc = LoadOrCreate();
            return doc.Root.Elements("User").Select(ToUser).ToList();
        }

        public static User GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("User").FirstOrDefault(e => 
                string.Equals((string)e.Attribute("ID"), id, StringComparison.OrdinalIgnoreCase));
            return el == null ? null : ToUser(el);
        }

        public static bool UpdateUser(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Id)) return false;
            
            // Protect admin account - cannot change role or delete
            if (string.Equals(user.Id, "admin", StringComparison.OrdinalIgnoreCase))
            {
                // Admin account role must always remain Admin
                if (user.Role != "Admin")
                {
                    user.Role = "Admin";
                }
            }
            
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("User").FirstOrDefault(e => 
                string.Equals((string)e.Attribute("ID"), user.Id, StringComparison.OrdinalIgnoreCase));
            if (el == null) return false;

            // Update properties
            var fullNameEl = el.Element("FullName");
            if (fullNameEl != null) fullNameEl.Value = user.FullName ?? string.Empty;
            else el.Add(new XElement("FullName", user.FullName ?? string.Empty));

            var emailEl = el.Element("Email");
            if (emailEl != null) emailEl.Value = user.Email ?? string.Empty;
            else el.Add(new XElement("Email", user.Email ?? string.Empty));

            var passwordEl = el.Element("Password");
            if (passwordEl != null && !string.IsNullOrEmpty(user.Password))
                passwordEl.Value = user.Password;
            else if (!string.IsNullOrEmpty(user.Password))
                el.Add(new XElement("Password", user.Password));

            var roleEl = el.Element("Role");
            // Protect admin account role
            if (string.Equals(user.Id, "admin", StringComparison.OrdinalIgnoreCase))
            {
                if (roleEl != null) roleEl.Value = "Admin";
                else el.Add(new XElement("Role", "Admin"));
            }
            else
            {
                if (roleEl != null) roleEl.Value = user.Role ?? "Viewer";
                else el.Add(new XElement("Role", user.Role ?? "Viewer"));
            }

            var genderEl = el.Element("Gender");
            if (genderEl != null) genderEl.Value = user.Gender ?? string.Empty;
            else el.Add(new XElement("Gender", user.Gender ?? string.Empty));

            var dobEl = el.Element("DateOfBirth");
            if (dobEl != null) dobEl.Value = user.DateOfBirth.HasValue ? user.DateOfBirth.Value.ToString("o") : string.Empty;
            else el.Add(new XElement("DateOfBirth", user.DateOfBirth.HasValue ? user.DateOfBirth.Value.ToString("o") : string.Empty));

            Save(doc);
            return true;
        }

        public static bool DeleteUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            
            // Protect admin account - cannot be deleted
            if (string.Equals(id, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("User").FirstOrDefault(e => 
                string.Equals((string)e.Attribute("ID"), id, StringComparison.OrdinalIgnoreCase));
            if (el == null) return false;
            el.Remove();
            Save(doc);
            return true;
        }

        private static User ToUser(XElement el)
        {
            DateTime dt;
            DateTime? dob = null;
            
            var createdAtEl = el.Element("CreatedAt");
            var createdAtStr = createdAtEl != null ? createdAtEl.Value : null;
            var parsed = DateTime.TryParse(createdAtStr, out dt);
            
            var dobEl = el.Element("DateOfBirth");
            if (dobEl != null && !string.IsNullOrWhiteSpace(dobEl.Value))
            {
                DateTime parsedDob;
                if (DateTime.TryParse(dobEl.Value, out parsedDob))
                {
                    dob = parsedDob;
                }
            }
            
            return new User
            {
                Id = (string)el.Attribute("ID"),
                FullName = (string)el.Element("FullName"),
                Email = (string)el.Element("Email"),
                Password = (string)el.Element("Password"),
                Role = (string)el.Element("Role") ?? "Viewer",
                Gender = (string)el.Element("Gender"),
                DateOfBirth = dob,
                CreatedAt = parsed ? dt : DateTime.MinValue
            };
        }
    }
}

