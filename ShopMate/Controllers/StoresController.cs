using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static ShopMate.Controllers.posController;

namespace ShopMate.Controllers
{
    public class StoresController : Controller
    {
        int userId = Convert.ToInt32(Env.GetUserInfo("userid"));
        // GET: Stores
        public ActionResult Index()
        {
            return View();
        }
        private SIContext db = new SIContext();
        public ActionResult GetManufacturedGrid()
        {
            return View();
        }
        public ActionResult GetManufacturedGridData()
        {
            var tak = db.Manufacturing.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            /*Convert.ToString(c.RawMaterialsId),*/
            Convert.ToString(db.RawMaterial.FirstOrDefault(i => i.Id == (c.RawMaterialsId)).Name),
            //Convert.ToString(c.OutputDescription),
            //Convert.ToString(c.OutputQuantity),
            Convert.ToString(c.Remaining),

};
            //return View(result);
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetGrid()
        {
            //var tak = db.Store.ToArray();

            //var result = from c in tak
            //             select new string[] { c.Id.ToString(), /*Convert.ToString(c.Id),*/
            //                 //Convert.ToString(db.RawMaterial.FirstOrDefault(i => i.Id == c.Id).Name),
            //                 Convert.ToString(db.Users.FirstOrDefault(i => i.Id == c.AddedBy).UserName),
            //Convert.ToString(c.purchasedate),
            //Convert.ToString(db.RawMaterialStocks.FirstOrDefault(i => i.Id == c.Id).Quantity),
            //Convert.ToString(db.Warehouses.FirstOrDefault(i => i.Id == c.WarehouseId).Name),
            var tak = db.Manufacturing.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            /*Convert.ToString(c.RawMaterialsId),*/
            Convert.ToString(db.RawMaterial.FirstOrDefault(i => i.Id == (c.RawMaterialsId)).Name),
            //Convert.ToString(c.OutputDescription),
            //Convert.ToString(c.OutputQuantity),
            Convert.ToString(c.Remaining),



             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Finished()
        {
            return View();
        }
        public ActionResult GetFinishedGrid()
        {

            var tak = db.FinishedItems.ToArray();
            var prd = db.Products;
            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Product_ProductId.Name),
            Convert.ToString(c.Quantity),
            Convert.ToString(c.unitprice),
            Convert.ToString(c.Total),
             Convert.ToString(db.Warehouses.FirstOrDefault(i=>i.Id==c.WarehouseId).Name),
             Convert.ToString(c.dateadded),




             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        //GET: Stores/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult FinishedGoods()
        {
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.RawMaterialId = new SelectList(db.Products, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult FinishedGoods(FinishedGoods ObjPurchase, finishedItem[] product)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Raw materials Not Saved: please start again!";
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));

            try
            {

                if (ModelState.IsValid)
                {

                    ObjPurchase.AddedBy = AddedBy;
                    ObjPurchase.finisheddate = DateTime.Now;
                    ObjPurchase.WarehouseId = warehouse;
                    db.FinishedGoods.Add(ObjPurchase);
                    db.SaveChanges(userId);

                    int vendorLedger = 0;

                    var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == "Raw Materials");
                    if (LedgerA != null)
                    {
                        vendorLedger = LedgerA.Id;
                    }
                    else
                    {
                        LedgerAccount la = new LedgerAccount();
                        la.Name = "Raw Materials";
                        la.ParentId = 2;
                        la.AddedBy = AddedBy;
                        la.DateAdded = DateTime.Now;
                        db.LedgerAccounts.Add(la);
                        db.SaveChanges(userId);

                        vendorLedger = la.Id;
                    }

                    Transaction tr = new Transaction();
                    tr.AddedBy = AddedBy;
                    tr.DebitLedgerAccountId = vendorLedger;
                    tr.DebitAmount = ObjPurchase.CostPrice;
                    tr.CreditLedgerAccountId = db.LedgerAccounts.FirstOrDefault(i => i.Name == "Finished Goods").Id;
                    tr.CreditAmount = ObjPurchase.CostPrice;
                    tr.DateAdded = DateTime.Now;
                    tr.Remarks = "Finished Goods From Manafacturing,Manafacturing Account credit and Product Stock account debit";
                    tr.Other = null;
                    tr.PurchaseOrSale = "Purchase";
                    tr.PurchaseIdOrSaleId = ObjPurchase.Id;
                    tr.WarehouseId = warehouse;
                    db.Transactions.Add(tr);


                    db.SaveChanges(userId);
                    foreach (var item in product)
                    {
                        var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.ProductId);
                        var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId);
                        ProductStock ps = new ProductStock();

                        ps.ProductId = item.ProductId;
                        ps.Quantity = item.Quantity;
                        ps.PurchasePrice = item.unitprice;
                        ps.TotalPurchaseAmount = (item.unitprice * item.Quantity);
                        ps.SalePrice = item.unitprice * 35 / 100;
                        ps.Discount = 20;
                        decimal TaxAmount = 0;
                        //if (selectedTax.Other == "GST")
                        //{
                        //    decimal taxSplit = selectedTax.TaxRate / 2;
                        //    ps.CGST = selectedProduct.TaxId;
                        //    ps.CGST_Rate = taxSplit;
                        //    ps.SGST = selectedProduct.TaxId;
                        //    ps.SGST_Rate = taxSplit;
                        //    TaxAmount = ((selectedTax.TaxRate) / (115)) * ps.TotalPurchaseAmount;
                        //}
                        //else if (selectedTax.Other == "IGST")
                        //{
                        //    ps.IGST = selectedProduct.TaxId;
                        //    ps.IGST_Rate = selectedTax.TaxRate;
                        //    TaxAmount = ((selectedTax.TaxRate) / (115)) * ps.TotalPurchaseAmount;
                        //}
                        //else if (selectedTax.Other == "Other")
                        //{
                        //    ps.TaxId = selectedProduct.TaxId;
                        //    ps.OtherTaxValue = selectedTax.TaxRate;
                        //    TaxAmount = ((selectedTax.TaxRate) / (115)) * ps.TotalPurchaseAmount;
                        //}

                        ps.TotalSaleAmount = ((item.unitprice * item.Quantity) * 35 / 100);
                        ps.TotalSaleAmountWithTax = ((item.unitprice * item.Quantity) * 35 / 100) + ((selectedTax.TaxRate) / 100) * ps.TotalSaleAmountWithTax;
                        ps.TaxAmount = TaxAmount;
                        ps.Profit = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);
                        ps.ProfitWithTax = (ps.TotalSaleAmountWithTax - ps.TotalPurchaseAmount);

