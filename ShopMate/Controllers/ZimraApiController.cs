using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class ZimraApiController : ApiController
    {
        private SIContext db = new SIContext();

        [System.Web.Http.HttpPost]
        public async Task<ActionResult> SendSales(int? WarehouseId)

        {
            // URL of the external server's API endpoint for creating a new entry
            string apiUrl = "";
            string result = "";

            try
            {
                var receiptList = db.InformalInvoices.Where(k => k.IsBilled == false&& k.WarehouseId== WarehouseId).ToList();
                var receiptList2 = db.Invoices.Where(k => k.IsBilled == false && k.IsPurchaseOrSale == "Sale" && k.WarehouseId == WarehouseId).ToList();

                var DeviceId = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == WarehouseId).DeviceId;
                //var   DeviceId = "25820";
                // apiUrl = $"http://fiscit.pythonanywhere.com/api/submit-invoice/api-v1/{DeviceId}/";
                //apiUrl = "http://griffinprod.pythonanywhere.com/api/submit-invoice/api-v1/" + DeviceId + "/";
               //  apiUrl = "http://194.163.176.79:5005/api/submit_receipt/" + DeviceId;
                // apiUrl = "http://192.168.100.8:5008/api/submit_receipt/" + DeviceId;
                apiUrl = "http://192.168.100.69:5000/api/submit_receipt/" + DeviceId;

                bool Ngodza = true;

  
                var me = receiptList.Count();
             //   var me2 = receiptList2.Count();
                if (me != 0)
                {


                    foreach (var receiptlistItems in receiptList)
                    {
                        receiptlistItems.CustomerVatReg = "Success";
                        var mydata = db.Sales.Where(i => i.isFiscalised != true && i.recieptNumber == receiptlistItems.InvoiceNo && i.WarehouseId == WarehouseId).ToList();
                        var saleData = db.Sales.Where(k => k.isFiscalised != true && k.recieptNumber== receiptlistItems.InvoiceNo && k.WarehouseId == WarehouseId).FirstOrDefault();
                        if (saleData !=null)
                        {
                            List<Zimra.receiptLines> receipts = new List<Zimra.receiptLines>();
                            var myreceiptGlobalNo = 30 + 1;
                            string receiptCurencies = "";

                            var reciptcount = db.Sales.Where(k => k.isFiscalised != true&&k.recieptNumber == receiptlistItems.InvoiceNo&& k.WarehouseId==WarehouseId).Count();
                            var recieptItems = db.Sales.Where(k => k.isFiscalised != true&& k.recieptNumber == receiptlistItems.InvoiceNo && k.WarehouseId ==WarehouseId).ToList();

                            var myreceiptLineNo = 1;
                            decimal myreceiptLineTotal = 0;
                            decimal totalTax = 0;
                            decimal lineTax = 0;


                            if (saleData.PaymentModeId == 6)
                            {
                                receiptCurencies = "USD";
                            }
                            else
                            {
                                receiptCurencies = "ZWG";
                            }

                            foreach (var item in recieptItems)
                            {
                                var myTaxCode = "";
                                double? taxPernt = null;
                                var ztaxId = 0;

                                var taxId = db.Products.Where(j => j.Name == item.Product_ProductId.Name && j.WarehouseId == WarehouseId).FirstOrDefault().TaxId;
                         
                                if (taxId == 2)
                                {
                                    myTaxCode = "E";
                                    taxPernt = 15.5;
                                    ztaxId =517;
                                }
                                else if (taxId == 5)
                                {
                                    myTaxCode = "B";
                                    taxPernt = 0;
                                    ztaxId = 2;
                                }
                                else
                                {
                                    myTaxCode = "C";
                                    ztaxId =3;
                                }
                         
                                var kyle = new Zimra.receiptLines
                                {
                                    receiptLineType = "Sale",
                                    receiptLineNo = myreceiptLineNo,
                                    receiptLineHSCode = (string.IsNullOrEmpty(item.Product_ProductId.HSNCode) || item.Product_ProductId.HSNCode == "0") ? "12345678" : item.Product_ProductId.HSNCode,
                                receiptLineName = item.Product_ProductId.Name,
                                    receiptLinePrice = item.SalePrice,
                                    receiptLineQuantity = item.Quantity,
                                    receiptLineTotal = Math.Round(item.Quantity * item.SalePrice, 2,MidpointRounding.AwayFromZero),
                                    taxPercent = taxPernt,
                                    taxCode = myTaxCode,
                                    taxID = ztaxId
                                };
                                receipts.Add(kyle);
                                myreceiptLineNo = myreceiptLineNo + 1;
                                myreceiptLineTotal = myreceiptLineTotal + kyle.receiptLineTotal;
                                lineTax = lineTax + (decimal)item.TotalAmountWithTax;
                                //}
                                //else
                                //{
                                //    //var mypayment = db.PaymentModes.FirstOrDefault(i => i.Id == saleData.PaymentModeId).Name;
                                //   // var mycurrency = db.Currencies.FirstOrDefault(i => i.Name == mypayment).Id;
                                //   // var priceRate = db.Rates.Where(i => i.CurrencyId == mycurrency).OrderByDescending(i => i.DateModified).First().CurrencyRate;

                                //    var kyle = new Zimra.receiptLines
                                //    {
                                //        receiptLineType = "Sale",
                                //        receiptLineNo = myreceiptLineNo,
                                //        receiptLineHSCode = "12345",
                                //        receiptLineName = item.Product_ProductId.Name,
                                //        receiptLinePrice = Math.Round(item.SalePrice * (decimal)priceRate, 2),
                                //        receiptLineQuantity = (int)item.Quantity,
                                //        receiptLineTotal = Math.Round((int)item.Quantity * (item.SalePrice * (decimal)priceRate), 2),
                                //        taxPercent = taxPernt,
                                //        taxCode = myTaxCode,
                                //        taxID = ztaxId
                                //    };
                                //    receipts.Add(kyle);
                                //    myreceiptLineNo = myreceiptLineNo + 1;
                                //    myreceiptLineTotal = myreceiptLineTotal + kyle.receiptLineTotal;
                                //    lineTax = lineTax + ((decimal)item.TotalAmountWithTax * (decimal)priceRate);
                                //}
                            }

                            totalTax = Math.Round(totalTax + lineTax, 2,MidpointRounding.AwayFromZero);
                            decimal formattedtotalTax = totalTax;

                            Zimra.BuyerData buyerData = null;

                            var Customer = db.Customers.FirstOrDefault(k => k.Id == saleData.CustomerUserId);
                            if (Customer != null&& (Customer.BuyerTIN !=null&& Customer.BuyerTIN!="0"))
                            {

                                buyerData = new Zimra.BuyerData
                                {
                                    BuyerRegisterName = Customer.BuyerRegisterName,
                                    BuyerTradeName = Customer.BuyerTradeName,
                                    BuyerContacts = new Zimra.BuyerContacts
                                    {
                                        PhoneNo = Customer.PhoneNo,
                                        Email = Customer.Email,
                                    },
                                    BuyerTIN = Customer.BuyerTIN,
                                    VATNumber = Customer.VATNumber,
                                    BuyerAddress = new Zimra.BuyerAddress
                                    {
                                        Province = Customer.Province,
                                        Street = Customer.Street,
                                        HouseNo = Customer.HouseNo,
                                        City = Customer.City
                                    }
                                };
                            }



                            var receiptData = new Zimra.receipt

                            {
                                receiptType = "FiscalInvoice",
                                receiptCurrency = receiptCurencies,
                                receiptCounter = reciptcount,
                                receiptGlobalNo = myreceiptGlobalNo,
                                invoiceNo = Convert.ToString(receiptlistItems.InvoiceNo),
                                buyerData = buyerData,
                                receiptNotes = null,
                                receiptDate = saleData.DateModied,
                                creditDebitNote = null,
                                receiptLinesTaxInclusive = false,
                                receiptLines = receipts,
                                receiptTaxes = new List<Zimra.receiptTaxs>
                                                {
                                                    new Zimra.receiptTaxs
                                                    {
                                                        taxCode = "E",
                                                        taxPercent = 15.5,
                                                        taxID =517,
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
                            var json = JsonConvert.SerializeObject(new { receipt = receiptData }, Formatting.Indented);

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
                                    string responseData = await response.Content.ReadAsStringAsync();
                                    System.Diagnostics.Debug.WriteLine("Test1 : " + responseData);

                                    string errorOnly;
                                    try
                                    {
                                        var j = JObject.Parse(responseData);

                                        // 1) Try explicit error_message
                                        errorOnly = j["error_message"]?.ToString();

                                        if (string.IsNullOrWhiteSpace(errorOnly))
                                            errorOnly = j["details"]?["errors"]?.First?.ToString();

                                        // 2) Try top-level error
                                        if (string.IsNullOrWhiteSpace(errorOnly))
                                            errorOnly = j["error"]?.ToString();

                                        // 3) Try nested VATNumber error


                                        // 4) Fallback to whole JSON
                                        if (string.IsNullOrWhiteSpace(errorOnly))
                                            errorOnly = responseData;
                                    }
                                    catch
                                    {
                                        // Not JSON, fallback to raw
                                        errorOnly = responseData;
                                    }

                                    Helper.WriteInformation(new Exception(), errorOnly);

                                    receiptlistItems.CustomerVatReg = "Failed " ;
                                    receiptlistItems.ErrorMessage = errorOnly;
                                    db.Entry(receiptlistItems).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //return Request.CreateResponse(HttpStatusCode.OK, result, JsonRequestBehavior.AllowGet);
                                    //string responseData = await response.Content.ReadAsStringAsync();
                                    //System.Diagnostics.Debug.WriteLine("Test1 : " + responseData);
                                    //Helper.WriteInformation(new Exception(), responseData.ToString());
                                    //errorResponse deserializedData = JsonConvert.DeserializeObject<errorResponse>(responseData);
                                    //Helper.WriteInformation(new Exception(), response.StatusCode.ToString());
                                    //result = deserializedData.error_message;
                                    //Ngodza = false;
                                    //// return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                                    //receiptlistItems.CustomerVatReg = "Failed " +deserializedData.error_message;
                                    //receiptlistItems.ErrorMessage = deserializedData.error_message;
                                    ////receiptlistItems.Re
                                    //db.Entry(receiptlistItems).State = EntityState.Modified;
                                    //db.SaveChanges();
                                    ////return Request.CreateResponse(HttpStatusCode.OK, result, JsonRequestBehavior.AllowGet);
                                }
                            }

                            receiptlistItems.IsBilled = true;
                            db.Entry(receiptlistItems).State = EntityState.Modified;
                            db.SaveChanges();
                        }
            

                    }
                }
                //else if (me2 != 0)
                //{
                //    foreach (var receiptlistItem2 in receiptList2)
                //    {
                //        var mydata = db.Sales.Where(i => i.isFiscalised != true && i.recieptNumber == receiptlistItem2.InvoiceNo).ToList();
                //        var saleData = db.Sales.Where(k => k.isFiscalised != true && k.InvoiceId == receiptlistItem2.InvoiceNo).FirstOrDefault();
                //        List<Zimra.receiptLines> receipts = new List<Zimra.receiptLines>();
                //        var myreceiptGlobalNo = 30 + 1;

                //        var reciptcount = db.Sales.Where(k => k.recieptNumber == receiptlistItem2.InvoiceNo).Count();
                //        var recieptItems = db.Sales.Where(k => k.recieptNumber == receiptlistItem2.InvoiceNo).ToList();

                //        var myreceiptLineNo = 1;
                //        decimal myreceiptLineTotal = 0;
                //        decimal totalTax = 0;

                //        foreach (var item in recieptItems)
                //        {
                //            var myTaxCode = "";
                //            int? taxPernt = null;
                //            var ztaxId = 0;
                //            var taxId = db.Products.Where(j => j.Name == item.Product_ProductId.Name).FirstOrDefault().TaxId;
                //            if (taxId == 2)
                //            {
                //                myTaxCode = "C";
                //                taxPernt = 15;
                //                ztaxId = 3;
                //            }
                //            else if (taxId == 5)
                //            {
                //                myTaxCode = "B";
                //                taxPernt = 0;
                //                ztaxId = 2;
                //            }
                //            else
                //            {
                //                myTaxCode = "A";
                //                ztaxId = 1;
                //            }

                //            //        decimal taxAmount = (item.SalePrice *taxPernt);
                //            //    string formattedtaxAmount = taxAmount.ToString("F2");

                //            var kyle = new Zimra.receiptLines
                //            {
                //                receiptLineType = "Sale",
                //                receiptLineNo = myreceiptLineNo,
                //                receiptLineHSCode = "12345",
                //                receiptLineName = item.Product_ProductId.Name,
                //                receiptLinePrice = item.SalePrice,
                //                receiptLineQuantity = item.Quantity,
                //                receiptLineTotal = (item.Quantity * item.SalePrice),
                //                taxCode = myTaxCode,
                //                taxPercent = taxPernt,
                //                taxID = ztaxId
                //            };
                //            receipts.Add(kyle);
                //            myreceiptLineNo = myreceiptLineNo + 1;
                //            myreceiptLineTotal = myreceiptLineTotal + kyle.receiptLineTotal;
                //        }

                //        totalTax = totalTax + (myreceiptLineTotal * (decimal)0.15);
                //        string formattedtotalTax = totalTax.ToString("F2");
                //        var receiptData = new Zimra.receipt

                //        {
                //            receiptType = "FiscalInvoice",
                //            receiptCurrency = "USD",
                //            receiptCounter = reciptcount,
                //            receiptGlobalNo = myreceiptGlobalNo,
                //            invoiceNo = Convert.ToString(saleData.recieptNumber),
                //            buyerData = null,
                //            receiptNotes = null,
                //            receiptDate = saleData.DateModied,
                //            creditDebitNote = null,
                //            receiptLinesTaxInclusive = false,
                //            receiptLines = receipts,
                //            receiptTaxes = new List<Zimra.receiptTaxs>
                //                                {
                //                                    new Zimra.receiptTaxs
                //                                    {
                //                                        taxCode = "C",
                //                                        taxPercent = 15,
                //                                        taxID = 3,
                //                                        taxAmount =  Convert.ToDecimal(formattedtotalTax),
                //                                        salesAmountWithTax = myreceiptLineTotal+ Convert.ToDecimal(formattedtotalTax),
                //                                    }
                //                                },
                //            receiptPayments = new List<Zimra.receiptPayments>
                //                                {
                //                                    new Zimra.receiptPayments
                //                                    {
                //                                        moneyTypeCode = "Cash",
                //                                        paymentAmount = myreceiptLineTotal+ Convert.ToDecimal(formattedtotalTax),
                //                                    }
                //                                },
                //            receiptTotal = myreceiptLineTotal + Convert.ToDecimal(formattedtotalTax),
                //            receiptPrintForm = "Receipt48"
                //        };
                //        var json = JsonConvert.SerializeObject(new { receipt = receiptData });

                //        Helper.WriteInformation(new Exception(), json.ToString());

                //        using (var HttpClient = new HttpClient())
                //        {
                //            // Set the content type
                //            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                //            // Send a POST request to the external server
                //            var response = await HttpClient.PostAsync(apiUrl, content);

                //            System.Diagnostics.Debug.WriteLine("Test1 : " + response.IsSuccessStatusCode);

                //            if (response.IsSuccessStatusCode)
                //            {
                //                string responseData = await response.Content.ReadAsStringAsync();
                //                Helper.WriteInformation(new Exception(), responseData.ToString());

                //                try
                //                {
                //                    string jsonFilePath = responseData;

                //                    // Read the JSON content from the file
                //                    //string jsonContent = System.IO.File.ReadAllText(jsonFilePath);

                //                    // Deserialize JSON content into the JsonModel class
                //                    myResponse deserializedData = JsonConvert.DeserializeObject<myResponse>(responseData);

                //                    Helper.WriteInformation(new Exception(), deserializedData.ToString());
                //                    foreach (var myitems in recieptItems)
                //                    {
                //                        myitems.isFiscalised = true;
                //                        myitems.qrCode = deserializedData.QrString;
                //                        myitems.zimraReceiptNo = deserializedData.receiptID;
                //                        myitems.VerificationCode = deserializedData.VerificationCode;
                //                        myitems.qrUrl = deserializedData.QrUrl;
                //                        myitems.deviceSerialNo = deserializedData.DeviceSerialNo;
                //                        myitems.fiscalDayNumber = deserializedData.FiscalDayNumber;
                //                        myitems.deviceID = deserializedData.DeviceID;

                //                        db.Entry(myitems).State = EntityState.Modified;
                //                        db.SaveChanges();
                //                    }
                //                }
                //                catch
                //                {
                //                }
                //            }
                //            else
                //            {
                //                Helper.WriteInformation(new Exception(), response.StatusCode.ToString());
                //                result = "Handina Response yandawana ";
                //                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //string responseData = await response.Content.ReadAsStringAsync();
                //System.Diagnostics.Debug.WriteLine("Test1 : " + responseData);
                //Helper.WriteInformation(new Exception(), responseData.ToString());
                //errorResponse deserializedData = JsonConvert.DeserializeObject<errorResponse>(responseData);
                //Helper.WriteInformation(new Exception(), response.StatusCode.ToString());
                //result = deserializedData.error_message;
                //Ngodza = false;
                //// return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //receiptlistItems.CustomerVatReg = "Failed " + deserializedData.error_message;
                //receiptlistItems.ErrorMessage = deserializedData.error_message;
                ////receiptlistItems.Re
                //db.Entry(receiptlistItems).State = EntityState.Modified;
                //db.SaveChanges();

                //                //return Request.CreateResponse(HttpStatusCode.OK, result, JsonRequestBehavior.AllowGet);
                //            }
                //        }

                //        receiptlistItem2.IsBilled = true;

                //        db.Entry(receiptlistItem2).State = EntityState.Modified;
                //        db.SaveChanges();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
            }

            Helper.WriteInformation(new Exception(), result.ToString());
            var data = new { message = "Success" };

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private ActionResult Json(string result, JsonRequestBehavior allowGet)
        {
            throw new NotImplementedException();
        }

        [System.Web.Http.HttpGet]
        public async Task<ActionResult> DebitCreditNote(int? WarehouseId)

        {
            // URL of the external server's API endpoint for creating a new entry
            string apiUrl = "";
            string result = "";

            try
            {
                var receiptList = db.DebitCreditNotes.Where(k => k.IsFiscilazed == false && k.WarehouseId == WarehouseId).ToList();
                var DeviceId = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == WarehouseId).DeviceId;
                //apiUrl = "http://griffinprod.pythonanywhere.com/api/submit-invoice/api-v1/" + DeviceId + "/";
                //   apiUrl = "http://griffinprod.pythonanywhere.com/api/submit-invoice/api-v1/{DeviceId}/";
                // apiUrl = $"http://fiscit.pythonanywhere.com/api/submit-invoice/api-v1/{DeviceId}/";
                // apiUrl = $"http://giftmashuro.pythonanywhere.com/api/submit-invoice/api-v1/{DeviceId}/";
                apiUrl = "http://194.163.176.79:5005/api/submit_receipt/" + DeviceId;
                //var me = receiptList.Count();
                //var me2 = receiptList2.Count();

                foreach (var receiptlistItems in receiptList)
                {
                    if (receiptlistItems.ReceiptType == "CreditNote")
                    {
                        var mydata = db.DebitCreditItems.Where(i => i.debitCreditNoteId == receiptlistItems.Id && i.ReceiptNo == receiptlistItems.receiptNo).ToList();
                        //var saleData = db.Sales.Where(k => k.isFiscalised != true).FirstOrDefault();
                        List<Zimra.receiptLines> receipts = new List<Zimra.receiptLines>();
                        receiptlistItems.Status = "Success";
                        var myreceiptGlobalNo = 30 + 1;

                        var reciptcount = db.DebitCreditItems.Where(k => k.debitCreditNoteId == receiptlistItems.Id).Count();
                        var recieptItems = db.DebitCreditItems.Where(k => k.ReceiptNo == receiptlistItems.receiptNo && k.isFiscal == false).ToList();

                        var myreceiptLineNo = 1;
              

                        foreach (var item in recieptItems)
                        {
                            var myTaxCode = "";
                            double? taxPernt = null;
                            var ztaxId = 0;
                            var taxId = db.Products.Where(j => j.Name == item.receiptLineName).FirstOrDefault().TaxId;
                            if (taxId == 2)
                            {
                                myTaxCode = "E";
                                taxPernt = 15.5;
                                ztaxId = 515;
                            }
                            else if (taxId == 5)
                            {
                                myTaxCode = "B";
                                taxPernt = 0;
                                ztaxId = 2;
                            }
                            else
                            {
                                myTaxCode = "C";
                                ztaxId = 3;
                            }

                            //        decimal taxAmount = (item.SalePrice *taxPernt);
                            //    string formattedtaxAmount = taxAmount.ToString("F2");

                            var kyle = new Zimra.receiptLines
                            {
                                receiptLineType = "Sale",
                                receiptLineNo = myreceiptLineNo,
                                receiptLineHSCode = "12345678",
                                receiptLineName = item.receiptLineName,
                                receiptLinePrice = item.receiptLinePrice,
                                receiptLineQuantity = item.receiptLineQuantity,
                                receiptLineTotal = item.receiptLineTotal,
                                taxCode = myTaxCode,
                                taxPercent = taxPernt,
                                taxID = ztaxId
                            };
                            receipts.Add(kyle);
                            myreceiptLineNo = myreceiptLineNo + 1;

                        }

                        /*    decimal formattedtotalTax = 0 - (decimal)receiptlistItems.vat;

                            var payement = (0 - receiptlistItems.total);
                            var reciepT = (decimal)(0 - receiptlistItems.total);
                            var saesTax = (0 - receiptlistItems.total);*/
                        decimal formattedtotalTax = 0 - (decimal)receiptlistItems.vat;
                        var payement = (0 - receiptlistItems.total );
                        var reciepT = (decimal)(0 - receiptlistItems.total );
                        var saesTax = (0 - receiptlistItems.total );
                        var receiptData = new Zimra.receipt

                        {
                            receiptType = "CreditNote",
                            receiptCurrency = receiptlistItems.ReceiptCurrency,
                            receiptCounter = reciptcount,
                            receiptGlobalNo = myreceiptGlobalNo,
                            invoiceNo = receiptlistItems.InvoiceNo,
                            buyerData = null,
                            receiptNotes = receiptlistItems.Remarks,
                            receiptDate = receiptlistItems.Duedate,
                            creditDebitNote = new Zimra.CreditDebitNote
                            {
                                receiptID = receiptlistItems.RecieptId
                            },
                            //creditDebitNote =
                            //{
                            //    receiptID
                            //}
                            receiptLinesTaxInclusive = false,
                            receiptLines = receipts,
                            receiptTaxes = new List<Zimra.receiptTaxs>
                                                {
                                                    new Zimra.receiptTaxs
                                                    {
                                                        taxCode = "E",
                                                        taxPercent = 15.5,
                                                        taxID =515,
                                                        taxAmount = Convert.ToDecimal(formattedtotalTax),
                                                        salesAmountWithTax =saesTax,
                                                    }
                                                },
                            receiptPayments = new List<Zimra.receiptPayments>
                                                {
                                                    new Zimra.receiptPayments
                                                    {
                                                        moneyTypeCode = "Cash",

                                                        paymentAmount =payement,
                                                    }
                                                },
                            receiptTotal = reciepT,
                            receiptPrintForm = "Receipt48"
                        };
                        var json = JsonConvert.SerializeObject(new { receipt = receiptData }, Formatting.Indented);

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
                                        myitems.isFiscal = true;
                                        myitems.qrCode = deserializedData.QrString;
                                        myitems.receiptId = deserializedData.receiptID;
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
                               
                                    string responseData = await response.Content.ReadAsStringAsync();
                                    System.Diagnostics.Debug.WriteLine("Test1 : " + responseData);

                                    string errorOnly;
                                    try
                                    {
                                        var j = JObject.Parse(responseData);

                                        // 1) Try explicit error_message
                                        errorOnly = j["error_message"]?.ToString();

                                        if (string.IsNullOrWhiteSpace(errorOnly))
                                            errorOnly = j["details"]?["errors"]?.First?.ToString();

                                        // 2) Try top-level error
                                        if (string.IsNullOrWhiteSpace(errorOnly))
                                            errorOnly = j["error"]?.ToString();

                                        // 3) Try nested VATNumber error


                                        // 4) Fallback to whole JSON
                                        if (string.IsNullOrWhiteSpace(errorOnly))
                                            errorOnly = responseData;
                                    }
                                    catch
                                    {
                                        // Not JSON, fallback to raw
                                        errorOnly = responseData;
                                    }

                                    Helper.WriteInformation(new Exception(), errorOnly);

                                    receiptlistItems.Status = "Failed ";
                                    receiptlistItems.ErrorMessage = errorOnly;
                                    db.Entry(receiptlistItems).State = EntityState.Modified;
                                    db.SaveChanges();
                                    //string responseData = await response.Content.ReadAsStringAsync();
                                    //System.Diagnostics.Debug.WriteLine("Test1 : " + responseData);
                                    //Helper.WriteInformation(new Exception(), responseData.ToString());
                                    //errorResponse deserializedData = JsonConvert.DeserializeObject<errorResponse>(responseData);
                                    //Helper.WriteInformation(new Exception(), response.StatusCode.ToString());
                                    //result = deserializedData.error_message;

                                    //// return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                                    //receiptlistItems.Status = "Failed ";
                                    //receiptlistItems.ErrorMessage = deserializedData.error_message;
                                    ////receiptlistItems.Re
                                    //db.Entry(receiptlistItems).State = EntityState.Modified;
                                    //db.SaveChanges();


                                    //Helper.WriteInformation(new Exception(), response.StatusCode.ToString());
                                    //result = "Handina Response yandawana ";
                                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                                    ////return Request.CreateResponse(HttpStatusCode.OK, result, JsonRequestBehavior.AllowGet);
                                }
                        }

                        receiptlistItems.IsFiscilazed = true;

                        db.Entry(receiptlistItems).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var mydata = db.DebitCreditItems.Where(i => i.debitCreditNoteId == receiptlistItems.Id && i.ReceiptNo == receiptlistItems.receiptNo).ToList();
                        //var saleData = db.Sales.Where(k => k.isFiscalised != true).FirstOrDefault();
                        List<Zimra.receiptLines> receipts = new List<Zimra.receiptLines>();

                        var myreceiptGlobalNo = 30 + 1;

                        var reciptcount = db.DebitCreditItems.Where(k => k.debitCreditNoteId == receiptlistItems.Id).Count();
                        var recieptItems = db.DebitCreditItems.Where(k => k.ReceiptNo == receiptlistItems.receiptNo).ToList();

                        var myreceiptLineNo = 1;

                        receiptlistItems.Status = "Success";
                        foreach (var item in recieptItems)
                        {
                            var myTaxCode = "";
                            double? taxPernt = null;
                            var ztaxId = 0;
                            var taxId = db.Products.Where(j => j.Name == item.receiptLineName).FirstOrDefault().TaxId;
                            if (taxId == 2)
                            {
                                myTaxCode = "E";
                                taxPernt = 15.5;
                                ztaxId = 515;
                            }
                            else if (taxId == 5)
                            {
                                myTaxCode = "B";
                                taxPernt = 0;
                                ztaxId = 2;
                            }
                            else
                            {
                                myTaxCode = "C";
                                ztaxId = 3;
                            }

                            //        decimal taxAmount = (item.SalePrice *taxPernt);
                            //    string formattedtaxAmount = taxAmount.ToString("F2");

                            var kyle = new Zimra.receiptLines
                            {
                                receiptLineType = "Sale",
                                receiptLineNo = myreceiptLineNo,
                                receiptLineHSCode = "12345578",
                                receiptLineName = item.receiptLineName,
                                receiptLinePrice = item.receiptLinePrice,
                                receiptLineQuantity = item.receiptLineQuantity,
                                receiptLineTotal = (item.receiptLineQuantity * item.receiptLinePrice),
                                taxCode = myTaxCode,
                                taxPercent = taxPernt,
                                taxID = ztaxId
                            };
                            receipts.Add(kyle);
                            myreceiptLineNo = myreceiptLineNo + 1;
                   
                        }
               
    
                        decimal formattedtotalTax = (decimal)receiptlistItems.vat;

                        var payement = (receiptlistItems.total);
                        var reciepT = (decimal)(receiptlistItems.total);
                        var saesTax = (receiptlistItems.total);
                        var receiptData = new Zimra.receipt

                        {
                            receiptType = "DebitNote",
                            receiptCurrency = receiptlistItems.ReceiptCurrency,
                            receiptCounter = reciptcount,
                            receiptGlobalNo = myreceiptGlobalNo,
                            invoiceNo = receiptlistItems.InvoiceNo,
                            buyerData = null,
                            receiptNotes = receiptlistItems.Remarks,
                            receiptDate = receiptlistItems.Duedate,
                            creditDebitNote = new Zimra.CreditDebitNote
                            {
                                receiptID = receiptlistItems.RecieptId
                            },
                            //creditDebitNote =
                            //{
                            //    receiptID
                            //}
                            receiptLinesTaxInclusive = false,
                            receiptLines = receipts,
                            receiptTaxes = new List<Zimra.receiptTaxs>
                                                {
                                                    new Zimra.receiptTaxs
                                                    {
                                                        taxCode = "E",
                                                        taxPercent = 15.5,
                                                        taxID =515,
                                                        taxAmount = Convert.ToDecimal(formattedtotalTax),
                                                        salesAmountWithTax =saesTax,
                                                    }
                                                },
                            receiptPayments = new List<Zimra.receiptPayments>
                                                {
                                                    new Zimra.receiptPayments
                                                    {
                                                        moneyTypeCode = "Cash",

                                                        paymentAmount =payement,
                                                    }
                                                },
                            receiptTotal = reciepT,
                            receiptPrintForm = "Receipt48"
                        };
                        var json = JsonConvert.SerializeObject(new { receipt = receiptData },Formatting.Indented);

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
                                        myitems.isFiscal = true;
                                        myitems.qrCode = deserializedData.QrString;
                                        myitems.receiptId = deserializedData.receiptID;
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
                                
                                    string responseData = await response.Content.ReadAsStringAsync();
                                    System.Diagnostics.Debug.WriteLine("Test1 : " + responseData);

                                    string errorOnly;
                                    try
                                    {
                                        var j = JObject.Parse(responseData);

                                        // 1) Try explicit error_message
                                        errorOnly = j["error_message"]?.ToString();

                                        if (string.IsNullOrWhiteSpace(errorOnly))
                                            errorOnly = j["details"]?["errors"]?.First?.ToString();

                                        // 2) Try top-level error
                                        if (string.IsNullOrWhiteSpace(errorOnly))
                                            errorOnly = j["error"]?.ToString();

                                        // 3) Try nested VATNumber error


                                        // 4) Fallback to whole JSON
                                        if (string.IsNullOrWhiteSpace(errorOnly))
                                            errorOnly = responseData;
                                    }
                                    catch
                                    {
                                        // Not JSON, fallback to raw
                                        errorOnly = responseData;
                                    }

                                    Helper.WriteInformation(new Exception(), errorOnly);

                                    receiptlistItems.Status = "Failed ";
                                    receiptlistItems.ErrorMessage = errorOnly;
                                    db.Entry(receiptlistItems).State = EntityState.Modified;
                                    db.SaveChanges();
                                    //string responseData = await response.Content.ReadAsStringAsync();
                                    //System.Diagnostics.Debug.WriteLine("Test1 : " + responseData);
                                    //Helper.WriteInformation(new Exception(), responseData.ToString());
                                    //errorResponse deserializedData = JsonConvert.DeserializeObject<errorResponse>(responseData);
                                    //Helper.WriteInformation(new Exception(), response.StatusCode.ToString());
                                    //result = deserializedData.error_message;

                                    //// return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                                    //receiptlistItems.Status = "Failed ";
                                    //receiptlistItems.ErrorMessage = deserializedData.error_message;
                                    ////receiptlistItems.Re
                                    //db.Entry(receiptlistItems).State = EntityState.Modified;
                                    //db.SaveChanges();


                                    //  return Request.CreateResponse(HttpStatusCode.OK, result, JsonRequestBehavior.AllowGet);
                                }
                        }

                        receiptlistItems.IsFiscilazed = true;
                   
                        receiptlistItems.RecieptId = db.DebitCreditItems.Where(k => k.debitCreditNoteId == receiptlistItems.Id).FirstOrDefault().receiptId;
                        db.Entry(receiptlistItems).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
            }

            Helper.WriteInformation(new Exception(), result.ToString());
            var data = new { message = "Success" };

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}