using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyWeChatPublic.Models;
using System.Threading.Tasks;

namespace MyWeChatPublic.Controllers
{
#if !DEBUG
    [Authorize]
#endif
    public class ArticlesController : Controller
    {
        private WeChatDbContext db = new WeChatDbContext();

        private const int _pageSize = 15;

        // GET: Articles
        public async Task<ActionResult> Index(string criteria, int page = 1)
        {
            try
            {
                var query = from item in db.Articles
                            where (string.IsNullOrEmpty(criteria)
                                || item.Published == criteria
                                || item.Title.Contains(criteria)
                                || item.Tags.Contains(criteria))
                            orderby item.Published descending
                            select item;

                var total= await query.CountAsync();
                ViewBag.Total = total;
                ViewBag.Page = page;
                ViewBag.Count = total / _pageSize + ((total % _pageSize) > 0 ? 1 : 0);
                ViewBag.Criteria = criteria;

                var lst = await query.Skip((page - 1) * _pageSize).Take(_pageSize).ToListAsync();
                return View(lst);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // GET: Articles/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // GET: Articles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Published,Title,Description,PicUrl,Url,Tags")] Article article)
        {
            if (ModelState.IsValid)
            {
                article.Id = Guid.NewGuid();
                db.Articles.Add(article);
                db.SaveChanges();
                return View("CreateSucceed");
            }

            return View(article);
        }

        // GET: Articles/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,Published,Title,Description,PicUrl,Url,Tags")] Article article)
        {
            if (ModelState.IsValid)
            {
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(article);
        }

        // GET: Articles/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
