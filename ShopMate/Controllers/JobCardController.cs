using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class JobCardController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        // GET: JobCard
        public ActionResult Index()
        {
            return View();
        }

        // GET JobCard/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.JobCards.OrderBy(dat => dat.purchasedate).ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
                         Convert.ToString(c.JobNo),
                         Convert.ToString(c.Description),
                         Convert.ToString(c.customername),
                         Convert.ToString(c.completed),
            Convert.ToString(c.OrderNumber),
            Convert.ToString(c.purchasedate),
            Convert.ToString(c.sandries),
            Convert.ToString(c.TotalAmountWithTax),
            Convert.ToString(c.totalbfvat),
            Convert.ToString(c.VAT)

            };

            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        // GET: /JobCard/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }

        // GET: JobCard/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            JobCard ObjJobCard = db.JobCards.Find(id);
            var jobCardMaterials = db.JobCardMaterials.Where(jm => jm.JobCard.Id == id).ToArray();
            var jobCardServices = db.JobCardServices.Where(js => js.JobCardId == id).ToArray();
            var user = db.Users.FirstOrDefault(i => i.Id == ObjJobCard.customername);

            if (ObjJobCard == null)
            {
                return HttpNotFound();
            }

            JobCardDto job = new JobCardDto();
            job.sandries = ObjJobCard.sandries;
            job.address = ObjJobCard.address;
            job.customername = user.UserName;
            job.Description = ObjJobCard.Description;
            job.JobNo = ObjJobCard.JobNo;
            job.TotalAmountWithTax = ObjJobCard.TotalAmountWithTax;
            job.VAT = ObjJobCard.VAT;
            job.purchasedate = ObjJobCard.purchasedate;
            job.completed = ObjJobCard.completed;

            List<JobcardMaterialsDto> materialsList = new List<JobcardMaterialsDto>();

            foreach (var items in jobCardMaterials)
            {
                JobcardMaterialsDto dto = new JobcardMaterialsDto();
                dto.material = items.material;
                dto.Quantity = items.Quantity;
                dto.price = items.price;

                materialsList.Add(dto);
            }

            List<servicesDto> servicesList = new List<servicesDto>();

            foreach (var item in jobCardServices)
            {
                servicesDto dto = new servicesDto();

                dto.artisan = item.User_UserId.UserName;
                dto.hours = item.hours;
                dto.machineused = item.Machine_MachineId.Name;
                dto.rate = item.rate;

                servicesList.Add(dto);
            }

            job.materialsDtos = materialsList;
            job.services = servicesList;

            return View(job);
        }

        // GET: JobCard/Create
        public ActionResult Create()
        {
            ViewBag.userId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer"), "Id", "UserName");
            ViewBag.artisanId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Artisan"), "Id", "UserName");
            ViewBag.machineId = new SelectList(db.Machines, "Id", "Name");
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(JobCard ObjJobCard, JobCardServices[] jobCardServices, JobCardMaterials[] jobCardMaterials)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! JobCard  Is Not Complete!";
            try
            {
                if (ModelState.IsValid)
                {
                    ObjJobCard.purchasedate = DateTime.Now;

                    db.JobCards.Add(ObjJobCard);
                    db.SaveChanges(userId);

                    foreach (var jbMaterial in jobCardMaterials)
                    {
                        JobCardMaterials objJobCardMaterials = new JobCardMaterials();
                        objJobCardMaterials.material = jbMaterial.material;
                        objJobCardMaterials.Quantity = jbMaterial.Quantity;
                        objJobCardMaterials.price = jbMaterial.price;
                        objJobCardMaterials.JobCard = ObjJobCard;
                        db.JobCardMaterials.Add(objJobCardMaterials);
                        db.SaveChanges(userId);
                    }

                    foreach (var jbServices in jobCardServices)
                    {
                        JobCardServices objJobCardServices = new JobCardServices();
                        objJobCardServices.artisan = jbServices.artisan;
                        objJobCardServices.hours = jbServices.hours;
                        objJobCardServices.JobCardId = ObjJobCard.Id;
                        objJobCardServices.machineused = jbServices.machineused;
                        objJobCardServices.rate = jbServices.rate;
                        db.JobCardServices.Add(objJobCardServices);
                        db.SaveChanges(userId);
                    }


                    result = "Success! JobCard Created";
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

        // POST: /JobCard/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //public ActionResult Create(JobCard ObjJobCard)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    try
        //    {
        //        //string CustomerName = db.Users.FirstOrDefault(i => i.Id == CustomerUserId).UserName;
        //        int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
        //        int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        //        if (ModelState.IsValid)
        //        {


        //            db.JobCards.Add(ObjJobCard);
        //            db.SaveChanges();

        //            sb.Append("Sumitted");
        //            //listitems
        //            //ListItems inv = new ListItems();
        //            //inv.AddedBy = AddedBy;
        //            //inv.DateAdded = DateTime.Now;
        //            //inv.DateModied = DateTime.Now;
        //            //inv.ModifiedBy = AddedBy;
        //            ////inv.Artisan = ArtisanController;
        //            ////inv.machine = Machine;
        //            ////inv.Material;
        //            ////inv.Hours;
        //            ////inv.Price;
        //            ////inv.quantity;
        //            //inv.WarehouseId = warehouse;
        //            //db.listitem.Add(inv);

        //            db.SaveChanges();
        //            return Content(sb.ToString());
        //        }
        //        else
        //        {
        //            foreach (var key in this.ViewData.ModelState.Keys)
        //            {
        //                foreach (var err in this.ViewData.ModelState[key].Errors)
        //                {
        //                    sb.Append(err.ErrorMessage + "<br/>");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sb.Append("Error :" + ex.Message);
        //    }

        //    return Content(sb.ToString());

        //}
        //stilll item list
       
   
        // GET: JobCard/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobCard ObjJobCard = db.JobCards.Find(id);
            ViewBag.userId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer"), "Id", "UserName");
            ViewBag.artisanId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Artisan"), "Id", "UserName");
            ViewBag.machineId = new SelectList(db.Machines, "Id", "Name");
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            if (ObjJobCard == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.JobCards, "Id", "Name", ObjJobCard.Id);

            return View(ObjJobCard);
        }

        // POST: JobCard/Edit/5        
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(JobCard objJobCard)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(objJobCard).State = EntityState.Modified;
                    db.SaveChanges(userId);

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

        // GET: JobCard/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobCard ObjJobCard = db.JobCards.Find(id);
            if (ObjJobCard == null)
            {
                return HttpNotFound();
            }
            return View(ObjJobCard);
        }

        // POST: /JobCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                JobCard ObjJobCard = db.JobCards.Find(id);

                db.JobCards.Remove(ObjJobCard);
                db.SaveChanges(userId);

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

        // GET: /JobCard/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            JobCard ObjJobCard = db.JobCards.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.ParentId = new SelectList(db.JobCards, "Id", "MenuText", ObjJobCard.Id);

            }

            return View(ObjJobCard);
        }

        // POST: /JobCard/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(JobCard ObjJobCard)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjJobCard).State = EntityState.Modified;
                    db.SaveChanges(userId);

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
            JobCard ObjJobCard = db.JobCards.Find(id);
            JobCardServices service = db.JobCardServices.Find(id);
            var jobCardMaterials = db.JobCardMaterials.Where(jm => jm.JobCard.Id == id).ToArray();
            var jobCardServices = db.JobCardServices.Where(js => js.JobCardId == id).ToArray();
            //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            //var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            //var jobcard = db.JobCards.FirstOrDefault(i => i.Id == id && i.WarehouseId == warehouse);
            var user = db.Users.FirstOrDefault(i => i.Id == ObjJobCard.customername);


            if (ObjJobCard == null)
            {
                return HttpNotFound();
            }

            JobCardDto job = new JobCardDto();
            job.sandries = ObjJobCard.sandries;
            job.address = user.Address;
            job.customername = user.UserName;
            job.Description = ObjJobCard.Description;
            job.JobNo = ObjJobCard.JobNo;
            job.TotalAmountWithTax = ObjJobCard.TotalAmountWithTax;
            job.VAT = ObjJobCard.VAT;
            job.purchasedate = ObjJobCard.purchasedate;
            job.completed = ObjJobCard.completed;
            job.totalbfvat = ObjJobCard.totalbfvat;
            job.OrderNumber = ObjJobCard.OrderNumber;
            job.jobcardId = ObjJobCard.Id;
            //job.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;
            //job.CompanyAddress = invoiceFormat.AddressInfo;
            //job.CompanyContact = invoiceFormat.OtherInfo;
            //job.CompanyName = invoiceFormat.CompanyName;
            //job.ToInfo = user.Address + "<br/> " + user.Mobile + "<br/> " + user.About;
            //job.InvoiceFooterText = invoiceFormat.FooterInfo;

            List<JobcardMaterialsDto> materialsList = new List<JobcardMaterialsDto>();

            foreach (var items in jobCardMaterials)
            {
                JobcardMaterialsDto dto = new JobcardMaterialsDto();
                dto.material = items.material;
                dto.Quantity = items.Quantity;
                dto.price = items.price;

                materialsList.Add(dto);
            }

            List<servicesDto> servicesList = new List<servicesDto>();

            foreach (var item in jobCardServices)
            {
                var artis = db.Users.FirstOrDefault(i => i.Id == item.artisan);
                var machine = db.Machines.FirstOrDefault(i => i.Id == item.machineused);
                servicesDto dto = new servicesDto();

                dto.artisan = artis.UserName;
                dto.hours = item.hours;
                dto.machineused = machine.Name;
                dto.rate = item.rate;

                servicesList.Add(dto);
            }

            job.materialsDtos = materialsList;
            job.services = servicesList;

            return View(job);


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
