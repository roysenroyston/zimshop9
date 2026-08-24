using ShopMate.ModelDto;
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
    public class DeliveryNoteController : BaseController
    {

        string userId = Env.GetUserInfo("name");
        // GET: DeliveryNote
        public ActionResult Index()
        {
            return View();
        }

        //Get: DeliveryNote/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.DeliveryNotes.ToArray();

            var result = from c in tak
                         select new string[]
                {
                    c.Id.ToString(), Convert.ToString(c.Id),
                  
                    Convert.ToString(c.invoiceNo),
                    Convert.ToString(c.OrderNo),
                   
                    Convert.ToString(c.ddate),
                    Convert.ToString(c.CustomerUserId)
                };

            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        //Get: DeliveryNote/DeliveredGetGrid
        public ActionResult DeliveredGetGrid()
        {
            var tak = db.DeliveryNotes.Where(i => i.delivered == true).ToArray();

            var result = from c in tak
                         select new string[]
{
                c.Id.ToString(), Convert.ToString(c.Id),
               
                Convert.ToString(c.invoiceNo),
                Convert.ToString(c.OrderNo),
                Convert.ToString(c.ddate),
                Convert.ToString(c.CustomerUserId)
};

            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        //Get: DeliveryNote/DeliveredGetGrid
        public ActionResult UnDeliveredGetGrid()
        {
            var tak = db.DeliveryNotes.Where(i => i.delivered == false).ToArray();

            var result = from c in tak
                         select new string[]
{
                c.Id.ToString(), Convert.ToString(c.Id),
                Convert.ToString(c.invoiceNo),
                Convert.ToString(c.OrderNo),
                Convert.ToString(c.ddate),
                Convert.ToString(c.CustomerUserId)
};

            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        // GET: /DeliveryNote/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }

        // GET: DeliveryNote/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryNote ObjDeliveryNote = db.DeliveryNotes.Find(id);
            if (ObjDeliveryNote == null)
            {
                return HttpNotFound();
            }
            return View(ObjDeliveryNote);
        }

        // GET: DeliveryNote/Create
        public ActionResult Create()
        {
            ViewBag.CustomerUserId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer"), "Id", "UserName");
            ViewBag.ordernumber = new SelectList(db.Orders, "Id", "UserName");
            return View();
        }

        // POST: DeliveryNote/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
      
        [ValidateInput(false)]
        public ActionResult Create(DeliveryNote objDeliveryNote,DNoteMaterial[] dnotematerial)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! DNote  Is Not Complete!";
            try
            {
                if (ModelState.IsValid)
                {


                    db.DeliveryNotes.Add(objDeliveryNote);

                    foreach (var quoteItems in dnotematerial)
                    {
                        DNoteMaterial Items = new DNoteMaterial();
                        Items.Quantity = quoteItems.Quantity;
                        Items.Description = quoteItems.Description;
                        Items.DNoteId = objDeliveryNote.Id;
                    //    Items.DeliveryNote = objDeliveryNote;
                   
                        db.DNoteMaterials.Add(Items);
                    }

                    db.SaveChanges();

                    result = "Success! Delivery Note Created";
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

        // GET: DeliveryNote/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.CustomerUserId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer"), "Id", "UserName");
            DeliveryNote ObjDeliveryNote = db.DeliveryNotes.Find(id);
            if (ObjDeliveryNote == null)
            {
                return HttpNotFound();
            }
            
            return View(ObjDeliveryNote);
        }

        // POST: DeliveryNote/Edit/5
        [HttpPost]
        public ActionResult Edit(DeliveryNote ObjDeliveryNote)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjDeliveryNote).State = EntityState.Modified;
                    db.SaveChanges();

                    sb.Append("Sumitted");
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

        // GET: DeliveryNote/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryNote ObjDeliveryNote = db.DeliveryNotes.Find(id);
            if (ObjDeliveryNote == null)
            {
                return HttpNotFound();
            }
            return View(ObjDeliveryNote);
        }

        // POST: DeliveryNote/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                DeliveryNote ObjDeliveryNote = db.DeliveryNotes.Find(id);
                db.DeliveryNotes.Remove(ObjDeliveryNote);
                db.SaveChanges();

                sb.Append("Sumitted");
                return Content(sb.ToString());

            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());
        }

        // GET: /DeliveryNote/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            DeliveryNote ObjDeliveryNote = db.DeliveryNotes.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;

            }

            return View(ObjDeliveryNote);
        }

        // POST: /DeliveryNote/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(DeliveryNote ObjDeliveryNote)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjDeliveryNote).State = EntityState.Modified;
                    db.SaveChanges();

                    sb.Append("Sumitted");
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
        public ActionResult print(int id)
        {
            DeliveryNote dnote = db.DeliveryNotes.Find(id);
            var DnoteItems = db.DNoteMaterials.Where(q => q.DNoteId == id).ToArray();

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            //var jobcard = db.JobCards.FirstOrDefault(i => i.Id == id && i.WarehouseId == warehouse);
            var user = db.Users.FirstOrDefault(i => i.Id == dnote.CustomerUserId);

            if (dnote == null)
            {
                return HttpNotFound();
            }

            DNoteDto dto = new DNoteDto();
           
            dto.invoiceNo = dnote.invoiceNo;
            dto.OrderNo = dnote.OrderNo;
            dto.CustomerUser = user.UserName;
            dto.delivered = dnote.delivered;
            dto.CompanyAddress = invoiceFormat.AddressInfo;
            dto.CompanyContact = invoiceFormat.OtherInfo;
            dto.CompanyName = invoiceFormat.CompanyName;
            dto.Id = dnote.Id;
            dto.Logo = invoiceFormat.Logo;
            dto.ToInfo =  user.Address + "<br/> " + user.Mobile + "<br/> " + user.About;

            List<DNoteMaterialDto> itemsList = new List<DNoteMaterialDto>();

            foreach (var items in DnoteItems)
            {
                DNoteMaterialDto itemDto = new DNoteMaterialDto();
                itemDto.Description = items.Description;
                itemDto.Quantity = items.Quantity;
               

                itemsList.Add(itemDto);
            }

            dto.items = itemsList;

            return View(dto);

        }
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
