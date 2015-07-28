using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MyWeChatPublic.Models;
using Rabbit.WeiXin.MP.Messages.Response;

namespace MyWeChatPublic.Managers
{
    public class ArticleManager : IDisposable
    {
        WeChatDbContext _db;
        public ArticleManager()
        {
            _db = new WeChatDbContext();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        private static IResponseMessage GetFromTextFile(string fileName)
        {
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + fileName);
            if (File.Exists(filePath))
            {
                return new ResponseMessageText(File.ReadAllText(filePath));
            }
            return new ResponseMessageText("文本信息缺失");
        }

        public IResponseMessage GetWelcome()
        {
            return GetFromTextFile("welcome.txt");
        }

        public IResponseMessage GetAbout()
        {
            return GetFromTextFile("about.txt");
        }

        public IResponseMessage GetHelp()
        {
            return GetFromTextFile("help.txt");
        }

        public IResponseMessage GetTop()
        {
            var wxArticles = new List<ResponseMessageNews.Article>();
            using (var db = new WeChatDbContext())
            {
                var items = from item in db.Articles
                            orderby item.Published descending
                            select item;
                foreach (var item in items.Take(5))
                {
                    var wxArticle = new ResponseMessageNews.Article
                    {
                        Description = item.Description,
                        Title = item.Title,
                        PicUrl = new Uri(item.PicUrl),
                        Url = new Uri(item.Url)
                    };
                    wxArticles.Add(wxArticle);
                }
            }
            return new ResponseMessageNews(wxArticles.ToArray());
        }

        public IResponseMessage GetAllTags()
        {
            var tags = new HashSet<string>();
            using (var db = new WeChatDbContext())
            {
                var query = from item in db.Articles
                            select item.Tags;

                foreach (var item in query)
                {
                    var split = item.Split(' ');
                    foreach (var tag in split)
                    {
                        tags.Add(tag);
                    }
                }
            }
            return new ResponseMessageText(string.Join(" ", tags.ToArray()));
        }

        public IResponseMessage GetByPublishDate(string content)
        {
            var wxArticles = new List<ResponseMessageNews.Article>();
            using (var db = new WeChatDbContext())
            {
                var items = from item in db.Articles
                            where item.Published == content
                            orderby item.Published descending
                            select item;
                if (items.Count() == 0)
                    return new ResponseMessageText("无此日期的文章，请尝试其他");
                foreach (var item in items)
                {
                    var wxArticle = new ResponseMessageNews.Article
                    {
                        Description = item.Description,
                        Title = item.Title,
                        PicUrl = new Uri(item.PicUrl),
                        Url = new Uri(item.Url)
                    };
                    wxArticles.Add(wxArticle);
                }
            }
            return new ResponseMessageNews(wxArticles.ToArray());
        }

        internal IResponseMessage GetByTag(string content)
        {
            var wxArticles = new List<ResponseMessageNews.Article>();
            using (var db = new WeChatDbContext())
            {
                var items = from item in db.Articles
                            where item.Tags.Contains(content)
                            orderby item.Published descending
                            select item;
                if (items.Count() == 0)
                    return null;
                foreach (var item in items.Take(10))
                {
                    var wxArticle = new ResponseMessageNews.Article
                    {
                        Description = item.Description,
                        Title = item.Title,
                        PicUrl = new Uri(item.PicUrl),
                        Url = new Uri(item.Url)
                    };
                    wxArticles.Add(wxArticle);
                }
            }
            return new ResponseMessageNews(wxArticles.ToArray());
        }
    }
}
