using System.Web.Mvc;

namespace InsideWordMVCWeb.Controllers
{
    public partial class InfoController : BugFixController
    {
        //
        // GET: /Info/About

        public virtual ActionResult About()
        {
            return View();
        }

        //
        // GET: /Info/Faq

        public virtual ActionResult Faq()
        {
            return View();
        }

        //
        // GET: /Info/ContactUs

        public virtual ActionResult ContactUs()
        {
            return View();
        }

        //
        // GET: /Info/Guidelines

        public virtual ActionResult Guidelines()
        {
            return View();
        }

        //
        // GET: /Info/Privacy

        public virtual ActionResult Privacy()
        {
            return View();
        }

        //
        // GET: /Info/Terms

        public virtual ActionResult Terms()
        {
            return View();
        }


        //
        // GET: /Info/PublishByEmail
        public virtual ActionResult PublishByEmail()
        {
            return View();
        }

        //
        // GET: /Info/Tutorial
        public virtual ActionResult Tutorial()
        {
            return View();
        }
    }
}
