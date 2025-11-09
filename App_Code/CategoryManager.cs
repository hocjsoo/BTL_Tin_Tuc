using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;

namespace NewsWebsite.App_Code
{
    public class CategoryManager
    {
        private static readonly string XmlPath = HostingEnvironment.MapPath("~/App_Data/Categories.xml");

        public class CategoryItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private static XDocument LoadOrCreate()
        {
            if (string.IsNullOrEmpty(XmlPath)) throw new InvalidOperationException("Cannot resolve App_Data/Categories.xml path.");
            if (!File.Exists(XmlPath))
            {
                var docNew = new XDocument(new XElement("Categories"));
                // Add default categories
                var root = docNew.Root;
                root.Add(new XElement("Category", 
                    new XAttribute("ID", Guid.NewGuid().ToString("N")),
                    new XElement("Name", "Công nghệ"),
                    new XElement("Description", "Tin tức về công nghệ"),
                    new XElement("CreatedAt", DateTime.UtcNow.ToString("o"))
                ));
                root.Add(new XElement("Category", 
                    new XAttribute("ID", Guid.NewGuid().ToString("N")),
                    new XElement("Name", "Kinh tế"),
                    new XElement("Description", "Tin tức về kinh tế"),
                    new XElement("CreatedAt", DateTime.UtcNow.ToString("o"))
                ));
                root.Add(new XElement("Category", 
                    new XAttribute("ID", Guid.NewGuid().ToString("N")),
                    new XElement("Name", "Du lịch"),
                    new XElement("Description", "Tin tức về du lịch"),
                    new XElement("CreatedAt", DateTime.UtcNow.ToString("o"))
                ));
                root.Add(new XElement("Category", 
                    new XAttribute("ID", Guid.NewGuid().ToString("N")),
                    new XElement("Name", "Sức khỏe"),
                    new XElement("Description", "Tin tức về sức khỏe"),
                    new XElement("CreatedAt", DateTime.UtcNow.ToString("o"))
                ));
                root.Add(new XElement("Category", 
                    new XAttribute("ID", Guid.NewGuid().ToString("N")),
                    new XElement("Name", "Văn hóa"),
                    new XElement("Description", "Tin tức về văn hóa"),
                    new XElement("CreatedAt", DateTime.UtcNow.ToString("o"))
                ));
                root.Add(new XElement("Category", 
                    new XAttribute("ID", Guid.NewGuid().ToString("N")),
                    new XElement("Name", "Ẩm thực"),
                    new XElement("Description", "Tin tức về ẩm thực"),
                    new XElement("CreatedAt", DateTime.UtcNow.ToString("o"))
                ));
                Directory.CreateDirectory(Path.GetDirectoryName(XmlPath));
                docNew.Save(XmlPath);
                return docNew;
            }
            return XDocument.Load(XmlPath);
        }

        private static void Save(XDocument doc)
        {
            doc.Save(XmlPath);
        }

        private static CategoryItem ToCategoryItem(XElement el)
        {
            if (el == null) return null;
            var id = (string)el.Attribute("ID");
            if (string.IsNullOrEmpty(id)) return null;
            
            DateTime createdAt = DateTime.UtcNow;
            var createdAtEl = el.Element("CreatedAt");
            if (createdAtEl != null && !string.IsNullOrEmpty(createdAtEl.Value))
            {
                DateTime.TryParse(createdAtEl.Value, out createdAt);
            }
            
            var nameEl = el.Element("Name");
            var descEl = el.Element("Description");
            
            return new CategoryItem
            {
                Id = id,
                Name = nameEl != null ? nameEl.Value : string.Empty,
                Description = descEl != null ? descEl.Value : string.Empty,
                CreatedAt = createdAt == default(DateTime) ? DateTime.UtcNow : createdAt
            };
        }

        public static IEnumerable<CategoryItem> GetAll()
        {
            var doc = LoadOrCreate();
            return doc.Root.Elements("Category")
                .Where(el => !string.IsNullOrEmpty((string)el.Attribute("ID")))
                .Select(ToCategoryItem)
                .Where(c => c != null && !string.IsNullOrEmpty(c.Id))
                .OrderBy(c => c.Name)
                .ToList();
        }

        public static CategoryItem GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("Category").FirstOrDefault(e => (string)e.Attribute("ID") == id);
            return el == null ? null : ToCategoryItem(el);
        }

        public static CategoryItem GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("Category")
                .FirstOrDefault(e => 
                {
                    var nameEl = e.Element("Name");
                    return nameEl != null && string.Equals(nameEl.Value, name, StringComparison.OrdinalIgnoreCase);
                });
            return el == null ? null : ToCategoryItem(el);
        }

        public static string Create(CategoryItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (string.IsNullOrWhiteSpace(item.Name)) throw new ArgumentException("Category name is required.");
            
            // Check if category with same name already exists
            if (GetByName(item.Name) != null)
            {
                throw new InvalidOperationException("Category with this name already exists.");
            }
            
            var doc = LoadOrCreate();
            var id = Guid.NewGuid().ToString("N");
            item.Id = id;
            if (item.CreatedAt == default(DateTime)) item.CreatedAt = DateTime.UtcNow;

            var el = new XElement("Category",
                new XAttribute("ID", id),
                new XElement("Name", item.Name ?? string.Empty),
                new XElement("Description", item.Description ?? string.Empty),
                new XElement("CreatedAt", item.CreatedAt.ToString("o"))
            );
            doc.Root.Add(el);
            Save(doc);
            return id;
        }

        public static bool Update(CategoryItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id)) return false;
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("Category").FirstOrDefault(e => (string)e.Attribute("ID") == item.Id);
            if (el == null) return false;
            
            // Check if another category with same name exists
            var existingWithSameName = doc.Root.Elements("Category")
                .FirstOrDefault(e => 
                {
                    var attrId = (string)e.Attribute("ID");
                    var nameElement = e.Element("Name");
                    return attrId != item.Id && nameElement != null && 
                        string.Equals(nameElement.Value, item.Name, StringComparison.OrdinalIgnoreCase);
                });
            if (existingWithSameName != null)
            {
                return false; // Duplicate name
            }
            
            var nameEl = el.Element("Name");
            if (nameEl != null) nameEl.SetValue(item.Name ?? string.Empty);
            else el.Add(new XElement("Name", item.Name ?? string.Empty));
            
            var descEl = el.Element("Description");
            if (descEl != null) descEl.SetValue(item.Description ?? string.Empty);
            else el.Add(new XElement("Description", item.Description ?? string.Empty));
            
            Save(doc);
            return true;
        }

        public static bool Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("Category").FirstOrDefault(e => (string)e.Attribute("ID") == id);
            if (el == null) return false;
            el.Remove();
            Save(doc);
            return true;
        }

        public static List<string> GetAllNames()
        {
            return GetAll().Select(c => c.Name).OrderBy(n => n).ToList();
        }
    }
}

