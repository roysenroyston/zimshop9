using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Http;
using WebErrorLogging.Utilities;
using static ShopMate.Models.Zimra;

namespace ShopMate.Controllers
{
    public class AppController : ApiController
    {
        private SIContext db = new SIContext();
        private string userId = Env.GetUserInfo("name");

        [Route("api/App/test")]
        [HttpGet, ActionName("test")]
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

            return Request.CreateResponse(HttpStatusCode.OK, "Email Sent : ");
        }

        [Route("api/App/getRates")]
        [HttpGet, ActionName("getRates")]
        public HttpResponseMessage getRates(int userWarehouse)
        {

            // userId = "Life";
            var warehouseId = userWarehouse;
            var paymentMethods = db.Currencies.Where(i => i.Name.ToLower() != "usd" && i.WarehouseId == userWarehouse).OrderBy(t => t.Name);
            List<string> listRates = new List<string>();
            List<string> listPays = new List<string>();

            foreach (var pay in paymentMethods.ToList())
            {
                double gonzo = 1 * Env.GetRate1(pay.Name.ToLower(), warehouseId);
                if (gonzo != 0.000147)
                {
                    listRates.Add(gonzo.ToString());
                    listPays.Add(pay.Name);
                }
            }

            string[] paymentMethodsRates = listRates.ToArray();
            string[] payMethod = listPays.ToArray();

            //        string[] payMethod = { "Cash", "Ecocash", "Zipit" };
            //string[] paymentMethodsRates = { "1500", "1500", "1500", "1500", "1500" };

            return Request.CreateResponse(
           HttpStatusCode.OK,
           new
           {
               rates = paymentMethodsRates,
               paymethods = payMethod
           },
           JsonMediaTypeFormatter.DefaultMediaType);
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
                //  login.JoinDate
                DateTime dateOfJoining = (DateTime)login.JoinDate; // Example

                // Calculate time difference
                TimeSpan timeDifference = DateTime.Now - dateOfJoining;

                // Check if one year has passed
                if (timeDifference.TotalDays >= 365)
                {
                    ModelState.AddModelError(string.Empty, "You are not allowed to log in as one year has passed since your date of joining.");
                    //    ViewBag.Msg = "Your Account Expired, Contact 0783 284 440";
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Your Account Expired, Contact 0783 284 440");
                }


                try
                {
                    if (BCrypt.Net.BCrypt.Verify(password, login.Password))
                    {
                        if (login.RoleId == 2 || login.RoleId == 7)
                        {
                            var shopdetails = db.Warehouses.FirstOrDefault(i => i.Id == login.WarehouseId);
                            var taxpayer = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == login.WarehouseId);
                            var valadation = 0;// db.Sales.Where(k => k.WarehouseId == login.WarehouseId && k.AddedBy == login.Id).Count();
                            var sales = 0;
                            if (valadation != 0)
                            {
                                sales = 78987;// db.Sales
                                            //       .Where(x => x.WarehouseId == login.WarehouseId && x.AddedBy == login.Id)
                                             //      .OrderByDescending(x => x.DateAdded)
                                               //    .First().recieptNumber;
                            }


                            int recieptNumber = sales + 1;
                            var paymentMethods = db.Currencies.Where(i => i.Name.ToLower() != "usd" && i.WarehouseId == login.WarehouseId).OrderBy(t => t.Name).ToArray();
                            List<string> listRates = new List<string>();
                            List<string> listPays = new List<string>();

                            foreach (var pay in paymentMethods.ToList())
                            {
                                double gonzo = 1 * Env.GetRate1(pay.Name.ToLower(), login.WarehouseId);
                                if (gonzo != 0.000147)
                                {
                                    listRates.Add(gonzo.ToString());
                                    listPays.Add(pay.Name);
                                }
                            }

                            string[] paymentMethodsRates = listRates.ToArray();
                            string[] payMethod = listPays.ToArray();

                            var rowCount = new
                            {
                                user = new
                                {
                                    id = login.Id,
                                    name = login.FullName.ToString(),
                                    warehouse = login.WarehouseId,
                                    storeName = shopdetails.Name.ToString(),
                                    storAddress = shopdetails.Address.ToString(),
                                    storeContact = shopdetails.Mobile.ToString(),
                                    paymentMethods = JsonConvert.SerializeObject(paymentMethods),
                                    paymentMethodsRates = JsonConvert.SerializeObject(paymentMethodsRates),
                                    roleId = login.RoleId,
                                    vatNo = taxpayer.VatNumber,
                                    taxTin = taxpayer.taxPayerTIN,
                                    email = shopdetails.Email,
                                    ReceiptNumber = recieptNumber,
                                    Negative = taxpayer.AllowNegative1,
                                    ShowStocks = taxpayer.ShowQuantity,
                                    DeviceId = taxpayer.DeviceId
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
        //[Route("api/App/login")]
        //[HttpPost, ActionName("login")]
        //public async Task<HttpResponseMessage> login([FromBody] JObject value)
        //{
        //    try
        //    {
        //        string email = value["email"]?.ToString();
        //        string password = value["password"]?.ToString();

        //        var login = db.Users.FirstOrDefault(i => i.UserName == email && i.CanLogin == true);
        //        if (login == null)
        //            return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid details please try again");

        //        DateTime dateOfJoining = (DateTime)login.JoinDate;
        //        if ((DateTime.Now - dateOfJoining).TotalDays >= 365)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.Forbidden, "Your Account Expired, Contact 0783 284 440");
        //        }

        //        if (!BCrypt.Net.BCrypt.Verify(password, login.Password))
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid details please try again");
        //        }

        //        if (login.RoleId != 2 && login.RoleId != 7)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Access denied you are unauthorized to access this platform");
        //        }

        //        var shopdetails = db.Warehouses.FirstOrDefault(i => i.Id == login.WarehouseId);
        //        var taxpayer = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == login.WarehouseId);
        //        if (shopdetails == null || taxpayer == null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Store configuration missing.");
        //        }

        //        // Make HTTP request to PythonAnywhere open-day API
        //        string deviceId = "25891";//taxpayer.DeviceId;
        //        int fiscalDayNo = 0;
        //        string operationID = "";
        //        // Enable TLS 1.2 for secure HTTPS connections
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //        using (var httpClient = new HttpClient())
        //        {
        //            try
        //            {
        //                httpClient.DefaultRequestHeaders.Accept.Clear();
        //                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //               // httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; YourAppName/1.0)");

        //                // Step 1: Check current day status
        //                string dayStatusUrl = $"https://griffintest.pythonanywhere.com/api/day-status-v1/api-v1/{deviceId}/";
        //                var dayStatusResponse = await httpClient.GetAsync(dayStatusUrl);

        //                if (!dayStatusResponse.IsSuccessStatusCode)
        //                {
        //                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Failed to check fiscal day status.");
        //                }

        //                var statusContent = await dayStatusResponse.Content.ReadAsStringAsync();
        //                dynamic statusJson = JsonConvert.DeserializeObject(statusContent);
        //                string fiscalDayStatus = statusJson.fiscalDayStatus;

        //                if (fiscalDayStatus == "FiscalDayOpened")
        //                {
        //                    // Already open, extract info
        //                    fiscalDayNo = statusJson.lastFiscalDayNo;
        //                    operationID = statusJson.operationID;
        //                }
        //                else if (fiscalDayStatus == "FiscalDayClosed")
        //                {
        //                    // Closed, try to open the day
        //                    string openDayUrl = $"https://griffintest.pythonanywhere.com/api/open-day-v1/api-v1/{deviceId}/";
        //                    var openResponse = await httpClient.PostAsync(openDayUrl, null);

        //                    if (!openResponse.IsSuccessStatusCode)
        //                    {
        //                        return Request.CreateResponse(HttpStatusCode.Forbidden, "Failed to open fiscal day.");
        //                    }

        //                    var openContent = await openResponse.Content.ReadAsStringAsync();
        //                    dynamic openJson = JsonConvert.DeserializeObject(openContent);
        //                    fiscalDayNo = openJson.fiscalDayNo;
        //                    operationID = openJson.operationID;
        //                }
        //                else
        //                {
        //                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Unexpected fiscal day status.");
        //                }
        //                //string openDayUrl = $"https://griffintest.pythonanywhere.com/api/open-day-v1/api-v1/{deviceId}/";
        //                //var response = await httpClient.PostAsync(openDayUrl, null);

        //                //if (!response.IsSuccessStatusCode)
        //                //{
        //                //    return Request.CreateResponse(HttpStatusCode.Forbidden, "Fiscal open day failed. Contact support.");
        //                //}

        //                //var responseContent = await response.Content.ReadAsStringAsync();
        //                //dynamic fiscalResponse = JsonConvert.DeserializeObject(responseContent);
        //                //fiscalDayNo = fiscalResponse.fiscalDayNo;
        //                //operationID = fiscalResponse.operationID;
        //            }
        //            catch (Exception ex)
        //            {
        //                Helper.WriteError(ex, "Failed to call fiscal open day endpoint.");
        //                return Request.CreateResponse(HttpStatusCode.Forbidden, "Fiscal open day failed. Contact support.");
        //            }
        //        }

        //        int sales = 0;
        //        var valadation = db.Sales.Where(k => k.WarehouseId == login.WarehouseId && k.AddedBy == login.Id).Count();
        //        if (valadation != 0)
        //        {
        //            sales = db.Sales
        //                     .Where(x => x.WarehouseId == login.WarehouseId && x.AddedBy == login.Id)
        //                     .OrderByDescending(x => x.DateAdded)
        //                     .First().recieptNumber;
        //        }

        //        int recieptNumber = sales + 1;

        //        var paymentMethods = db.Currencies
        //            .Where(i => i.Name.ToLower() != "usd" && i.WarehouseId == login.WarehouseId)
        //            .OrderBy(t => t.Name)
        //            .ToList();

        //        List<string> listRates = new List<string>();
        //        List<string> listPays = new List<string>();

        //        foreach (var pay in paymentMethods)
        //        {
        //            double gonzo = 1 * Env.GetRate1(pay.Name.ToLower(), login.WarehouseId);
        //            if (gonzo != 0.000147)
        //            {
        //                listRates.Add(gonzo.ToString());
        //                listPays.Add(pay.Name);
        //            }
        //        }

        //        string[] paymentMethodsRates = listRates.ToArray();
        //        string[] payMethod = listPays.ToArray();

        //        var rowCount = new
        //        {
        //            user = new
        //            {
        //                id = login.Id,
        //                name = login.FullName,
        //                warehouse = login.WarehouseId,
        //                storeName = shopdetails.Name,
        //                storAddress = shopdetails.Address,
        //                storeContact = shopdetails.Mobile,
        //                paymentMethods = JsonConvert.SerializeObject(paymentMethods),
        //                paymentMethodsRates = JsonConvert.SerializeObject(paymentMethodsRates),
        //                roleId = login.RoleId,
        //                vatNo = taxpayer.VatNumber,
        //                taxTin = taxpayer.taxPayerTIN,
        //                email = shopdetails.Email,
        //                ReceiptNumber = recieptNumber,
        //                Negative = taxpayer.AllowNegative1,
        //                ShowStocks = taxpayer.ShowQuantity,
        //                DeviceId = taxpayer.DeviceId,
        //                fiscalDayNo = fiscalDayNo,
        //                operationID = operationID
        //            }
        //        };

        //        return Request.CreateResponse(HttpStatusCode.OK, rowCount, JsonMediaTypeFormatter.DefaultMediaType);
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.WriteError(ex, ex.Message);
        //        return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid details please try again");
        //    }
        //}

        [HttpGet, ActionName("getCustomers")]
        public HttpResponseMessage GetCustomers(int userWarehouse)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Test1 : " + userWarehouse);

                var stockdata = db.Customers
                                  .Where(i => i.WarehouseId == userWarehouse && i.isActive == true)
                                  .ToList();

                if (stockdata.Count == 0)
                {
                    // No customers found, return 500
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No active customers found.");
                }

                var res = stockdata
                            .OrderBy(sd => sd.Id)
                            .Select(sd => new
                            {
                                id = sd.Id,
                                name = sd.BuyerRegisterName,
                        // balance = sd.b, // Uncomment and correct if needed
                    })
                            .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, res, JsonMediaTypeFormatter.DefaultMediaType);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet, ActionName("getCustomers1")]
        public HttpResponseMessage GetCustomers1(int userWarehouse)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Test1 : " + userWarehouse);

                var stockdata = db.Customers
                                  .Where(i => i.WarehouseId == userWarehouse && i.isActive == true)
                                  .ToList();

                if (stockdata.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No active customers found.");
                }

                var res = stockdata
                            .OrderBy(sd => sd.Id)
                            .Select(sd => new BuyerData
                            {
                                BuyerRegisterName = sd.BuyerRegisterName,
                                BuyerTradeName = sd.BuyerTradeName,
                                BuyerTIN = sd.BuyerTIN,
                                VATNumber = sd.VATNumber,
                                BuyerContacts = new BuyerContacts
                                {
                                    PhoneNo = sd.PhoneNo, // Adjust if these fields are in a related table
                            Email = sd.Email
                                },
                                BuyerAddress = new BuyerAddress
                                {
                                    Province = sd.Province,   // Adjust if nested
                            Street = sd.Street,
                                    HouseNo = sd.HouseNo,
                                    City = sd.City
                                }
                            })
                            .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, res, JsonMediaTypeFormatter.DefaultMediaType);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"An error occurred: {ex.Message}");
            }
        }

        //[HttpGet, ActionName("getCustomers")]
        //public HttpResponseMessage getCustomers(int userWarehouse)
        //{
        //    System.Diagnostics.Debug.WriteLine("Test1 : " + userWarehouse);

        //    var stockdata = db.Customers.Where(i =>  i.WarehouseId == userWarehouse &&i.isActive==true);

        //    var res = from sd in stockdata.ToList()
        //              orderby sd.BuyerRegisterName
        //              select new
        //              {
        //                  id = sd.Id,
        //                  name = sd.BuyerRegisterName,
        //                 // balance = sd.b,

        //              };

        //    if (res.ToArray().Length != 0)
        //    {
        //        return Request.CreateResponse(
        //        HttpStatusCode.OK,
        //        res.ToList(),
        //        JsonMediaTypeFormatter.DefaultMediaType);

        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound, "Customer not found , please try again");
        //    }



        //}
        [HttpGet, ActionName("getProducts")]
        public HttpResponseMessage getProducts(int userWarehouse)
        {
            var mywarehouse = db.Warehouses.FirstOrDefault(r => r.Id == userWarehouse).Name;
            System.Diagnostics.Debug.WriteLine("Test1 : " + userWarehouse);

            var stockdata = db.WarehouseStocks.Where(i => i.WarehouseId == userWarehouse);
            var wh = db.Warehouses.Where(w => w.Id == userWarehouse).FirstOrDefault();

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


        }
        [HttpPost]
        public HttpResponseMessage getSells([FromBody] JObject sell)
        {
            //   System.Diagnostics.Debug.WriteLine("Test1 : " + wareId);
            int warehouseId = Convert.ToInt32(sell["w"]);
            int receiptNo = Convert.ToInt32(sell["id"]);

            var stockdata = db.Sales.Where(i => i.WarehouseId == warehouseId && i.recieptNumber == receiptNo && i.isFiscalised == true).FirstOrDefault();

            if (stockdata != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { invoiceId = stockdata.recieptNumber, qrCode = stockdata.qrCode, verificationCode = stockdata.VerificationCode, zimraReceiptNo = stockdata.zimraReceiptNo, qrUrl = stockdata.qrUrl, deviceSn = stockdata.deviceSerialNo, fiscalDayNo = stockdata.fiscalDayNumber, deviceId = stockdata.deviceID }, JsonMediaTypeFormatter.DefaultMediaType);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Sale not found , please try again");
            }

            // return Request.CreateResponse<IEnumerable<string[]>>(HttpStatusCode.OK, res);
        }

        //[Route("api/App/getSell")]
        //[HttpGet, ActionName("getSell")]
        //public HttpResponseMessage getSell()
        //{
        //    //    System.Diagnostics.Debug.WriteLine("Test1 : " + userWarehouse);
        //    //   int wareId = db.Warehouses.FirstOrDefault(m => m.Name == userWarehouse).Id;
        //    var stockdata = db.Sales;
        //    //   var wh = db.Warehouses.Where(w => w.Id == userWarehouse).FirstOrDefault();

        //    var res = from sd in stockdata.ToList()
        //                  //  join pd in db.Products on sd.ProductId equals pd.Id
        //                  //   where pd.IsActive == true

        //              orderby sd.recieptNumber
        //              select new
        //              {
        //                  id = sd.Id,
        //                  name = sd.Product_ProductId.Name,
        //                  price = sd.SalePrice,
        //                  priceRTGS = sd.RtgsPrice,
        //                  customer = sd.CustomerName,
        //                  Date = sd.DateAdded,
        //                  receiptNumber = sd.recieptNumber,
        //                  quantity = sd.Quantity,
        //                  WarehouseId = sd.WarehouseId
        //              };

        //    if (res.ToArray().Length != 0)
        //    {
        //        return Request.CreateResponse(
        //        HttpStatusCode.OK,
        //        res.ToList(),
        //        JsonMediaTypeFormatter.DefaultMediaType);
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound, "Product not found , please try again");
        //    }

        //    //   System.Diagnostics.Debug.WriteLine("Test1 : " + userWarehouse);
        //}

        //[HttpGet, ActionName("getCustomers")]
        //public HttpResponseMessage getCustomers(int userWarehouse)
        //{
        //    System.Diagnostics.Debug.WriteLine("Test1 : " + userWarehouse);

        //    var stockdata = db.Users.Where(i => i.RoleId == 4 && i.WarehouseId == userWarehouse);

        //    var res = from sd in stockdata.ToList()
        //              orderby sd.UserName
        //              select new
        //              {
        //                  id = sd.Id,
        //                  name = sd.UserName,
        //                  balance = sd.credit,

        //              };

        //    if (res.ToArray().Length != 0)
        //    {
        //        return Request.CreateResponse(
        //        HttpStatusCode.OK,
        //        res.ToList(),
        //        JsonMediaTypeFormatter.DefaultMediaType);

        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound, "Product not found , please try again");
        //    }

        //    System.Diagnostics.Debug.WriteLine("Test1 : " + userWarehouse);

        //    //return Request.CreateResponse(HttpStatusCode.OK, res);

        //    //return Request.CreateResponse<IEnumerable<string[]>>(HttpStatusCode.OK, res);

        //}

        // https://domain?user=1&macaddress=1212445
        [HttpGet, ActionName("userPrinterMacAddress")]
        public HttpResponseMessage userPrinterMacAddress(int user, string macaddress)
        {
            var currentUser = db.Users.FirstOrDefault(i => i.Id == user);
            if (currentUser != null)
            {
                currentUser.printerMacAddress = macaddress;
                db.Entry(currentUser).State = EntityState.Modified;
                db.SaveChanges();
                return Request.CreateResponse(
                 HttpStatusCode.OK,
                "zvaita");
            }
            return Request.CreateResponse(
                 HttpStatusCode.OK,
                "zvaramba");
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



            if (res.ToArray().Length != 0)
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
        public async Task<HttpResponseMessage> sellAsync([FromBody] JObject sell)
        {
            Helper.WriteDebug(new Exception(), sell["sell"].ToString());
            var test = false;

            String value = sell["sell"].ToString();
            List<MySell> maSells = JsonConvert.DeserializeObject<List<MySell>>(value);
            var csello = new JArray();
            var duplicates = new JArray();
            var duplicates1 = new JArray();
            var sellCount = new JArray();
            var mysellCount = new JArray();
            var disCount = mysellCount.Count();

            foreach (MySell mySell in maSells)
            {
                foreach (var item in mySell.products)
                {
                    mysellCount.Add(item.prodId);
                }

                csello.Add(mySell.paymentMethod);
                if (!test)
                {
                    User seller_user = db.Users.FirstOrDefault(i => i.Id == mySell.userId);
                    DateTime nowDate = DateTime.ParseExact(mySell.date + " " + mySell.time, "dd/MM/yyyy HH:mm:ss", null);
                    var findinvoice = db.InformalInvoices.Where(j => j.InvoiceNo == mySell.invoiceId).Count();
                    //  var customer = db.Customers.Where(j => j.BuyerRegisterName == mySell.customer).FirstOrDefault();
                    var customer = db.Customers
                   .FirstOrDefault(j => j.BuyerRegisterName == mySell.customer)
                ?? db.Customers.FirstOrDefault(c => c.Id == 1);
                    DateTime myInvoice = DateTime.Today;
                    var me = myInvoice.ToString("ddMMyyy");
                    System.Diagnostics.Debug.WriteLine("Test1 : " + me);
              

                    var myreCount1 = db.InformalInvoices.Where(j => j.WarehouseId == seller_user.WarehouseId).Count();
                //    customer.Id = 8;
                    InformalInvoice inv = new InformalInvoice();
                        inv.CustomerId = customer.Id;
                        inv.IsBilled = Convert.ToBoolean(mySell.online);
                        inv.AddedBy = seller_user.Id;
                        inv.DateAdded = nowDate;
                        inv.DateModied = DateTime.Now;                   
                        inv.IsPurchaseOrSale = "Sale";
                        inv.ModifiedBy = seller_user.Id;
                        inv.UserId = customer.Id;
                        inv.WarehouseId = (int)seller_user.WarehouseId;
                        inv.subtotal = Convert.ToDecimal(mySell.subtotal);
                 
                        inv.InvoiceNo =mySell.invoiceId ;
                        inv.orderNumber = Convert.ToInt32(me) + myreCount1;
                            if (inv.IsBilled)
                            {
                        inv.CustomerVatReg = "Success";
                            }
   
                        inv.vat = (decimal)mySell.tax;
                        inv.total = inv.subtotal + inv.vat;
                         inv.Currencysubtotal = 0;
                        inv.Currencytotal = 0;
                        inv.Currencyvat = 0;                     
                        try
                        {
                            db.InformalInvoices.Add(inv);
                            db.SaveChanges();
                        }
                        catch (Exception msg)
                        {
                            duplicates1.Add(Convert.ToString(new { reciept = mySell.invoiceId, warehouse = seller_user.WarehouseId, prodName = seller_user.vatNumber }));
                        }
                    
                    Sale ObjSale = new Sale();
                    foreach (var item in mySell.products)
                    {
                        sellCount.Add(item.prodId);
                        var selectedProduct = db.Products.Where(i => i.Id == item.prodId && i.WarehouseId== seller_user.WarehouseId).FirstOrDefault();
                        var ObjWarehouseStock = db.WarehouseStocks.Where(i => i.ProductId == item.prodId && i.WarehouseId == seller_user.WarehouseId).FirstOrDefault();
           
                        decimal discount = Convert.ToDecimal(mySell.discount);
                        if (selectedProduct != null)
                        {
                            if (mySell.currency == "USD")
                            {
                                ObjSale.ProductId = item.prodId;

                                ObjSale.Quantity = item.quantity;
                                ObjSale.UnitPrice = selectedProduct.PurchasePrice * item.quantity;
                                ObjSale.SalePrice = item.price;
                                ObjSale.TotalAmount = (item.price * ObjSale.Quantity);
                                if (selectedProduct.TaxId != 2)
                                {
                                    ObjSale.TotalAmountWithTax = ObjSale.TotalAmount * 0;
                                }
                                else
                                {
                                    ObjSale.TotalAmountWithTax = Math.Round(ObjSale.TotalAmount * (decimal)0.15, 2, MidpointRounding.AwayFromZero);
                                }
                                ObjSale.WarehouseId = (int)seller_user.WarehouseId;
                                ObjSale.AddedBy = seller_user.Id;
                                ObjSale.CustomerUserId = customer.Id;
                                ObjSale.DateAdded = nowDate;
                                ObjSale.DateModied = DateTime.Now;
                                ObjSale.ModifiedBy = seller_user.Id;
                                ObjSale.PaidAmount = (item.price * ObjSale.Quantity) - (discount / mysellCount.Count);
                                ObjSale.PaymentModeId = db.PaymentModes.FirstOrDefault(i => i.Name == mySell.currency).Id; /*PaymentModeId;*/
                                ObjSale.InventoryTypeId = 2;
                                ObjSale.isFormalSale = false;
                            }
                            else
                            {
                                var mypayment = db.PaymentModes.FirstOrDefault(i => i.Name == mySell.paymentMethod).Name;
                                var mycurrency = db.Currencies.FirstOrDefault(i => i.Name == mypayment && i.WarehouseId == seller_user.WarehouseId).Id;
                                decimal priceRate = (decimal)db.Rates.Where(i => i.CurrencyId == mycurrency && i.WarehouseId == seller_user.WarehouseId).OrderByDescending(i => i.DateModified).First().CurrencyRate;
                                ObjSale.ProductId = item.prodId;
                                ObjSale.Quantity = item.quantity;
                                ObjSale.SalePrice = Math.Round((item.price * priceRate), 2, MidpointRounding.AwayFromZero);
                                ObjSale.TotalAmount = Math.Round(ObjSale.SalePrice * ObjSale.Quantity, 2, MidpointRounding.AwayFromZero);
                                if (selectedProduct.TaxId != 2)
                                {
                                    ObjSale.TotalAmountWithTax = ObjSale.TotalAmount * 0;
                                }
                                else
                                {
                                    ObjSale.TotalAmountWithTax = Math.Round((ObjSale.TotalAmount * (decimal)0.15), 2, MidpointRounding.AwayFromZero);
                                }
                                ObjSale.WarehouseId = (int)seller_user.WarehouseId;
                                ObjSale.AddedBy = seller_user.Id;
                                ObjSale.CustomerUserId = customer.Id;
                                ObjSale.DateAdded = nowDate;
                                ObjSale.DateModied = DateTime.Now;
                                ObjSale.ModifiedBy = seller_user.Id;
                                ObjSale.PaidAmount = Math.Round((ObjSale.SalePrice * ObjSale.Quantity) - (discount / mysellCount.Count), 2, MidpointRounding.AwayFromZero);
                                ObjSale.PaymentModeId = db.PaymentModes.FirstOrDefault(i => i.Name == mySell.paymentMethod).Id; /*PaymentModeId;*/
                                ObjSale.InventoryTypeId = 2;
                                ObjSale.rtgs = ObjSale.TotalAmount;
                                ObjSale.isFormalSale = false;
                            }
                            ObjSale.discount = Convert.ToDecimal(mySell.discount);
                            ObjSale.recieptNumber = mySell.invoiceId;
                            ObjSale.InvoiceId = mySell.invoiceId;
                            ObjSale.CustomerName = mySell.customer;

                            if (inv.IsBilled)
                            {
                                ObjSale.isFiscalised = true;
                                ObjSale.qrCode = mySell.qrcode;
                                ObjSale.zimraReceiptNo = mySell.zimraReceiptNo;
                                ObjSale.VerificationCode = mySell.verificationCode;
                                ObjSale.qrUrl = mySell.qrUrl;
                                ObjSale.deviceSerialNo = mySell.deviceSerialNo;
                                ObjSale.fiscalDayNumber = mySell.fiscalDayNumber;
                                ObjSale.deviceID = mySell.deviceID;

                            }

                            try
                            {
                                db.Sales.Add(ObjSale);
                                db.SaveChanges(seller_user.FullName);

                                WarehouseStock warehse = new WarehouseStock();
                                warehse = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.prodId && i.WarehouseId == seller_user.WarehouseId);
                                warehse.RemainingQuantity = ObjWarehouseStock.RemainingQuantity - (decimal)item.quantity;
                                db.Entry(warehse).State = EntityState.Modified;
                                db.SaveChanges();

                                //ProductStock begin
                                ProductStock ps = new ProductStock();
                                ps.ProductId = ObjSale.ProductId;
                                ps.Quantity = ObjSale.Quantity;
                                ps.PurchasePrice = selectedProduct.PurchasePrice;
                                ps.TotalPurchaseAmount = (selectedProduct.PurchasePrice * ObjSale.Quantity);
                                ps.SalePrice = ObjSale.SalePrice;
                                ps.Discount = selectedProduct.Discount;
                                ps.TotalSaleAmount = (ObjSale.SalePrice * ObjSale.Quantity);
                                ps.TotalSaleAmountWithTax = (ObjSale.SalePrice * ObjSale.Quantity);//+ TaxAmount
                                ps.TaxAmount = (decimal)ObjSale.TotalAmountWithTax;
                                ps.ProductName = selectedProduct.Name;
                                ps.Profit = (ps.TotalSaleAmount - (ps.TotalPurchaseAmount)) - (discount / mysellCount.Count);//+ TaxAmount
                                ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount
                                ps.SaleId = ObjSale.Id;
                                ps.Description = "SaleNote";
                                ps.AddedBy = seller_user.Id;
                                ps.DateAdded = nowDate;
                                ps.ModifiedBy = seller_user.Id;
                                ps.DateModied = DateTime.Now;
                                ps.InventoryTypeId = 2;
                                ps.WarehouseId = (int)seller_user.WarehouseId;
                                ps.IsFormal = true;
                                ps.RemainingQuantity = warehse.RemainingQuantity;
                                db.ProductStock.Add(ps);
                                db.SaveChanges();
                            }
                            catch (Exception msg)
                            {
                                duplicates.Add(Convert.ToString(new { reciept = mySell.invoiceId, prodId = item.prodId.ToString(), prodName = selectedProduct.Name.ToString() }));
                            }



                        }

                    }



                    if (!inv.IsBilled)
                    {
                        await new ZimraApiController().SendSales(seller_user.WarehouseId);
                    }
                }
            }
            return Request.CreateResponse(
            HttpStatusCode.OK,
            new { trynos = csello.ToString(), sellsCount = sellCount.Count(), duplicatesCount = duplicates.Count(), duplicatesList = duplicates.ToString() },
            JsonMediaTypeFormatter.DefaultMediaType);
        }

        [Route("api/App/getSaleOrders")]
        [HttpGet, ActionName("getSaleOrders")]
        public HttpResponseMessage GetSaleOrders(int userWarehouse, int? orderId = null)
        {
            try
            {
                var query = db.SaleOrders.Where(o => o.WarehouseId == userWarehouse);
                if (orderId.HasValue)
                {
                    query = query.Where(o => o.Id == orderId.Value);
                }

                var orders = query
                    .OrderByDescending(o => o.DateAdded)
                    .ToList()
                    .Select(o => new
                    {
                        orderId = o.Id.ToString(),
                        customerName = o.CustomerId.HasValue ? (db.Users.FirstOrDefault(u => u.Id == o.CustomerId.Value) != null ? db.Users.FirstOrDefault(u => u.Id == o.CustomerId.Value).UserName : null) : null,
                        total = db.SaleOrderItems.Where(i => i.SaleOrderId == o.Id).Select(i => (decimal?)i.TotalAmount + (decimal?)i.TaxAmount).DefaultIfEmpty(0).Sum() ?? 0,
                        currency = "USD",
                        orderDate = o.DateAdded.ToString("dd/MM/yyyy"),
                        orderTime = o.DateAdded.ToString("HH:mm:ss"),
                        orderStatus = o.IsProcessed ? "PROCESSED" : "PENDING"
                    })
                    .ToList();

                if (orders.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No sale orders found.");
                }

                return Request.CreateResponse(HttpStatusCode.OK, orders, JsonMediaTypeFormatter.DefaultMediaType);
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"An error occurred: {ex.Message}");
            }
        }

        [Route("api/App/createSaleOrder")]
        [HttpPost, ActionName("createSaleOrder")]
        public HttpResponseMessage CreateSaleOrder([FromBody] SaleOrderRequest request)
        {
            if (request == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid payload");
            }

            try
            {
                int parsedUserId;
                if (!int.TryParse(request.userId, out parsedUserId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid userId");
                }

                var sellerUser = db.Users.FirstOrDefault(i => i.Id == parsedUserId);
                if (sellerUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "User not found");
                }

                DateTime orderDateTime;
                var dateTimeString = (request.orderDate ?? DateTime.Now.ToString("dd/MM/yyyy")) + " " + (request.orderTime ?? DateTime.Now.ToString("HH:mm:ss"));
                if (!DateTime.TryParseExact(dateTimeString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out orderDateTime))
                {
                    orderDateTime = DateTime.Now;
                }

                int? customerUserId = null;
                int parsedCustomerId;
                if (!string.IsNullOrWhiteSpace(request.customerId) && int.TryParse(request.customerId, out parsedCustomerId))
                {
                    customerUserId = parsedCustomerId;
                }
                else if (!string.IsNullOrWhiteSpace(request.customerName))
                {
                    var customerUser = db.Users.FirstOrDefault(u => u.UserName == request.customerName);
                    if (customerUser != null) customerUserId = customerUser.Id;
                }

                var saleOrder = new SaleOrder();
                saleOrder.AddedBy = sellerUser.Id;
                saleOrder.DateAdded = orderDateTime;
                saleOrder.DateModified = DateTime.Now;
                saleOrder.ModifiedBy = sellerUser.Id;
                saleOrder.CustomerId = customerUserId;
                saleOrder.WarehouseId = (int)sellerUser.WarehouseId;
                saleOrder.IsProcessed = string.Equals(request.orderStatus, "PROCESSED", StringComparison.OrdinalIgnoreCase);

                db.SaleOrders.Add(saleOrder);
                db.SaveChanges(userId);

                if (request.orderItems != null)
                {
                    foreach (var item in request.orderItems)
                    {
                        var orderItem = new SaleOrderItem();
                        orderItem.ProductId = item.productId;
                        orderItem.Quantity = Convert.ToDecimal(item.quantity);
                        orderItem.SalePrice = Convert.ToDecimal(item.unitPrice);
                        orderItem.TotalAmount = Convert.ToDecimal(item.totalPrice);
                        orderItem.TaxAmount = Convert.ToDecimal(item.taxAmount);
                        orderItem.TotalAmountWithTax = Convert.ToDecimal(item.totalPrice + item.taxAmount);
                        orderItem.TaxId = null;
                        orderItem.DateAdded = orderDateTime;
                        orderItem.SaleOrderId = saleOrder.Id;

                        db.SaleOrderItems.Add(orderItem);
                    }
                    db.SaveChanges(userId);
                }

                return Request.CreateResponse(
                    HttpStatusCode.Created,
                    new
                    {
                        saleOrderId = saleOrder.Id,
                        warehouseId = saleOrder.WarehouseId,
                        itemsSaved = request.orderItems == null ? 0 : request.orderItems.Count,
                        externalOrderId = request.orderId,
                        currency = request.currency,
                        subtotal = request.subtotal,
                        tax = request.tax,
                        total = request.total
                    },
                    JsonMediaTypeFormatter.DefaultMediaType);
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create sale order");
            }
        }


        [Route("api/App/Dsell")]
        [HttpPost, ActionName("Dsell")]
        public async Task<HttpResponseMessage> Dsell([FromBody] JToken salesData)
        {
            // Use a local database context for thread safety in async methods
            using (var dbContext = new SIContext())
            {
                Helper.WriteDebug(new Exception(), salesData?.ToString() ?? "null");
                var test = false;
                List<Dsales> sales = null;

                try
                {
                    // Handle different input formats:
                    // 1. Single object: {...}
                    // 2. Array of objects: [{...}, {...}]
                    // 3. Nested array: [[{...}]]
                    
                    if (salesData == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No sales data provided.");
                    }

                    // Check if it's a single object
                    if (salesData is JObject singleObject)
                    {
                        // Convert single object to list
                        var singleSale = singleObject.ToObject<Dsales>();
                        sales = new List<Dsales> { singleSale };
                    }
                    // Check if it's an array
                    else if (salesData is JArray arrayData)
                    {
                        if (arrayData.Count > 0)
                        {
                            // Check if it's a nested array: [[{...}]]
                            var firstElement = arrayData[0];
                            if (firstElement is JArray innerArray && innerArray.Count > 0)
                            {
                                // Nested array structure
                                sales = innerArray.ToObject<List<Dsales>>();
                            }
                            else
                            {
                                // Regular array structure: [{...}, {...}]
                                sales = arrayData.ToObject<List<Dsales>>();
                            }
                        }
                    }

                    // Check if there are sales data received
                    if (sales != null && sales.Count > 0)
                    {
                        //String value = sales.ToString();
                        //value = value.Trim();
                        //maSells = JsonConvert.DeserializeObject<List<Dsales>>(value);
                        //// Log the received data (optional)
                        //Helper.WriteDebug(new Exception(), sales.ToString());
                        var duplicates = new JArray();
                        var duplicates1 = new JArray();
                        // Process the sales data
                        foreach (var sale in sales)
                        {



                            if (!test)
                            {
                                int myUserId = Convert.ToInt32(sale.UserId);

                                // Add null check and better error handling for database query
                                User seller_user = null;
                                try
                                {
                                    seller_user = dbContext.Users.FirstOrDefault(i => i.Id == myUserId);
                                }
                                catch (Exception dbEx)
                                {
                                    Helper.WriteDebug(dbEx, $"Database error when fetching user {myUserId}: {dbEx.Message}");
                                    throw new Exception($"Database error: {dbEx.Message}", dbEx);
                                }

                                if (seller_user == null)
                                {
                                    Helper.WriteDebug(new Exception(), $"User not found with ID: {myUserId}");
                                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"User with ID {myUserId} not found.");
                                }

                                var myreCount1 = dbContext.InformalInvoices.Where(j => j.WarehouseId == seller_user.WarehouseId).Count();



                                //DateTime nowDate;
                                //try
                                //{
                                //    nowDate = DateTime.ParseExact(sale.date + " " + sale.time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                //}
                                //catch (FormatException ex)
                                //{
                                //    // Log the actual date/time strings for debugging
                                //    Helper.WriteDebug(ex, $"Date parsing failed. Date: '{sale.date}', Time: '{sale.time}', Combined: '{sale.date + " " + sale.time}'");
                                //    // Fallback to current date/time if parsing fails
                                //    nowDate = DateTime.Now;
                                //}


                                //string dateOnly = nowDate.ToString("yyyy-MM-dd"); // Extract date
                                // string timeOnly = nowDate.ToString("HH:mm:ss");   // Extract time

                                // DateTime nowDate = DateTime.ParseExact(sale.Date + " " + sale.time, "dd/MM/yyyy HH:mm:ss", null);
                                var findinvoice = dbContext.InformalInvoices.Where(j => j.InvoiceNo == sale.InvoiceId).Count();
                                //var customer = dbContext.Customers.Where(j => j.BuyerRegisterName == sale.Customer).FirstOrDefault();
                                var customer = dbContext.Customers.Where(j => j.BuyerRegisterName == sale.Customer).FirstOrDefault();
                                DateTime myInvoice = DateTime.Today;
                                var me = myInvoice.ToString("ddMMyyy");
                                System.Diagnostics.Debug.WriteLine("Test1 : " + me);

                                //customer.Id = 1;
                                InformalInvoice inv = new InformalInvoice();
                                inv.CustomerId = (customer != null) ? customer.Id : 1;
                                inv.IsBilled = Convert.ToBoolean(sale.Online);
                                inv.AddedBy = seller_user.Id;
                                inv.DateAdded = DateTime.Now;
                                inv.DateModied = DateTime.Now;
                                inv.IsPurchaseOrSale = "Sale";
                                inv.ModifiedBy = seller_user.Id;
                                inv.UserId = (int)inv.CustomerId;
                                inv.WarehouseId = (int)seller_user.WarehouseId;
                                inv.subtotal = Convert.ToDecimal(sale.Subtotal - sale.tax);

                                inv.InvoiceNo = sale.InvoiceId;
                                inv.orderNumber = Convert.ToInt32(me) + myreCount1;
                                if (inv.IsBilled)
                                {
                                    inv.CustomerVatReg = "Success";
                                }

                                inv.vat = (decimal)sale.tax;
                                inv.total = inv.subtotal;
                                //inv.Currencysubtotal = 0;
                                //inv.Currencytotal = 0;
                                //inv.Currencyvat = 0;
                                try
                                {
                                    dbContext.InformalInvoices.Add(inv);
                                    dbContext.SaveChanges();
                                }
                                catch (Exception msg)
                                {
                                    duplicates1.Add(Convert.ToString(new { reciept = sale.InvoiceId, warehouse = seller_user.WarehouseId, prodName = seller_user.vatNumber }));
                                }



                                Sale ObjSale = new Sale();
                                foreach (var item in sale.products)
                                {
                                    //sellCount.Add(item.prodId);
                                    var selectedProduct = dbContext.Products.Where(i => i.Id == item.prodId).FirstOrDefault();
                                    var ObjWarehouseStock = dbContext.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.prodId && i.WarehouseId == seller_user.WarehouseId);



                                    decimal taxAmount = dbContext.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId).TaxRate;
                                    decimal discount = Convert.ToDecimal(sale.Discount);
                                    var WarehouseName = dbContext.Warehouses.FirstOrDefault(j => j.Id == seller_user.WarehouseId).Name;
                                    //var isSellAvailable = db.Sales.Where(i => i.recieptNumber == mySell.invoiceId && i.ProductId == item.prodId).Count();

                                    //if (isSellAvailable == 0)
                                    //{

                                    if (sale.Currency == "USD")
                                    {
                                        ObjSale.ProductId = item.prodId;

                                        ObjSale.Quantity = item.quantity;
                                        //ObjSale.UnitPrice = selectedProduct.PurchasePrice * item.quantity;
                                        ObjSale.SalePrice = item.price;
                                        ObjSale.TotalAmount = (item.price * ObjSale.Quantity);
                                        if (selectedProduct.TaxId != 2)
                                        {
                                            ObjSale.TotalAmountWithTax = ObjSale.TotalAmount * 0;
                                        }
                                        else
                                        {
                                            ObjSale.TotalAmountWithTax = ObjSale.TotalAmount * (decimal)0.15;
                                        }
                                        ObjSale.WarehouseId = (int)seller_user.WarehouseId;
                                        ObjSale.AddedBy = seller_user.Id;
                                        ObjSale.CustomerUserId = 1;
                                        ObjSale.DateAdded = DateTime.Now;
                                        ObjSale.DateModied = DateTime.Now;
                                        ObjSale.ModifiedBy = seller_user.Id;
                                        ObjSale.PaidAmount = ObjSale.TotalAmount;
                                        ObjSale.PaymentModeId = dbContext.PaymentModes.FirstOrDefault(i => i.Name == sale.Currency).Id; /*PaymentModeId;*/
                                        ObjSale.InventoryTypeId = 2;
                                        ObjSale.isFormalSale = false;
                                    }
                                    else
                                    {
                                        //var mypayment = dbContext.PaymentModes.FirstOrDefault(i => i.Name == mySell.paymentMethod).Name;
                                        //var mycurrency = dbContext.Currencies.FirstOrDefault(i => i.Name == mypayment&& i.WarehouseId==seller_user.WarehouseId).Id;
                                        decimal priceRate = sale.Rate; //(//decimal)dbContext.Rates.Where(i => i.CurrencyId == mycurrency && i.WarehouseId==seller_user.WarehouseId).OrderByDescending(i => i.DateModified).First().CurrencyRate;
                                        ObjSale.ProductId = item.prodId;
                                        ObjSale.Quantity = item.quantity;
                                        //    ObjSale.UnitPrice = selectedProduct.PurchasePrice * item.quantity;
                                        ObjSale.SalePrice = item.price;
                                        ObjSale.TotalAmount = Math.Round((ObjSale.SalePrice * ObjSale.Quantity) * priceRate, 2);
                                        if (selectedProduct.TaxId != 2)
                                        {
                                            ObjSale.TotalAmountWithTax = ObjSale.TotalAmount * 0;
                                        }
                                        else
                                        {
                                            ObjSale.TotalAmountWithTax = Math.Round((ObjSale.TotalAmount * (decimal)0.15), 2);
                                        }
                                        ObjSale.WarehouseId = (int)seller_user.WarehouseId;
                                        ObjSale.AddedBy = seller_user.Id;
                                        ObjSale.CustomerUserId = 1;
                                        ObjSale.DateAdded = DateTime.Now;
                                        ObjSale.DateModied = DateTime.Now;
                                        ObjSale.ModifiedBy = seller_user.Id;
                                        //    ObjSale.PaidAmount = Math.Round((ObjSale.SalePrice * ObjSale.Quantity) - (discount / mysellCount.Count), 2);
                                        ObjSale.PaymentModeId = dbContext.PaymentModes.FirstOrDefault(i => i.Name == sale.PaymentMethod).Id; /*PaymentModeId;*/
                                        ObjSale.InventoryTypeId = 2;
                                        ObjSale.rtgs = Math.Round(ObjSale.TotalAmount * priceRate, 2);
                                        ObjSale.isFormalSale = false;
                                    }


                                    // ObjSale.discount = Convert.ToDecimal(sale.Discount);
                                    ObjSale.recieptNumber = sale.InvoiceId;
                                    ObjSale.CustomerName = sale.Customer;
                                    ObjSale.InvoiceId = inv.orderNumber;

                                    if (inv.IsBilled)
                                    {
                                        ObjSale.isFiscalised = true;
                                        ObjSale.qrCode = sale.QrString;
                                        ObjSale.zimraReceiptNo = sale.receiptID;
                                        ObjSale.VerificationCode = sale.VerificationCode;
                                        ObjSale.qrUrl = sale.QrUrl;
                                        ObjSale.deviceSerialNo = sale.DeviceSerialNo;
                                        ObjSale.fiscalDayNumber = sale.FiscalDayNumber;
                                        ObjSale.deviceID = sale.DeviceID;

                                    }

                                    try
                                    {
                                        dbContext.Sales.Add(ObjSale);
                                        dbContext.SaveChanges(seller_user.FullName);

                                        WarehouseStock warehse = new WarehouseStock();
                                        warehse = dbContext.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.prodId && i.WarehouseId == seller_user.WarehouseId);
                                        warehse.RemainingQuantity = ObjWarehouseStock.RemainingQuantity - (decimal)item.quantity;
                                        dbContext.Entry(warehse).State = EntityState.Modified;
                                        dbContext.SaveChanges();
                                        //ProductStock begin

                                        ProductStock ps = new ProductStock();
                                        ps.ProductId = ObjSale.ProductId;
                                        ps.Quantity = ObjSale.Quantity;

                                        ps.PurchasePrice = selectedProduct.PurchasePrice;

                                        ps.TotalPurchaseAmount = (selectedProduct.PurchasePrice * ObjSale.Quantity);

                                        ps.SalePrice = ObjSale.SalePrice;
                                        ps.Discount = selectedProduct.Discount;
                                        ps.TotalSaleAmount = (ObjSale.SalePrice * ObjSale.Quantity);

                                        decimal TaxAmount = (decimal)ObjSale.TotalAmountWithTax;

                                        ps.TotalSaleAmountWithTax = (ObjSale.SalePrice * ObjSale.Quantity) + TaxAmount;
                                        ps.TaxAmount = TaxAmount;
                                        ps.ProductName = selectedProduct.Name;
                                        ps.Profit = ps.TotalSaleAmount - ps.TotalPurchaseAmount;//+ TaxAmount
                                        ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount) + TaxAmount;
                                        ps.SaleId = ObjSale.Id;
                                        ps.Description = "SaleNote";
                                        ps.AddedBy = seller_user.Id;
                                        ps.DateAdded = DateTime.Now;
                                        ps.ModifiedBy = seller_user.Id;
                                        ps.DateModied = DateTime.Now;
                                        ps.InventoryTypeId = 2;
                                        ps.WarehouseId = (int)seller_user.WarehouseId;
                                        ps.IsFormal = true;
                                        ps.RemainingQuantity = warehse.RemainingQuantity;
                                        //   ps.ProductBatchId = dbContext.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;
                                        dbContext.ProductStock.Add(ps);
                                        dbContext.SaveChanges();
                                    }
                                    catch (Exception msg)
                                    {
                                        duplicates.Add(Convert.ToString(new { reciept = sale.InvoiceId, prodId = item.prodId.ToString(), prodName = selectedProduct.Name.ToString() }));
                                    }

                                    //}
                                    //else
                                    //{
                                    //    duplicates.Add(Convert.ToString(new { reciept = mySell.invoiceId, prodId = item.prodId.ToString(), prodName = selectedProduct.Name.ToString() }));
                                    //}
                                }
                            }

                            Helper.WriteDebug(new Exception(), $"Processing sale for Invoice ID: {sale.InvoiceId}");

                            // Implement your business logic here (e.g., save to database, calculate totals, etc.)
                        }

                        // If all goes well
                        return Request.CreateResponse(HttpStatusCode.OK, "Sales processed successfully.");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No sales data provided.");
                    }
                }
                catch (JsonReaderException ex)
                {
                    // Log the error during JSON parsing
                    Helper.WriteDebug(ex, "JSON parsing error during deserialization.");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Deserialization failed: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Log other exceptions
                    Helper.WriteDebug(ex, "Error processing the sale data.");
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the sale.");
                }
            }
        }





        public class SaleOrderRequest
        {
            public string orderId { get; set; }
            public string customerId { get; set; }
            public string customerName { get; set; }
            public string customerPhone { get; set; }
            public List<SaleOrderItemRequest> orderItems { get; set; }
            public decimal subtotal { get; set; }
            public decimal tax { get; set; }
            public decimal total { get; set; }
            public string currency { get; set; }
            public string orderDate { get; set; }
            public string orderTime { get; set; }
            public string orderStatus { get; set; }
            public string paymentMethod { get; set; }
            public string notes { get; set; }
            public string userId { get; set; }
            public string userName { get; set; }
            public int isOnline { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
        }

        public class SaleOrderItemRequest
        {
            public string orderId { get; set; }
            public int? productId { get; set; }
            public string productName { get; set; }
            public string productBarcode { get; set; }
            public decimal quantity { get; set; }
            public decimal unitPrice { get; set; }
            public decimal totalPrice { get; set; }
            public decimal taxRate { get; set; }
            public decimal taxAmount { get; set; }
            public string currency { get; set; }
            public string notes { get; set; }
        }
    }
}