using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace extreemt.Controllers
{
    public class PhotoController : Controller
    {
        private extreemtEntities db = new extreemtEntities();

        //
        // GET: /Photo/

        public ActionResult Index()
        {
            var photos = db.photos.Include(p => p.product);
            return View(photos.ToList());
        }

        //
        // GET: /Photo/Details/5

        public ActionResult Details(int id = 0)
        {
            photo photo = db.photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        //
        // GET: /Photo/Create

        public ActionResult Create()
        {
            ViewBag.productId = new SelectList(db.products, "id", "name");
            return View();
        }

        //
        // POST: /Photo/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(photo photo)
        {
            if (ModelState.IsValid)
            {
                db.photos.Add(photo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.productId = new SelectList(db.products, "id", "name", photo.productId);
            return View(photo);
        }

        //
        // GET: /Photo/Edit/5

        public ActionResult Edit(int id = 0)
        {
            photo photo = db.photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            ViewBag.productId = new SelectList(db.products, "id", "name", photo.productId);
            return View(photo);
        }

        //
        // POST: /Photo/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.productId = new SelectList(db.products, "id", "name", photo.productId);
            return View(photo);
        }

        //
        // GET: /Photo/Delete/5

        public ActionResult Delete(int id = 0)
        {
            photo photo = db.photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        //
        // POST: /Photo/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            photo photo = db.photos.Find(id);
            db.photos.Remove(photo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}