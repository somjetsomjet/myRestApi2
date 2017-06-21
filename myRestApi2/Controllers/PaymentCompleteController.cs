using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myRestApi2.Controllers
{
    public class PaymentCompleteController : Controller
    {
        // GET: PaymentComplete
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Error(string error)
		{
			ViewBag.error = error;

			return View("Error");
		}
	}
}