using System.Linq;
using System.Web.Mvc;
using BlogWebsite.Models; // Assuming your Blog model and DbContext are in the Models namespace

namespace BlogWebsite.Controllers
{
    public class HomeController : Controller
    {
        private BlogEntities db = new BlogEntities(); // Assuming BlogEntities is your DbContext class

        public ActionResult Index()
        {
            // Fetch all blogs from the database
            var blogs = db.Blogs.ToList();

            // Fetch all categories from the database
            var categories = db.Categories.ToList(); // Assuming you have a Categories DbSet in your context

            if (blogs == null || !blogs.Any())
            {
                // Optionally, you can log or handle this case
                ViewBag.Message = "No blog posts available.";
            }

            // Pass both blogs and categories to the view
            ViewBag.Blogs = blogs;
            ViewBag.Categories = categories;

            return View(blogs);
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
