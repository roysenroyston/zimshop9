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
    public class QuotationController : Controller
    {
        string userId = Env.GetUserInfo("name");
        // GET: Quotation
        public ActionResult Index()
        {
            return View();
        }

        // GET Quotation/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.Quotations.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.AddedBy),
            Convert.ToString(c.IssueDate),
            Convert.ToString(c.SubTotal),
            Convert.ToString(c.VAT),
            Convert.ToString(c.Total),
            Convert.ToString(c.ValidUntil),
          Convert.ToString(c.approved),
          //  Convert.ToString(c.ModifiedBy),               
                                       
                                       
            };

            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Quotation/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }

        // GET: Quotation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Quotation ObjQuotation = db.Quotations.Find(id);
            if (ObjQuotation == null)
            {
                return HttpNotFound();
            }
            return View(ObjQuotation);
        }

        // GET: Quotation/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.Quotations, "Id", "Name");
            ViewBag.PaymentModes = new SelectList(db.Currencies, "id", "Name");
            ViewBag.userId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "SaleMan"), "Id", "UserName");
            ViewBag.CustomerUserId = new SelectList(db.Users, "Id", "UserName");
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            //ViewBag.customerId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer"), "Id", "UserName");
            return View();
        }

        // POST: /Quotation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        //public ActionResult Create(Quotation ObjQuotation, QuotationItems[] QuotationItems)
        //public ActionResult NewInvoice(int customerid, int? orderNo, string vatReg, int ProjectNumber, decimal subtotal, decimal vat, decimal total, decimal payment, decimal balance, int wareId, int PaymentMethodId, InvoiceMaterials[] invoicemat, int? salesrepid)


        public ActionResult Create(int customerId, decimal SubTotal, decimal Total, decimal VAT, int wareId, bool approved , QuotationItems[] QuotationItems)
        {
            // public ActionResult Create(string customername, string jobNo, string address, string Description, string OrderNumber, decimal sandries, decimal totalbfvat, decimal VAT, decimal TotalAmountWithTax, QuotationItems[] QuotationItems, QuotationMaterials[] QuotationMaterials)

            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Quotation  Is Not Complete!";
            Quotation ObjQuotation = new Quotation();
            try
            {
                if (ModelState.IsValid)
                {
                    ObjQuotation.customerId = customerId;
                    ObjQuotation.SubTotal =SubTotal;
                    ObjQuotation.Total = Total;
                    ObjQuotation.VAT = VAT;
                    ObjQuotation.WarehouseId = wareId;
                    ObjQuotation.approved=approved;
                    ObjQuotation.IssueDate = DateTime.Now;
                    ObjQuotation.AddedBy = AddedBy;

                    db.Quotations.Add(ObjQuotation);

                    foreach (var quoteItems in QuotationItems)
                    {
                        QuotationItems objQuotationItems = new QuotationItems();
                        objQuotationItems.ProductId = quoteItems.ProductId;
                        objQuotationItems.Quantity = quoteItems.Quantity;
                        objQuotationItems.Description = quoteItems.Description;
                        objQuotationItems.TotalPrice = quoteItems.TotalPrice;
                        objQuotationItems.QuotationId = ObjQuotation.Id;
                        objQuotationItems.TaxId = db.Products.FirstOrDefault(i => i.Id == quoteItems.ProductId).TaxId;
                        objQuotationItems.UnitPrice = quoteItems.UnitPrice;
                        db.QuotationItems.Add(objQuotationItems);
                        db.SaveChanges(userId);
                    }

                    db.SaveChanges(userId);

                    result = "Submitted";
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

        // GET: Quotation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Quotation ObjQuotation = db.Quotations.Find(id);
            if (ObjQuotation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.Quotations, "Id", "Name", ObjQuotation.Id);

            IEnumerable<QuotationItems> quotation = db.QuotationItems.Where(qt => qt.QuotationId.Equals(ObjQuotation.Id));
            ObjQuotation.items = quotation;
            return View(ObjQuotation);
        }

        // POST: Quotation/Edit/5        
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Quotation objQuotation)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            try
            {
                if (ModelState.IsValid)
                {

                    objQuotation.ModifiedBy = AddedBy;
                    objQuotation.ValidUntil = DateTime.Now;
                    db.Entry(objQuotation).State = EntityState.Modified;
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

        // GET: Quotation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quotation ObjQuotation = db.Quotations.Find(id);
            if (ObjQuotation == null)
            {
                return HttpNotFound();
            }
            return View(ObjQuotation);
        }

        // POST: /Quotation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                Quotation ObjQuotation = db.Quotations.Find(id);
                db.Quotations.Remove(ObjQuotation);
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

        // GET: /Quotation/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            Quotation ObjQuotation = db.Quotations.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.ParentId = new SelectList(db.Quotations, "Id", "MenuText", ObjQuotation.Id);

            }

            return View(ObjQuotation);
        }

        // POST: /Quotation/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(Quotation ObjQuotation)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjQuotation).State = EntityState.Modified;
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
            Quotation ObjQuotation = db.Quotations.Find(id);
            var quotationItems = db.QuotationItems.Where(q => q.QuotationId == id).ToArray();

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            //var jobcard = db.JobCards.FirstOrDefault(i => i.Id == id && i.WarehouseId == warehouse);
            var user = db.Users.FirstOrDefault(i => i.Id == ObjQuotation.AddedBy);
            var customer = db.Users.FirstOrDefault(i => i.Id == ObjQuotation.customerId);

            if (ObjQuotation == null)
            {
                return HttpNotFound();
            }

           QuotationDto dto = new QuotationDto();
          //  dto.AddedBy = user.UserName;
            dto.IssueDate = ObjQuotation.IssueDate;

            dto.SubTotal = ObjQuotation.SubTotal;
            dto.Total = ObjQuotation.Total;
            dto.customer = customer.UserName;
            dto.ValidUntil = ObjQuotation.ValidUntil;
            dto.VAT = ObjQuotation.VAT;

            dto.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;
            dto.CompanyAddress = invoiceFormat.AddressInfo;
            dto.CompanyContact = invoiceFormat.OtherInfo;
            dto.CompanyName = invoiceFormat.CompanyName;
            dto.ToInfo = customer.Address + "<br/> " + customer.Mobile + "<br/> " + customer.About;

            List<QuotationItemsDto> itemsList = new List<QuotationItemsDto>();

            foreach (var items in quotationItems)
            {
                QuotationItemsDto itemDto = new QuotationItemsDto();
                itemDto.Description = items.Description;
                itemDto.Quantity = items.Quantity;
                itemDto.TotalPrice = items.TotalPrice;
                itemDto.UnitPrice = items.UnitPrice;
                itemDto.Name =db.Products.FirstOrDefault(i => i.Id == items.ProductId).Name;
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