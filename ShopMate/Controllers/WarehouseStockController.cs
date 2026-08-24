using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopMate.Models;
using System.Data;


using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class WarehouseStockController :BaseController
    {
        // GET: WarehouseStock
        public ActionResult Index()
        {
            return View();
        }

        // GET Warehouse/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.WarehouseStocks.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(db.Warehouses.FirstOrDefault(i => i.Id == c.WarehouseId).Name),
            Convert.ToString(c.Product_ProductId.Name),
            Convert.ToString(c.RemainingQuantity),
            //Convert.ToString(c.Email),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: /Warehouse/ModelBindIndex

        private SIContext db = new SIContext();


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