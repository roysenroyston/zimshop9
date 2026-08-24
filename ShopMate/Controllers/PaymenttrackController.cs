using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class PaymenttrackController : Controller
    {
        // GET: Paymenttrack
        public ActionResult Index()
        {
            return View();
        }

        // GET: Paymenttrack/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Paymenttrack/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Paymenttrack/Create
        [HttpPost]
        public ActionResult Create(Paymenttrack tr)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }

        // GET: Paymenttrack/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Paymenttrack/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }

        // GET: Paymenttrack/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Paymenttrack/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }
    }
}
