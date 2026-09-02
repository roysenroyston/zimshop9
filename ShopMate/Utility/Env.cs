using Newtonsoft.Json;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebErrorLogging.Utilities;

namespace ShopMate
{
    public static class Env
    {
        /// <summary>
        /// Its used for get role id and role name from Claims
        /// </summary>
        /// <param name="s"></param>
        /// <param name="IsRoleID">If you want role ID then pass true , if role name then pass false</param>
        /// <returns></returns>
        public static string GetUserRoleOrUsername(this HtmlHelper s, bool IsRoleID)
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string role = string.Empty;
            if (IsRoleID == true)
            {
                role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();
            }
            else
            {
                role = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            }

            return role;
        }

        /// <summary>
        /// This Method will used for take all data from Claims Cookies 
        /// </summary>
        /// <param name="value">use "name" for Get UserName, 
        /// use "userid" for Get Logedin UserId,
        /// use "company" for Get Company Name,
        /// use "email" for Get Email,
        /// use "roleid" for Get RoleId,
        /// use "rolename" for Get RoleName,
        /// use "image" for Get User Profile Image,
        /// use "theme" for Get Theme (color scheme)
        /// </param>
        /// <returns>String</returns>
        public static string GetUserInfo(string value)
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string ReturnVal = string.Empty;
            switch (value)
            {
                case "name":
                    ReturnVal = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).DefaultIfEmpty("").SingleOrDefault();
                    break;
                case "userid":
                    ReturnVal = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                    break;
                case "roleid":
                    ReturnVal = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();
                    break;
                case "WarehouseId":
                    ReturnVal = identity.Claims.Where(c => c.Type == ClaimTypes.Actor).Select(c => c.Value).SingleOrDefault();
                    break;
                default:
                    ReturnVal = "";
                    break;
            }

            return ReturnVal;

        }
        //Get the payment mode
        //public static string GetPaymentInfo(string value)
        //{
        //    var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        //    string ReturnVal = string.Empty;
        //    switch (value)
        //    {
        //        case "Name":
        //            ReturnVal = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
        //            break;
        //        case "Id":
        //            ReturnVal = identity.Claims.Where(c => c.Type == ClaimTypes.Id).Select(c => c.Value).SingleOrDefault();
        //            break;

        //        default:
        //            ReturnVal = "";
        //            break;
        //    }

        //    return ReturnVal;

        //}

        public static string Language()
        {
            var currentContext = new HttpContextWrapper(System.Web.HttpContext.Current);
            try
            {
                var routeData = RouteTable.Routes.GetRouteData(currentContext);
                string languageCode = (string)routeData.Values["cultureName"];
                return languageCode.ToLower();
            }
            catch (Exception)
            {
                return "en";
            }

        }

        public static string Decrypt(string cryptedString)
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");
            if (String.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }

        /// <summary>
        /// Encrypt Method used for Encrypt to any String. you may use this for password encryption and decryption or other string.
        /// </summary>
        /// <param name="originalString"></param>
        /// <returns></returns>
        public static string Encrypt(string originalString)
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);

            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            string output = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            //if (output.Contains('+'))
            //{
            //    output = output.Replace("+", "%2B");
            //}
            return output;
        }
        static SIContext db = new SIContext();
        public static double GetRate(string selectedCurrecy)
        {
            try
            {
                //Convert.ToString(db.Users.FirstOrDefault(i => i.Id == Int32.Parse(c.UserName)).UserName),

                var CurrencyId = db.Currencies.FirstOrDefault(x => x.Name.ToLower().Equals(selectedCurrecy)).Id;
                double currencyRate = db.Rates
                               .Where(x => x.CurrencyId == CurrencyId)
                               .OrderByDescending(x => x.DateModified)
                               .First().CurrencyRate;
                // decimal currencyRate = db.Rates.LastOrDefault(rate => rate.Currency.Name == (selectedCurrecy)).CurrencyRate;
                return currencyRate;
            }
            catch (Exception ex)
            {
                return 0.000147;
            }
        }


        public static double GetRate1(string selectedCurrecy, int? warehouseId)
        {
            try
            {
                //Convert.ToString(db.Users.FirstOrDefault(i => i.Id == Int32.Parse(c.UserName)).UserName),

                //  var WareId = db.Users.FirstOrDefault(n => n.UserName == userid).WarehouseId;
                var CurrencyId = db.Currencies.FirstOrDefault(x => x.Name.ToLower().Equals(selectedCurrecy) && x.WarehouseId == warehouseId).Id;
                double currencyRate = db.Rates
                               .Where(x => x.CurrencyId == CurrencyId && x.WarehouseId == warehouseId)
                               .OrderByDescending(x => x.DateModified)
                               .First().CurrencyRate;
                // decimal currencyRate = db.Rates.LastOrDefault(rate => rate.Currency.Name == (selectedCurrecy)).CurrencyRate;
                return currencyRate;
            }
            catch (Exception ex)
            {
                return 0.000147;
            }
        }
        public static string GetSiteRoot()
        {
            string sOut = "";
            if (System.Web.HttpContext.Current != null)
            {
                string Port = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
                if (Port == null || Port == "80" || Port == "443")
                    Port = string.Empty;
                else
                    Port = ":" + Port;

                string Protocol = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
                if (Protocol == null || Protocol.Equals("0"))
                    Protocol = "http://";
                else
                    Protocol = "https://";

                string appPath = System.Web.HttpContext.Current.Request.ApplicationPath;
                if (appPath == "/")
                    appPath = "";

                sOut = Protocol + System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + Port + appPath;
            }
            return sOut;
        }
        public static MvcHtmlString GetMenuBarPage(Nullable<int> ParentId, string OpenedPage)
        {

            StringBuilder sb = new StringBuilder();
            SIContext db = new SIContext();
            //get role id and role regarding to role bind this
            var userId = Convert.ToInt32(Env.GetUserInfo("userid"));
            var RoleId = Convert.ToInt32(Env.GetUserInfo("roleid"));

            var cacheItemKey = "jApMenuBar" + userId + "Us" + RoleId;

            var globle = HttpRuntime.Cache.Get(cacheItemKey);
            if (globle == null)
            {
                globle = db.MenuPermissions.Where(i => i.RoleId == RoleId || i.UserId == userId).ToArray();
                HttpRuntime.Cache.Insert(cacheItemKey, globle, null, DateTime.Now.AddMinutes(50), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            if (RoleId == 1)
            {
                sb.Append("<ul class=\"sidebar-menu\">");

                sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/Home\"> <i class=\"fa fa-dashboard\"></i> <span>Dashboard</span> </a> </li>");
                sb.Append("<li class=\"\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/pos\"> <i class=\"fa fa-calculator\"></i> <span>Point Of Sale (POS)</span> </a> </li>");
                sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/DeclaredayEnd\"> <i class=\"fa fa-dashboard\"></i> <span>Day End </span> </a> </li>");

                sb.Append(GetMenuBar(ParentId, (MenuPermission[])globle, OpenedPage));
                sb.Append("</ul>");
                return MvcHtmlString.Create(sb.ToString());
            }
            else if (RoleId == 5 || RoleId == 2 || RoleId == 7)
            {
                sb.Append("<ul class=\"sidebar-menu\">");

               // sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/Home\"> <i class=\"fa fa-dashboard\"></i> <span>Dashboard</span> </a> </li>");
                sb.Append("<li class=\"\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/pos\"> <i class=\"fa fa-calculator\"></i> <span>Point Of Sale (POS)</span> </a> </li>");
                sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/DeclaredayEnd\"> <i class=\"fa fa-dashboard\"></i> <span>Day End </span> </a> </li>");

                //sb.Append("<li class=\"\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/pos\"> <i class=\"fa fa-calculator\"></i> <span>Point Of Sale (POS)</span> </a> </li>");
                //sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/DeclaredayEnd/Create\"> <i class=\"fa fa-dashboard\"></i> <span>Day End </span> </a> </li>");
                //  sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/Expense\"> <i class=\"fa fa-dashboard\"></i> <span>Expense </span> </a> </li>");

                sb.Append(GetMenuBar(ParentId, (MenuPermission[])globle, OpenedPage));
                sb.Append("</ul>");

                return MvcHtmlString.Create(sb.ToString());
            }
            else if (RoleId == 11)
            {
                sb.Append("<ul class=\"sidebar-menu\">");
                sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/DeclaredayEnd\"> <i class=\"fa fa-dashboard\"></i> <span>Day End </span> </a> </li>");
                sb.Append(GetMenuBar(ParentId, (MenuPermission[])globle, OpenedPage));
                sb.Append("</ul>");

                return MvcHtmlString.Create(sb.ToString());
            }
            else if (RoleId == 12)
            {
                sb.Append("<ul class=\"sidebar-menu\">");

                sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/Home\"> <i class=\"fa fa-dashboard\"></i> <span>Dashboard</span> </a> </li>");
                sb.Append("<li class=\"\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/pos\"> <i class=\"fa fa-calculator\"></i> <span>Point Of Sale (POS)</span> </a> </li>");
                sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/DeclaredayEnd\"> <i class=\"fa fa-dashboard\"></i> <span>Day End </span> </a> </li>");

                sb.Append(GetMenuBar(ParentId, (MenuPermission[])globle, OpenedPage));
                sb.Append("</ul>");
                return MvcHtmlString.Create(sb.ToString());
            }
            else
            {
                sb.Append("<ul class=\"sidebar-menu\">");

                //sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/Home\"> <i class=\"fa fa-dashboard\"></i> <span>Dashboard</span> </a> </li>");
                //sb.Append("<li class=\"\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/pos\"> <i class=\"fa fa-calculator\"></i> <span>Point Of Sale (POS)</span> </a> </li>");
                //sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/DeclaredayEnd\"> <i class=\"fa fa-dashboard\"></i> <span>Day End </span> </a> </li>");

                //sb.Append("<li class=\"\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/pos\"> <i class=\"fa fa-calculator\"></i> <span>Point Of Sale (POS)</span> </a> </li>");
                //sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/DeclaredayEnd/Create\"> <i class=\"fa fa-dashboard\"></i> <span>Day End </span> </a> </li>");
                //  sb.Append("<li class=\"active\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/Expense\"> <i class=\"fa fa-dashboard\"></i> <span>Expense </span> </a> </li>");

                sb.Append(GetMenuBar(ParentId, (MenuPermission[])globle, OpenedPage));
                sb.Append("</ul>");

                return MvcHtmlString.Create(sb.ToString());
            }

        }




        private static MvcHtmlString GetMenuBar(Nullable<int> ParentId, MenuPermission[] q, string OpenedPage)
        {
            StringBuilder sb = new StringBuilder();
            if (q != null)
            {
                foreach (var item in q.Where(i => i.Menu_MenuId.ParentId == ParentId).OrderBy(i => i.SortOrder))
                {
                    var js = q;

                    if (js.Count(j => j.Menu_MenuId.ParentId == item.Menu_MenuId.Id) > 0)
                    {
                        string active = "";
                        string style = "";
                        if (OpenedPage == item.Menu_MenuId.MenuText)
                        {
                            active = " active";
                            style = "style=\"display: block;\"";
                        }

                        if (item.Menu_MenuId.ParentId == null)
                        {
                            sb.Append("<li class=\"treeview " + active + "\"> <a href=\"#\">  " + item.Menu_MenuId.MenuIcon + "  <span>" + item.Menu_MenuId.MenuText + "</span> <i class=\"fa fa-angle-left pull-right\"></i>  </a><ul class=\"treeview-menu\" " + style + " >");
                        }
                        else
                        {
                            sb.Append("<li class=\"treeview\"> <a href=\"#\">  " + item.Menu_MenuId.MenuIcon + "  <span>" + item.Menu_MenuId.MenuText + "</span> <i class=\"fa fa-angle-left pull-right\"></i>  </a><ul class=\"treeview-menu\">");
                        }
                        sb.Append(GetMenuBar(item.Menu_MenuId.Id, q, OpenedPage));
                    }
                    else
                    {
                        if (item.Menu_MenuId.ParentId == null)
                        {
                            sb.Append("<li class=\"\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/" + item.Menu_MenuId.MenuURL + "\"> " + item.Menu_MenuId.MenuIcon + "   " + item.Menu_MenuId.MenuText + "</a></li>");
                        }
                        else
                        {
                            sb.Append("<li class=\"\"> <a href=\"" + MicrosoftHelper.MSHelper.GetSiteRoot() + "/" + item.Menu_MenuId.MenuURL + "\"> " + item.Menu_MenuId.MenuIcon + "   " + item.Menu_MenuId.MenuText + "</a></li>");
                        }

                    }

                }
                sb.Append("</ul>");
            }


            return MvcHtmlString.Create(sb.ToString());
        }
        public static DateTime AddTimeInDate(DateTime comingDate, string time)
        {

            DateTime retrunDate = new DateTime();
            try
            {
                string[] tim = time.Split(':');
                int hour = 00;
                int min = 00;
                try
                {
                    hour = Convert.ToInt32(tim[0]);
                    min = Convert.ToInt32(tim[1]);
                }
                catch (Exception) { }

                System.TimeSpan duration = new System.TimeSpan(hour, min, 0);
                //DateTime finalDate = DateTime.Today + duration;
                DateTime finalDate = comingDate.Date + duration;
                // System.DateTime finalDate = comingDate.Add(duration);
                retrunDate = finalDate;
                // retrunDate = Convert.ToDateTime(finalDate.ToString("M/d/yyyy"), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }
            //return retrunDate;
            return DateTime.ParseExact(Convert.ToDateTime(retrunDate).ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        }

        public static string GetUserExpiry()
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                int userId = Convert.ToInt32(identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());
                var ngodza = db.Users.FirstOrDefault(k => k.Id == userId);
                String Value = "";
                if (ngodza != null)
                {
                    DateTime dateOfJoining = (DateTime)ngodza.JoinDate;
                    TimeSpan timeDifference = DateTime.Now - dateOfJoining;
                    DateTime newDate = dateOfJoining.AddDays(365);
                    TimeSpan daysleft = newDate - DateTime.Now;
                    int more = (int)daysleft.TotalDays;
                    if (timeDifference.TotalDays >= 335)
                    {
                        Value = "Your account is about to expire you are left with " + more + " days";
                    }
                }
                return Value;
            }
            catch (Exception)
            {
            }

            return "";
        }


        public static MvcHtmlString WareHouseUC(int? selected = null)
        {
            StringBuilder sb = new StringBuilder();
            SIContext db = new SIContext();

            sb.Append("<select id=\"WarehouseId\" name=\"WarehouseId\">");
            sb.Append("<option value=\"\"> -Select- </option>");

            foreach (var item in db.Warehouses.ToArray())
            {
                if (selected == item.Id)
                {
                    sb.Append("<option selected=\"selected\" value=\"" + item.Id + "\">" + item.Name + "</option>");
                }
                else
                {
                    sb.Append("<option value=\"" + item.Id + "\">" + item.Name + "</option>");
                }

            }

            sb.Append("</select>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString TaxUC(int? selected = null)
        {
            StringBuilder sb = new StringBuilder();
            SIContext db = new SIContext();

            sb.Append("<select id=\"TaxId\" name=\"TaxId\">");
            sb.Append("<option value=\"\"> -Select- </option>");
            foreach (var item in db.Taxs.ToArray())
            {
                if (selected == item.Id)
                {
                    sb.Append("<option selected=\"selected\" value=\"" + item.Id + "\">" + item.Name + "</option>");
                }
                else
                {
                    sb.Append("<option value=\"" + item.Id + "\">" + item.Name + "</option>");
                }

            }

            sb.Append("</select>");
            return MvcHtmlString.Create(sb.ToString());
        }


        //Send Email Function
        public static async Task<bool> sendMail(string[] emails, string body, string subject)
        {
            foreach (string email in emails)
            {
                try
                {
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(email));
                    message.Subject = subject;
                    message.Body = body;
                    System.Diagnostics.Debug.WriteLine("Email : " + email);

                    message.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        await smtp.SendMailAsync(message);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("EmailError : " + email);

                }
            }

            return true;


        }



        [System.Web.Http.HttpPost]
        // public async Task<ActionResult> SendSales1(int? WarehouseId)
        public static async Task<string> SendSales1Async()
        {
            int WarehouseId = int.Parse(Env.GetUserInfo("WarehouseId"));
            // URL of the external server's API endpoint for creating a new entry
            var DeviceId = db.InvoiceFormats.FirstOrDefault(k => k.WarehouseId == WarehouseId).DeviceId;
            //string apiUrl = "";
            string result = "";
            //string apiUrl = "http://giftmashuro.pythonanywhere.com/api/submit-invoice/api-v1//";
            string apiUrl = "http://giftmashuro.pythonanywhere.com/api/submit-invoice/api-v1/" + DeviceId;



            try
            {
                var receiptList = db.InformalInvoices.Where(k => k.IsBilled == false).ToList();
                var receiptList2 = db.Invoices.Where(k => k.IsBilled == false && k.IsPurchaseOrSale == "Sale").ToList();

                var receiptId = db.InformalInvoices.FirstOrDefault(i => i.WarehouseId == WarehouseId);

                // apiUrl = "http://giftmashuro.pythonanywhere.com/api/submit-invoice/api-v1/" + receiptList.;




                var me = receiptList.Count();
                var me2 = receiptList2.Count();
                if (me != 0)
                {


                    foreach (var receiptlistItems in receiptList)
                    {
                        var mydata = db.Sales.Where(i => i.isFiscalised != true && i.recieptNumber == receiptlistItems.InvoiceNo).ToList();
                        var saleData = db.Sales.Where(k => k.isFiscalised != true).FirstOrDefault();
                        List<Zimra.receiptLines> receipts = new List<Zimra.receiptLines>();
                        var myreceiptGlobalNo = 30 + 1;

                        var reciptcount = db.Sales.Where(k => k.recieptNumber == receiptlistItems.InvoiceNo).Count();
                        var recieptItems = db.Sales.Where(k => k.recieptNumber == receiptlistItems.InvoiceNo).ToList();

                        var myreceiptLineNo = 1;
                        decimal myreceiptLineTotal = 0;
                        decimal totalTax = 0;

                        foreach (var item in recieptItems)
                        {
                            var myTaxCode = "";
                            var taxPernt = 0;
                            var ztaxId = 0;
                            var taxId = db.Products.Where(j => j.Name == item.Product_ProductId.Name).FirstOrDefault().TaxId;
                            if (taxId == 2)
                            {
                                myTaxCode = "C";
                                taxPernt = 15;
                                ztaxId = 3;
                            }
                            else
                            {
                                myTaxCode = "B";
                                taxPernt = 0;
                                ztaxId = 2;
                            }

                            //        decimal taxAmount = (item.SalePrice *taxPernt);
                            //    string formattedtaxAmount = taxAmount.ToString("F2");

                            var kyle = new Zimra.receiptLines
                            {
                                receiptLineType = "Sale",
                                receiptLineNo = myreceiptLineNo,
                                receiptLineHSCode = "12345",
                                receiptLineName = item.Product_ProductId.Name,
                                receiptLinePrice = item.SalePrice,
                                receiptLineQuantity = (int)item.Quantity,
                                receiptLineTotal = ((int)item.Quantity * item.SalePrice),
                                taxCode = myTaxCode,
                                taxPercent = taxPernt,
                                taxID = ztaxId
                            };
                            receipts.Add(kyle);
                            myreceiptLineNo = myreceiptLineNo + 1;
                            myreceiptLineTotal = myreceiptLineTotal + kyle.receiptLineTotal;
                        }

                        totalTax = totalTax + (myreceiptLineTotal * (decimal)0.15);
                        string formattedtotalTax = totalTax.ToString("F2");
                        var receiptData = new Zimra.receipt

                        {
                            receiptType = "FiscalInvoice",
                            receiptCurrency = "USD",
                            receiptCounter = reciptcount,
                            receiptGlobalNo = myreceiptGlobalNo,
                            invoiceNo = Convert.ToString(saleData.recieptNumber),
                            buyerData = null,
                            receiptNotes = null,
                            receiptDate = saleData.DateModied,
                            creditDebitNote = null,
                            receiptLinesTaxInclusive = false,
                            receiptLines = receipts,
                            receiptTaxes = new List<Zimra.receiptTaxs>
                                                {
                                                    new Zimra.receiptTaxs
                                                    {
                                                        taxCode = "C",
                                                        taxPercent = 15,
                                                        taxID = 3,
                                                        taxAmount =  Convert.ToDecimal(formattedtotalTax),
                                                        salesAmountWithTax = myreceiptLineTotal+ Convert.ToDecimal(formattedtotalTax),
                                                    }
                                                },
                            receiptPayments = new List<Zimra.receiptPayments>
                                                {
                                                    new Zimra.receiptPayments
                                                    {
                                                        moneyTypeCode = "Cash",
                                                        paymentAmount = myreceiptLineTotal+ Convert.ToDecimal(formattedtotalTax),
                                                    }
                                                },
                            receiptTotal = myreceiptLineTotal + Convert.ToDecimal(formattedtotalTax),
                            receiptPrintForm = "Receipt48"
                        };
                        var json = JsonConvert.SerializeObject(new { receipt = receiptData });

                        Helper.WriteInformation(new Exception(), json.ToString());

                        using (var HttpClient = new HttpClient())
                        {
                            // Set the content type
                            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                            // Send a POST request to the external server
                            var response = await HttpClient.PostAsync(apiUrl, content);

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
                                    myResponse deserializedData = JsonConvert.DeserializeObject<myResponse>(responseData);

                                    Helper.WriteInformation(new Exception(), deserializedData.ToString());
                                    foreach (var myitems in recieptItems)
                                    {
                                        myitems.isFiscalised = true;
                                        myitems.qrCode = deserializedData.QrString;
                                        myitems.zimraReceiptNo = deserializedData.receiptID;
                                        myitems.VerificationCode = deserializedData.VerificationCode;
                                        myitems.qrUrl = deserializedData.QrUrl;
                                        myitems.deviceSerialNo = deserializedData.DeviceSerialNo;
                                        myitems.fiscalDayNumber = deserializedData.FiscalDayNumber;
                                        myitems.deviceID = deserializedData.DeviceID;

                                        db.Entry(myitems).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                Helper.WriteInformation(new Exception(), response.StatusCode.ToString());
                                result = "Handina Response yandawana ";
                                return "500";

                                //return Request.CreateResponse(HttpStatusCode.OK, result, JsonRequestBehavior.AllowGet);
                            }
                        }

                        receiptlistItems.IsBilled = true;

                        db.Entry(receiptlistItems).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (me2 != 0)
                {
                    foreach (var receiptlistItem2 in receiptList2)
                    {
                        var mydata = db.Sales.Where(i => i.isFiscalised != true && i.recieptNumber == receiptlistItem2.InvoiceNo).ToList();
                        var saleData = db.Sales.Where(k => k.isFiscalised != true && k.InvoiceId == receiptlistItem2.InvoiceNo).FirstOrDefault();
                        List<Zimra.receiptLines> receipts = new List<Zimra.receiptLines>();
                        var myreceiptGlobalNo = 30 + 1;

                        var reciptcount = db.Sales.Where(k => k.recieptNumber == receiptlistItem2.InvoiceNo).Count();
                        var recieptItems = db.Sales.Where(k => k.recieptNumber == receiptlistItem2.InvoiceNo).ToList();

                        var myreceiptLineNo = 1;
                        decimal myreceiptLineTotal = 0;
                        decimal totalTax = 0;

                        foreach (var item in recieptItems)
                        {
                            var myTaxCode = "";
                            var taxPernt = 0;
                            var ztaxId = 0;
                            var taxId = db.Products.Where(j => j.Name == item.Product_ProductId.Name).FirstOrDefault().TaxId;
                            if (taxId == 2)
                            {
                                myTaxCode = "C";
                                taxPernt = 15;
                                ztaxId = 3;
                            }
                            else
                            {
                                myTaxCode = "B";
                                taxPernt = 0;
                                ztaxId = 2;
                            }

                            //        decimal taxAmount = (item.SalePrice *taxPernt);
                            //    string formattedtaxAmount = taxAmount.ToString("F2");

                            var kyle = new Zimra.receiptLines
                            {
                                receiptLineType = "Sale",
                                receiptLineNo = myreceiptLineNo,
                                receiptLineHSCode = "12345",
                                receiptLineName = item.Product_ProductId.Name,
                                receiptLinePrice = item.SalePrice,
                                receiptLineQuantity = (int)item.Quantity,
                                receiptLineTotal = ((int)item.Quantity * item.SalePrice),
                                taxCode = myTaxCode,
                                taxPercent = taxPernt,
                                taxID = ztaxId
                            };
                            receipts.Add(kyle);
                            myreceiptLineNo = myreceiptLineNo + 1;
                            myreceiptLineTotal = myreceiptLineTotal + kyle.receiptLineTotal;
                        }

                        totalTax = totalTax + (myreceiptLineTotal * (decimal)0.15);
                        string formattedtotalTax = totalTax.ToString("F2");
                        var receiptData = new Zimra.receipt

                        {
                            receiptType = "FiscalInvoice",
                            receiptCurrency = "USD",
                            receiptCounter = reciptcount,
                            receiptGlobalNo = myreceiptGlobalNo,
                            invoiceNo = Convert.ToString(saleData.recieptNumber),
                            buyerData = null,
                            receiptNotes = null,
                            receiptDate = saleData.DateModied,
                            creditDebitNote = null,
                            receiptLinesTaxInclusive = false,
                            receiptLines = receipts,
                            receiptTaxes = new List<Zimra.receiptTaxs>
                                                {
                                                    new Zimra.receiptTaxs
                                                    {
                                                        taxCode = "C",
                                                        taxPercent = 15,
                                                        taxID = 3,
                                                        taxAmount =  Convert.ToDecimal(formattedtotalTax),
                                                        salesAmountWithTax = myreceiptLineTotal+ Convert.ToDecimal(formattedtotalTax),
                                                    }
                                                },
                            receiptPayments = new List<Zimra.receiptPayments>
                                                {
                                                    new Zimra.receiptPayments
                                                    {
                                                        moneyTypeCode = "Cash",
                                                        paymentAmount = myreceiptLineTotal+ Convert.ToDecimal(formattedtotalTax),
                                                    }
                                                },
                            receiptTotal = myreceiptLineTotal + Convert.ToDecimal(formattedtotalTax),
                            receiptPrintForm = "Receipt48"
                        };
                        var json = JsonConvert.SerializeObject(new { receipt = receiptData });

                        Helper.WriteInformation(new Exception(), json.ToString());

                        using (var HttpClient = new HttpClient())
                        {
                            // Set the content type
                            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                            // Send a POST request to the external server
                            var response = await HttpClient.PostAsync(apiUrl, content);

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
                                    myResponse deserializedData = JsonConvert.DeserializeObject<myResponse>(responseData);

                                    Helper.WriteInformation(new Exception(), deserializedData.ToString());
                                    foreach (var myitems in recieptItems)
                                    {
                                        myitems.isFiscalised = true;
                                        myitems.qrCode = deserializedData.QrString;
                                        myitems.zimraReceiptNo = deserializedData.receiptID;
                                        myitems.VerificationCode = deserializedData.VerificationCode;
                                        myitems.qrUrl = deserializedData.QrUrl;
                                        myitems.deviceSerialNo = deserializedData.DeviceSerialNo;
                                        myitems.fiscalDayNumber = deserializedData.FiscalDayNumber;
                                        myitems.deviceID = deserializedData.DeviceID;

                                        db.Entry(myitems).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                Helper.WriteInformation(new Exception(), response.StatusCode.ToString());
                                result = "Handina Response yandawana ";
                                return "400";

                                //return Request.CreateResponse(HttpStatusCode.OK, result, JsonRequestBehavior.AllowGet);
                            }
                        }

                        receiptlistItem2.IsBilled = true;

                        db.Entry(receiptlistItem2).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return "500";
            }

            Helper.WriteInformation(new Exception(), result.ToString());
            var data = new { message = "Success" };

            return "200";
        }




















    }
}
