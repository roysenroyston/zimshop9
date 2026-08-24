using ShopMate.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class InvoiceFormatController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        // GET: /InvoiceFormat/
        public ActionResult Index()
        {
            return View();
        }

        // GET InvoiceFormat/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.InvoiceFormats.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.CompanyName),
            Convert.ToString(c.Logo),
            Convert.ToString(c.AddressInfo),
            Convert.ToString(c.OtherInfo),
            Convert.ToString(c.FooterInfo),
            Convert.ToString(c.WarehouseId),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: /InvoiceFormat/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /InvoiceFormat/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceFormat ObjInvoiceFormat = db.InvoiceFormats.Find(id);
            if (ObjInvoiceFormat == null)
            {
                return HttpNotFound();
            }
            return View(ObjInvoiceFormat);
        }
        // GET: /InvoiceFormat/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: /InvoiceFormat/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(InvoiceFormat ObjInvoiceFormat, HttpPostedFileBase Logo, string HideImage1)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {
                    string base64String = "";

                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Save the image to the MemoryStream in a specific format (e.g., PNG)
                        if (Logo != null)
                        {
                            Logo.InputStream.CopyTo(ms);

                            // Convert the byte array in the MemoryStream to a Base64 string
                            byte[] imageBytes = ms.ToArray();
                            base64String = Convert.ToBase64String(imageBytes);
                        }



                        // Display the Base64 string (or do something else with it)
                        // MessageBox.Show(base64String);
                    }





                    if (Logo != null)
                    {
                        var fileName = MicrosoftHelper.MSHelper.StarkFileUploaderCSharp(Logo, Server.MapPath("~/Uploads"));
                        ModelState.Clear();
                        ObjInvoiceFormat.baseLogo = base64String;
                        ObjInvoiceFormat.Logo = Logo.FileName;
                    }
                    else
                    {
                        ObjInvoiceFormat.baseLogo = HideImage1;
                        ObjInvoiceFormat.Logo = "";
                    }


                    db.InvoiceFormats.Add(ObjInvoiceFormat);
                    db.SaveChanges(userId);

                    sb.Append("Sumitted");
                    return Content(sb.ToString());
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

            return Content(sb.ToString());

        }
        // GET: /InvoiceFormat/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceFormat ObjInvoiceFormat = db.InvoiceFormats.Find(id);
            if (ObjInvoiceFormat == null)
            {
                return HttpNotFound();
            }

            return View(ObjInvoiceFormat);
        }

        // POST: /InvoiceFormat/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(InvoiceFormat ObjInvoiceFormat, HttpPostedFileBase Logo, string HideImage1)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {
                    string base64String = "";

                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Save the image to the MemoryStream in a specific format (e.g., PNG)


                        Logo.InputStream.CopyTo(ms);

                        // Convert the byte array in the MemoryStream to a Base64 string
                        byte[] imageBytes = ms.ToArray();
                        base64String = Convert.ToBase64String(imageBytes);

                        // Display the Base64 string (or do something else with it)
                        // MessageBox.Show(base64String);
                    }





                    if (Logo != null)
                    {
                        var fileName = MicrosoftHelper.MSHelper.StarkFileUploaderCSharp(Logo, Server.MapPath("~/Uploads"));
                        ModelState.Clear();
                        ObjInvoiceFormat.baseLogo = base64String;
                    }
                    else
                    {
                        ObjInvoiceFormat.baseLogo = HideImage1;
                    }


                    db.Entry(ObjInvoiceFormat).State = EntityState.Modified;
                    db.SaveChanges(userId);

                    sb.Append("Sumitted");
                    return Content(sb.ToString());
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


            return Content(sb.ToString());

        }
        // GET: /InvoiceFormat/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceFormat ObjInvoiceFormat = db.InvoiceFormats.Find(id);
            if (ObjInvoiceFormat == null)
            {
                return HttpNotFound();
            }
            return View(ObjInvoiceFormat);
        }

        // POST: /InvoiceFormat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                InvoiceFormat ObjInvoiceFormat = db.InvoiceFormats.Find(id);
                db.InvoiceFormats.Remove(ObjInvoiceFormat);
                db.SaveChanges(userId);

                sb.Append("Sumitted");
                return Content(sb.ToString());

            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());

        }
        // GET: /InvoiceFormat/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            InvoiceFormat ObjInvoiceFormat = db.InvoiceFormats.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;

            }

            return View(ObjInvoiceFormat);
        }

        // POST: /InvoiceFormat/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(InvoiceFormat ObjInvoiceFormat, HttpPostedFileBase Logo, string HideImage1)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {
                    if (Logo != null)
                    {
                        var fileName = MicrosoftHelper.MSHelper.StarkFileUploaderCSharp(Logo, Server.MapPath("~/Uploads"));
                        ModelState.Clear();
                        ObjInvoiceFormat.Logo = fileName;
                    }
                    else
                    {
                        ObjInvoiceFormat.Logo = HideImage1;
                    }


                    db.Entry(ObjInvoiceFormat).State = EntityState.Modified;
                    db.SaveChanges(userId);

                    sb.Append("Sumitted");
                    return Content(sb.ToString());
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

            return Content(sb.ToString());

        }

        private SIContext db = new SIContext();


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

