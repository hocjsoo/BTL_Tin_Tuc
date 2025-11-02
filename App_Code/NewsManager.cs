using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;

namespace NewsManagement.App_Code
{
    public class NewsManager
    {
        private static readonly string XmlPath = HostingEnvironment.MapPath("~/App_Data/News.xml");

        public class NewsItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Summary { get; set; }
            public string Content { get; set; }
            public string Author { get; set; }
            public string Role { get; set; }
            public string Category { get; set; }
            public string ImageUrl { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private static XDocument LoadOrCreate()
        {
            if (string.IsNullOrEmpty(XmlPath)) throw new InvalidOperationException("Cannot resolve App_Data/News.xml path.");
            if (!File.Exists(XmlPath))
            {
                var docNew = new XDocument(new XElement("News"));
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

        public static IEnumerable<NewsItem> GetAll()
        {
            var doc = LoadOrCreate();
            // Only return articles that have valid ID (from attribute)
            return doc.Root.Elements("Article")
                .Where(el => !string.IsNullOrEmpty((string)el.Attribute("ID")))
                .Select(ToNewsItem)
                .Where(n => !string.IsNullOrEmpty(n.Id)) // Double check ID is not null
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public static NewsItem GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("Article").FirstOrDefault(e => (string)e.Attribute("ID") == id);
            return el == null ? null : ToNewsItem(el);
        }

        public static IEnumerable<NewsItem> Search(string keyword)
        {
            keyword = (keyword ?? string.Empty).Trim();
            if (keyword.Length == 0) return GetAll();
            var lower = keyword.ToLowerInvariant();
            return GetAll().Where(n =>
                (n.Title ?? string.Empty).ToLowerInvariant().Contains(lower) ||
                (n.Summary ?? string.Empty).ToLowerInvariant().Contains(lower) ||
                (n.Content ?? string.Empty).ToLowerInvariant().Contains(lower));
        }

        public static IEnumerable<NewsItem> GetByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return GetAll();
            var lower = category.ToLowerInvariant();
            return GetAll().Where(n => string.Equals(n.Category ?? string.Empty, category, StringComparison.OrdinalIgnoreCase));
        }

        public static string Create(NewsItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            var doc = LoadOrCreate();
            var id = Guid.NewGuid().ToString("N");
            item.Id = id;
            if (item.CreatedAt == default(DateTime)) item.CreatedAt = DateTime.UtcNow;

            var el = new XElement("Article",
                new XAttribute("ID", item.Id),
                new XElement("Title", item.Title ?? string.Empty),
                new XElement("Summary", item.Summary ?? string.Empty),
                new XElement("Content", item.Content ?? string.Empty),
                new XElement("Author", item.Author ?? string.Empty),
                new XElement("Role", item.Role ?? string.Empty),
                new XElement("Category", item.Category ?? string.Empty),
                new XElement("ImageUrl", item.ImageUrl ?? string.Empty),
                new XElement("CreatedAt", item.CreatedAt.ToString("o"))
            );
            doc.Root.Add(el);
            Save(doc);
            return id;
        }

        public static bool Update(NewsItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id)) return false;
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("Article").FirstOrDefault(e => (string)e.Attribute("ID") == item.Id);
            if (el == null) return false;
            var t = el.Element("Title"); if (t != null) t.SetValue(item.Title ?? string.Empty);
            var s = el.Element("Summary"); if (s != null) s.SetValue(item.Summary ?? string.Empty);
            var c = el.Element("Content"); if (c != null) c.SetValue(item.Content ?? string.Empty);
            var a = el.Element("Author"); if (a != null) a.SetValue(item.Author ?? string.Empty);
            var r = el.Element("Role"); if (r != null) r.SetValue(item.Role ?? string.Empty);
            var cat = el.Element("Category"); if (cat != null) cat.SetValue(item.Category ?? string.Empty);
            else el.Add(new XElement("Category", item.Category ?? string.Empty));
            var img = el.Element("ImageUrl"); if (img != null) img.SetValue(item.ImageUrl ?? string.Empty);
            else el.Add(new XElement("ImageUrl", item.ImageUrl ?? string.Empty));
            var createdAtEl = el.Element("CreatedAt");
            DateTime existing;
            if (createdAtEl != null && DateTime.TryParse(createdAtEl.Value, out existing))
            {
                var toWrite = item.CreatedAt == default(DateTime) ? existing : item.CreatedAt;
                createdAtEl.SetValue(toWrite.ToString("o"));
            }
            Save(doc);
            return true;
        }

        public static bool Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            var doc = LoadOrCreate();
            var el = doc.Root.Elements("Article").FirstOrDefault(e => (string)e.Attribute("ID") == id);
            if (el == null) return false;
            el.Remove();
            Save(doc);
            return true;
        }

        private static NewsItem ToNewsItem(XElement el)
        {
            DateTime dt;
            var createdAtEl = el.Element("CreatedAt");
            var createdAtStr = createdAtEl != null ? createdAtEl.Value : null;
            var parsed = DateTime.TryParse(createdAtStr, out dt);
            return new NewsItem
            {
                Id = (string)el.Attribute("ID"),
                Title = (string)el.Element("Title"),
                Summary = (string)el.Element("Summary"),
                Content = (string)el.Element("Content"),
                Author = (string)el.Element("Author"),
                Role = (string)el.Element("Role"),
                Category = (string)el.Element("Category"),
                ImageUrl = (string)el.Element("ImageUrl"),
                CreatedAt = parsed ? dt : DateTime.MinValue
            };
        }
    }
}


