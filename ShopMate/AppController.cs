using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ShopMate.Models;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using WebErrorLogging.Utilities;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Net.Mail;

namespace ShopMate.Controllers
{
    public class AppController : ApiController
    {
        SIContext db = new SIContext();

        [Route("api/App/test")]
        [HttpPost, ActionName("test")]
        public async Task<HttpResponseMessage> test()
        {
            //string[] emails = { "trynosmuch@gmail.com", "ngonidzashe@zimhope.co.zw" };
            //var body = File.ReadAllText(HttpContext.Current.Server.MapPath("/Views/Mail/vancreate.mail.htm"));
            //body = string.Format(body, "New Van Sell : CF85-1");

            var message = new MailMessage();
            message.To.Add(new MailAddress("trynosmuch@gmail.com"));
            message.Subject = "New Van Sell";
            message.Body = "Ndiripo";
          //  System.Diagnostics.Debug.WriteLine("Email : " + email);

            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, "Email Sent : " );

        }

        [Route("api/App/login")]
        [HttpPost, ActionName("login")]
        public HttpResponseMessage login([FromBody] JObject value)
        {            
            try
            {
                string email = value["email"].ToString();
                string password = value["password"].ToString();

                User login = db.Users.FirstOrDefault(i => i.UserName == email && i.CanLogin == true);

                try
                {
                    if (BCrypt.Net.BCrypt.Verify(password, login.Password))
                    {
                      
                        if (login.RoleId == 2)
                        {                          

                            var shopdetails  = db.Warehouses.FirstOrDefault(i => i.Id == login.WarehouseId);
                            string[] paymentMethods = { "Cash", "Zipit", "Ecocash","Acl","Fbc" };
                            var rowCount = new { user = new {

                                id = login.Id,
                                name = login.FullName.ToString(),
                                warehouse = login.WarehouseId,
                                storeName = shopdetails.Name.ToString(),
                                storAddress = shopdetails.Address.ToString(),
                                storeContact = shopdetails.Mobile.ToString(),
                                payments = false,
                                specialPayment = true,
                                payMethods = true,
                                paymentMethods = JsonConvert.SerializeObject(paymentMethods),
                            }
                                
                            };


                            //var user = new string[] {
                                
                            //    login.Id.ToString(),
                            //    login.FullName.ToString(),
                            //    login.WarehouseId.ToString()
                               
                            //};
                            return Request.CreateResponse(
                                HttpStatusCode.OK,
                                rowCount,
                                JsonMediaTypeFormatter.DefaultMediaType);
                            //return Request.CreateResponse(HttpStatusCode.OK, userApp.ToString());
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Access denied you are unauthorized to access this platform");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid details please try again");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Test1 : " + ex.Message.ToString());

                    Helper.WriteError(ex, ex.Message);
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid details please try again");


                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Test1 : " + ex.Message.ToString());
                    Helper.WriteError(ex, ex.Message);
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid details please try again");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Test1 : " + ex.Message.ToString());
                Helper.WriteError(ex, ex.Message);
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid details please try again");
            }

        }


        [HttpGet, ActionName("getProducts")]
        public HttpResponseMessage getProducts(int userWarehouse)
        {

          
            System.Diagnostics.Debug.WriteLine("Test1 : " + userWarehouse);

            var stockdata = db.WarehouseStocks.Where(i => i.WarehouseId == userWarehouse );


            var res = from sd in stockdata.ToList()
                       join pd in db.Products on sd.ProductId equals pd.Id
                       where pd.IsActive == true
                       orderby pd.Name
                       select new  
                       {
                           id = pd.Id,
                           name = pd.Name,
                           price = pd.SalePrice,
                           priceRTGS = pd.RtgsPrice,
                           image = pd.ProductImage,
                           tax = db.Taxs.FirstOrDefault(i => i.Id == pd.TaxId).TaxRate,
                           barcode = pd.BarCode,
                           quantity = sd.RemainingQuantity
                       };
            //          select new  {
            //              id = pd.Id, //ID
            //pd.Name, //Product Name
            //pd.SalePrice, //Price
            //pd.ProductImage , //Image
            //db.Taxs.FirstOrDefault(i=>i.Id== pd.TaxId).TaxRate, //Tax
            // pd.BarCode, //Barcode
            // sd.RemainingQuantity //Quantity

            // }).ToList();


            System.Diagnostics.Debug.WriteLine("Test1 : " + userWarehouse);
            
            //return Request.CreateResponse(HttpStatusCode.OK, res);



            if (res.ToArray().Length != 0)
            {
                return Request.CreateResponse(
                HttpStatusCode.OK,
                res.ToList(),
                JsonMediaTypeFormatter.DefaultMediaType);

            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Product not found , please try again");
            }

            //return Request.CreateResponse<IEnumerable<string[]>>(HttpStatusCode.OK, res);

        }


        [HttpGet, ActionName("searchProduct")]
        public HttpResponseMessage searchProduct(int userWarehouse, string barcode)
        {


            System.Diagnostics.Debug.WriteLine("Test1 : " + barcode);

            var stockdata = db.WarehouseStocks.Where(i => i.WarehouseId == userWarehouse);

            var res = from sd in stockdata.ToList()
                      join pd in db.Products on sd.ProductId equals pd.Id
                      where pd.BarCode == barcode
                      where pd.IsActive == true
                      orderby pd.Name
                      select new
                      {
                          id = pd.Id,
                          name = pd.Name,
                          price = pd.SalePrice,
                          priceRTGS = pd.RtgsPrice,
                          image = pd.ProductImage,
                          tax = db.Taxs.FirstOrDefault(i => i.Id == pd.TaxId).TaxRate,
                          barcode = pd.BarCode,
                          quantity = sd.RemainingQuantity
                      };

            if(res.ToArray().Length != 0)
            {
                return Request.CreateResponse(
                HttpStatusCode.OK,
                res.ToList().Single(),
                JsonMediaTypeFormatter.DefaultMediaType);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Product not found , please try again");
            }


            

        }



        [Route("api/App/sell")]
        [HttpPost, ActionName("sell")]
        public HttpResponseMessage sell([FromBody] JObject sell)
        {
            var test = false;

            Helper.WriteDebug(new Exception(), sell["sell"].ToString());

            String value = sell["sell"].ToString();
            List<MySell> maSells = JsonConvert.DeserializeObject<List<MySell>>(value);
            var csello = new JArray();
            var available = new JArray();
            var unavailable = new JArray();
            foreach (MySell mySell in maSells)
            {
                csello.Add(mySell.paymentMethod);
                if (!test)
                {
                    User seller_user = db.Users.FirstOrDefault(i => i.Id == mySell.userId);
                    Sale ObjSale = new Models.Sale();
                    foreach (var item in mySell.products)
                    {



                        var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.prodId);

                        if (selectedProduct != null) { 

                            var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.prodId && i.WarehouseId == seller_user.WarehouseId);
                            DateTime nowDate = DateTime.Parse(mySell.date + " " + mySell.time);
                            decimal taxAmount = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId).TaxRate;

                            var isSellAvailable = db.Sales.Where(i => i.recieptNumber == mySell.invoiceId.ToString() && i.ProductId == item.prodId).Count();

                            if (isSellAvailable == 0)
                            {
                                if (mySell.currency == "USD")
                                {
                                    ObjSale.ProductId = item.prodId;
                                    ObjSale.Quantity = item.quantity;

                                    ObjSale.UnitPrice = selectedProduct.PurchasePrice * item.quantity;
                                    ObjSale.SalePrice = selectedProduct.SalePrice;
                                    ObjSale.TotalAmount = (selectedProduct.SalePrice * ObjSale.Quantity);
                                    if (taxAmount != 0)
                                    {
                                        ObjSale.TotalAmountWithTax = (selectedProduct.SalePrice + (selectedProduct.SalePrice * taxAmount / 100));
                                    }
                                    ObjSale.WarehouseId = (int)seller_user.WarehouseId;
                                    ObjSale.AddedBy = seller_user.Id;
                                    ObjSale.CustomerUserId = 3;
                                    ObjSale.DateAdded = nowDate;
                                    ObjSale.DateModied = nowDate;
                                    ObjSale.ModifiedBy = seller_user.Id;
                                    ObjSale.PaidAmount = (selectedProduct.SalePrice * ObjSale.Quantity);
                                    ObjSale.PaymentModeId = db.PaymentModes.FirstOrDefault(i => i.Name == mySell.currency).Id; /*PaymentModeId;*/
                                    ObjSale.InventoryTypeId = 2;
                                    ObjSale.isFormalSale = false;
                                }
                                else
                                {
                                    ObjSale.ProductId = item.prodId;
                                    ObjSale.Quantity = item.quantity;
                                    ObjSale.UnitPrice = selectedProduct.PurchasePrice * item.quantity;
                                    ObjSale.RtgsPrice = (decimal)selectedProduct.RtgsPrice;
                                    ObjSale.TotalRtgsAmount = (decimal)(selectedProduct.RtgsPrice * ObjSale.Quantity);
                                    if (taxAmount != 0)
                                    {
                                        ObjSale.TotalAmountWithTax = (selectedProduct.RtgsPrice + (selectedProduct.RtgsPrice * taxAmount / 100));
                                    }
                                    ObjSale.WarehouseId = (int)seller_user.WarehouseId;
                                    ObjSale.AddedBy = seller_user.Id;
                                    ObjSale.CustomerUserId = 3;
                                    ObjSale.DateAdded = nowDate;
                                    ObjSale.DateModied = nowDate;
                                    ObjSale.ModifiedBy = seller_user.Id;
                                    ObjSale.PaidRtgsAmount = (decimal)(selectedProduct.RtgsPrice * ObjSale.Quantity);
                                    ObjSale.PaymentModeId = db.PaymentModes.FirstOrDefault(i => i.Name == mySell.paymentMethod).Id; /*PaymentModeId;*/
                                    ObjSale.InventoryTypeId = 2;
                                    ObjSale.isFormalSale = false;

                                }
                                ObjSale.recieptNumber = Convert.ToString(mySell.invoiceId);
                                //Paymenttrack trk = new Paymenttrack
                                //{
                                //    SaleId = ObjSale.Id,
                                //    DateAdded = DateTime.Now,
                                //    AddedBy = seller_user.Id,
                                //    WarehouseId = (int)seller_user.WarehouseId,
                                //    cash = mySell.paymentMethod.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("CASH")).Amount,
                                //    ecocash = mySell.paymentMethod.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("ECOCASH")).Amount,
                                //    fbc = mySell.paymentMethod.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("FBC")).Amount,
                                //    usd = mySell.paymentMethod.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("USD")).Amount,
                                //    zipit = mySell.paymentMethod.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("ZIPIT")).Amount,
                                //    acl = mySell.paymentMethod.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("ACL")).Amount,
                                //    //onemoney = mySell.paymentMethod.FirstOrDefault(currencyAmount => currencyAmount.Currency.Equals("One Money")).Amount,
                                //    // Change = Convert.ToDecimal(change)                          
                                //};
                                //db.Paymenttracks.Add(trk);
                                //db.SaveChanges();

                                db.Sales.Add(ObjSale);
                                db.SaveChanges(seller_user.FullName);

                                WarehouseStock warehse = new WarehouseStock();
                                warehse = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.prodId && i.WarehouseId == seller_user.WarehouseId);
                                warehse.RemainingQuantity = ObjWarehouseStock.RemainingQuantity - (decimal)item.quantity;
                                db.Entry(warehse).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                        }
                        else
                        {
                            unavailable.Add(item.prodId);
                          

                        }
                    }

                }
                else
                {
                    foreach (var item in mySell.products)
                    {



                        var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.prodId);

                        if (selectedProduct != null)
                        {
                            available.Add(item.prodId);
                         
                        }
                        else
                        {
                            unavailable.Add(item.prodId);

                        }
                    }

                    }
                
            }
            if(unavailable.Count() > 0)
            {
                Helper.WriteError(new Exception(), unavailable.ToString());
            }
           

            return Request.CreateResponse(
                        HttpStatusCode.OK,
                        new { trynos = csello.ToString() },
                        JsonMediaTypeFormatter.DefaultMediaType);

        }

    }


}