                        ps.Description = item.description;
                        ps.AddedBy = ObjPurchase.AddedBy;
                        ps.DateAdded = DateTime.Now;
                        ps.ModifiedBy = ObjPurchase.AddedBy;
                        ps.DateModied = DateTime.Now;
                        ps.InventoryTypeId = db.InventoryTypes.FirstOrDefault(i => i.Name == "Finished Goods").Id;
                        ps.WarehouseId = warehouse;
                        db.ProductStock.Add(ps);
                        db.SaveChanges(userId);

                        finishedItem its = new finishedItem();
                        its.dateadded = DateTime.Now;
                        its.description = item.description;
                        its.finishedgoods = ObjPurchase;
                        its.InventoryTypeId = db.InventoryTypes.FirstOrDefault(i => i.Name == "Finished Goods").Id;
                        its.ProductId = item.ProductId;
                        its.ProductStockId = ps.Id;
                        its.Quantity = item.Quantity;
                        its.Total = item.Total;
                        its.TransactionId = tr.Id;
                        its.unitprice = item.unitprice;
                        its.WarehouseId = warehouse;

                        db.FinishedItems.Add(its);
                        db.SaveChanges(userId);
                        selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity + item.Quantity;
                        selectedProduct.RemainingAmount = selectedProduct.RemainingAmount + item.Total;
                        db.Entry(selectedProduct).State = EntityState.Modified;
                        db.Entry(selectedProduct).State = EntityState.Modified;
                        db.SaveChanges(userId);

                        //Udate how much of manufactured items used to create finished goods

                        var FinishedItemsTotalWeight = db.Products.FirstOrDefault(i => i.Id == (its.ProductId)).Weight * its.Quantity;
                        var ManufacturedItemsUsed = FinishedItemsTotalWeight / 3500;

                        ManufacturedItems(ManufacturedItemsUsed, its);
                        ////FinishedItemsManufactured fim = new FinishedItemsManufactured();
                        ////var man = db.Manufacturing.ToArray();
                    }

                    //  Get Ledger Account

                    //end 

                    //transaction

