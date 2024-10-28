using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BlogWebsite;

namespace BlogWebsite.Controllers
{
    [Authorize]
    public class BlogsController : Controller
    {
        private BlogEntities db = new BlogEntities();

        // GET: Blogs
        public ActionResult Index()
        {
            var blogs = db.Blogs.Include(b => b.Category);
            return View(blogs.ToList());
        }

        [HttpPost]
        public ActionResult AddComment(string comment, string commenterName, int blogPostId, DateTime datePosted)
        {
            if (ModelState.IsValid)
            {
                var newComment = new Comment
                {
                    CommentText = comment,
                    CommenterName = commenterName,
                    DatePosted = datePosted,
                    BlogPostId = blogPostId
                };

                // Add the comment to the database
                db.Comments.Add(newComment);
                db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }


        // GET: Blogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // GET: Blogs/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Authorname,PostContent,CategoryId,IsPublished")] Blog blog, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                // Set DatePosted to now
                blog.DatePosted = DateTime.Now;

                // Handle file upload
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    ImageUpload.SaveAs(path);
                    blog.ImageUrl = "~/Images/" + fileName; // Save the path to the model
                }
                else
                {
                    blog.ImageUrl = "NoImage";
                }

                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Populate categories if ModelState is not valid
            ViewBag.Categories = new SelectList(db.Categories, "CategoryId", "CategoryName", blog.CategoryId);
            return View(blog);
        }







        // GET: Blogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }

            // Use a unique key name for the ViewBag item to avoid conflicts
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryId", "Name", blog.CategoryId);

            return View(blog);
        }



        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Blog blog, HttpPostedFileBase ImageUrl)
        {
            if (ModelState.IsValid)
            {
                // Find the existing blog post
                var existingBlog = await db.Blogs.AsNoTracking().FirstOrDefaultAsync(b => b.BlogPostId == blog.BlogPostId);
                if (existingBlog != null)
                {
                    // Check if any fields have changed or a new image is uploaded
                    if (existingBlog.Title != blog.Title ||
                        existingBlog.PostContent != blog.PostContent ||
                        existingBlog.AuthorName != blog.AuthorName ||
                        existingBlog.CategoryId != blog.CategoryId ||
                         existingBlog.IsPublished != blog.IsPublished ||
                        (ImageUrl != null && ImageUrl.ContentLength > 0))
                    {
                        // Update the blog properties
                        existingBlog.Title = blog.Title;
                        existingBlog.PostContent = blog.PostContent;
                        existingBlog.AuthorName = blog.AuthorName;
                        existingBlog.CategoryId = blog.CategoryId;
                        existingBlog.IsPublished = blog.IsPublished;
                        existingBlog.IsPublished = blog.IsPublished;

                        // Handle image upload
                        if (ImageUrl != null && ImageUrl.ContentLength > 0)
                        {
                            // Generate a unique filename with date suffix
                            string fileExtension = Path.GetExtension(ImageUrl.FileName);
                            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(ImageUrl.FileName);
                            string dateSuffix = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            string fileName = $"{fileNameWithoutExtension}_{dateSuffix}{fileExtension}";
                            string path = Path.Combine(Server.MapPath("~/Images/"), fileName);

                            // Save the new image
                            ImageUrl.SaveAs(path);

                            // Update the ImageUrl field in the blog
                            existingBlog.ImageUrl = "/Images/" + fileName;
                        }
                        else
                        {
                            // If no new image is uploaded, retain the existing image
                            existingBlog.ImageUrl = existingBlog.ImageUrl; // Retain the current image
                        }

                        // Set the current date if any changes were made
                        existingBlog.DatePosted = DateTime.Now;

                        // Mark the blog as modified and save the changes
                        db.Entry(existingBlog).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        // Redirect back to the blog index page
                        return RedirectToAction("Index");
                    }
                }
            }

            // If the model state is invalid, return to the view
            return View(blog);
        }




        // GET: Blogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = db.Blogs.Find(id);
            db.Blogs.Remove(blog);
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
