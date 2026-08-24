using Newtonsoft.Json.Linq;
using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class posController : Controller
    {
        string userId = Env.GetUserInfo("name");
        int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        //
        // GET: /pos/
        public ActionResult Index()
        {

            var customerUser = db.Users.Where(i => i.Role_RoleId.RoleName == "Customer");
            ViewBag.CustomerUserId = new SelectList(customerUser, "Id", "UserName", customerUser.FirstOrDefault().Id);
            // ViewBag.CustomerUserId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer"), "Id", "UserName");
            // ViewBag.CustomerUserId = new SelectList(customerUser, "Id", "UserName", customerUser.FirstOrDefault().Id);
            ViewBag.UserId = new SelectList(customerUser, "Id", "UserName");
            var paye = db.Currencies.Where(k=> k.WarehouseId== warehouse);
            //var paye = db.PaymentModes.Where(i => i.Id == 1);  

            ViewBag.PaymentModes = new SelectList(db.Currencies.Where(i=> i.WarehouseId == warehouse), "id", "Name");
            //ViewBag.PaymentModeId = new SelectList(paye, "Id", "Name",paye.FirstOrDefault().Sale_PaymentModeIds);

            StringBuilder sbMoreTax = new StringBuilder();
            var tax = db.Taxs.Where(i => i.Other == "Tax").ToArray();
            foreach (var item in tax)
            {
                sbMoreTax.Append("<option value=\"" + item.Name + "\">" + item.Name + "</option>");
            }

            ViewBag.moreTax = sbMoreTax.ToString();

            return View();
        }

        public JsonResult Accountpay(int Ids, decimal addbalance, string description, decimal cash, decimal ecocash, decimal swipe)
        {
            List<SaleReturn> retVal = new List<SaleReturn>();
            //issue with remaing quantities
     



            //  
            try
            {
                string CustomerName = db.Users.FirstOrDefault(i => i.Id == Ids).UserName;
                int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));

                try
                {

                    string constring = System.Configuration.ConfigurationManager.ConnectionStrings["SIConnectionString"]
                  .ConnectionString;

                    string qury = "UPDATE [User] SET credit=credit+('" + addbalance + "')  WHERE Id='" + Ids + "'";
                    using (SqlConnection con = new SqlConnection(constring))
                    {
                        using (SqlCommand cmd = new SqlCommand(qury, con))
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    Accountpayment ap = new Accountpayment();
                    ap.AddedBy = AddedBy;
                    ap.Amount = addbalance;
                    ap.DateAdded = DateTime.Now;
                    ap.Remarks = description;
                    ap.UserId = Ids;
                    ap.WarehouseId = warehouse;
                    ap.cash = cash;
                    ap.ecocash = ecocash;
                    ap.swipe = swipe;
                    db.AccountPayments.Add(ap);
                    db.SaveChanges(userId);
                    //}
                    //Get Ledger Account
                    int vendorLedger = 0;

                    var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == CustomerName.Trim());
                    if (LedgerA != null)
                    {
                        vendorLedger = LedgerA.Id;
                    }
                    else
                    {
                        LedgerAccount la = new LedgerAccount();
                        la.Name = CustomerName.Trim();
                        la.ParentId = 12;
                        la.AddedBy = AddedBy;
                        la.DateAdded = DateTime.Now;
                        db.LedgerAccounts.Add(la);
                        db.SaveChanges(userId);

                        vendorLedger = la.Id;
                    }
                    //end 

                    //transaction
                    Transaction tr = new Transaction();
                    tr.AddedBy = AddedBy;
                    tr.DebitLedgerAccountId = vendorLedger;
                    tr.DebitAmount = addbalance;
                    tr.CreditLedgerAccountId = 11;
                    tr.CreditAmount = addbalance;
                    tr.DateAdded = DateTime.Now;
                    tr.Remarks = "Sale, Sale Account credit and " + CustomerName + " account debit";
                    tr.Other = null;
                    tr.PurchaseOrSale = "Sale";
                    tr.PurchaseIdOrSaleId = ap.Id;
                    tr.WarehouseId = warehouse;
                    tr.IsFormal = true;
                    db.Transactions.Add(tr);
                    db.SaveChanges(userId);
                    //Paymenttrack trk = new Paymenttrack();
                    //trk.AccountpaymentId = ap.Id;

                    //// trk.SaleId = 1;
                    //trk.cash = cash;
                    //trk.ecocash = ecocash;
                    //trk.swipe = swipe;
                    //trk.DateAdded = DateTime.Now;
                    //trk.AddedBy = AddedBy;
                    //trk.WarehouseId = warehouse;
                    //db.Paymenttracks.Add(trk);
                    //db.SaveChanges(userId);

                    retVal.Add(new SaleReturn { msg = "Done", value = ap.Id });
                }
                catch (Exception ex)
                {
                    Helper.WriteError(ex, ex.Message);
                    retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
                }


            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
            }

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        // [HttpPost]
        //public void Accountpay(int Ids, decimal addbalance,string description,decimal cash,decimal eco,decimal swip)
        // {
        //     System.Text.StringBuilder sb = new System.Text.StringBuilder();


        //     try
        //     {
        //         //  ViewBag.ProductCategoryId = db.Products.Where(i => i.ProductCategoryId == category);
        //         string constring = System.Configuration.ConfigurationManager.ConnectionStrings["SIConnectionString"]
        //            .ConnectionString;

        //         string qury = "UPDATE [User] SET credit=credit+('" + addbalance + "')  WHERE Id='" + Ids + "'";
        //         using (SqlConnection con = new SqlConnection(constring))
        //         {
        //             using (SqlCommand cmd = new SqlCommand(qury, con))
        //             {
        //                 con.Open();
        //                 cmd.ExecuteNonQuery();
        //             }


        //         }
        //         string CustomerName = db.Users.FirstOrDefault(i => i.Id == Ids).UserName;
        //         int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
        //         int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        //         int vendorLedger = 0;
        //         try
        //         {
        //             Accountpayment ap = new Accountpayment();
        //             ap.AddedBy = AddedBy;
        //             ap.Amount = addbalance;
        //             ap.DateAdded = DateTime.Now;
        //             ap.Remarks = description;
        //             ap.UserId = Ids;
        //             ap.WarehouseId = warehouse;
        //             db.AccountPayments.Add(ap);
        //             db.SaveChanges(userId);

        //             var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == CustomerName.Trim());
        //             if (LedgerA != null)
        //             {
        //                 vendorLedger = LedgerA.Id;
        //             }
        //             else
        //             {
        //                 LedgerAccount la = new LedgerAccount();
        //                 la.Name = CustomerName.Trim();
        //                 la.ParentId = 12;
        //                 la.AddedBy = AddedBy;
        //                 la.DateAdded = DateTime.Now;
        //                 db.LedgerAccounts.Add(la);
        //                 db.SaveChanges(userId);

        //                 vendorLedger = la.Id;
        //             }
        //             //end 

        //             //transaction
        //             Transaction tr = new Transaction();
        //             tr.AddedBy = AddedBy;
        //             tr.DebitLedgerAccountId = vendorLedger;
        //             tr.DebitAmount = addbalance;
        //             tr.CreditLedgerAccountId = 11;
        //             tr.CreditAmount = addbalance;
        //             tr.DateAdded = DateTime.Now;
        //             tr.Remarks = "Account Payment, Accounts Payables and " + CustomerName + " account Credit";
        //             tr.Other = null;
        //             tr.PurchaseOrSale = "Sale";
        //             tr.PurchaseIdOrSaleId = 1;
        //             tr.WarehouseId = warehouse;
        //             db.Transactions.Add(tr);
        //             db.SaveChanges(userId);


        //             Paymenttrack trk = new Paymenttrack();
        //             trk.AccountpaymentId = ap.Id;
        //             //trk.InvoiceId = inv.Id;
        //             trk.cash = cash;
        //             trk.ecocash = eco;
        //             trk.swipe = swip;
        //             trk.DateAdded = DateTime.Now;
        //             trk.AddedBy = AddedBy;
        //             trk.WarehouseId = warehouse;
        //             db.Paymenttracks.Add(trk);

        //             db.SaveChanges();
        //         }

        //         catch (Exception)
        //         {

        //         }
        //     }

        //     catch (Exception ex)
        //     {
        //         sb.Append("Error :" + ex.Message);

        //     }




        // }

        private SIContext db = new SIContext();
        public ActionResult GetProduct(String currency)
        {


           // var tak = db.Products.OrderBy(i => i.Name).ToArray();
            var resul = new string[] { };

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            // Get the claims values
            int userWarehouse = Int16.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Actor)
                               .Select(c => c.Value).SingleOrDefault());

            System.Diagnostics.Debug.WriteLine("Test1 : " + currency);

            //var stockdata = db.WarehouseStocks.Where(i => i.WarehouseId == userWarehouse).GroupBy(x => x.ProductId).Select(x => x.FirstOrDefault());



            var stockdata = db.WarehouseStocks
    .Where(i => i.WarehouseId == userWarehouse)
    .GroupBy(x => x.ProductId)
    .Select(x => x.FirstOrDefault())
    .Where(s => s.RemainingQuantity > 0);

            var tak = db.Products
                .OrderBy(p => p.Name)
                .Join(stockdata,
                      product => product.Id,
                      stock => stock.ProductId,
                      (product, stock) => product)
                .ToArray();

            if (currency == "Rtgs")
            {

                var res = from sd in stockdata.ToList()
                          join pd in db.Products on sd.ProductId equals pd.Id
                          where pd.IsActive == true
                          orderby pd.Name
                          select new string[] {
            Convert.ToString(pd.Name.Replace("'","")),
            //Convert.ToString(pd.SalePrice),
            Convert.ToString(pd.RtgsPrice),
            Convert.ToString(pd.Id) ,
            Convert.ToString(pd.ProductImage) ,
            Convert.ToString(pd.SalePrice),
            Convert.ToString(db.Taxs.FirstOrDefault(i=>i.Id== pd.TaxId).TaxRate),
             Convert.ToString(pd.BarCode),
             Convert.ToString(sd.RemainingQuantity),
             //Convert.ToString(pd.w),//ngonie came back here if not work replace with c.remaining

             };
                return Json(new { aaData = res }, JsonRequestBehavior.AllowGet);

            }
            else
            {

                var res = from sd in stockdata.ToList()
                          join pd in db.Products on sd.ProductId equals pd.Id
                          where pd.IsActive == true
                          orderby sd.RemainingQuantity
                          select new string[] {
            Convert.ToString(pd.Name.Replace("'","")),
            Convert.ToString(pd.SalePrice),
            //Convert.ToString(pd.RtgsPrice),
            Convert.ToString(pd.Id) ,
            Convert.ToString(pd.ProductImage) ,
            Convert.ToString(pd.SalePrice),
            Convert.ToString(db.Taxs.FirstOrDefault(i=>i.Id== pd.TaxId).TaxRate),
             Convert.ToString(pd.BarCode),
               Convert.ToString(sd.RemainingQuantity)
             //Convert.ToString(pd.w),//ngonie came back here if not work replace with c.remaining

             };

                return Json(new { aaData = res }, JsonRequestBehavior.AllowGet);

            }

            //            var tak = db.Products.OrderBy(i => i.Name).ToArray();
            //                var resul = new string[] { };

            //          var result = from c in tak select new string[] {
            //            Convert.ToString(c.Name.Replace("'","")),
            //            Convert.ToString(c.SalePrice),
            //            //Convert.ToString(c.RtgsPrice),
            //            Convert.ToString(c.Id) ,
            //            Convert.ToString(c.ProductImage) ,
            //            Convert.ToString(c.SalePrice),
            //            //Convert.ToString(c.RtgsPrice),
            //            Convert.ToString(db.Taxs.FirstOrDefault(i=>i.Id== c.TaxId).TaxRate),
            //             Convert.ToString(c.BarCode),
            //             //Convert.ToString(c.RemainingQuantity),

            //             };

        }
        public class Cart
        {
            public int product { get; set; }
            public decimal PurchasePrice { get; set; }
            public decimal qty { get; set; }
        }

        public class SaleReturn
        {
            public string msg { get; set; }
            public int value { get; set; }

        }
        public ActionResult accountpurchase()
        {

            var customerUser = db.Users.Where(i => i.Role_RoleId.RoleName == "Customer");
            ViewBag.CustomerUserId = new SelectList(customerUser, "Id", "UserName", customerUser.FirstOrDefault().Id);
            // ViewBag.CustomerUserId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer"), "Id", "UserName");
            // ViewBag.CustomerUserId = new SelectList(customerUser, "Id", "UserName", customerUser.FirstOrDefault().Id);
            ViewBag.UserId = new SelectList(customerUser, "Id", "UserName");
            var paye = db.PaymentModes.Where(i => i.Id == 1);
            ViewBag.PaymentModeId = new SelectList(paye, "Id", "Name", paye.FirstOrDefault().Id);

            StringBuilder sbMoreTax = new StringBuilder();
            var tax = db.Taxs.Where(i => i.Other == "Tax").ToArray();
            foreach (var item in tax)
            {
                sbMoreTax.Append("<option value=\"" + item.Name + "\">" + item.Name + "</option>");
            }

            ViewBag.moreTax = sbMoreTax.ToString();
            return View();
        }

        public JsonResult CashToCurrencyConvertor(string selectedCurrecy, decimal amountToBeConverted)
        {
            try
            {
                //Convert.ToString(db.Users.FirstOrDefault(i => i.Id == Int32.Parse(c.UserName)).UserName),

                var CurrencyId = db.Currencies.FirstOrDefault(x => x.Name==selectedCurrecy&&x.WarehouseId ==warehouse).Id;
                double currencyRate = db.Rates
                               .Where(x => x.CurrencyId == CurrencyId && x.WarehouseId==warehouse)
                               .OrderByDescending(x => x.DateModified)
                               .First().CurrencyRate;
                // decimal currencyRate = db.Rates.LastOrDefault(rate => rate.Currency.Name == (selectedCurrecy)).CurrencyRate;
                double convertedAmount = (double)amountToBeConverted * currencyRate;
                return Json(convertedAmount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [WebMethod]
        public JsonResult CurrencyToCashConvertor(List<CurrencyAmount> currencyAmounts)
        {
            double totalCustomerAmount = 0;

            currencyAmounts.ForEach(CurrencyAmount =>
            {
                if (!CurrencyAmount.Amount.Equals(0))
                {
                    var CurrencyId = db.Currencies.FirstOrDefault(x => x.Name.Equals(CurrencyAmount.Currency)&& x.WarehouseId==warehouse).Id;

                    double currencyRate = db.Rates
                              .Where(x => x.CurrencyId == CurrencyId &&x.WarehouseId==warehouse)
                              .OrderByDescending(x => x.DateModified)
                              .First().CurrencyRate;
                    //decimal currencyRate = db.Rates.FirstOrDefault(rate => rate.Currency.Name .Equals(CurrencyAmount.Currency)).CurrencyRate;
                    totalCustomerAmount += (double)(CurrencyAmount.Amount) / currencyRate;
                }
            });

            return Json(totalCustomerAmount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult accountsale(List<Cart> products, int CustomerUserId, int PaymentModeId, string SaleNote, string TaxName, decimal gross)
        {
            List<SaleReturn> retVal = new List<SaleReturn>();
            string result = "Error! Sale Not processed, Issue, Customer do not have Enough Funds!";
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            //  
            try
            {
                string CustomerName = db.Users.FirstOrDefault(i => i.Id == CustomerUserId).UserName;
                decimal credit = db.Users.FirstOrDefault(i => i.Id == CustomerUserId).credit;
                var selectedcustomer = db.Users.FirstOrDefault(i => i.Id == CustomerUserId);
                int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));

                try
                {
                    if (credit >= gross)
                    {

                        Invoice inv = new Invoice();
                        inv.AddedBy = AddedBy;
                        inv.DateAdded = DateTime.Now;
                        inv.DateModied = DateTime.Now;
                        inv.IsBilled = false;
                        inv.IsPurchaseOrSale = "Sale";
                        inv.ModifiedBy = AddedBy;
                        inv.UserId = CustomerUserId;
                        inv.WarehouseId = warehouse;
                        db.Invoices.Add(inv);

                        db.SaveChanges(userId);

                        Sale ObjSale = new Models.Sale();
                        foreach (var item in products)
                        {

                            var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.product);

                            //if (selectedProduct.RemainingQuantity >= item.qty)
                            // {

                            var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId);
                            if (TaxName.Contains("IGST") || TaxName.Contains("Other"))
                            {
                                selectedTax = db.Taxs.FirstOrDefault(i => i.Name == TaxName.Trim());
                            }

                            //here

                            ObjSale.ProductId = item.product;
                            ObjSale.Quantity = item.qty;
                            ObjSale.SalePrice = selectedProduct.SalePrice;
                            ObjSale.TotalAmount = (selectedProduct.SalePrice * ObjSale.Quantity);
                            ObjSale.WarehouseId = warehouse;
                            ObjSale.AddedBy = AddedBy;
                            ObjSale.CustomerUserId = CustomerUserId;
                            ObjSale.DateAdded = DateTime.Now;
                            ObjSale.DateModied = DateTime.Now;
                            ObjSale.ModifiedBy = AddedBy;
                            ObjSale.PaidAmount = (selectedProduct.SalePrice * ObjSale.Quantity);
                            ObjSale.PaymentModeId = PaymentModeId;
                            ObjSale.InventoryTypeId = 2;
                            // ObjSale.bond = bond;
                            //    ObjSale.rtgs = swipe;
                            //   ObjSale.ecocash = ecocash;

                            db.Sales.Add(ObjSale);
                            db.SaveChanges(userId);


                            //ProductStock begin
                            ProductStock ps = new ProductStock();
                            ps.ProductId = ObjSale.ProductId;
                            ps.Quantity = ObjSale.Quantity;

                            ps.PurchasePrice = selectedProduct.PurchasePrice;

                            ps.TotalPurchaseAmount = (selectedProduct.PurchasePrice * ObjSale.Quantity);

                            ps.SalePrice = selectedProduct.SalePrice;
                            ps.Discount = selectedProduct.Discount;
                            ps.TotalSaleAmount = (selectedProduct.SalePrice * ObjSale.Quantity);

                            decimal TaxAmount = 0;
                            //if (selectedTax.Other == "GST")
                            //{
                            //    decimal taxSplit = selectedTax.TaxRate / 2;
                            //    ps.CGST = selectedTax.Id;
                            //    ps.CGST_Rate = taxSplit;
                            //    ps.SGST = selectedTax.Id;
                            //    ps.SGST_Rate = taxSplit;

                            //    TaxAmount = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                            //}
                            //else if (selectedTax.Other == "IGST")
                            //{
                            //    ps.IGST = selectedTax.Id;
                            //    ps.IGST_Rate = selectedTax.TaxRate;

                            //    TaxAmount = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                            //}
                            //else if (selectedTax.Other == "Other")
                            //{
                            //    ps.TaxId = selectedTax.Id;
                            //    ps.OtherTaxValue = selectedTax.TaxRate;
                            //    TaxAmount = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                            //}


                            ps.TotalSaleAmountWithTax = (selectedProduct.SalePrice * ObjSale.Quantity);//+ TaxAmount
                            ps.TaxAmount = TaxAmount;
                            ps.Profit = (ps.TotalSaleAmount - (ps.TotalPurchaseAmount));//+ TaxAmount
                            ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount

                            ps.Description = SaleNote;
                            ps.AddedBy = AddedBy;
                            ps.DateAdded = DateTime.Now;
                            ps.ModifiedBy = AddedBy;
                            ps.DateModied = DateTime.Now;
                            ps.InventoryTypeId = 2;
                            ps.WarehouseId = warehouse;
                            db.ProductStock.Add(ps);
                            db.SaveChanges(userId);

                            //end

                            //Get Ledger Account
                            int vendorLedger = 0;

                            var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == CustomerName.Trim());
                            if (LedgerA != null)
                            {
                                vendorLedger = LedgerA.Id;
                            }
                            else
                            {
                                LedgerAccount la = new LedgerAccount();
                                la.Name = CustomerName.Trim();
                                la.ParentId = 12;
                                la.AddedBy = AddedBy;
                                la.DateAdded = DateTime.Now;
                                db.LedgerAccounts.Add(la);
                                db.SaveChanges(userId);

                                vendorLedger = la.Id;
                            }
                            //end 

                            //transaction
                            Transaction tr = new Transaction();
                            tr.AddedBy = AddedBy;
                            tr.DebitLedgerAccountId = vendorLedger;
                            tr.DebitAmount = (ps.TotalPurchaseAmount + TaxAmount);
                            tr.CreditLedgerAccountId = 11;
                            tr.CreditAmount = (ps.TotalPurchaseAmount + TaxAmount);
                            tr.DateAdded = DateTime.Now;
                            tr.Remarks = "Sale, Sale Account credit and " + CustomerName + " account debit";
                            tr.Other = null;
                            tr.PurchaseOrSale = "Sale";
                            tr.PurchaseIdOrSaleId = ObjSale.Id;
                            tr.WarehouseId = warehouse;
                            tr.IsFormal = true;
                            db.Transactions.Add(tr);
                            //end

                            db.SaveChanges(userId);


                            InvoiceItems Iitem = new InvoiceItems();

                            Iitem.ProductId = ObjSale.ProductId;
                            Iitem.Quantity = ObjSale.Quantity;
                            Iitem.TaxAmount = TaxAmount;
                            Iitem.AddedBy = AddedBy;
                            Iitem.DateAdded = DateTime.Now;
                            Iitem.SalePrice = selectedProduct.SalePrice;
                            Iitem.TotalAmount = ps.TotalSaleAmount;
                            Iitem.TotalAmountWithTax = ps.TotalSaleAmountWithTax;
                            Iitem.TaxId = selectedTax.Id;
                            Iitem.PurchaseId = null;
                            Iitem.SaleId = ObjSale.Id;
                            Iitem.ProductStockId = ps.Id;
                            Iitem.TransactionId = tr.Id;
                            Iitem.WarehouseId = warehouse;
                            Iitem.InvoiceId = inv.Id;
                            db.InvoiceItemss.Add(Iitem);

                            db.SaveChanges(userId);


                            selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjSale.Quantity;
                            selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - ps.TotalSaleAmountWithTax;
                            selectedcustomer.credit = selectedcustomer.credit - gross;
                            var saleUpdate = db.Sales.FirstOrDefault(i => i.Id == ObjSale.Id);
                            saleUpdate.TotalAmountWithTax = ps.TotalSaleAmountWithTax;
                            db.Entry(saleUpdate).State = EntityState.Modified;

                            db.Entry(selectedProduct).State = EntityState.Modified;
                            db.SaveChanges(userId);




                        }



                        //}
                        // result = "Success! Sale Processed";
                        // return Json(result, JsonRequestBehavior.AllowGet);
                        retVal.Add(new SaleReturn { msg = "Done", value = inv.Id });

                    }
                    else if (credit < gross)
                    {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = "Unexpected error occured!!";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    Helper.WriteError(ex, ex.Message);
                    retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
                }


            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
            }

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Sale(List<Cart> products, int CustomerUserId, string CustomerInfo, int orderNo, int PaymentModeId, string SaleNote,  decimal GrossTax, List<CurrencyAmount> currencyAmounts, decimal? change, decimal TotalAmount, decimal CurrencyTotals)
        {
            List<SaleReturn> retVal = new List<SaleReturn>();
            //issue with remaing quantities
            //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            //string WarehouseName = db.Warehouses.FirstOrDefault(i => i.Id == WarehouseId).Name;
            //Get the current claims principal
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            // Get the claims values
            int warehouse = Int16.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Actor)
                               .Select(c => c.Value).SingleOrDefault());

            //  
            try
            {
                string CustomerName = db.Users.FirstOrDefault(i => i.Id == CustomerUserId).UserName;
                int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
                var PaymentMode = "";
               // decimal Amount = 0;
                var duplicates1 = new JArray();
                foreach (var item in currencyAmounts)
                {
                    if (item.Amount > 0)
                    {
                        PaymentMode = item.Currency;
                 
                    }

                }
                var me3 = change;
            
                try
                {
                    //DateTime nowDate = DateTime.ParseExact(mySell.date + " " + mySell.time, "dd/MM/yyyy HH:mm:ss", null);
                    DateTime Kyle = DateTime.Today;
               
                    DateTime myInvoice = DateTime.Today;
                    var me = myInvoice.ToString("ddMyy");
                    var myreCount1 = db.InformalInvoices.Where(j=> j.DateModied == Kyle).Count();
                    //InformalInvoice inv = new InformalInvoice();
                    //inv.AddedBy = AddedBy;
                    //inv.DateAdded = DateTime.Now;
                    //inv.DateModied = DateTime.Now;
                    //inv.IsBilled = false;
                    //inv.IsPurchaseOrSale = "Sale";
                    //inv.ModifiedBy = AddedBy;
                    //inv.UserId = CustomerUserId;
                    //inv.WarehouseId = warehouse;
                    ////inv.DispatchAt = wareId;
                    //db.InformalInvoices.Add(inv);

                    //db.SaveChanges(userId);
                  //  var mypayment = db.PaymentModes.FirstOrDefault(i => i.Name == mySell.paymentMethod).Name;
                    var mycurrency = db.Currencies.FirstOrDefault(i => i.Name == PaymentMode && i.WarehouseId == warehouse).Id;
                    decimal priceRate = (decimal)db.Rates.Where(i => i.CurrencyId == mycurrency && i.WarehouseId == warehouse).OrderByDescending(i => i.DateModified).First().CurrencyRate;

                    InformalInvoice inv = new InformalInvoice();
                    inv.CustomerId = 1;
                    inv.AddedBy = AddedBy;
                    inv.DateAdded = DateTime.Now;
                    inv.DateModied = DateTime.Today;
                    inv.IsBilled = false;
                    inv.IsPurchaseOrSale = "Sale";
                    inv.ModifiedBy = AddedBy;
                    inv.UserId = 1;
                    inv.WarehouseId = warehouse;
                    inv.total = TotalAmount;
                    inv.subtotal = TotalAmount - GrossTax;
                    inv.vat = GrossTax;
                    inv.Change = change;
                   // inv.vat = (decimal)mySell.tax;
                  // inv.UserId =
                  //  inv.total = inv.subtotal - inv.vat;
                    inv.Currencysubtotal = Math.Round((TotalAmount - GrossTax)*priceRate, 2);
                    inv.Currencytotal = Math.Round(TotalAmount * priceRate, 2);
                    inv.Currencyvat = Math.Round(GrossTax * priceRate, 2);
                    inv.InvoiceNo =Convert.ToInt32(me + "" + Convert.ToString(myreCount1));
                    try
                    {
                        db.InformalInvoices.Add(inv);
                        db.SaveChanges();
                    }
                    catch (Exception msg)
                    {
                        duplicates1.Add(Convert.ToString(new { reciept = inv.InvoiceNo, warehouse = warehouse,  }));
                    }

                    Sale ObjSale = new Models.Sale();
                    foreach (var item in products)
                    {

                        var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.product);
                        var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.product && i.WarehouseId == warehouse);
                        //if (selectedProduct.RemainingQuantity >= item.qty)
                        // {

                        var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId);

                        //if (TaxName.Contains("IGST") || TaxName.Contains("Other"))
                        //{
                        //    selectedTax = db.Taxs.FirstOrDefault(i => i.Name == TaxName.Trim());
                        //}

                     //   //var mypayment = db.PaymentModes.FirstOrDefault(i => i.Name == PaymentMode).Name;
                     //   var mycurrency = db.Currencies.FirstOrDefault(i => i.Name == PaymentMode && i.WarehouseId== warehouse).Id;
                     //   var priceRate = db.Rates.Where(i => i.CurrencyId == mycurrency && i.WarehouseId ==warehouse).OrderByDescending(i => i.DateModified).First().CurrencyRate;
                        //here

                        ObjSale.ProductId = item.product;
                        ObjSale.Quantity = item.qty;
                        ObjSale.SalePrice = Math.Round(selectedProduct.SalePrice* (decimal)priceRate,2);
                        ObjSale.TotalAmount = (ObjSale.SalePrice * ObjSale.Quantity);
                        ObjSale.WarehouseId = warehouse;
                        ObjSale.AddedBy = AddedBy;
                        ObjSale.CustomerUserId = CustomerUserId;
                        //ObjSale.CustomerInfo = CustomerInfo;//ngoni
                        ObjSale.DateAdded = DateTime.Now;
                        ObjSale.DateModied = DateTime.Now;
                        ObjSale.ModifiedBy = AddedBy;
                        ObjSale.PaidAmount = (ObjSale.SalePrice * ObjSale.Quantity);
                        ObjSale.PaymentModeId = db.PaymentModes.FirstOrDefault(i => i.Name == PaymentMode).Id; /*PaymentModeId;*/
                        ObjSale.InventoryTypeId = 2;
                        ObjSale.isFormalSale = false;
                        ObjSale.InvoiceId = inv.Id;
                        //ObjSale.InvoiceId = inv.Id;
                        // ObjSale.bond = bond;
                        ObjSale.rtgs = ObjSale.TotalAmount;
                        //   ObjSale.ecocash = ecocash;
                        ObjSale.recieptNumber = (int)inv.InvoiceNo;
                       
                        db.Sales.Add(ObjSale);
                        db.SaveChanges(userId);
                        Purchase ObjPurchase = new Purchase();

                        //ProductStock begin
                        ProductStock ps = new ProductStock();
                        ps.ProductId = ObjSale.ProductId;
                        ps.Quantity = ObjSale.Quantity;

                        ps.PurchasePrice = selectedProduct.PurchasePrice;

                        ps.TotalPurchaseAmount = (selectedProduct.PurchasePrice * ObjSale.Quantity);

                        ps.SalePrice = selectedProduct.SalePrice;
                        ps.Discount = selectedProduct.Discount;
                        ps.TotalSaleAmount = (selectedProduct.SalePrice * ObjSale.Quantity);

                      

                        if (selectedTax.Id ==2) {
                            ps.TotalSaleAmountWithTax = Math.Round((selectedProduct.SalePrice * ObjSale.Quantity) * (decimal)0.15,2);//+ TaxAmount
                          
                        } else
                        {
                            ps.TotalSaleAmountWithTax = Math.Round((selectedProduct.SalePrice * ObjSale.Quantity) *(decimal)0.00,2);//+ TaxAmount
                        
                        }
                       
                        ps.TaxAmount = Math.Round(ps.TotalSaleAmountWithTax* (decimal)priceRate,2);
                        ps.Profit = (ps.TotalSaleAmount - (ps.TotalPurchaseAmount));//+ TaxAmount
                        ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount

                        ps.Description = SaleNote;
                        ps.AddedBy = AddedBy;
                        ps.DateAdded = DateTime.Now;
                        ps.ModifiedBy = AddedBy;
                        ps.DateModied = DateTime.Now;
                        ps.InventoryTypeId = 2;
                        ps.WarehouseId = warehouse;
                        ps.IsFormal = false;
                        decimal TaxAmount = ps.TaxAmount;

                        WarehouseStock warehse = new WarehouseStock();
                        warehse = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.product && i.WarehouseId == warehouse);
                        warehse.RemainingQuantity = ObjWarehouseStock.RemainingQuantity - item.qty;
                        db.Entry(warehse).State = EntityState.Modified;
                        db.SaveChanges();

                        ps.RemainingQuantity = warehse.RemainingQuantity;
                        db.ProductStock.Add(ps);
                        db.SaveChanges(userId);

                        //end

                        //Get Ledger Account
                        int vendorLedger = 0;

                        var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == CustomerName.Trim());
                        if (LedgerA != null)
                        {
                            vendorLedger = LedgerA.Id;
                        }
                        else
                        {
                            LedgerAccount la = new LedgerAccount();
                            la.Name = CustomerName.Trim();
                            la.ParentId = 12;
                            la.AddedBy = AddedBy;
                            la.DateAdded = DateTime.Now;
                            db.LedgerAccounts.Add(la);
                            db.SaveChanges(userId);

                            vendorLedger = la.Id;
                        }
                        //end 

                        //transaction
                        Transaction tr = new Transaction();
                        //tr.AddedBy = AddedBy;
                        //tr.DebitLedgerAccountId = vendorLedger;
                        //tr.DebitAmount = (ps.TotalPurchaseAmount + TaxAmount);
                        //tr.CreditLedgerAccountId = db.LedgerAccounts.FirstOrDefault(i => i.Name == ("Sale")).Id;
                        //tr.CreditAmount = (ps.TotalPurchaseAmount + TaxAmount);
                        //tr.DateAdded = DateTime.Now;
                        //tr.Remarks = "Sale, Sale Account credit and " + CustomerName + " account debit";
                        //tr.Other = null;
                        //tr.PurchaseOrSale = "Sale";
                        //tr.PurchaseIdOrSaleId = ObjSale.Id;
                        //tr.WarehouseId = warehouse;
                        //tr.IsFormal = false;
                        //db.Transactions.Add(tr);

                        tr.AddedBy = AddedBy;
                        tr.DebitLedgerAccountId = db.LedgerAccounts.FirstOrDefault(i => i.Name == ("Sale")).Id;
                        tr.DebitAmount = (ps.TotalSaleAmount + TaxAmount);
                        tr.CreditLedgerAccountId = vendorLedger;
                        tr.CreditAmount = (ps.TotalSaleAmount + TaxAmount);
                        tr.DateAdded = DateTime.Now;
                        tr.Remarks = "Sale, Sale Account credit and " + CustomerName + " account debit";
                        tr.Other = null;
                        tr.PurchaseOrSale = "Sale";
                        tr.PurchaseIdOrSaleId = ObjSale.Id;
                        tr.WarehouseId = warehouse;
                        tr.IsFormal = true;
                        db.Transactions.Add(tr);
                        //end

                        db.SaveChanges(userId);
                        //end

                        db.SaveChanges(userId);


                        InvoiceItems Iitem = new InvoiceItems();

                        Iitem.ProductId = ObjSale.ProductId;
                        Iitem.Quantity = ObjSale.Quantity;
                        Iitem.TaxAmount = TaxAmount;
                        Iitem.AddedBy = AddedBy;
                        Iitem.DateAdded = DateTime.Now;
                        Iitem.SalePrice = ObjSale.SalePrice;
                        Iitem.TotalAmount = ObjSale.TotalAmount;
                        Iitem.TotalAmountWithTax = ps.TotalSaleAmountWithTax;
                        Iitem.TaxId = selectedTax.Id;
                        Iitem.PurchaseId = null;
                        Iitem.SaleId = ObjSale.Id;
                        Iitem.ProductStockId = ps.Id;
                        Iitem.TransactionId = tr.Id;
                        Iitem.WarehouseId = warehouse;
                        Iitem.InvoiceId = inv.Id;
                        db.InvoiceItemss.Add(Iitem);

                        db.SaveChanges(userId);


                        // selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjSale.Quantity;
                        //ObjWarehouseStock.RemainingQuantity = ObjWarehouseStock.RemainingQuantity - ObjSale.Quantity;// other warestocks
                        selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - ps.TotalSaleAmountWithTax;

                        var saleUpdate = db.Sales.FirstOrDefault(i => i.Id == ObjSale.Id);
                        saleUpdate.TotalAmountWithTax = ps.TaxAmount;
                        db.Entry(saleUpdate).State = EntityState.Modified;

                        //    db.Entry(selectedProduct).State = EntityState.Modified;
                        db.SaveChanges(userId);


                    }

                   // await new ZimraApiController().SendSales(warehouse);

                    retVal.Add(new SaleReturn { msg = "Done", value = inv.Id });
                    //Paymenttrack trk = new Paymenttrack
                    //{
                    //    SaleId = ObjSale.Id,
                    //    InvoiceId = inv.Id,
                    //    cash = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Cash")).Amount,
                    //    ecocash = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Swipe External")).Amount,
                    //    swipe = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Swipe Internal")).Amount,
                    //    DateAdded = DateTime.Now,
                    //    AddedBy = AddedBy,
                    //    WarehouseId = warehouse,
                    //    usd = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("USD")).Amount,
                    //    //telecash = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Telecash")).Amount,
                    //    //Rand = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Rand")).Amount,
                    //    //pula = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Pula")).Amount,
                    //    //onemoney = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("One Money")).Amount,
                    //    Change = Convert.ToDecimal(change)
                    //    //Change = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Change")).Amount,
                    //};
                    //db.Paymenttracks.Add(trk);

                    //db.SaveChanges();
                    //print(retVal[0].value);

                }
                catch (Exception ex)
                {
                    Helper.WriteError(ex, ex.Message);
                    retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
                }


            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
            }

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }


        //public JsonResult Sale(List<Cart> products, int CustomerUserId, int orderNo, int PaymentModeId, string SaleNote, string TaxName, List<CurrencyAmount> currencyAmounts, decimal change, string currency, string CustomerNames = "")
        //{
        //    List<SaleReturn> retVal = new List<SaleReturn>();
        //    //issue with remaing quantities
        //    //  int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        //    //  
        //    var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

        //    // Get the claims values
        //    int warehouse = Int16.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Actor)
        //                       .Select(c => c.Value).SingleOrDefault());
        //    var mytime = (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000);
        //    DateTime myInvoice = DateTime.Today;
        //    var me = myInvoice.ToString("ddMMyyy");
        //    System.Diagnostics.Debug.WriteLine("Test1 : " + me);
        //    var myreCount1 = db.Invoices.Where(j => j.WarehouseId == warehouse).Count();

        //    try
        //    {
        //        string CustomerName = db.Users.FirstOrDefault(i => i.Id == CustomerUserId).UserName;

        //        int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
        //        var PaymentMode = "";
        //        foreach (var item in currencyAmounts)
        //        {
        //            if (item.Amount > 0)
        //            {
        //                PaymentMode = item.Currency;
        //            }

        //        }
        //        try
        //        {

        //            //Invoice inv = new Invoice();
        //            //inv.AddedBy = AddedBy;
        //            //inv.DateAdded = DateTime.Now;
        //            //inv.DateModied = DateTime.Now;
        //            //inv.IsBilled = false;
        //            //inv.IsPurchaseOrSale = "Sale";
        //            //inv.ModifiedBy = AddedBy;
        //            //inv.UserId = CustomerUserId;
        //            //inv.WarehouseId = warehouse;
        //            //inv.CustomerVatReg = CustomerNames;
        //            //db.Invoices.Add(inv);

        //            //db.SaveChanges(userId);

        //            InformalInvoice inv = new InformalInvoice();
        //            inv.CustomerId = 1;
        //            inv.AddedBy = AddedBy;
        //            inv.DateAdded = DateTime.Now;
        //            inv.DateModied = DateTime.Now;
        //            inv.IsBilled = false;
        //            inv.IsPurchaseOrSale = "Sale";
        //            inv.ModifiedBy = AddedBy;
        //            inv.UserId = 1;
        //            inv.WarehouseId = warehouse;
        //            //inv.subtotal = Convert.ToDecimal(mySell.subtotal);
        //            //inv.InvoiceNo = mySell.invoiceId;
        //            //inv.vat = (decimal)mySell.tax;
        //            //inv.total = inv.subtotal - inv.vat;
        //            //inv.Currencysubtotal = 0;
        //            //inv.Currencytotal = 0;
        //            //inv.Currencyvat = 0;
        //            //try
        //            //{
        //                db.InformalInvoices.Add(inv);
        //                db.SaveChanges();
        //            //}
        //            //catch (Exception msg)
        //            //{
        //            //    duplicates1.Add(Convert.ToString(new { reciept = mySell.invoiceId, warehouse = seller_user.WarehouseId, prodName = seller_user.vatNumber }));
        //            //}

        //            Sale ObjSale = new Models.Sale();
        //            foreach (var item in products)
        //            {
        //                var litre = db.Products.FirstOrDefault(i => i.PurchasePrice == item.PurchasePrice);
        //                var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.product && i.WarehouseId == warehouse);
        //                //  var ObjProduct = db.Products.FirstOrDefault(i => i.Id == item.product && i.PurchasePrice == item.PurchasePrice);
        //                var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.product);
        //                Console.WriteLine(litre);
        //                //if (selectedProduct.RemainingQuantity >= item.qty)
        //                // {

        //                var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId);
        //                if (TaxName.Contains("IGST") || TaxName.Contains("Other"))
        //                {
        //                    selectedTax = db.Taxs.FirstOrDefault(i => i.Name == TaxName.Trim());
        //                }

        //                //here
        //                if (currency == "Rtgs")
        //                {


        //                    ObjSale.ProductId = item.product;
        //                    ObjSale.Quantity = item.qty;
        //                    ObjSale.UnitPrice = selectedProduct.PurchasePrice * item.qty;
        //                    ObjSale.RtgsPrice = (decimal)selectedProduct.RtgsPrice;
        //                    ObjSale.TotalRtgsAmount = (decimal)selectedProduct.RtgsPrice * ObjSale.Quantity;
        //                    ObjSale.WarehouseId = warehouse;
        //                    ObjSale.AddedBy = AddedBy;
        //                    ObjSale.CustomerUserId = CustomerUserId;
        //                    ObjSale.DateAdded = DateTime.Now;
        //                    ObjSale.DateModied = DateTime.Now;
        //                    ObjSale.ModifiedBy = AddedBy;
        //                    ObjSale.PaidRtgsAmount = (decimal)(selectedProduct.RtgsPrice * ObjSale.Quantity);
        //                    ObjSale.PaymentModeId = db.PaymentModes.FirstOrDefault(i => i.Name == PaymentMode).Id; /*PaymentModeId;*/
        //                    ObjSale.InventoryTypeId = 2;
        //                    ObjSale.isFormalSale = true;
        //                    ObjSale.recieptNumber = mytime;
        //                    // ObjSale.bond = bond;
        //                    //    ObjSale.rtgs = swipe;
        //                    //   ObjSale.ecocash = ecocash;
        //                }
        //                else
        //                {
        //                    ObjSale.ProductId = item.product;
        //                    ObjSale.UnitPrice = selectedProduct.PurchasePrice * item.qty;
        //                    //ObjSale.UnitPrice = litres * item.qty;
        //                    ObjSale.Quantity = item.qty;
        //                    ObjSale.SalePrice = selectedProduct.SalePrice;
        //                    ObjSale.TotalAmount = (selectedProduct.SalePrice * ObjSale.Quantity);
        //                    ObjSale.WarehouseId = warehouse;
        //                    ObjSale.AddedBy = AddedBy;
        //                    ObjSale.CustomerUserId = CustomerUserId;
        //                    ObjSale.DateAdded = DateTime.Now;
        //                    ObjSale.DateModied = DateTime.Now;
        //                    ObjSale.ModifiedBy = AddedBy;
        //                    ObjSale.PaidAmount = (selectedProduct.SalePrice * ObjSale.Quantity);
        //                    ObjSale.PaymentModeId = db.PaymentModes.FirstOrDefault(i => i.Name == PaymentMode).Id; /*PaymentModeId;*/
        //                    ObjSale.InventoryTypeId = 2;
        //                    ObjSale.isFormalSale = true;
        //                    ObjSale.recieptNumber = mytime;
        //                    // ObjSale.bond = bond;
        //                    //    ObjSale.rtgs = swipe;
        //                    //   ObjSale.ecocash = ecocash;
        //                }
        //                //  ObjSale.UnitPrice = item.PurchasePrice * item.qty;
        //                db.Sales.Add(ObjSale);
        //                db.SaveChanges(userId);
        //                Purchase ObjPurchase = new Purchase();

        //                //ProductStock begin
        //                ProductStock ps = new ProductStock();
        //                ps.ProductId = ObjSale.ProductId;
        //                ps.Quantity = ObjSale.Quantity;

        //                ps.PurchasePrice = selectedProduct.PurchasePrice;

        //                ps.TotalPurchaseAmount = (selectedProduct.PurchasePrice * ObjSale.Quantity);

        //                ps.SalePrice = selectedProduct.SalePrice;
        //                ps.Discount = selectedProduct.Discount;
        //                ps.TotalSaleAmount = (selectedProduct.SalePrice * ObjSale.Quantity);

        //                decimal TaxAmount = 0;

        //                ps.ProductName = selectedProduct.Name;
        //                ps.TotalSaleAmountWithTax = (selectedProduct.SalePrice * ObjSale.Quantity);//+ TaxAmount
        //                ps.TaxAmount = TaxAmount;
        //                ps.Profit = (ps.TotalSaleAmount - (ps.TotalPurchaseAmount));//+ TaxAmount
        //                ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount

        //                ps.Description = SaleNote;
        //                ps.AddedBy = AddedBy;
        //                ps.DateAdded = DateTime.Now;
        //                ps.ModifiedBy = AddedBy;
        //                ps.DateModied = DateTime.Now;
        //                ps.InventoryTypeId = 2;
        //                ps.WarehouseId = warehouse;
        //                ps.IsFormal = true;
        //                //   ps.ProductBatchId = db.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;

        //                WarehouseStock warehse = new WarehouseStock();
        //                warehse = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.product && i.WarehouseId == warehouse);
        //                warehse.RemainingQuantity = ObjWarehouseStock.RemainingQuantity - item.qty;
        //                db.Entry(warehse).State = EntityState.Modified;
        //                db.SaveChanges();

        //                ps.RemainingQuantity = warehse.RemainingQuantity;
        //                db.ProductStock.Add(ps);
        //                db.SaveChanges(userId);

        //                //end

        //                //Get Ledger Account
        //                int vendorLedger = 0;

        //                var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == CustomerName.Trim());
        //                if (LedgerA != null)
        //                {
        //                    vendorLedger = LedgerA.Id;
        //                }
        //                else
        //                {
        //                    LedgerAccount la = new LedgerAccount();
        //                    la.Name = CustomerName.Trim();
        //                    la.ParentId = 12;
        //                    la.AddedBy = AddedBy;
        //                    la.DateAdded = DateTime.Now;
        //                    db.LedgerAccounts.Add(la);
        //                    db.SaveChanges(userId);

        //                    vendorLedger = la.Id;
        //                }
        //                //end 

        //                //transaction
        //                Transaction tr = new Transaction();
        //                tr.AddedBy = AddedBy;
        //                tr.DebitLedgerAccountId = vendorLedger;
        //                tr.DebitAmount = (ps.TotalPurchaseAmount + TaxAmount);
        //                tr.CreditLedgerAccountId = db.LedgerAccounts.FirstOrDefault(i => i.Name == ("Sale")).Id;
        //                tr.CreditAmount = (ps.TotalPurchaseAmount + TaxAmount);
        //                tr.DateAdded = DateTime.Now;
        //                tr.Remarks = "Sale, Sale Account credit and " + CustomerName + " account debit";
        //                tr.Other = null;
        //                tr.PurchaseOrSale = "Sale";
        //                tr.PurchaseIdOrSaleId = ObjSale.Id;
        //                tr.WarehouseId = warehouse;
        //                tr.IsFormal = true;
        //                db.Transactions.Add(tr);
        //                //end

        //                db.SaveChanges(userId);


        //                InvoiceItems Iitem = new InvoiceItems();

        //                Iitem.ProductId = ObjSale.ProductId;
        //                Iitem.Quantity = ObjSale.Quantity;
        //                Iitem.TaxAmount = TaxAmount;
        //                Iitem.AddedBy = AddedBy;
        //                Iitem.DateAdded = DateTime.Now;
        //                Iitem.SalePrice = selectedProduct.SalePrice;
        //                Iitem.TotalAmount = ps.TotalSaleAmount;
        //                Iitem.TotalAmountWithTax = ps.TotalSaleAmountWithTax;
        //                Iitem.TaxId = selectedTax.Id;
        //                Iitem.PurchaseId = null;
        //                Iitem.SaleId = ObjSale.Id;
        //                Iitem.ProductStockId = ps.Id;
        //                Iitem.TransactionId = tr.Id;
        //                Iitem.WarehouseId = warehouse;
        //                Iitem.InvoiceId = inv.Id;
        //                db.InvoiceItemss.Add(Iitem);

        //                db.SaveChanges(userId);


        //                //selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjSale.Quantity;
        //                //selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - ps.TotalSaleAmountWithTax;

        //                var saleUpdate = db.Sales.FirstOrDefault(i => i.Id == ObjSale.Id);
        //                saleUpdate.TotalAmountWithTax = ps.TotalSaleAmountWithTax;
        //                db.Entry(saleUpdate).State = EntityState.Modified;

        //                //db.Entry(selectedProduct).State = EntityState.Modified;
        //                db.SaveChanges(userId);


        //            }
        //            retVal.Add(new SaleReturn { msg = "Done", value = inv.Id });
        //            Paymenttrack trk = new Paymenttrack
        //            {
        //                SaleId = ObjSale.Id,
        //                InvoiceId = inv.Id,
        //                cash = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Cash")).Amount,
        //                //ecocash = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Ecocash")).Amount,
        //                swipe = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("RTGS Swipe")).Amount,
        //                DateAdded = DateTime.Now,
        //                AddedBy = AddedBy,
        //                WarehouseId = warehouse,
        //                usd = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("USD")).Amount,
        //                //telecash = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Telecash")).Amount,
        //                //Rand = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Rand")).Amount,
        //                pula = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Pula")).Amount,
        //                onemoney = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("One Money")).Amount,
        //                acl = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("ACL")).Amount,
        //                fbc = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("FBC")).Amount,
        //                zipit = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("ZIPIT")).Amount,
        //                Change = Convert.ToDecimal(change)
        //                //Change = currencyAmounts.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("Change")).Amount,
        //            };
        //            db.Paymenttracks.Add(trk);

        //            db.SaveChanges();
        //            //print(retVal[0].value);

        //        }
        //        catch (Exception ex)
        //        {
        //            Helper.WriteError(ex, ex.Message);
        //            retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.WriteError(ex, ex.Message);
        //        retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
        //    }

        //    return Json(retVal, JsonRequestBehavior.AllowGet);
        //}



        public ActionResult GetProductPurchase()
        {
            var tak = db.Products.OrderBy(i => i.Name).ToArray();

            var result = from c in tak
                         select new string[] {
            Convert.ToString(c.Name.Replace("'","")),
            Convert.ToString(c.PurchasePrice),
            Convert.ToString(c.Id) ,
            Convert.ToString(c.ProductImage) ,
            Convert.ToString(c.PurchasePrice),
            Convert.ToString(db.Taxs.FirstOrDefault(i=>i.Id== c.TaxId).TaxRate),
             //Convert.ToString(c.HSN),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);


        }


        public ActionResult purchase()
        {
            ViewBag.VendorUserId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Supplier"), "Id", "UserName");
            ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "Id", "Name", 1);

            StringBuilder sbMoreTax = new StringBuilder();
            var tax = db.Taxs.Where(i => i.Other == "Tax").ToArray();
            foreach (var item in tax)
            {
                sbMoreTax.Append("<option value=\"" + item.Name + "\">" + item.Name + "</option>");
            }

            ViewBag.moreTax = sbMoreTax.ToString();
            return View();
        }


        public JsonResult newPurchase(List<Cart> products, int VendorUserId, int PaymentModeId, string PurchaseNote, string TaxName)
        {
            List<SaleReturn> retVal = new List<SaleReturn>();
            try
            {
                string VendorName = db.Users.FirstOrDefault(i => i.Id == VendorUserId).UserName;
                int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
                int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));

                try
                {

                    Invoice inv = new Invoice();
                    inv.AddedBy = AddedBy;
                    inv.DateAdded = DateTime.Now;
                    inv.DateModied = DateTime.Now;
                    inv.IsBilled = false;
                    inv.IsPurchaseOrSale = "Purchase";
                    inv.ModifiedBy = AddedBy;
                    inv.UserId = VendorUserId;
                    inv.WarehouseId = warehouse;
                    db.Invoices.Add(inv);

                    db.SaveChanges(userId);

                    foreach (var item in products)
                    {
                        var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.product);


                        var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId);
                        if (TaxName.Contains("IGST") || TaxName.Contains("Other"))
                        {
                            selectedTax = db.Taxs.FirstOrDefault(i => i.Name == TaxName.Trim());
                        }

                        Purchase ObjPurchase = new Models.Purchase();

                        ObjPurchase.ProductId = item.product;
                        ObjPurchase.Quantity = item.qty;
                        ObjPurchase.UnitPrice = selectedProduct.PurchasePrice;
                        // ObjPurchase.UnitPrice = item.PurchasePrice;
                        ObjPurchase.TotalAmount = (selectedProduct.PurchasePrice * ObjPurchase.Quantity);
                        ObjPurchase.WarehouseId = warehouse;
                        ObjPurchase.AddedBy = AddedBy;
                        ObjPurchase.VendorUserId = VendorUserId;
                        ObjPurchase.DateAdded = DateTime.Now;
                        ObjPurchase.InventoryTypeId = 1;

                        db.Purchases.Add(ObjPurchase);
                        db.SaveChanges(userId);

                        //product begin here
                        //Product pr = new Product();
                        //pr.Id = selectedProduct.Id;
                        //pr.PurchasePrice = item.PurchasePrice;
                        //db.Products.Add(pr);
                        //db.SaveChanges();
                        //ProductStock begin
                        ProductStock ps = new ProductStock();
                        ps.ProductId = ObjPurchase.ProductId;
                        ps.Quantity = ObjPurchase.Quantity;
                        ps.PurchasePrice = selectedProduct.PurchasePrice;

                        ps.TotalPurchaseAmount = (selectedProduct.PurchasePrice * ObjPurchase.Quantity);

                        ps.SalePrice = selectedProduct.SalePrice;

                        ps.Discount = selectedProduct.Discount;

                        decimal TaxAmount = 0;
                        decimal vatonreturn = 0;
                        //if (selectedTax.Other == "GST")
                        //{
                        //    decimal taxSplit = selectedTax.TaxRate / 2;
                        //    ps.CGST = selectedProduct.TaxId;
                        //    ps.CGST_Rate = taxSplit;
                        //    ps.SGST = selectedProduct.TaxId;
                        //    ps.SGST_Rate = taxSplit;

                        //    TaxAmount = ((selectedTax.TaxRate) / (115)) * ps.TotalPurchaseAmount;
                        //    vatonreturn = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                        //    //  TaxAmount = (15 /115) * ps.TotalPurchaseAmount;

                        //}
                        //else if (selectedTax.Other == "IGST")
                        //{
                        //    ps.IGST = selectedProduct.TaxId;
                        //    ps.IGST_Rate = selectedTax.TaxRate;
                        //    //TaxAmount = (15 / 115) * ps.TotalPurchaseAmount;
                        //    TaxAmount = ((selectedTax.TaxRate) / (115)) * ps.TotalPurchaseAmount;
                        //    vatonreturn = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                        //}
                        //else if (selectedTax.Other == "Other")
                        //{
                        //    ps.TaxId = selectedProduct.TaxId;
                        //    ps.OtherTaxValue = selectedTax.TaxRate;
                        //    //TaxAmount = (15 / 115) * ps.TotalPurchaseAmount;
                        //    TaxAmount = ((selectedTax.TaxRate) / (115)) * ps.TotalPurchaseAmount;
                        //    vatonreturn = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                        //}

                        ps.TotalSaleAmount = (selectedProduct.SalePrice * ObjPurchase.Quantity) - vatonreturn;
                        ps.TotalSaleAmountWithTax = (selectedProduct.SalePrice * ObjPurchase.Quantity);//+ TaxAmount
                        ps.TaxAmount = TaxAmount;
                        ps.Profit = (ps.TotalSaleAmount - ps.TotalPurchaseAmount) - vatonreturn;
                        ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount

                        ps.Description = PurchaseNote;
                        ps.AddedBy = ObjPurchase.AddedBy;
                        ps.DateAdded = DateTime.Now;
                        ps.ModifiedBy = ObjPurchase.AddedBy;
                        ps.DateModied = DateTime.Now;
                        ps.InventoryTypeId = 1;
                        ps.WarehouseId = warehouse;
                        //ps.ProductBatchId = db.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;
                        db.ProductStock.Add(ps);
                        db.SaveChanges(userId);

                        //end

                        //Get Ledger Account
                        int vendorLedger = 0;

                        var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == VendorName.Trim());
                        if (LedgerA != null)
                        {
                            vendorLedger = LedgerA.Id;
                        }
                        else
                        {
                            LedgerAccount la = new LedgerAccount();
                            la.Name = VendorName.Trim();
                            la.ParentId = 12;
                            la.AddedBy = AddedBy;
                            la.DateAdded = DateTime.Now;
                            db.LedgerAccounts.Add(la);
                            db.SaveChanges(userId);

                            vendorLedger = la.Id;
                        }
                        //end 

                        //transaction
                        Transaction tr = new Transaction();
                        tr.AddedBy = ObjPurchase.AddedBy;
                        tr.DebitLedgerAccountId = 12;//Purchase ledger account
                        tr.DebitAmount = (ps.TotalPurchaseAmount);//+ TaxAmount
                        tr.CreditLedgerAccountId = vendorLedger;
                        tr.CreditAmount = (ps.TotalPurchaseAmount);//+ TaxAmount
                        tr.DateAdded = DateTime.Now;
                        tr.Remarks = "Purchase, Purchase Account debit and " + VendorName + " account credit";
                        tr.Other = null;
                        tr.PurchaseOrSale = "Purchase";
                        tr.PurchaseIdOrSaleId = ObjPurchase.Id;
                        tr.WarehouseId = warehouse;
                        tr.IsFormal = true;
                        db.Transactions.Add(tr);
                        //end

                        db.SaveChanges(userId);


                        InvoiceItems Iitem = new InvoiceItems();

                        Iitem.ProductId = ObjPurchase.ProductId;
                        Iitem.Quantity = ObjPurchase.Quantity;
                        Iitem.TaxAmount = TaxAmount;
                        Iitem.AddedBy = ObjPurchase.AddedBy;
                        Iitem.DateAdded = DateTime.Now;
                        //Iitem.SalePrice = selectedProduct.PurchasePrice;
                        Iitem.SalePrice = item.PurchasePrice;
                        Iitem.TotalAmount = ps.TotalPurchaseAmount;
                        Iitem.TotalAmountWithTax = ps.TotalPurchaseAmount;//+ TaxAmount
                        Iitem.TaxId = selectedTax.Id;
                        Iitem.PurchaseId = ObjPurchase.Id;
                        Iitem.SaleId = null;
                        Iitem.ProductStockId = ps.Id;
                        Iitem.TransactionId = tr.Id;
                        Iitem.WarehouseId = warehouse;

                        //Iitem.InvoiceId = inv.Id;
                        Iitem.InvoiceId = 8;
                        db.InvoiceItemss.Add(Iitem);

                        db.SaveChanges(userId);


                        //if (selectedProduct.RemainingQuantity == 0.00m && selectedProduct.RemainingAmount == 0.00m)
                        //{
                        //    selectedProduct.RemainingQuantity = ObjPurchase.Quantity;
                        //    selectedProduct.RemainingAmount = ps.TotalSaleAmountWithTax;
                        //}
                        //else
                        //{
                        selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity + ObjPurchase.Quantity;
                        selectedProduct.RemainingAmount = selectedProduct.RemainingAmount + ps.TotalSaleAmountWithTax;
                        ObjPurchase.TotalAmountWithTax = ps.TotalPurchaseAmount;//+ TaxAmount
                        db.Entry(selectedProduct).State = EntityState.Modified;
                        db.Entry(selectedProduct).State = EntityState.Modified;
                        db.Entry(ObjPurchase).State = EntityState.Modified;
                        db.SaveChanges(userId);



                    }
                    retVal.Add(new SaleReturn { msg = "Done", value = inv.Id });


                }
                catch (Exception ex)
                {
                    Helper.WriteError(ex, ex.Message);
                    retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
                }


            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
            }

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult print(int id)
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            // Get the claims values
            int warehouse = Int16.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Actor)
                               .Select(c => c.Value).SingleOrDefault());
            try
            {
                var me = db.Sales.FirstOrDefault(j => j.InvoiceId == id);
                var kyle = db.PaymentModes.Where(k => k.Id == me.PaymentModeId).FirstOrDefault();
                var mycurrencySymbloe = db.Currencies.FirstOrDefault(i => i.Name == kyle.Name&& i.WarehouseId==warehouse).CurrencySymbol;
                //   var myItem = db.Sales.FirstOrDefault(h => h.InvoiceId == id);

                InvoiceDto inv = new InvoiceDto();
                //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
                var invoice = db.InformalInvoices.FirstOrDefault(i => i.Id == id && i.WarehouseId == warehouse);
                var invoiceitem = db.InvoiceItemss.Where(i => i.InvoiceId == id).ToArray();
                var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
                var setting = db.Settings.Where(i => i.sGroup == "Invoice").ToArray();
                var user = db.Customers.FirstOrDefault(i => i.Id == invoice.CustomerId);
                var tax = db.Taxs.ToArray();

                inv.InvoiceId = id;
                inv.InvoiceDate = invoice.DateAdded.Value;
                inv.InvoiceFooterText = invoiceFormat.FooterInfo;
                inv.Type = invoice.IsPurchaseOrSale;
                inv.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;
                inv.CurrencySymbol = mycurrencySymbloe;
                inv.ToName = user.BuyerTradeName;
                inv.ToInfo = user.HouseNo + "<br/> " + user.Street + "<br/> " + user.City;
                inv.CompanyAddress = invoiceFormat.AddressInfo;
                inv.CompanyContact = invoiceFormat.OtherInfo;
                inv.CompanyName = invoiceFormat.CompanyName;
                inv.tinNo = invoiceFormat.taxPayerTIN;
                inv.vatNo = invoiceFormat.VatNumber;
                //inv.customerInfo = invoiceFormat.CustomerInfo;
                //inv.DispatchAt = db.Warehouses.FirstOrDefault(i => i.Id == invoice.DispatchAt).Name;
                if (me.isFiscalised != false) {


                    inv.verificationCode = me.VerificationCode;
                    inv.Zimraemail = me.qrUrl;
                    inv.deviceSerialNo = me.deviceSerialNo;
                    inv.fiscalDayNumber = me.fiscalDayNumber;
                    inv.deviceID = me.deviceID;
                    inv.qrCode = me.qrCode;
                    inv.receiptId = me.zimraReceiptNo;
                    inv.email = db.Warehouses.FirstOrDefault(j => j.Id == warehouse).Email;
                }
                inv.Change = invoice.Change;
                var mypayment = db.PaymentModes.FirstOrDefault(i => i.Id == me.PaymentModeId).Name;
                var mycurrency = db.Currencies.FirstOrDefault(i => i.Name == mypayment).Id;
                decimal priceRate = (decimal)(db.Rates.Where(i => i.CurrencyId == mycurrency).OrderByDescending(i => i.DateModified).First().CurrencyRate);
                List<InvoiceItemsDto> listItem = new List<InvoiceItemsDto>();
                foreach (var item in invoiceitem)
                {
                    InvoiceItemsDto li = new InvoiceItemsDto();
                    li.Price = item.SalePrice;
                    li.ProcuctName = item.Product_ProductId.Name;
                    li.Quantity = item.Quantity;
                    li.Tax = item.TaxAmount;

                    li.TaxInfo = tax.FirstOrDefault(i => i.Id == item.TaxId).Name;

                    li.SubTotal = item.TotalAmount;
                    listItem.Add(li);
                }

                inv.invoiceItem = listItem;

                //inv.SubTotal = Math.Round(invoiceitem.Sum(i => i.TotalAmount) * priceRate, 2);
                //inv.Tax = Math.Round(invoiceitem.Sum(i => i.TaxAmount) * priceRate, 2);
                //inv.TaxInfo = "";
                //inv.TotalAmount = Math.Round(invoiceitem.Sum(i => i.TotalAmountWithTax) * priceRate, 2);
                inv.SubTotal = invoice.Currencysubtotal;
                inv.Tax = invoice.Currencyvat;
                //inv.TaxInfo = "";
                inv.TotalAmount = invoice.Currencytotal;
               // inv.Change = invoice.ch

                return View(inv);
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View(ex.Message);
            }
        }



        //public ActionResult print(int id)
        //{
        //    try
        //    {


        //        InvoiceDto inv = new InvoiceDto();
        //        int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        //        var invoice = db.Invoices.FirstOrDefault(i => i.Id == id && i.WarehouseId == warehouse);

        //        var invoiceitem = db.InvoiceItemss.Where(i => i.InvoiceId == id).ToArray();
        //        var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
        //        var setting = db.Settings.Where(i => i.sGroup == "Invoice").ToArray();
        //        var user = db.Users.FirstOrDefault(i => i.Id == invoice.UserId);
        //        var tax = db.Taxs.ToArray();

        //        inv.InvoiceId = id;
        //        inv.InvoiceDate = invoice.DateAdded.Value;
        //        inv.InvoiceFooterText = invoiceFormat.FooterInfo;
        //        inv.Type = invoice.IsPurchaseOrSale;
        //        inv.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;
        //        inv.CurrencySymbol = setting.FirstOrDefault(i => i.sKey == "CurrencySymbol").sValue;
        //        //inv.ToName = user.UserName;
        //        inv.ToName = invoice.CustomerVatReg;
        //        inv.ToInfo = user.Address + "<br/> " + user.Mobile + "<br/> " + user.About;
        //        inv.CompanyAddress = invoiceFormat.AddressInfo;
        //        inv.CompanyContact = invoiceFormat.OtherInfo;
        //        inv.CompanyName = invoiceFormat.CompanyName;

        //        List<InvoiceItemsDto> listItem = new List<InvoiceItemsDto>();
        //        foreach (var item in invoiceitem)
        //        {
        //            InvoiceItemsDto li = new InvoiceItemsDto();
        //            li.Price = item.SalePrice;
        //            li.ProcuctName = item.Product_ProductId.Name;
        //            li.Quantity = item.Quantity;
        //            li.Tax = item.TaxAmount;

        //            li.TaxInfo = tax.FirstOrDefault(i => i.Id == item.TaxId).Name;

        //            li.SubTotal = item.TotalAmount;
        //            listItem.Add(li);
        //        }

        //        inv.invoiceItem = listItem;

        //        inv.SubTotal = invoiceitem.Sum(i => i.TotalAmount);
        //        inv.Tax = invoiceitem.Sum(i => i.TaxAmount);
        //        inv.TaxInfo = "";
        //        inv.TotalAmount = invoiceitem.Sum(i => i.TotalAmountWithTax);


        //        return View(inv);
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.WriteError(ex, ex.Message);
        //        return View(ex.Message);
        //    }
        //}


    }
}