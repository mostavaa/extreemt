using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace extreemt.Controllers
{
    public class ProductController : Controller
    {
        private extreemtEntities db = new extreemtEntities();

        //
        // GET: /Product/
        public ActionResult Index(int id = 0)
        {


            int totalRows, page = 1, size, offset, numOfPages;

            if (Request.Params["page"] != null && Request.Params["page"] != "")
            {
                page = int.Parse(Request.Params["page"]);
            }

            ViewBag.categoryId = id;
            var products = new List<product>();
            page--;
            size = 9;
            offset = page * size;
            if (id == 0)
            {
                totalRows = db.products.Count();
                products = db.products.Include(p => p.category).OrderBy(o => o.id).Skip(offset).Take(size).ToList();

            }
            else
            {
                totalRows = db.products.Where(c => c.categoryId == id).Count();
                products = db.products.Where(o => o.categoryId == id).Include(p => p.category).OrderBy(o => o.id).Skip(offset).Take(size).ToList();
            }

            //pagination
            //totalRows 
            //page = 1;


            numOfPages = totalRows / size;
            if (totalRows % size == 0)
            {
                ViewData["numOfPages"] = numOfPages;
            }
            else
            {
                ViewData["numOfPages"] = numOfPages + 1;
            }

            return View(products);
        }

        //
        // GET: /Product/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewData["error"] = TempData["error"];
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //
        // GET: /Product/Create

        public ActionResult Create()
        {
            ViewBag.categoryId = new SelectList(db.categories, "id", "name");

            return View();
        }

        //
        // POST: /Product/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(product product)
        {
            if (ModelState.IsValid)
            {

                user admmin = Account.staticGetLoggedUser();

                if (Account.isAdmin(admmin))
                {
                    product.userId = admmin.id;
                }
                product.date = DateTime.Now;
                db.products.Add(product);

                insertProductPhotos(product.id);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.categoryId = new SelectList(db.categories, "id", "name", product.categoryId);
            return View(product);
        }

        public void insertProductPhotos(int productId)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                if (file.ContentLength > 0)
                {
                    string newfilename = DateTime.Now.ToString("yyyy-MM-dd") + DateTime.Now.Millisecond + file.FileName;
                    string filePath = Server.MapPath("~/Upload/" + newfilename);
                    file.SaveAs(filePath);
                    photo ph = new photo();
                    ph.photo1 = newfilename;
                    ph.productId = productId;
                    db.photos.Add(ph);
                }
            }
        }

        public void deleteProductPhotos(int productId)
        {
            foreach (photo ph in db.photos.Where(p => p.productId == productId))
            {
                db.Entry(ph).State = EntityState.Deleted;
                if (System.IO.File.Exists(ph.photo1))
                {
                    System.IO.File.Delete(ph.photo1);
                }
            }
            db.SaveChanges();
        }
        public ActionResult DeleteAllPhotosEdit(int id = 0)
        {
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            deleteProductPhotos(id);

            return Redirect(Url.Action("Edit", "Product", new { id = id }));
        }
        //
        // GET: /Product/Edit/5

        public ActionResult Edit(int id = 0)
        {
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.categoryId = new SelectList(db.categories, "id", "name", product.categoryId);
            return View(product);
        }

        //
        // POST: /Product/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(product product)
        {
            if (ModelState.IsValid)
            {
                insertProductPhotos(product.id);

                db.Entry(product).State = EntityState.Modified;

                db.Entry(product).Property(x => x.date).IsModified = false;
                db.Entry(product).Property(x => x.userId).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.categoryId = new SelectList(db.categories, "id", "name", product.categoryId);
            return View(product);
        }

        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id = 0)
        {
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //
        // POST: /Product/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            product product = db.products.Find(id);
            db.products.Remove(product);
            deleteProductPhotos(id);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult selectUser()
        {
            string proId = Request.QueryString["proId"];
            ViewData["proId"] = proId;
            return View();
        }
        public ActionResult Buy()
        {
            string wkala = Request.Form["wkala"];
            if (wkala != "1" && wkala != "2" && wkala != "3")
            {
                return null;
            }
            string proId = Request.QueryString["proId"].Replace("25252", "+");
            proId = proId.Replace("25252", "+");
            extreemt.crypt algo = new extreemt.crypt();
            proId = algo.Decrypt(proId);


            extreemtEntities db = new extreemtEntities();
            int _proId = int.Parse(proId);
            if (db.products.Where(o => o.id == _proId).Count() == 0)
            {
                return null;
            }
            user loggedUser = Account.staticGetLoggedUser();
            if (loggedUser != null)
            {
                // it was product by misatake
                int cashBank = loggedUser.cashBank;
                int productCost = db.products.Find(_proId).price;
                if (cashBank >= productCost)
                {
                    userPayProduct up = new userPayProduct();
                    up.productId = _proId;
                    user userRefForActivate = null;
                    if (wkala == "1")
                    {
                        up.userId = loggedUser.id;
                        userRefForActivate = loggedUser;
                    }
                    else if (wkala == "2")
                    {

                        up.userId = db.users.Where(u => u.genNumber == 2 && u.parentGenNum == 1 && u.parentUserId == loggedUser.userId && u.position == "left").First().id;
                        userRefForActivate = db.users.Where(u => u.genNumber == 2 && u.parentGenNum == 1 && u.parentUserId == loggedUser.userId && u.position == "left").First();

                    }
                    else if (wkala == "3")
                    {
                        up.userId = db.users.Where(u => u.genNumber == 3 && u.parentGenNum == 1 && u.parentUserId == loggedUser.userId && u.position == "right").First().id;
                        userRefForActivate = db.users.Where(u => u.genNumber == 3 && u.parentGenNum == 1 && u.parentUserId == loggedUser.userId && u.position == "right").First();

                    }

                    up.date = DateTime.Now;
                    up.price = db.products.Find(_proId).price;
                    up.productName = db.products.Find(_proId).name;
                    db.userPayProducts.Add(up);
                    db.SaveChanges();
                    user user = db.users.Find(loggedUser.id);
                    user.productBank -= productCost;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    this.activateUser(userRefForActivate);
                    return Redirect(Url.Action("Index", "Account"));
                }
                else
                {
                    TempData["error"] = "You Don't Have Enough Cash in Product Bank";
                    return Redirect(Url.Action("Details", "Product", new { id = _proId }));
                }
            }
            else
            {
                return null;
            }
            return null;
        }
        public void activateUser(user user)
        {
            if (user.isActive)
            {
                return;
            }
            List<userPayProduct> userProducts = user.userPayProducts.ToList();
            // check if he buyed a membership and another product 
            bool buyedMembership = false, buyedAnotherProduct = false, active = false;

            int membershipCount = 0;
            foreach (userPayProduct up in userProducts)
            {



                if (up.product.category.name.ToLower().Replace(" ", "").Contains("membership"))
                {
                    membershipCount = membershipCount + (int)up.price;
                    buyedMembership = true;
                }
                else
                {
                    buyedAnotherProduct = true;
                }
                if (buyedMembership && buyedAnotherProduct)
                {
                    if (membershipCount > 350)
                    {
                        active = true;
                        break;
                    }
                }
            }
            if (active)
            {
                extreemtEntities db2 = new extreemtEntities();
                user myUser = db2.users.Find(user.id);
                db2.Entry(myUser).State = EntityState.Modified;
                myUser.isActive = true;
                db2.SaveChanges();
                this.updateActiveParents(myUser);
            }
        }
        private void updateActiveParents(user user)
        {
            extreemtEntities db2 = new extreemtEntities();
            user parent = null;
            while (db2.users.Where(u => u.userId == user.parentUserId && u.genNumber == user.parentGenNum).Count() > 0)
            {
                parent = db2.users.Where(u => u.userId == user.parentUserId && u.genNumber == user.parentGenNum).First();
                if (parent.isActive)
                {
                    db2.Entry(parent).State = EntityState.Modified;
                    if (user.position == "left")
                    {
                        parent.leftActiveCount++;
                        parent.dailyLeftActiveCount++;
                    }
                    else
                    {
                        parent.rightActiveCount++;
                        parent.dailyRightActiveCount++;
                    }
                }
                user = parent;
            }
            db2.SaveChanges();
        }
    }
}