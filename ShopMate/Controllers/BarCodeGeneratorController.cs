using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class BarCodeGeneratorController : Controller
    {
        // GET: BarCodeGenerator
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string barcode)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Bitmap bitMap = new Bitmap(barcode.Length * 40, 80))
                {
                    using (Graphics graphics = Graphics.FromImage(bitMap))
                    {
                        Font oFont = new Font("IDAutomationHC39M", 16, FontStyle.Regular);
                        PointF point = new PointF(2f, 2f);
                        SolidBrush whiteBrush = new SolidBrush(Color.White);
                        graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);
                        SolidBrush blackBrush = new SolidBrush(Color.Black);
                        graphics.DrawString("*" + barcode + "*", oFont, blackBrush, point);
                    }

                    bitMap.Save(memoryStream, ImageFormat.Jpeg);

                    ViewBag.BarcodeImage = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            return View();
        }
        // GET: BarCodeGenerator/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        private SIContext db = new SIContext();
        // GET: BarCode reader
        public ActionResult BarCodeRead()
        {
            string[] data = Spire.Barcode.BarcodeScanner.Scan("Code39.png",Spire.Barcode.BarCodeType.Code39);

            return View();
        }
        // POST: BarCodeGenerator/Create
        [HttpPost]
        public ActionResult BarCodeRead(Bitmap btmap)
        {
            try
            {
                // TODO: Add insert logic here
                
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }

        // GET: BarCodeGenerator/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BarCodeGenerator/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }

        // GET: BarCodeGenerator/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BarCodeGenerator/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }
    }
}
