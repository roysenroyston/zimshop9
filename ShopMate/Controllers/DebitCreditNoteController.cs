using Newtonsoft.Json;
using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class DebitCreditNoteController : Controller
    {
        int userId = int.Parse(Env.GetUserInfo("userid"));
        int WarehouseId = int.Parse(Env.GetUserInfo("WarehouseId"));
        private SIContext db = new SIContext();
        // GET: DebitCreditNote
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products.Where(m => m.WarehouseId == WarehouseId), "Id", "Name");
            ViewBag.ReceiptId = new SelectList(db.Sales.Where(j => j.isFiscalised == true&&j.WarehouseId == WarehouseId), "Id", "zimraReceiptNo");
            ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(K => K.Id == WarehouseId), "Id", "Name");

            return View();
        }
        public ActionResult GetQuotationItems(int? id)
        {


            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var sale = db.Sales.FirstOrDefault(j => j.zimraReceiptNo == id);
                if (sale == null)
                {
                    return HttpNotFound();
                }

                var receiptNumber = sale.recieptNumber;

                //     var invoices = db.Invoices
                //   .Where(k => k.InvoiceNo == receiptNumber && k.WarehouseId == WarehousId)
                //   .ToArray();

                var ObjQuotationtems = db.Sales.Where(i => i.zimraReceiptNo == (id)).ToArray();
                var objQty = db.InformalInvoices.FirstOrDefault(m => m.InvoiceNo == receiptNumber);

                List<Invoice> lstQuotation = new List<Invoice>();
                Invoice materials = new Invoice();
                materials.Id = objQty.Id;
                //materials.re = objQty.Remarks;
                materials.vat = objQty.vat;
                materials.total = objQty.total;
                materials.subtotal = objQty.subtotal;
                //materials.FinishedGoodsQuantity = objQty.FinishedGoodsQuantity;
                lstQuotation.Add(materials);


                List<Sale> lstQuotationItem = new List<Sale>();
                foreach (var item in ObjQuotationtems)
                {
                    //  var productname = db.Products.FirstOrDefault(j=> j.Id== item.ProductId)
                    Sale QuotationItem = new Sale();
                    QuotationItem.ProductId = item.ProductId;
                    QuotationItem.Quantity = item.Quantity;
                    QuotationItem.CustomerName = item.Product_ProductId.Name;
                    QuotationItem.SalePrice = item.SalePrice;
                    QuotationItem.TotalAmount = item.TotalAmount;
                    QuotationItem.TotalAmountWithTax = item.TotalAmountWithTax;
                    QuotationItem.zimraReceiptNo = item.zimraReceiptNo;
                    QuotationItem.recieptNumber = item.recieptNumber;
                    //QuotationItem.totalAmountWithTax = item.TotalAmountWithTax;
                    //QuotationItem.saleOrderId = item.SaleOrderId;
                    //   QuotationItem.Name = db.Products.FirstOrDefault(i => i.Id == item.ProductId).Name;
                    lstQuotationItem.Add(QuotationItem);
                }
                if (ObjQuotationtems == null)
                {
                    return HttpNotFound();
                }
                var result = JsonConvert.SerializeObject(new { data = lstQuotationItem, ngoni = materials }, Formatting.Indented,
                 // var result = JsonConvert.SerializeObject(lstQuotationItem, Formatting.Indented,
                 new JsonSerializerSettings
                 {
                     ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                 });
                //Response.StatusCode = 200;

                ////Needed for IIS7.0
                //Response.TrySkipIisCustomErrors = true;

                //return new ContentResult
                //{
                //    Content =JsonConvert.SerializeObject(result),

                //    ContentEncoding = System.Text.Encoding.UTF8
                //};
                return Json(result, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return HttpNotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(int ReceiptId, DebitCreditItem[] productss, int WarehouseId, string ReceiptCurrency, string ReceiptType, decimal Total, decimal Totalvat, string Description = "")
        // public ActionResult Create(int? ReceiptId, DebitCreditItem[] productss, decimal Totalvat, int WarehouseId, string ReceiptType, decimal Total, string Description = "")
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string result = "Error! Adjustment  Is Not Complete!";
            DateTime myInvoice = DateTime.Today;
            var me = myInvoice.ToString("ddMMyyy");
            System.Diagnostics.Debug.WriteLine("Test1 : " + me);
            var myreCount1 = db.DebitCreditNotes.Count();
            decimal rate = 14;
            try          
            {
                var Recc = db.Sales.Where(l => l.Id == ReceiptId).FirstOrDefault().recieptNumber;
                var CustomerId = db.InformalInvoices.Where(p => p.InvoiceNo == Recc).FirstOrDefault().CustomerId;
               // int customerId = db.Customers.Where(k=> k.Id ==(int)invoice.CustomerId).FirstOrDefault().


                if (ModelState.IsValid)
                {
                    DebitCreditNote newNote = new DebitCreditNote();
                    if (ReceiptCurrency != "USD")
                    {
                        newNote.IsFiscilazed = false;
                        newNote.InvoiceNo = me + "" + myreCount1;
                        newNote.total = Math.Round((Total + Totalvat) * rate, 2);
                        newNote.subtotal = Math.Round(Total * rate, 2);
                        newNote.vat = Math.Round(Totalvat * rate, 2);
                        newNote.receiptNo = Convert.ToString(db.Sales.Where(l => l.Id == ReceiptId).FirstOrDefault().recieptNumber); 
                        newNote.ReceiptType = ReceiptType;
                        newNote.AddedBy = userId;
                        newNote.RecieptId = db.Sales.Where(l => l.Id == ReceiptId).FirstOrDefault().zimraReceiptNo;
                        newNote.WarehouseId = WarehouseId;
                        newNote.Duedate = DateTime.Now;
                        newNote.ReceiptCurrency = ReceiptCurrency;
                        newNote.Remarks = Description;
                        newNote.CustomerId =(int)CustomerId;
                        db.DebitCreditNotes.Add(newNote);
                    }
                    else
                    {

                        newNote.IsFiscilazed = false;
                        newNote.InvoiceNo = me + "" + myreCount1;
                        newNote.total = Total + Totalvat;
                        newNote.subtotal = Total;
                        newNote.vat = Totalvat;
                        newNote.receiptNo =Convert.ToString(  db.Sales.Where(l => l.Id == ReceiptId).FirstOrDefault().recieptNumber);
                        newNote.ReceiptType = ReceiptType;
                        newNote.AddedBy = userId;
                        newNote.RecieptId = db.Sales.Where(l => l.Id == ReceiptId).FirstOrDefault().zimraReceiptNo;
                        newNote.WarehouseId = WarehouseId;
                        newNote.Duedate = DateTime.Now;
                        newNote.ReceiptCurrency = ReceiptCurrency;
                        newNote.Remarks = Description;
                        newNote.CustomerId =(int) CustomerId;
                        db.DebitCreditNotes.Add(newNote);
                    }
                    db.SaveChanges();


                    foreach (var item in productss)

                    {
                        /* var selectedProduct = db.Products.Where(h => h.Name == item.receiptLineName).First();
                         DebitCreditItem newEntry = new DebitCreditItem();
                         newEntry.receiptLineName = selectedProduct.Name;
                         newEntry.receiptLinePrice = selectedProduct.SalePrice;
                         newEntry.receiptLineQuantity = item.receiptLineQuantity;
                         newEntry.receiptLineTotal = Math.Round(newEntry.receiptLinePrice * newEntry.receiptLineQuantity,2);
                         newEntry.receiptLineType = "Sale";
                         newEntry.lineVat = Math.Round(item.lineVat,2);
                         newEntry.debitCreditNoteId = newNote.Id;
                         newEntry.ReceiptNo = newNote.receiptNo;
                         newEntry.receiptId = newNote.RecieptId;
                         db.DebitCreditItems.Add(newEntry);
                         db.SaveChanges();*/
                        if (ReceiptCurrency != "USD")
                        {

                            var selectedProduct = db.Products.Where(h => h.Name == item.receiptLineName).First();
                            //  decimal price = db.WarehouseStocks.FirstOrDefault(g => g.ProductId == selectedProduct.Id && g.WarehouseId == WarehouseId).SalePrice;
                            DebitCreditItem newEntry = new DebitCreditItem();
                            newEntry.receiptLineName = selectedProduct.Name;
                            newEntry.HsnCode = selectedProduct.HSNCode;
                            newEntry.receiptLinePrice = Math.Round(item.receiptLinePrice * rate, 2);
                            newEntry.receiptLineQuantity = item.receiptLineQuantity;
                            newEntry.receiptLineTotal = Math.Round(newEntry.receiptLinePrice * newEntry.receiptLineQuantity, 2);
                            newEntry.receiptLineType = "Sale";
                            newEntry.lineVat = item.lineVat;
                            newEntry.debitCreditNoteId = newNote.Id;
                            newEntry.ReceiptNo = newNote.receiptNo;
                            newEntry.receiptId = newNote.RecieptId;
                            db.DebitCreditItems.Add(newEntry);
                            db.SaveChanges();
                            //   taxAmount = newEntry.receiptLineTotal;
                        }
                        else
                        {
                            var selectedProduct = db.Products.Where(h => h.Name == item.receiptLineName).First();
                        //    decimal price = db.WarehouseStocks.FirstOrDefault(g => g.ProductId == selectedProduct.Id && g.WarehouseId == WarehouseId).SalePrice;
                            DebitCreditItem newEntry = new DebitCreditItem();
                            newEntry.receiptLineName = selectedProduct.Name;
                            newEntry.HsnCode = selectedProduct.HSNCode;
                            newEntry.receiptLinePrice = item.receiptLinePrice;
                            newEntry.receiptLineQuantity = item.receiptLineQuantity;
                            newEntry.receiptLineTotal = newEntry.receiptLinePrice * newEntry.receiptLineQuantity;
                            newEntry.receiptLineType = "Sale";
                            newEntry.lineVat = item.lineVat;
                            newEntry.debitCreditNoteId = newNote.Id;
                            newEntry.ReceiptNo = newNote.receiptNo;
                            newEntry.receiptId = newNote.RecieptId;
                            db.DebitCreditItems.Add(newEntry);
                            db.SaveChanges();
                            //       taxAmount = newEntry.receiptLineTotal;
                        }

                    }
                   // await new ZimraApiController().DebitCreditNote(WarehouseId);

                    result = "Success! Adjustment Completed";
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
            return Json(result, JsonRequestBehavior.AllowGet);
            //return Content(sb.ToString());

        }
        public ActionResult GetGrid()
        {
            try
            {

                var tak = db.DebitCreditNotes.ToArray();

                var result = from c in tak.Where(k=> k.WarehouseId == WarehouseId)
                             select new string[] {
                            c.Id.ToString(),
                            Convert.ToString(c.Id),
            Convert.ToString(c.IsFiscilazed),
           // Convert.ToString(c.InvoiceNo),
                Convert.ToString(c.receiptNo),
            Convert.ToString(c.ReceiptType),
            Convert.ToString(c.RecieptId),
            Convert.ToString(c.total),
            Convert.ToString(c.AddedBy),
            Convert.ToString(c.WarehouseId),

            Convert.ToString(c.Status),
            Convert.ToString(c.ErrorMessage)
            };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View(ex.Message);
            }

            //bool isAdmin = false;
            ////TODO: Check the user if it is admin or normal user, (true-Admin, false- Normal user)
            //string output = isAdmin ? "Welcome to the Admin User" : "Welcome to the User";

            //return Json(output, JsonRequestBehavior.AllowGet);
        }
        public ActionResult print(int id)
        {


            DebitCreditNote ObjDebeitCredit = db.DebitCreditNotes.Find(id);
            var debitCreditItems = db.DebitCreditItems.Where(q => q.debitCreditNoteId == id).ToArray();
            // var CustVat = db.Users.FirstOrDefault(n => n.Id == ObjDebeitCredit.customerId).vatNumber;
            var debititems = db.DebitCreditItems.FirstOrDefault(k => k.debitCreditNoteId == id);
            ///  string currencySymbol;
            Customers customer = db.Customers.FirstOrDefault(k => k.Id == ObjDebeitCredit.CustomerId);



            if (customer == null)
            {
                customer =db.Customers.FirstOrDefault(k=> k.BuyerRegisterName == "Customer");
            }


            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            //var jobcard = db.JobCards.FirstOrDefault(i => i.Id == id && i.WarehouseId == warehouse);
            //var user = db.Users.FirstOrDefault(i => i.Id == ObjDebeitCredit.AddedBy);




            if (ObjDebeitCredit == null)
            {
                return HttpNotFound();
            }

            DebitCreditNoteDto dto = new DebitCreditNoteDto();

            dto.baseLogo = invoiceFormat.baseLogo;



           

            dto.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;

           // dto.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;
            dto.CompanyAddress = invoiceFormat.AddressInfo;
            dto.CompanyContact = invoiceFormat.OtherInfo;
            dto.CompanyName = invoiceFormat.CompanyName;
            dto.CompanyVat = invoiceFormat.VatNumber;

            //  dto.AddedBy = user.UserName;
            dto.Duedate = ObjDebeitCredit.Duedate;

            dto.subtotal = ObjDebeitCredit.subtotal;
            dto.total = ObjDebeitCredit.total;
            dto.vat = ObjDebeitCredit.vat;
            //dto.customer = customer.FullName;
            dto.receiptNo = ObjDebeitCredit.receiptNo;
            dto.ReceiptType = ObjDebeitCredit.ReceiptType;
            //dto.BP = ObjDebeitCredit.
            dto.Id = id;
            dto.IsFiscilazed = ObjDebeitCredit.IsFiscilazed;
            dto.InvoiceNo = ObjDebeitCredit.InvoiceNo;
            dto.Remarks = ObjDebeitCredit.Remarks;
            dto.AddedBy = ObjDebeitCredit.AddedBy;


            if (debititems.isFiscal)
            {
                dto.qrCode = debititems.qrCode;
                dto.qrUrl = debititems.qrUrl;
                dto.VerificationCode = debititems.VerificationCode;
                dto.deviceSerialNo = debititems.deviceSerialNo;
                dto.deviceID = debititems.deviceID;
                dto.fiscalDayNumber = debititems.fiscalDayNumber;
            }
           
            dto.tinNo = invoiceFormat.taxPayerTIN;
            dto.email = db.Warehouses.FirstOrDefault(j => j.Id == warehouse).Email;


            dto.customer = customer.BuyerRegisterName;
            dto.customerTin = customer.BuyerTIN;
            dto.customerAddress = customer.HouseNo + "  " + customer.Street + "  " + customer.City;
            dto.customerVat = customer.VATNumber;
            dto.customerEmail = customer.Email;
            dto.customerPhone = customer.PhoneNo;




            List<DebitCreditNoteItemsDto> itemsList = new List<DebitCreditNoteItemsDto>();

            foreach (var items in debitCreditItems)
            {
                var taxId = db.Products.Where(n => n.Name == items.receiptLineName && n.IsActive == true).FirstOrDefault();
                decimal taxrate;

                if (taxId ==null)
                {
                     taxrate = db.Taxs.FirstOrDefault(h => h.Id ==2).TaxRate;
                }
                else
                {
                     taxrate = db.Taxs.FirstOrDefault(h => h.Id ==taxId.TaxId).TaxRate;
                }
               
                // var taxrate = db.Taxs.Where(h => h.Id == taxId ?? 2).FirstOrDefault().TaxRate;
                DebitCreditNoteItemsDto itemDto = new DebitCreditNoteItemsDto();

                itemDto.receiptLineName = items.receiptLineName;
                itemDto.receiptLineQuantity = items.receiptLineQuantity;
                itemDto.vat = taxrate + "%";
                //itemDto.UnitVat = System.Math.Round(itemDto.UnitVat, 2);
                itemDto.receiptLinePrice = items.receiptLinePrice;
                itemDto.receiptLineTotal = items.receiptLineTotal;
                //itemDto.Name = db.Products.FirstOrDefault(i => i.Id == items.ProductId).Name;

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