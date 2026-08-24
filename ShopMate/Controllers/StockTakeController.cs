using ExcelDataReader;
using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class StockTakeController : Controller
    {
        // GET: StockTake

        string userId = Env.GetUserInfo("name");
        int warehouses = int.Parse(Env.GetUserInfo("WarehouseId"));
        //
        public ActionResult stockupdate()
        {
            return View();
        }
        public ActionResult Index()
        {
            var customerUser = db.Users.Where(i => i.Role_RoleId.RoleName == "Customer");
            ViewBag.CustomerUserId = new SelectList(customerUser, "Id", "UserName", customerUser.FirstOrDefault().Id);


            StringBuilder sbMoreTax = new StringBuilder();
            var tax = db.Taxs.Where(i => i.Other == "Tax").ToArray();
            foreach (var item in tax)
            {
                sbMoreTax.Append("<option value=\"" + item.Name + "\">" + item.Name + "</option>");
            }

            ViewBag.moreTax = sbMoreTax.ToString();
            return View();
        }
        private SIContext db = new SIContext();
        public ActionResult GetProduct()
        {

            try
            {
                var tak = db.Products.OrderBy(i => i.Name).ToArray();
                var resul = new string[] { };

                var result = from c in tak
                             select new string[] {
            Convert.ToString(c.Name.Replace("'","")),
            Convert.ToString(c.PurchasePrice),
            Convert.ToString(c.ProductImage) ,
            Convert.ToString(c.Id) ,

             Convert.ToString(c.BarCode),

             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
            catch (NullReferenceException ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View(ex.Message);
            }
        }
        public class Cart
        {
            public int product { get; set; }
            public decimal PurchasePrice { get; set; }
            public decimal qty { get; set; }
        }
        public void Sale(List<Cart> products)
        {

            //issue with remaing quantities
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            //  
            try
            {
                try
                {

                    StockTake inv = new StockTake();

                    inv.DateAdded = DateTime.Now;


                    inv.WarehouseId = warehouse;
                    db.StockTakes.Add(inv);

                    db.SaveChanges();

                    StockTakeDetails ObjSale = new Models.StockTakeDetails();
                    foreach (var item in products)
                    {

                        var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.product);

                        //here

                        ObjSale.ProductId = item.product;
                        ObjSale.counted = item.qty;
                        ObjSale.countedvalue = (selectedProduct.PurchasePrice * ObjSale.counted);
                        ObjSale.WarehouseId = warehouse;

                        ObjSale.actualquantity = selectedProduct.RemainingQuantity;
                        ObjSale.countedvalue = (selectedProduct.PurchasePrice * selectedProduct.RemainingQuantity);
                        ObjSale.variance = ObjSale.actualquantity - ObjSale.counted;
                        ObjSale.variancevalue = ObjSale.actualvalue - ObjSale.countedvalue;
                        db.StockTakeDetail.Add(ObjSale);
                        db.SaveChanges();



                    }


                }
                catch (Exception ex)
                {
                    Helper.WriteError(ex, ex.Message);
                }


            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
            }

            //  return Json(retVal, JsonRequestBehavior.AllowGet);
        }
        // GET: StockTake/Details/5
        public ActionResult Details(int id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockTake Objstocktake = db.StockTakes.Find(id);
            var materials = db.StockTakeDetail.Where(n => n.StockTakeId == id);

            if (Objstocktake == null)
            {
                return HttpNotFound();
            }
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse).CompanyName;
            var mywarehouse = db.Warehouses.FirstOrDefault(n => n.Id == warehouse).Name;
            StockTakeDto dto = new StockTakeDto();
            dto.Id = Objstocktake.Id;
            dto.addedby = Objstocktake.addedby;
            dto.DateAdded = (DateTime)Objstocktake.DateAdded;
            dto.companayname = invoiceFormat;

            List<StockTakeDetailsDto> materialsDtos = new List<StockTakeDetailsDto>();
            foreach (var item in materials)
            {
                //var selectedProduct = db.Products.Find(i => i.Id == item.ProductId);

                StockTakeDetailsDto stk = new StockTakeDetailsDto();
                stk.ProductId = item.productName;

                stk.actualquantity = item.actualquantity;
                stk.counted = item.counted;
                stk.variance = item.variance;
                stk.variancevalue = item.variancevalue;
                stk.WarehouseId = mywarehouse;
                materialsDtos.Add(stk);
            }

            dto.StockTakeDetails = materialsDtos;
            return View(dto);
        }
        public ActionResult GetGridList()
        {
            try
            {
                if (warehouses == 1)
                {
                    var tak = db.StockTakes.ToArray();
                    var result = from c in tak
                                 select new string[] { c.Id.ToString(),
            Convert.ToString(db.Users.FirstOrDefault(n => n.Id==c.addedby).UserName),
            Convert.ToString(c.DateAdded),
            Convert.ToString(db.Warehouses.FirstOrDefault(n=> n.Id==c.WarehouseId).Name),



             };
                    return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var tak = db.StockTakes.ToArray();
                    var result = from c in tak.Where(m => m.WarehouseId == warehouses)
                                 select new string[] { c.Id.ToString(),
            Convert.ToString(db.Users.FirstOrDefault(n => n.Id==c.addedby).UserName),
            Convert.ToString(c.DateAdded),
            Convert.ToString(db.Warehouses.FirstOrDefault(n=> n.Id==c.WarehouseId).Name),



             };
                    return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);


                }



            }
            catch (NullReferenceException ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View(ex.Message);
            }
        }
        // GET: StockTake/Create
        public ActionResult Create()
        {
            var userwarehouse = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;
            if (userwarehouse == 1)
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            }
            else
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(n => n.Id == userwarehouse), "Id", "Name");
            }

            return View();
        }

        // get grid
        public ActionResult GetGrid()
        {
            var myWarehouse = Convert.ToInt32(warehouses);
            try
            {
                var tak = db.WarehouseStocks.ToArray();
                var result = from c in tak.Where(n => n.WarehouseId == myWarehouse && n.Product_ProductId.IsActive == true)
                             select new string[] { c.ProductId.ToString(),
            Convert.ToString(c.Product_ProductId.Name),
           // Convert.ToString(c.Product_ProductId.ProductCategory_ProductCategoryId.Name),
            Convert.ToString(c.Product_ProductId.BarCode),
              //Convert.ToString(c.RemainingQuantity),
          // Convert.ToString(c.PurchasePrice),
                
                

             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);




            }
            catch (NullReferenceException ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View(ex.Message);
            }
        }
        public void Savestocktake(DataTable tb)
        {
            string constring = System.Configuration.ConfigurationManager.ConnectionStrings["SIConnectionString"]
                  .ConnectionString;
            DataTable dt = new DataTable();
            if (dt.Rows.Count > 0)
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name
                        sqlBulkCopy.DestinationTableName = "dbo.StockTakes";

                        //[OPTIONAL]: Map the DataTable columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("ProductCategoryId", "Category");
                        sqlBulkCopy.ColumnMappings.Add("ProductId", "Serial Number");
                        sqlBulkCopy.ColumnMappings.Add("actualquantity", "Actual Quantity");
                        sqlBulkCopy.ColumnMappings.Add("counted", "Counted Quantity");
                        //  sqlBulkCopy.ColumnMappings.Add("addedby", "Name");
                        sqlBulkCopy.ColumnMappings.Add("variancevalue", "Variance");
                        // sqlBulkCopy.ColumnMappings.Add("DateAdded", "CustomerId");
                        //sqlBulkCopy.ColumnMappings.Add("WarehouseId", "Name");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }
        }
        // POST: StockTake/Create
        [HttpPost]
        public ActionResult Create(WarehouseStockTake[] ObjStocktake)
        {
            string result = "Error! Purchase  Is Not Complete!";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int CustomerName = db.Users.FirstOrDefault(i => i.UserName == "Customer").Id;
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            var warehouse = db.Users.FirstOrDefault(n => n.Id == AddedBy).WarehouseId;
            var PaymentMode = "";
            try
            {
                //if (ModelState.IsValid)
                //{
                //Invoice inv = new Invoice();
                //inv.AddedBy = AddedBy;
                //inv.DateAdded = DateTime.Now;
                //inv.DateModied = DateTime.Now;
                //inv.IsBilled = false;
                //inv.IsPurchaseOrSale = "Sale";
                //inv.ModifiedBy = AddedBy;
                //inv.UserId = CustomerName;
                //inv.WarehouseId = (int)warehouse;
                //db.Invoices.Add(inv);

                StockTake OBJStockTakemain = new StockTake();
                //OBJStockTakemain.ProductCategoryId = selectedProduct.ProductCategoryId;
                //OBJStockTakemain.ProductId = selectedProduct.Id;
                OBJStockTakemain.WarehouseId = (int)warehouse;
                //OBJStockTakemain.counted = item.counted;
                //OBJStockTakemain.actualquantity = item.actualquantity;
                //OBJStockTakemain.variancevalue = Quantity;
                OBJStockTakemain.addedby = AddedBy;
                OBJStockTakemain.DateAdded = DateTime.Now;
                db.StockTakes.Add(OBJStockTakemain);

                db.SaveChanges(userId);



                foreach (var item in ObjStocktake)
                {
                    int newactualquantity = item.counted;
                    var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.ProductId && i.WarehouseId == warehouse);
                    var selectedWarehouseProduct = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.ProductId && i.WarehouseId == warehouse);

                    StockTakeDetails OBJStockTake = new StockTakeDetails();
                    //OBJStockTake.ProductCategoryId = selectedProduct.ProductCategoryId;
                    OBJStockTake.ProductId = selectedProduct.Id;
                    OBJStockTake.productName = selectedProduct.Name;
                    OBJStockTake.WarehouseId = (int)warehouse;
                    OBJStockTake.counted = item.counted;
                    OBJStockTake.actualquantity = selectedWarehouseProduct.RemainingQuantity;
                    OBJStockTake.variance = OBJStockTake.counted + OBJStockTake.actualquantity;
                    OBJStockTake.variancevalue = selectedProduct.SalePrice * OBJStockTake.variance;
                    OBJStockTake.StockTakeId = OBJStockTakemain.Id;
                    OBJStockTake.DateAdded = DateTime.Now;
                    db.StockTakeDetail.Add(OBJStockTake);
                    db.SaveChanges(userId);
                    //here


                    //selectedWarehouseProduct.RemainingQuantity = newactualquantity;
                    //db.Entry(selectedWarehouseProduct).State = EntityState.Modified;
                    //db.SaveChanges(userId);
                    sb.Append("Sumitted");
                    //return Content(sb.ToString());
                    result = "Success! Purchase Completed";

                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            //return Content(sb.ToString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: StockTake/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: StockTake/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }

        // GET: StockTake/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StockTake/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }
        public class WarehouseStockTake
        {
            public int ProductId { get; set; }
            public decimal actualquantity { get; set; }
            public int counted { get; set; }
            public int purchase { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult> UpdateFile(HttpPostedFileBase importFile)
        {
            int UserId = db.Users.FirstOrDefault(n => n.UserName == userId).Id;
            int wareid = int.Parse(Env.GetUserInfo("WarehouseId"));
            if (importFile == null)
            {
                return Json(new { Status = 0, Message = "No File Selected" });
            }

            try
            {
                var fileData = GetDataFromCSVFile(importFile.InputStream);




                StockTake stck = new StockTake();
                stck.addedby = UserId;
                stck.WarehouseId = wareid;
                stck.DateAdded = DateTime.Now;
                stck.WarehouseId = warehouses;
                db.StockTakes.Add(stck);
                db.SaveChanges();




                foreach (Product product in fileData)
                {
                    StockTakeDetails updateProduct = new StockTakeDetails();
                    Product price = db.Products.Where(i => i.Id == product.Id).FirstOrDefault();
                    var remaining = db.WarehouseStocks.FirstOrDefault(m => m.ProductId == product.Id && m.WarehouseId == wareid);
                    //if (updateProduct != null)
                    {

                        //  if (updateProduct.Id == product.Id && updateProduct.WarehouseId == product.WarehouseId)


                        updateProduct.ProductId = product.Id;
                        updateProduct.productName = product.Name;
                        updateProduct.actualquantity = remaining.RemainingQuantity;
                        updateProduct.counted = product.RemainingQuantity;
                        updateProduct.StockTakeId = stck.Id;
                        updateProduct.variance = updateProduct.actualquantity - updateProduct.counted;
                        updateProduct.variancevalue = updateProduct.variance * price.SalePrice;
                        updateProduct.WarehouseId = wareid;
                        updateProduct.DateAdded = DateTime.Now;
                        db.StockTakeDetail.Add(updateProduct);
                        db.SaveChanges();



                        ProductStock ps = new ProductStock();
                        ps.ProductId = product.Id;
                        ps.Quantity = product.RemainingQuantity - remaining.RemainingQuantity;

                        ps.PurchasePrice = product.PurchasePrice;

                        ps.TotalPurchaseAmount = (product.PurchasePrice * ps.Quantity);

                        ps.SalePrice = product.SalePrice;
                        ps.Discount = 0;
                        ps.TotalSaleAmount = (ps.SalePrice * ps.Quantity);

                        decimal TaxAmount = 0;

                        ps.ProductName = product.Name;
                        ps.TotalSaleAmountWithTax = (ps.SalePrice * ps.Quantity);//+ TaxAmount
                        ps.TaxAmount = TaxAmount;

                        //  ps.Profit = (ps.TotalSaleAmount - (ps.TotalPurchaseAmount)) - (discount / mysellCount.Count);//+ TaxAmount
                        ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount

                        ps.Description = "Stock Take Import";
                        ps.AddedBy = UserId;
                        ps.DateAdded = DateTime.Now;
                        ps.ModifiedBy = UserId;
                        ps.DateModied = DateTime.Now;
                        ps.InventoryTypeId = 6;
                        ps.WarehouseId = product.WarehouseId;
                        ps.IsFormal = true;
                        ps.RemainingQuantity = product.RemainingQuantity;
                        //         ps.ProductBatchId = db.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;
                        db.ProductStock.Add(ps);
                        db.SaveChanges();
                        //sb.Append("Sumitted");

                        remaining.RemainingQuantity = updateProduct.counted;
                        db.Entry(remaining).State = EntityState.Modified;
                        db.SaveChanges(userId);

                    }
                }


                return Json(new { Status = 1, Message = "File Imported Successfully ", items = fileData.ToArray() });


            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message });
            }
        }

        private List<Product> GetDataFromCSVFile(Stream stream)
        {
            var empList = new List<Product>();
            try
            {
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                {
                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true // To set First Row As Column Names    
                        }
                    });

                    if (dataSet.Tables.Count > 0)
                    {
                        var dataTable = dataSet.Tables[0];
                        foreach (DataRow objDataRow in dataTable.Rows)
                        {
                            if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                            empList.Add(new Product()
                            {
                                Id = Convert.ToInt16(objDataRow["Product Id"].ToString()),
                                Name = Convert.ToString(objDataRow["Product"].ToString()),
                                BarCode = Convert.ToString(objDataRow["BarCode"].ToString()),
                                RemainingQuantity = Convert.ToDecimal(objDataRow["Counted Quantity"].ToString()),

                                //Ngoni to add spefic parameters for the product model

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // return Json(new { Status = 0, Message = ex.Message });
                throw;
            }
            return empList;
        }





    }
}
