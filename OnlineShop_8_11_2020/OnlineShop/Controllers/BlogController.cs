using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using Common;
using OnlineShop.Common;
using System.Xml.Linq;
using System.Net;
using System.Text;

namespace OnlineShop.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        public ActionResult Index()
        {
            return View();
        }

        //View page Read RSS
        public ActionResult ReadRSS()
        {
            return View();
        }
        //Giá trị mặc định type = "tin"--> lấy RSS mục tin
        public ActionResult PostFeed(string type="tin")
        {
            //Lấy chuyên mục
            Category category = new CategoryDao().TakeByAlias(type);
            //Nếu chuyên mục không tồn tại, trả về trang NotFound
            if (category == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Content> posts = new ContentDao().ListAllByCategoryAlias(type);

            var feed = new SyndicationFeed(category.Name, "RSS Feed",
                       new Uri("https://localhost:44394/RSS"),
                       Guid.NewGuid().ToString(),
                       DateTime.Now);
            var items = new List<SyndicationItem>();
            foreach (Content content in posts)
            {
                string postUrl = String.Format("https://localhost:44394/" + content.MetaTitle + "-{0}", content.ID);
                var item =
                    new SyndicationItem(StringHelper.RemoveIllegalCharacters(content.Name),
                        StringHelper.RemoveIllegalCharacters(content.Description),
                        new Uri(postUrl),
                        content.ID.ToString(),
                        content.CreatedDate.Value);
                items.Add(item);
            }
            feed.Items = items;
            return new RSSActionResult { Feed = feed };
        }
        //Read RSS from URL
        [HttpPost]
        public ActionResult ReadRSS(string url)
        {
            WebClient wclient = new WebClient();
            wclient.Encoding = ASCIIEncoding.UTF8;
            string RSSData = wclient.DownloadString(url);

            XDocument xml = XDocument.Parse(RSSData, LoadOptions.PreserveWhitespace);
            var RSSFeedData = (from x in xml.Descendants("item")
                               select new Models.RSSFeed
                               {
                                   Title =((string)x.Element("title")),
                                   Link =((string)x.Element("link")),
                                   Description = ((string)x.Element("description")),
                                   PubDate = ((string)x.Element("pubDate"))
                               });
            ViewBag.RSSFeed = RSSFeedData;
            ViewBag.URL = url;
            return View();
        }

    }
}