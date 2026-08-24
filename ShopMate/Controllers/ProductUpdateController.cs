using ExcelDataReader;
using Newtonsoft.Json.Linq;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class ProductUpdateController : BaseController
    {

        private SIContext db = new SIContext();
        string warehouse = Env.GetUserInfo("WarehouseId");
        string userId = Env.GetUserInfo("name");
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: ProductUpdate
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CustomerImport()
        {
            return View();
        }
        public ActionResult SupplierImport()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateFile(HttpPostedFileBase importFile)
        {
            if (importFile == null)
            {
                return Json(new { Status = 0, Message = "No File Selected" });
            }

            try
            {
                var fileData = GetDataFromCSVFile(importFile.InputStream);
                var duplicates = new JArray();
                var sellCount = new JArray();
                foreach (Product product in fileData)
                {
                    Product updateProduct = db.Products.Where(i => i.Id == product.Id).FirstOrDefault();
                    int UserId = db.Users.FirstOrDefault(n => n.UserName == userId).Id;

                    if (updateProduct != null)
                    {
                        var ngoni = db.WarehouseStocks.FirstOrDefault(n => n.ProductId == product.Id && n.WarehouseId == product.WarehouseId);
                        if (updateProduct.Id == product.Id && updateProduct.WarehouseId == product.WarehouseId)
                        {

                            //var isSellAvailable = db.Products.Where(i => i.Name == product.Name && i.WarehouseId == product.WarehouseId).Count();

                            //if (isSellAvailable == 1)
                            //{

                            //}
                            //else
                            //{
                            //    duplicates.Add(Convert.ToString(new { Name = product.Name, prodId = product.Id.ToString(), }));
                            //    updateProduct.IsActive = false;
                            //    db.Entry(updateProduct).State = EntityState.Modified;
                            //    db.SaveChanges();

                            //}





                            updateProduct.Id = product.Id;
                            updateProduct.Name = product.Name;
                            updateProduct.BarCode = product.BarCode;
                            updateProduct.SalePrice = product.SalePrice;
                            updateProduct.HSNCode = product.HSNCode;
                            updateProduct.ProductDescription = product.ProductDescription;
                            updateProduct.PurchasePrice = product.PurchasePrice;
                             updateProduct.TaxId = product.TaxId;
                            updateProduct.IsActive = true;
                            // 

                            db.Entry(updateProduct).State = EntityState.Modified;
                            db.SaveChanges();

                            WarehouseStock updateStock = db.WarehouseStocks.Where(m => m.ProductId == product.Id).FirstOrDefault();
                            updateStock.WarehouseId = product.WarehouseId;
                            updateStock.RemainingQuantity = product.RemainingQuantity;
                            db.Entry(updateStock).State = EntityState.Modified;
                            db.SaveChanges();

                            //ProductStock ps = new ProductStock();
                            //ps.ProductId = product.Id;
                            //ps.Quantity = product.RemainingQuantity - ngoni.RemainingQuantity;
                            //ps.ProductName = product.Name;
                            //ps.PurchasePrice = product.PurchasePrice;

                            //ps.TotalPurchaseAmount = (product.PurchasePrice * ps.Quantity);

                            //ps.SalePrice = product.SalePrice;
                            //ps.Discount = 0;
                            //ps.TotalSaleAmount = (ps.SalePrice * ps.Quantity);

                            //decimal TaxAmount = 0;


                            //ps.TotalSaleAmountWithTax = (ps.SalePrice * ps.Quantity);//+ TaxAmount
                            //ps.TaxAmount = TaxAmount;

                            ////  ps.Profit = (ps.TotalSaleAmount - (ps.TotalPurchaseAmount)) - (discount / mysellCount.Count);//+ TaxAmount
                            //ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount

                            //ps.Description = "Product Import";
                            //ps.AddedBy = UserId;
                            //ps.DateAdded = DateTime.Now;
                            //ps.ModifiedBy = UserId;
                            //ps.DateModied = DateTime.Now;
                            //ps.InventoryTypeId = 5;
                            //ps.WarehouseId = product.WarehouseId;
                            //ps.IsFormal = true;
                            //ps.RemainingQuantity = product.RemainingQuantity;
                            ////ps.ProductBatchId = db.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;
                            //db.ProductStock.Add(ps);
                            //db.SaveChanges();


                        }
                        else
                        {
                            Product newprod = new Product();
                            //  int UserId = db.Users.FirstOrDefault(n => n.UserName == userId).Id;
                            // newprod.Id = product.Id;
                            newprod.Name = product.Name;
                            newprod.BarCode = product.BarCode;
                            newprod.SalePrice = product.SalePrice;
                            newprod.ProductDescription = product.ProductDescription;
                            newprod.PurchasePrice = product.PurchasePrice;
                            newprod.IsActive = true;
                            newprod.AddedBy = UserId;
                            newprod.WarehouseId = product.WarehouseId;
                            newprod.HSNCode = product.HSNCode;
                            newprod.StockAlert = 10;
                            newprod.ProductCategoryId = 1;
                            newprod.NumOfSinglesInCase = 0;
                            newprod.DateAdded = DateTime.Now;
                            newprod.DateModied = DateTime.Now;
                            newprod.TaxId =product.TaxId;
                            newprod.ProductCaseId = 0;
                            db.Products.Add(newprod);
                            db.SaveChanges();



                            ProductStock ps = new ProductStock();
                            ps.ProductId = product.Id;
                            ps.Quantity = product.RemainingQuantity;

                            ps.PurchasePrice = product.PurchasePrice;

                            ps.TotalPurchaseAmount = (product.PurchasePrice * ps.Quantity);
                            ps.ProductName = product.Name;
                            ps.SalePrice = product.SalePrice;
                            ps.Discount = 0;
                            ps.TotalSaleAmount = (ps.SalePrice * ps.Quantity);

                            decimal TaxAmount = 0;


                            ps.TotalSaleAmountWithTax = (ps.SalePrice * ps.Quantity);//+ TaxAmount
                            ps.TaxAmount = TaxAmount;

                            //  ps.Profit = (ps.TotalSaleAmount - (ps.TotalPurchaseAmount)) - (discount / mysellCount.Count);//+ TaxAmount
                            ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount

                            ps.Description = "Product Import";
                            ps.AddedBy = UserId;
                            ps.DateAdded = DateTime.Now;
                            ps.ModifiedBy = UserId;
                            ps.DateModied = DateTime.Now;
                            ps.InventoryTypeId = 5;
                            ps.WarehouseId = product.WarehouseId;
                            ps.IsFormal = true;
                            ps.RemainingQuantity = product.RemainingQuantity;
                            //        ps.ProductBatchId = db.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;
                            db.ProductStock.Add(ps);
                            db.SaveChanges();

                            WarehouseStock newProduct = new WarehouseStock();
                            newProduct.ProductId = newprod.Id;
                            newProduct.WarehouseId = newprod.WarehouseId;
                            newProduct.RemainingQuantity = product.RemainingQuantity;
                            // newProduct.r = 0;
                            db.WarehouseStocks.Add(newProduct);
                            db.SaveChanges();
                        }

                    }
                    else
                    {
                        Product newprod = new Product();
                        //int UserId = db.Users.FirstOrDefault(n => n.UserName == userId).Id;
                        newprod.Id = product.Id;
                        newprod.Name = product.Name;
                        newprod.BarCode = product.BarCode;
                        newprod.SalePrice = product.SalePrice;
                        newprod.ProductDescription = product.ProductDescription;
                        newprod.PurchasePrice = product.PurchasePrice;
                        newprod.IsActive = true;
                        newprod.AddedBy = UserId;
                        newprod.WarehouseId = product.WarehouseId;
                        newprod.StockAlert = 10;
                        newprod.ProductCategoryId = 1;
                        newprod.NumOfSinglesInCase = 0;
                        newprod.DateAdded = DateTime.Now;
                        newprod.DateModied = DateTime.Now;
                        newprod.TaxId = product.TaxId;
                        newprod.ProductCaseId = 0;
                        newprod.HSNCode = product.HSNCode;
                        db.Products.Add(newprod);





                        ProductStock ps = new ProductStock();
                        ps.ProductId = product.Id;


                        ps.Quantity = product.RemainingQuantity;

                        ps.PurchasePrice = product.PurchasePrice;
                        ps.ProductName = product.Name;
                        ps.TotalPurchaseAmount = (product.PurchasePrice * ps.Quantity);

                        ps.SalePrice = product.SalePrice;
                        ps.Discount = 0;
                        ps.TotalSaleAmount = (ps.SalePrice * ps.Quantity);

                        decimal TaxAmount = 0;


                        ps.TotalSaleAmountWithTax = (ps.SalePrice * ps.Quantity);//+ TaxAmount
                        ps.TaxAmount = TaxAmount;

                        //  ps.Profit = (ps.TotalSaleAmount - (ps.TotalPurchaseAmount)) - (discount / mysellCount.Count);//+ TaxAmount
                        ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount

                        ps.Description = "Product Import";
                        ps.AddedBy = UserId;
                        ps.DateAdded = DateTime.Now;
                        ps.ModifiedBy = UserId;
                        ps.DateModied = DateTime.Now;
                        ps.InventoryTypeId = 5;
                        ps.WarehouseId = product.WarehouseId;
                        ps.IsFormal = true;
                        ps.RemainingQuantity = product.RemainingQuantity;
                        //    ps.ProductBatchId = db.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;
                        db.ProductStock.Add(ps);
                        db.SaveChanges();


                        WarehouseStock newProduct = new WarehouseStock();
                        newProduct.ProductId = newprod.Id;
                        newProduct.WarehouseId = newprod.WarehouseId;
                        newProduct.RemainingQuantity = product.RemainingQuantity;
                        //newProduct. = 0;
                        db.WarehouseStocks.Add(newProduct);
                        db.SaveChanges();
                    }




                }
                Helper.WriteDebug(new Exception(), duplicates.ToString());
                return Json(new { Status = 1, Message = "File Imported Successfully ", items = fileData.ToArray() });

                //var dtProducts = fileData.ToDataTable();
                //var tblProductParameter = new SqlParameter("Product", SqlDbType.Structured)
                //{
                //    TypeName = "dbo.Product",
                //    Value = dtProducts
                //};
                //await db.Database.ExecuteSqlCommandAsync("EXEC spBulkImportProduct @Product", tblProductParameter);
                //return Json(new { Status = 1, Message = "File Imported Successfully " });
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
                                Id = Convert.ToInt16(objDataRow["Id"].ToString()),
                                Name = Convert.ToString(objDataRow["Name"].ToString()),
                                BarCode = Convert.ToString(objDataRow["Bar Code"].ToString()),
                                HSNCode = Convert.ToString(objDataRow["HSN Code"].ToString()),
                                SalePrice = Convert.ToDecimal(objDataRow["Sale Price"].ToString()),
                                PurchasePrice = Convert.ToDecimal(objDataRow["Purchase Price"].ToString()),
                                  TaxId = Convert.ToInt16(objDataRow["TaxId"].ToString()),
                                RemainingQuantity = Convert.ToDecimal(objDataRow["RemainingQuantity"].ToString()),
                                ProductDescription = Convert.ToString(objDataRow["Product Description"].ToString()),
                                WarehouseId = Convert.ToInt16(objDataRow["Warehouse Id"].ToString()),
                                // productType = Convert.ToString(objDataRow["Product Type"].ToString())
                                //Ngoni to add spefic parameters for the product model

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return empList;
        }
        //public class ProductPriceInsert
        //{
        //    public int Id { get; set; }
        //    //[Required]
        //    public string Name { get; set; }
        //    public string BarCode { get; set; }//ngoni
        //    public Decimal SalePrice { get; set; }
        //    public Decimal PurchasePrice { get; set; }//ngoni
        //    public Decimal RtgsPrice { get; set; }//ngoni         
        //    public Decimal RemainingQuantity { get; set; }

        //    public int WarehouseId { get; set; }
        //    public string ProductDescription { get; set; }
        //}


        [HttpPost]
    /*    public async Task<ActionResult> UpdateFileCustomer(HttpPostedFileBase importFile)
        {
            if (importFile == null)
            {
                return Json(new { Status = 0, Message = "No File Selected" });
            }

            try
            {
                var fileData = GetDataFromCSVFileCustomer(importFile.InputStream);
                int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
                //    var warehouses = db.Warehouses.FirstOrDefault(n => n.Name == warehouse).Id;
                foreach (CustomerInsert product in fileData)
                {
                    Customers updateProduct = db.Customers.Find(product.Id);
                    if (updateProduct != null)
                    {
                        updateProduct.Id = product.Id;
                        updateProduct.UserName = product.UserName;
                        updateProduct.FullName = product.FullName;
                        updateProduct.Mobile = product.Mobile;
                        updateProduct.Email = "test@gmail";
                        updateProduct.Address = product.Address;
                        updateProduct.vatNumber = product.vatNumber;
                        updateProduct.WarehouseId = warehouse;
                        updateProduct.IsActive = product.IsActive;
                        db.Entry(updateProduct).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        updateProduct.Id = product.Id;
                        updateProduct.UserName = product.UserName;
                        updateProduct.FullName = product.FullName;
                        updateProduct.Mobile = product.Mobile;
                        updateProduct.Email = "test@gmail";
                        updateProduct.Address = product.Address;
                        updateProduct.About = product.UserName;
                        updateProduct.JoinDate = DateTime.Now;
                        updateProduct.vatNumber = product.vatNumber;
                        updateProduct.WarehouseId = warehouse;
                        updateProduct.IsActive = product.IsActive;
                        //db.Entry(updateProduct).State = EntityState.Modified;
                        db.Customers.Add(updateProduct);
                        db.SaveChanges();
                    }


                }

                return Json(new { Status = 1, Message = "File Imported Successfully ", items = fileData.ToArray() });

                //var dtProducts = fileData.ToDataTable();
                //var tblProductParameter = new SqlParameter("Product", SqlDbType.Structured)
                //{
                //    TypeName = "dbo.Product",
                //    Value = dtProducts
                //};
                //await db.Database.ExecuteSqlCommandAsync("EXEC spBulkImportProduct @Product", tblProductParameter);
                //return Json(new { Status = 1, Message = "File Imported Successfully " });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message });
            }
        }
*/
        private List<CustomerInsert> GetDataFromCSVFileCustomer(Stream stream)
        {
            var empList = new List<CustomerInsert>();
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
                            empList.Add(new CustomerInsert()
                            {
                                Id = Convert.ToInt16(objDataRow["Id"].ToString()),
                                UserName = Convert.ToString(objDataRow["User Name"].ToString()),
                                FullName = Convert.ToString(objDataRow["Full Name"].ToString()),
                                Mobile = Convert.ToString(objDataRow["Mobile"].ToString()),
                                //Email = Convert.ToString(objDataRow["Literage"].ToString()),
                                Address = Convert.ToString(objDataRow["Address"].ToString()),
                                IsActive = Convert.ToBoolean(objDataRow["Is Active"].ToString()),
                                vatNumber = Convert.ToString(objDataRow["Vat Number"].ToString()),
                                //  warehouseId = Convert.ToInt32(objDataRow[""].ToString())
                                //Ngoni to add spefic parameters for the product model

                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return empList;
        }


        public class CustomerInsert
        {
            public int Id { get; set; }
            //[Required]
            public string UserName { get; set; }

            public string FullName { get; set; }//ngoni

            public string Mobile { get; set; }


            //public string Email { get; set; }//ngoni

            public Boolean IsActive { get; set; }//ngoni
            public string vatNumber { get; set; }
            //public int warehouseId { get; set; }
            public string Address { get; set; }
        }



        [HttpPost]
        public async Task<ActionResult> UpdateFileSupplier(HttpPostedFileBase importFile)
        {
            if (importFile == null)
            {
                return Json(new { Status = 0, Message = "No File Selected" });
            }

            try
            {
                var fileData = GetDataFromCSVFileSuplier(importFile.InputStream);

                foreach (SupplierInsert product in fileData)
                {

                    Vendor updateProduct = db.Vendors.Find(product.Id);
                    if (updateProduct != null)
                    {
                        updateProduct.Id = product.Id;
                        updateProduct.UserName = product.UserName;
                        updateProduct.FullName = product.FullName;
                        updateProduct.Mobile = product.Mobile;
                        updateProduct.Email = product.Email;
                        updateProduct.Address = product.Address;
                        updateProduct.vatNumber = product.vatNumber;
                        updateProduct.WarehouseId = product.warehouseId;
                        updateProduct.IsActive = product.IsActive;
                        db.Entry(updateProduct).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        updateProduct.Id = product.Id;
                        updateProduct.UserName = product.UserName;
                        updateProduct.FullName = product.FullName;
                        updateProduct.Mobile = product.Mobile;
                        updateProduct.Email = product.Email;
                        updateProduct.Address = product.Address;
                        updateProduct.About = product.UserName;
                        updateProduct.JoinDate = DateTime.Now;
                        updateProduct.vatNumber = product.vatNumber;
                        updateProduct.WarehouseId = product.warehouseId;
                        updateProduct.IsActive = product.IsActive;
                        //db.Entry(updateProduct).State = EntityState.Modified;
                        db.Vendors.Add(updateProduct);
                        db.SaveChanges();
                    }


                }

                return Json(new { Status = 1, Message = "File Imported Successfully ", items = fileData.ToArray() });

                //var dtProducts = fileData.ToDataTable();
                //var tblProductParameter = new SqlParameter("Product", SqlDbType.Structured)
                //{
                //    TypeName = "dbo.Product",
                //    Value = dtProducts
                //};
                //await db.Database.ExecuteSqlCommandAsync("EXEC spBulkImportProduct @Product", tblProductParameter);
                //return Json(new { Status = 1, Message = "File Imported Successfully " });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message });
            }
        }

        private List<SupplierInsert> GetDataFromCSVFileSuplier(Stream stream)
        {
            var empList = new List<SupplierInsert>();
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
                            empList.Add(new SupplierInsert()
                            {
                                Id = Convert.ToInt16(objDataRow["Id"].ToString()),
                                UserName = Convert.ToString(objDataRow["Name"].ToString()),
                                FullName = Convert.ToString(objDataRow["Bar Code"].ToString()),
                                Mobile = Convert.ToString(objDataRow["Sale USD Price"].ToString()),
                                Email = Convert.ToString(objDataRow["Literage"].ToString()),
                                Address = Convert.ToString(objDataRow["Sale Rtgs Price"].ToString()),
                                IsActive = Convert.ToBoolean(objDataRow["Event Usd Price"].ToString()),
                                vatNumber = Convert.ToString(objDataRow["Event Rtgs Price"].ToString()),
                                warehouseId = Convert.ToInt32(objDataRow[""].ToString())
                                //Ngoni to add spefic parameters for the product model

                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return empList;
        }


        public class SupplierInsert
        {
            public int Id { get; set; }
            //[Required]
            public string UserName { get; set; }

            public string FullName { get; set; }//ngoni

            public string Mobile { get; set; }


            public string Email { get; set; }//ngoni

            public Boolean IsActive { get; set; }//ngoni
            public string vatNumber { get; set; }
            public int warehouseId { get; set; }
            public string Address { get; set; }
        }

    }
}