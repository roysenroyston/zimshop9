using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;

using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class DispatchController : Controller
    {
        SIContext db = new SIContext();
        string userId = Env.GetUserInfo("name");

        // GET: Dispatch
        public ActionResult Index()
        {
            return View();
        }
        // GET Dispatch/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.Dispatches.ToArray();
            var users = db.Users.ToArray();
            var ware = db.Warehouses.ToArray();
            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.DispatchTo),
            Convert.ToString(c.AddedBy) ,
            //Convert.ToString(users.FirstOrDefault(i=>i.Id==c.AddedBy).UserName) ,
            Convert.ToString(c.invoiceNo),
                             //Convert.ToString(ware.FirstOrDefault(i=>i.Id==c.WarehouseId).Name),
                             Convert.ToString(c.WarehouseId) ,
                             Convert.ToString(c.DateAdded) ,

             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        // GET: Dispatch/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dispatch ObjDispatch = db.Dispatches.Find(id);
            if (ObjDispatch == null)
            {
                return HttpNotFound();
            }
            return View(ObjDispatch);
        }

        // GET: Dispatch/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.Dispatches, "Id", "Name");

            ViewBag.userId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Admin"), "Id", "UserName");
            ViewBag.customerId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer"), "Id", "UserName");
            return View();
        }

        [HttpPost]

        [ValidateInput(false)]
        public ActionResult Create(Dispatch objDispatch, DispatchMaterials[] dispatchmaterials)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Dispatch  Is Not Complete!";
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            try
            {
                if (ModelState.IsValid)
                {
                    objDispatch.DateAdded = DateTime.Now;
                    objDispatch.WarehouseId = warehouse;
                    objDispatch.AddedBy = AddedBy;
                    db.Dispatches.Add(objDispatch);

                    db.SaveChanges();
                    foreach (var quoteItems in dispatchmaterials)
                    {
                        DispatchMaterials Items = new DispatchMaterials();
                      //  {
                             Items.Quantity = quoteItems.Quantity;
                             Items.Description = quoteItems.Description;
                             Items.DispatchId = objDispatch.Id;
                             Items.DateAdded = DateTime.Now;
                             Items.AddedBy = AddedBy;
                             Items.WarehouseId = warehouse;
                        // };
                        db.Dispatchmaterial.Add(Items);
                        db.SaveChanges();

                    }

                    

                    result = "Success! Dispatch Created";
                    return Json(result, JsonRequestBehavior.AllowGet);
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
        // GET: Dispatch/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Dispatch/Edit/5
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

        // GET: Dispatch/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Dispatch/Delete/5
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
        // GET: /DeliveryNote/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            Dispatch ObjDispatch = db.Dispatches.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;

            }

            return View(ObjDispatch);
        }

        // POST: /Dispatch/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       
       
        public ActionResult Print(int id)
        {
            Dispatch dispatch = db.Dispatches.Find(id);
            var DispatchItems = db.Dispatchmaterial.Where(q => q.DispatchId == id).ToArray();

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
          //  var user = db.Users.FirstOrDefault(i => i.Id == dispatch.DispatchTo);

            if (dispatch == null)
            {
                return HttpNotFound();
            }

            DispatchDto dto = new DispatchDto
            {
                InvoiceNo = dispatch.invoiceNo,
                DispatchedTo = dispatch.DispatchTo,
                CompanyAddress = invoiceFormat.AddressInfo,
                CompanyContact = invoiceFormat.OtherInfo,
                CompanyName = invoiceFormat.CompanyName,
                Id = dispatch.Id,
                Logo = invoiceFormat.Logo,
               // ToInfo = user.Address + "<br/> " + user.Mobile + "<br/> " + user.About
            };

            List<DispatchMaterialsDto> itemsList = new List<DispatchMaterialsDto>();

            foreach (var items in DispatchItems)
            {
                DispatchMaterialsDto itemDto = new DispatchMaterialsDto
                {
                    Description = items.Description,
                    Quantity = items.Quantity
                };


                itemsList.Add(itemDto);
            }

            dto.items = itemsList;

            return View(dto);

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