                    sb.Append("Sumitted");
                    result = "Success! Materials Saved";
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
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());

        }

        //private void ManufacturedItems(decimal manufacturedItemsUsed, finishedItem finishedItem)
        //{
        //    List<FinishedManufacturedItem> rawMaterialsUsed = new List<FinishedManufacturedItem>();
        //    var miu = db.Manufacturing.ToArray();
        //    var numQuery =
        //     from num in miu
        //     where num.Remaining > 0
        //     select num;

        //    do
        //    {
        //        decimal itemsR;
        //        foreach (var item in numQuery)
        //        {
        //            FinishedManufacturedItem fim = new FinishedManufacturedItem();
        //            if (item.Remaining >= manufacturedItemsUsed)
        //            {
        //                itemsR = manufacturedItemsUsed;
        //                fim.ManufacturingId = item.Id;
        //                fim.finishedItemId = finishedItem.Id;
        //                fim.Quantity = manufacturedItemsUsed;
        //                // subtract manufacturedItemsUsed from Remaining
        //                //build object to save
        //                // save db FiishedItemsManufactured
        //                //Adjust remaining in db MAnufactured
        //                rawMaterialsUsed.Add(fim);
        //                //Subtract remaining from manufacturedItemsUsed
        //                item.Remaining = item.Remaining - itemsR;
        //            }
        //            else
        //            {
        //                itemsR = item.Remaining;
        //                fim.ManufacturingId = item.Id;
        //                fim.finishedItemId = finishedItem.Id;
        //                fim.Quantity = item.Remaining;
        //                //build object to save quantity == remaining
        //                // save db FinishedItemsManufactured
        //                //Adjust remaining in db Manufactured by subtracting remaining
        //                //Subtract remaining from manufacturedItemsUsed
        //                rawMaterialsUsed.Add(fim);
        //                //Subtract remaining from manufacturedItemsUsed
        //                item.Remaining = item.Remaining - itemsR;
        //            }
        //            manufacturedItemsUsed -= itemsR;
        //        }


        //    }
        //    while (manufacturedItemsUsed > 0);
        //    foreach (var item in rawMaterialsUsed)

        //    {
        //        db.FinishedManufacturedItems.Add(item);
        //        db.SaveChanges(userId);
        //    }
        //    //throw new NotImplementedException();
        //}
        private void ManufacturedItems(decimal manufacturedItemsUsed, finishedItem finishedItem)
        {
            List<FinishedManufacturedItem> rawMaterialsUsed = new List<FinishedManufacturedItem>();
            var miu = db.Manufacturing.ToArray();
            var numQuery =
             from num in miu
             where num.Remaining > 0
             select num;

            do
            {
                decimal itemsR;
                foreach (var item in numQuery)
                {
                    FinishedManufacturedItem fim = new FinishedManufacturedItem();
                    if (item.Remaining >= manufacturedItemsUsed)
                    {
                        if (item.Remaining == manufacturedItemsUsed)
                        {
                            itemsR = item.Remaining;
                            fim.ManufacturingId = item.Id;
                            fim.finishedItemId = finishedItem.Id;
                            fim.Quantity = item.Remaining;
                            //build object to save quantity == remaining
                            // save db FinishedItemsManufactured
                            //Adjust remaining in db Manufactured by subtracting remaining
                            //Subtract remaining from manufacturedItemsUsed
                            rawMaterialsUsed.Add(fim);
                            //Subtract remaining from manufacturedItemsUsed
                            item.Remaining = item.Remaining - itemsR;
                        }
                        else
                        {
                            itemsR = item.Remaining;
                            fim.ManufacturingId = item.Id;
                            fim.finishedItemId = finishedItem.Id;
                            fim.Quantity = item.Remaining;
                            //build object to save quantity == remaining
                            // save db FinishedItemsManufactured
                            //Adjust remaining in db Manufactured by subtracting remaining
                            //Subtract remaining from manufacturedItemsUsed
                            rawMaterialsUsed.Add(fim);
                            //Subtract remaining from manufacturedItemsUsed
                            item.Remaining = item.Remaining - itemsR;
                        }
                        manufacturedItemsUsed -= itemsR;
                    }

                    else
                    {
                        //sb.Append("Sumitted");
                        //result = "Success! Materials Saved";
                        //return Json(result, JsonRequestBehavior.AllowGet);
                        var result = "Fail! Materials Saved";
                        //return Json(result, JsonRequestBehavior.AllowGet);
                    }

                }


            }
            while (manufacturedItemsUsed > 0);
            foreach (var item in rawMaterialsUsed)

            {
                db.FinishedManufacturedItems.Add(item);
                db.SaveChanges(userId);
            }
            //throw new NotImplementedException();
        }
        public ActionResult Create()
        {
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.RawMaterialId = new SelectList(db.RawMaterial, "Id", "Name");
            return View();
        }


        // POST: Stores/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Stores Objstores, StoresMaterials[] materials)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Raw materials Not Saved: please start again!";
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            
            try
            {
                if (ModelState.IsValid)
                {

                    Objstores.AddedBy = AddedBy;
                    Objstores.purchasedate = DateTime.Now;
                    Objstores.WarehouseId = warehouse;                   
                    db.Store.Add(Objstores);
                    db.SaveChanges(userId);

                    int vendorLedger = 0;

                    var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == "Raw Materials");
                    if (LedgerA != null)
                    {
                        vendorLedger = LedgerA.Id;
                    }
                    else
                    {
                        LedgerAccount la = new LedgerAccount();
                        la.Name = "Raw Materials";
                        //la.ParentId = 2;
                        la.ParentId = db.LedgerAccounts.FirstOrDefault(i => i.Name == ("Raw Materials")).Id;
                        la.AddedBy = AddedBy;
                        la.DateAdded = DateTime.Now;
                        db.LedgerAccounts.Add(la);
                        db.SaveChanges(userId);

                        vendorLedger = la.Id;
                    }

                    Transaction tr = new Transaction();
                    tr.AddedBy = AddedBy;
                    tr.DebitLedgerAccountId = vendorLedger;
                    tr.DebitAmount = Objstores.totalprice;
                    tr.CreditLedgerAccountId = db.LedgerAccounts.FirstOrDefault(i => i.Name == ("Raw Materials")).Id;
                    tr.CreditAmount = Objstores.totalprice;
                    tr.DateAdded = DateTime.Now;
                    tr.Remarks = "Purchase raw Materials, Bank Account credit and Expense account debit";
                    tr.Other = null;
                    tr.PurchaseOrSale = "Purchase";
                    tr.PurchaseIdOrSaleId = Objstores.Id;
                    tr.WarehouseId = warehouse;
                    tr.IsFormal =true;
                    db.Transactions.Add(tr);


                    db.SaveChanges(userId);
                    foreach (var item in materials)
                    {
                        var selectedRawMaterial = db.RawMaterial.FirstOrDefault(i => i.Id == item.RawMaterialsId); 
                        //var store = db.Store.FirstOrDefault(i=>i.Id = );
                        RawMaterialStock ps = new RawMaterialStock();
                        ps.RawMaterialsId = item.RawMaterialsId;
                        ps.Quantity = item.Quantity;
                        ps.Description = "Raw Materials Purchase";
                        ps.AddedBy = AddedBy;
                        ps.PurchasePrice = item.unitprice;
                        ps.TotalPurchaseAmount = item.Total;
                        ps.DateAdded = DateTime.Now;
                        ps.InventoryTypeId = db.InventoryTypes.FirstOrDefault(i => i.Name == "Purchase").Id;
                        ps.WarehouseId = warehouse;
                        db.RawMaterialStocks.Add(ps);
                        db.SaveChanges(userId);

                        StoresMaterials mat = new StoresMaterials();
                        mat.Quantity = item.Quantity;
                        mat.goods = item.goods;
                        mat.unitprice = item.unitprice;
                        mat.Total = item.Total;
                        mat.InventoryTypeId = db.InventoryTypes.FirstOrDefault(i => i.Name == "Purchase").Id;
                        mat.store = Objstores;
                        mat.RawMaterialsId = item.RawMaterialsId;
                        mat.rawmaterialStockId = ps.Id;
                        mat.TransactionId = tr.Id;
                        db.StoreMaterial.Add(mat);
                        db.SaveChanges();

                        selectedRawMaterial.RemainingQuantity = selectedRawMaterial.RemainingQuantity +item.Quantity;
                        db.Entry(selectedRawMaterial).State = EntityState.Modified;
                        db.SaveChanges(userId);

                    }

                    //  Get Ledger Account

                    //end 

                    //transaction

                    sb.Append("Sumitted");
                    result = "Success! Materials Saved";
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
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());

        }
        public ActionResult Print(int id)
        {
            Stores stor = db.Store.Find(id);
            var mat = db.RawMaterial.ToArray();
            var Materials = db.StoreMaterial.Where(jm => jm.store.Id == id).ToArray();

            //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            //var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            //var jobcard = db.JobCards.FirstOrDefault(i => i.Id == id && i.WarehouseId == warehouse);
            var user = db.Users.FirstOrDefault(i => i.Id == stor.AddedBy);


            if (stor == null)
            {
                return HttpNotFound();
            }

            StoresDto job = new StoresDto();
            job.purchasedate = stor.purchasedate;
            job.strnum = id;
            job.receivedby = user.UserName;
            job.Totalprice = stor.totalprice;

            List<StoreMaterialDto> materialsList = new List<StoreMaterialDto>();

            foreach (var items in Materials)
            {

                StoreMaterialDto dto = new StoreMaterialDto();
                dto.name = mat.FirstOrDefault(i => i.Id == items.RawMaterialsId).Name; ;
                dto.goods = items.goods;
                dto.Quantity = items.Quantity;
                dto.price = items.unitprice;
                dto.total = items.Total;

                materialsList.Add(dto);
            }



            job.material = materialsList;

            return View(job);


        }
        // GET: Stores/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }
        // manufacturing
        public class Cart
        {
            public int product { get; set; }
            public decimal PurchasePrice { get; set; }
            public decimal qty { get; set; }
            public string OutputDescription { get; set; }
            public decimal OutputQuantity { get; set; }
        }
        public ActionResult GetRawMaterialManufuctre()
        {
            var tak = db.RawMaterial.OrderBy(i => i.Name).ToArray();

            var result = from c in tak
                         select new string[] {
            Convert.ToString(c.Name.Replace("'","")),
            Convert.ToString(c.Id) ,
            Convert.ToString(c.RemainingQuantity) ,
           // Convert.ToString(db.Taxs.FirstOrDefault(i=>i.Id== c.TaxId).TaxRate),
             
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Manufacture()
        {
            ViewBag.VendorUserId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Supplier"), "Id", "UserName");
            ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "Id", "Name", 1);
            ViewBag.RawMaterialId = new SelectList(db.Products, "Id", "Name");

            StringBuilder sbMoreTax = new StringBuilder();
            var tax = db.Taxs.Where(i => i.Other == "Tax").ToArray();
            foreach (var item in tax)
            {
                sbMoreTax.Append("<option value=\"" + item.Name + "\">" + item.Name + "</option>");
            }

            ViewBag.moreTax = sbMoreTax.ToString();
            return View();
        }
        public JsonResult AddToManufucturing(List<Cart> products, string PurchaseNote)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            List<SaleReturn> retVal = new List<SaleReturn>();
            string result = "Error! Raw materials Not Saved to Manufacturing: please start again!";
            try
            {
                
               
                int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
                int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));

                try
                {
                    //selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ps.Quantity;

                    foreach (var item in products)
                    {
                        var selectedProduct = db.RawMaterial.FirstOrDefault(i => i.Id == item.product);
                        var selectedRawinstores = db.Store.FirstOrDefault(i => i.Id == item.product);

                        Manufacturing ObjPurchase = new Models.Manufacturing();

                        ObjPurchase.RawMaterialsId = item.product;
                        ObjPurchase.Quantity = item.qty;
                        //ObjPurchase.OutputDescription = item.OutputDescription;
                        //ObjPurchase.OutputQuantity = item.OutputQuantity;
                        ObjPurchase.Remaining = item.qty;
                        ObjPurchase.WarehouseId = warehouse;
                        ObjPurchase.AddedBy = AddedBy;

                        ObjPurchase.DateAdded = DateTime.Now;
                        ObjPurchase.InventoryTypeId = db.InventoryTypes.FirstOrDefault(i => i.Name == "Raw Materials Out").Id;

                        db.Manufacturing.Add(ObjPurchase);
                        db.SaveChanges(userId);

                        RawMaterialStock ps = new RawMaterialStock();
                        ps.RawMaterialsId = ObjPurchase.RawMaterialsId;
                        ps.Quantity = ObjPurchase.Quantity;
                        ps.Description = PurchaseNote;
                        ps.AddedBy = AddedBy;
                        ps.DateAdded = DateTime.Now;
                        ps.InventoryTypeId = db.InventoryTypes.FirstOrDefault(i => i.Name == "Raw Materials Out").Id;




                        ps.WarehouseId = warehouse;
                        db.RawMaterialStocks.Add(ps);
                        db.SaveChanges(userId);

                        //end

                        //Get Ledger Account
                        int vendorLedger = 0;

                        var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == "Raw Materials");
                        if (LedgerA != null)
                        {
                            vendorLedger = LedgerA.Id;
                        }
                        else
                        {
                            LedgerAccount la = new LedgerAccount();
                            la.Name = "Raw Materials";
                            la.ParentId = db.LedgerAccounts.FirstOrDefault(i => i.Name == ("Raw Materials")).Id;
                            la.AddedBy = AddedBy;
                            la.DateAdded = DateTime.Now;
                            db.LedgerAccounts.Add(la);
                            db.SaveChanges(userId);

                            vendorLedger = la.Id;
                        }
                        //end 

                        // transaction
                        Transaction tr = new Transaction();
                        tr.AddedBy = AddedBy;
                        tr.DebitLedgerAccountId = vendorLedger;
                        tr.DebitAmount = ObjPurchase.Quantity * ps.PurchasePrice;
                        tr.CreditLedgerAccountId = db.LedgerAccounts.FirstOrDefault(i => i.Name == ("Raw Materials")).Id;
                        tr.CreditAmount = ObjPurchase.Quantity * ps.PurchasePrice;
                        tr.DateAdded = DateTime.Now;
                        tr.Remarks = "Deduct to manufacturing , Raw Materials Account credit and Manufacturing account debit";
                        tr.Other = null;
                        tr.PurchaseOrSale = "ManufacturingInput";
                        // tr.PurchaseIdOrSaleId = Objstores.Id;
                        tr.WarehouseId = warehouse;
                        tr.IsFormal = true;
                        db.Transactions.Add(tr);
                        db.SaveChanges(userId);
                        sb.Append("Sumitted");
                        result = "Success! Materials Saved";

                        //end

                        selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ps.Quantity;
                        selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - ps.Quantity * ps.PurchasePrice;
                        db.Entry(selectedProduct).State = EntityState.Modified;
                        db.Entry(ObjPurchase).State = EntityState.Modified;
                        db.SaveChanges(userId);


                    }
                    retVal.Add(new SaleReturn { msg = "Done", value = 0 });
                    sb.Append("Sumitted");
                    result = "Success! Materials Saved";
                    return Json(result, JsonRequestBehavior.AllowGet); 


                }
                catch (Exception ex)
                {
                    sb.Append(sb.Append("Error :" + ex.Message));
                    retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
                }


            }
            catch (Exception ex)
            {
                retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
            }

            //return Json(retVal, JsonRequestBehavior.AllowGet);
            sb.Append("Sumitted");
            result = "Success! Materials Saved";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // POST: Stores/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Stores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stores ObjPurchase = db.Store.Find(id);
            if (ObjPurchase == null)
            {
                return HttpNotFound();
            }
            return View(ObjPurchase);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {


                Stores ObjStores = db.Store.Find(id);

                StoresMaterials ObjStoreItems = db.StoreMaterial.FirstOrDefault(i => i.store == ObjStores);

                // Invoice ObjInvoice = db.Invoices.FirstOrDefault(i => i.Id == ObjInvoiceItems.InvoiceId);

                RawMaterialStock ObjProductStock = db.RawMaterialStocks.FirstOrDefault(i => i.Id == ObjStoreItems.rawmaterialStockId);

                Transaction ObjTransaction = db.Transactions.FirstOrDefault(i => i.Id == ObjStoreItems.TransactionId);


                if (ObjStoreItems.InventoryTypeId == 7)
                {
                    var selectedProduct = db.RawMaterial.FirstOrDefault(i => i.Id == ObjStoreItems.RawMaterialsId);
                    selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjStoreItems.Quantity;
                    selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - (ObjStoreItems.Total);

                    db.Entry(selectedProduct).State = EntityState.Modified;
                    db.SaveChanges(userId);
                }


                db.RawMaterialStocks.Remove(ObjProductStock);

                db.Transactions.Remove(ObjTransaction);

                db.StoreMaterial.Remove(ObjStoreItems);
                db.Store.Remove(ObjStores);


                db.SaveChanges(userId);

                try
                {
                    //if double antry of purchase or purchase retrun in transaction 
                    Transaction ObjTran2 = db.Transactions.FirstOrDefault(i => i.PurchaseOrSale == "Purchase" && i.PurchaseIdOrSaleId == id);
                    db.Transactions.Remove(ObjTran2);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                }

                sb.Append("Sumitted");
                return Content(sb.ToString());

            }
            catch (Exception ex)
            {
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());

        }
    }
}
