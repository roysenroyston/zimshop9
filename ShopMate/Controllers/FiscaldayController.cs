using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;
using System.Web.Http;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;

namespace ShopMate.Controllers
{
    public class FiscaldayController : Controller
    {
        private SIContext db = new SIContext();
        int AddedBy = int.Parse(Env.GetUserInfo("userid"));
        int warehouses = int.Parse(Env.GetUserInfo("WarehouseId"));
        // GET: Fiscalday
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult OpenCloseDay()
        {
            return View();
        }


    
        public async Task<ActionResult> OpenCloseDayAsync(string Day)
        {
            if (Day == null)
            {
                return Json(new { Status = 0, Message = "No File Selected" });
            }

            try
            {
                string result = "";
                string apiUrl = "";
                // URL of the external server's API endpoint for creating a new entry
                var DeviceId = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouses).DeviceId;
                DeviceId = "17045";

                
                if (Day=="OpenDay")
                {
                    apiUrl = "http://griffintest.pythonanywhere.com/api/open-day-v1/api-v1/" + DeviceId + "/";


                    var json = JsonConvert.SerializeObject(new { day = Day }, Formatting.Indented);
                    using (var HttpClient = new HttpClient())
                    {
                        // Set the content type
                       StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                        // Send a POST request to the external server
                         var response = await HttpClient.PostAsync(apiUrl, null);
                      //  HttpResponseMessage response = await HttpClient.SendAsync(apiUrl, null);

                        System.Diagnostics.Debug.WriteLine("Test1 : " + response.IsSuccessStatusCode);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            Helper.WriteInformation(new Exception(), responseData.ToString());

                            try
                            {
                                string jsonFilePath = responseData;

                                // Read the JSON content from the file
                                //string jsonContent = System.IO.File.ReadAllText(jsonFilePath);

                                // Deserialize JSON content into the JsonModel class
                                OpenDay deserializedData = JsonConvert.DeserializeObject<OpenDay>(responseData);

                                Helper.WriteInformation(new Exception(), deserializedData.ToString());
                              //  foreach (var myitems in recieptItems)
                                {
                                    fiscalday myitems = new fiscalday();
                                    myitems.DeviceId = Convert.ToInt32(DeviceId);
                                    myitems.DateOpened = DateTime.Now;
                                  //  myitems.DateClosed = deserializedData.receiptID;
                                    myitems.OperationId = deserializedData.operationID;
                                    myitems.WarehouseId = warehouses;
                                    myitems.IsOpen = true;
                                    myitems.FiscalStatus = "FiscalDayOpened";
                                    myitems.AddedBy = AddedBy;
                                    myitems.FiscalDayNo = deserializedData.fiscalDayNo;
                            
                                    db.Fiscaldays.Add(myitems);
                                    db.SaveChanges();

                                    result = "Day Opened Successfully ";
                                }
                            
                            }
                            catch (Exception ex)
                            {
                                return Json(new { Status = 0, Message = ex.Message });
                            }
                        }
                        else
                        {
                            Helper.WriteInformation(new Exception(), response.StatusCode.ToString());
                            result = "Open Day Failed ";
                            //   return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                            return Json(new { Status = 0, Message = result });
                            // return Request.CreateResponse(HttpStatusCode.OK, result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { Status = 1, Message = result, });
                }
                else
                {
                    apiUrl = "http://griffintest.pythonanywhere.com/api/api/close-day/api-v1/" + DeviceId + "/";
                    var json = JsonConvert.SerializeObject(new { day = Day }, Formatting.Indented);
                    using (var HttpClient = new HttpClient())
                    {
                        // Set the content type
                        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                        // Send a POST request to the external server
                        //var response = await HttpClient.PostAsync(apiUrl);
                        HttpResponseMessage response = await HttpClient.GetAsync(apiUrl);
                        System.Diagnostics.Debug.WriteLine("Test1 : " + response.IsSuccessStatusCode);

                        if (response.IsSuccessStatusCode)
                        {
                           var apiUrl1 = "http://griffintest.pythonanywhere.com/api/get-status-v1/api-v1/" + DeviceId + "/";
                            HttpResponseMessage response1 = await HttpClient.GetAsync(apiUrl1);
                            string responseData1 = await response.Content.ReadAsStringAsync();
                            string jsonFilePath = responseData1;
                            if (response.IsSuccessStatusCode)
                            {
                                GetStatus deserializedData = JsonConvert.DeserializeObject<GetStatus>(responseData1);
                                fiscalday myitems = new fiscalday();
                                    myitems.DeviceId = Convert.ToInt32(DeviceId);
                                    myitems.DateOpened = DateTime.Now;
                                    //  myitems.DateClosed = deserializedData.receiptID;
                                    myitems.OperationId = deserializedData.operationID;
                                    myitems.WarehouseId = warehouses;
                                myitems.IsOpen = false;
                                if (deserializedData.fiscalDayStatus== "FiscalDayCloseFailed")
                                {
                                    myitems.IsOpen = true;
                                }
                                  
                                    myitems.FiscalStatus = deserializedData.fiscalDayStatus;
                                    myitems.AddedBy = AddedBy;
                                    myitems.FiscalDayNo = deserializedData.lastFiscalDayNo;

                                    db.Fiscaldays.Add(myitems);
                                    db.SaveChanges();

                                    result = "Day Opened Successfully ";
                                
                            }
                                //string responseData = await response.Content.ReadAsStringAsync();
                                //Helper.WriteInformation(new Exception(), responseData.ToString());

                                //try
                                //{
                                //    string jsonFilePath = responseData;

                                //    // Read the JSON content from the file
                                //    //string jsonContent = System.IO.File.ReadAllText(jsonFilePath);

                                //    // Deserialize JSON content into the JsonModel class
                                //    OpenDay deserializedData = JsonConvert.DeserializeObject<OpenDay>(responseData);

                                //    Helper.WriteInformation(new Exception(), deserializedData.ToString());
                                //    //  foreach (var myitems in recieptItems)
                                //    {
                                //        fiscalday myitems = new fiscalday();
                                //        myitems.DeviceId = Convert.ToInt32(DeviceId);
                                //        myitems.DateOpened = DateTime.Now;
                                //        //  myitems.DateClosed = deserializedData.receiptID;
                                //        myitems.OperationId = deserializedData.operationID;
                                //        myitems.WarehouseId = warehouses;
                                //        myitems.IsOpen = true;
                                //        myitems.FiscalStatus = "FiscalDayOpened";
                                //        myitems.AddedBy = AddedBy;
                                //        myitems.FiscalDayNo = deserializedData.fiscalDayNo;

                                //        db.Fiscaldays.Add(myitems);
                                //        db.SaveChanges();

                                //        result = "Day Opened Successfully ";
                                //    }

                                //}
                                //catch (Exception ex)
                                //{
                                //    return Json(new { Status = 0, Message = ex.Message });
                                //}
                            }
                        else
                        {
                            Helper.WriteInformation(new Exception(), response.StatusCode.ToString());
                            result = "Close Day Failed ";
                            //   return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                            return Json(new { Status = 0, Message = result });
                            // return Request.CreateResponse(HttpStatusCode.OK, result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { Status = 1, Message = result, });
                }
                //var fileData = GetDataFromCSVFile(importFile.InputStream);

              //  foreach (Product product in fileData)
             //if   {
             //       int warehouses = WarehousId;
             //       fiscalday updateProduct = db.Fiscaldays.Where(i => i.WarehouseId == warehouses).FirstOrDefault();
             //       //     int wareid = db.Warehouses.FirstOrDefault(k => k.Name == product.ProductType).Id;
             //       if (updateProduct != null)
             //       {

             //           if (updateProduct.Id == product.Id)
             //           {

             //               updateProduct.Id = product.Id;
             //               updateProduct.Name = product.Name;
             //               updateProduct.BarCode = product.BarCode;
             //               updateProduct.SalePrice = product.SalePrice;
             //               updateProduct.ProductDescription = product.ProductDescription;
             //               updateProduct.PurchasePrice = product.PurchasePrice;

             //               db.Entry(updateProduct).State = EntityState.Modified;
             //               db.SaveChanges();

             //               WarehouseStock stock = db.WarehouseStocks.FirstOrDefault(b => b.ProductId == product.Id && b.WarehouseId == warehouses);
             //               stock.RemainingQuantity = product.RemainingQuantity;
             //               db.Entry(stock).State = EntityState.Modified;
             //               db.SaveChanges();
             //           }
                      
             //       }
     


                //}

               

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





        public ActionResult GetGrid()
        {
            try
            {
                var tak = db.Fiscaldays.ToArray();
                // var tax =  db.Taxs;
                var user = db.Users.ToArray();
                var tax = db.Taxs.ToArray();

                var result = from c in tak.Where(j => j.WarehouseId == warehouses)
                           
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.DeviceId),
            Convert.ToString(c.FiscalDayNo),
            Convert.ToString(c.OperationId),
            Convert.ToString(c.FiscalStatus),
            Convert.ToString(c.IsOpen),
           // Convert.ToString(c.ProductImage),         
            //Convert.ToString(c.AddedBy),
            Convert.ToString(c.DateOpened),
            //Convert.ToString(db.Warehouses.FirstOrDefault(k=> k.Id ==c.WarehouseId).Name),
            //Convert.ToString(c.StockAlert),
           Convert.ToString(c.DateClosed),          
            //Convert.ToString(tax.FirstOrDefault(i=>i.Id==c.TaxId).TaxRate+" %")
                
                
                
                   // Convert.ToString(tax.FirstOrDefault(i => i.Id == c.TaxId).TaxRate + " %")


             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);




            }
            catch (NullReferenceException ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View(ex.Message);
            }
        }

    }
}