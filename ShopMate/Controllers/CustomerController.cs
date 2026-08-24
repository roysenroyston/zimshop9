using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class CustomerController : Controller
    {

        string userId = Env.GetUserInfo("name");
        int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        private SIContext db = new SIContext();
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: Currencies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //  [ValidateAntiForgeryToken]

        public ActionResult Create(Customers ObjCustomer)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Customer not added! The Customer already exist.";
            var tak = db.Customers.Where(i => i.BuyerRegisterName == ObjCustomer.BuyerRegisterName && i.WarehouseId== warehouse).FirstOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (tak != null)
                    {
                        // sb.Append("Cu Already Exists ");
                        result = "Customer Already Exists";
                        //  return Content(sb.ToString());
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjCustomer.WarehouseId = warehouse;
                        ObjCustomer.JoinedDate = DateTime.Now;
                        db.Customers.Add(ObjCustomer);
                        db.SaveChanges(userId);

                        // sb.Append("Submitted");
                        result = "Submitted";

                        return Json(result, JsonRequestBehavior.AllowGet);
                        //return View(result);

                    }


                    // sb.Append("Sumitted");
                    //  return Content(sb.ToString());
                }
                else
                {
                    foreach (var key in this.ViewData.ModelState.Keys)
                    {
                        foreach (var err in this.ViewData.ModelState[key].Errors)
                        {
                            sb.Append(err.ErrorMessage + "<br/>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());

        }

        // GET: Currencies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: Currencies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(Customers ObjCustomer)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            try
            {
                //  List<ProductStock> stock = db.ProductStocks.Where(i=>i.Id==ObjProduct.ProductStock_ProductIds);
                if (ModelState.IsValid)
                {

                    ObjCustomer.WarehouseId = warehouse;
                    //ObjProduct.RemainingQuantity = ObjProduct.RemainingQuantity;
                    db.Entry(ObjCustomer).State = EntityState.Modified;
                    db.SaveChanges();


                    sb.Append("Save ");






                    //end
                    return Content(sb.ToString());
                }
                else
                {
                    foreach (var key in this.ViewData.ModelState.Keys)
                    {
                        foreach (var err in this.ViewData.ModelState[key].Errors)
                        {
                            sb.Append(err.ErrorMessage + "<br/>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }


            return Content(sb.ToString());

        }

        public ActionResult GetGrid()
        {
            var tak = db.Customers.ToArray();

            var result = from c in tak.Where(k => k.WarehouseId == warehouse)
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.BuyerRegisterName),
           Convert.ToString(c.VATNumber),
               Convert.ToString(c.BuyerTIN),
           Convert.ToString(c.Email),
           $"{c.HouseNo} {c.Street} {c.City}"
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
    }
}