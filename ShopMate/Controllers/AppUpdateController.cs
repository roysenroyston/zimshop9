using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopMate.Controllers
{
    public class AppUpdateController : Controller
    {




        // This could be fetched from database or web.config for flexibility
        private const int LatestVersionCode = 126;
        private const string ApkFileName = "zipos.apk";
        private const string ApkBaseUrl = "https://zimshop9.zimhope.co.zw/apk/"; // Ensure this is correct and publicly accessible

        [HttpGet]
        public JsonResult CheckVersion()
        {
            var updateInfo = new
            {
                latestVersionCode = LatestVersionCode,
                apkUrl = ApkBaseUrl + ApkFileName,
                releaseNotes = "- Fixed bugs\n- Improved stability\n- New features added"
            };

            return Json(updateInfo, JsonRequestBehavior.AllowGet);
        }
        // GET: AppUpdate
        public ActionResult Index()
        {
            return View();
        }
    }
}