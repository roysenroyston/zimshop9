using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebErrorLogging.Utilities;
//using System.Security.Claims;
//using System.Threading;


namespace ShopMate.Controllers
{
    public class GRVController : Controller
    {
        // GET: GRV
        public ActionResult Index()
        {
            return View();
        }

        //Get: GRV/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.GRVs.ToArray();

            var result = from c in tak
                         select new string[]
                {
                    c.Id.ToString(), Convert.ToString(c.Id),
                     Convert.ToString(c.supplier),
                     Convert.ToString(c.receivedby),
                    Convert.ToString(c.OrderNumber),
                    Convert.ToString(c.purchasedate),
                      Convert.ToString(c.approved),
                    //Convert.ToString(c.Description),
                    //Convert.ToString(c.Quantity),
                    //Convert.ToString(c.UnitPrice),
                    //Convert.ToString(c.TotalPrice),
                   //Convert.ToString(c.ValidUntil),
                    //Convert.ToString(c.approved),
                   // Convert.ToString(c.Warehouse),
                };

            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        // GET: /GRV/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        //GetStockshippingOrderItems

        public ActionResult GetGrvOrderItems(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var ObjSaleOrdertems = db.GRVMaterials.Where(i => i.GRVId == (id)).ToArray();
                List<GRVMaterialsDto> lstOrderSaleItem = new List<GRVMaterialsDto>();
                foreach (var item in ObjSaleOrdertems)
                {
                    GRVMaterialsDto orderItem = new GRVMaterialsDto();
                    orderItem.ProductId = item.ProductId;
                    orderItem.Name = item.Product_ProductId.Name;
                    orderItem.Description = item.Product_ProductId.ProductDescription;
                    orderItem.Quantity = item.Quantity;
                    orderItem.Status = "Good";

                    lstOrderSaleItem.Add(orderItem);

                }
                if (ObjSaleOrdertems == null)
                {
                    return HttpNotFound();
                }
                var result = JsonConvert.SerializeObject(lstOrderSaleItem, Formatting.Indented,
                               new JsonSerializerSettings
                               {
                                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                               });

                return Json(result, JsonRequestBehavior.AllowGet);
                //return Json(new { saleorderItems = ObjSaleOrdertems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return HttpNotFound(ex.Message);
            }
        }

        public ActionResult GrvOrder()
        {
            return View();
        }
        // GET: GRV/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GRV ObjGRV = db.GRVs.Find(id);
            var materials = db.GRVMaterials.Where(grv => grv.GRVId == id);
            //var ObjGRVs = db.GRVs.Where(i => i.Id == (id) && i.approved == false).ToArray();

            if (ObjGRV == null)
            {
                return HttpNotFound();
            }

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            GRVDto dto = new GRVDto();
            dto.Id = ObjGRV.Id;
            //dto.OrderNumber = (int)ObjGRV.OrderNumber;
            dto.purchasedate = ObjGRV.purchasedate;
            dto.receivedby = ObjGRV.receivedby;
            dto.supplier = ObjGRV.supplier;
            dto.CompanyAddress = invoiceFormat.AddressInfo;
            dto.CompanyContact = invoiceFormat.OtherInfo;

            List<GRVMaterialsDto> materialsDtos = new List<GRVMaterialsDto>();

            foreach (var item in materials)
            {
                //var Name = db.Products.FirstOrDefault(i => i.Id == item.ProductId).Name;
                GRVMaterialsDto gRV = new GRVMaterialsDto();
                //gRV.Name = item.Product_ProductId.Name;
                gRV.Description = item.Description;
                gRV.Quantity = item.Quantity;
                gRV.Id = item.Id;


                materialsDtos.Add(gRV);
            }

            dto.GRVMaterials = materialsDtos;

            return View(dto);
        }

        // GET: GRV/Create
        public ActionResult Create()
        {
            ViewBag.OrderNumber = new SelectList(db.Orders, "Id", "Goods");
            //ViewBag.OrderNumber = new SelectList(db.StockShippingOrders.Where(i => i.IsReceived == false), "Id", "Id");
            ViewBag.CustomerUserId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Supplier"), "Id", "FullName");
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");

            return View();
        }

        // POST: GRV/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //public ActionResult Create(GRV objGRV)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {


        //            db.GRVs.Add(objGRV);
        //            db.SaveChanges();

        //            sb.Append("Sumitted");
        //            return Content(sb.ToString());
        //        }
        //        else
        //        {
        //            foreach (var key in this.ViewData.ModelState.Keys)
        //            {
        //                foreach (var err in this.ViewData.ModelState[key].Errors)
        //                {
        //                    sb.Append(err.ErrorMessage + "<br/>");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sb.Append("Error :" + ex.Message);
        //    }

        //    return Content(sb.ToString());
        //}

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(GRV objGRV, bool approved, GRVMaterials[] GRVMaterials)
        {
            //var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            //// Get the claims values
            //int warehouse = Int16.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Actor)
            //                   .Select(c => c.Value).SingleOrDefault());

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Order Is Not Complete!";
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            try
            {
                if (ModelState.IsValid)
                {
                    objGRV.approved = approved;
                    objGRV.purchasedate = DateTime.Now;
                    //objGRV.receivedby = AddedBy;
                    db.GRVs.Add(objGRV);
                    db.SaveChanges();


                    foreach (var item in GRVMaterials)
                    {
                        GRVMaterials materials = new GRVMaterials();
                        materials.Description = item.Description;
                        materials.ProductId = item.ProductId;
                        materials.GRVId = objGRV.Id;
                        materials.Quantity = item.Quantity;
                        materials.Status = item.Status;
                        //materials.approved =item.approved;
                        db.GRVMaterials.Add(materials);
                        db.SaveChanges();

                        //var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.ProductId && i.WarehouseId == objGRV.Warehouse);//ngonie start
                        //var SelectedProduct = db.Products.FirstOrDefault(i => i.Id == item.ProductId);
                        ////if (ObjWarehouseStock.WarehouseId == 1)
                        //{
                        //    SelectedProduct.RemainingQuantity = SelectedProduct.RemainingQuantity - item.Quantity;//dispatch
                        //    ObjWarehouseStock.RemainingQuantity = ObjWarehouseStock.RemainingQuantity + item.Quantity;// other warestocks
                        //    db.Entry(ObjWarehouseStock).State = EntityState.Modified;
                        //    db.SaveChanges();
                        //}
                        //else if (ObjWarehouseStock.WarehouseId == 5 || ObjWarehouseStock.WarehouseId == 6)
                        //{
                        //    ObjWarehouseStock.RemainingQuantity = ObjWarehouseStock.RemainingQuantity + item.Quantity;
                        //    db.Entry(ObjWarehouseStock).State = EntityState.Modified;
                        //    db.SaveChanges();
                        //}
                        //end here

                        //var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.ProductId && i.WarehouseId == objGRV.Warehouse);
                        //var SelectedProduct = db.Products.FirstOrDefault(i => i.Id == item.ProductId);
                        //SelectedProduct.RemainingQuantity = SelectedProduct.RemainingQuantity - item.Quantity;//dispatch
                        //ObjWarehouseStock.RemainingQuantity = ObjWarehouseStock.RemainingQuantity + item.Quantity;// other warestocks
                        //db.Entry(SelectedProduct).State = EntityState.Modified;
                        //db.Entry(ObjWarehouseStock).State = EntityState.Modified;
                        //db.SaveChanges();


                        //var SelectedProduct = db.Products.FirstOrDefault(i => i.Id == item.ProductId );
                        ////SelectedProduct.RemainingQuantity = SelectedProduct.RemainingQuantity + item.Quantity;
                        //SelectedProduct.RemainingAmount = SelectedProduct.RemainingAmount + (item.Quantity * SelectedProduct.SalePrice) ;
                        //db.Entry(SelectedProduct).State = EntityState.Modified;
                        //db.SaveChanges();
                    }

                    if (objGRV.OrderNumber > 0)
                    {
                        var selectedOrder = db.Orders.FirstOrDefault(i => i.Id == objGRV.OrderNumber);
                        //selectedOrder.IsReceived = true;
                        db.Entry(selectedOrder).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    result = "Success! Transfer Completed!";
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
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());
        }

        // GET: GRV/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GRV ObjGRV = db.GRVs.Find(id);

            if (ObjGRV == null)
            {
                return HttpNotFound();
            }

            return View(ObjGRV);
        }

        // POST: GRV/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(int? id, bool approved, GRV ObjGRV, GRVMaterials[] GRVMaterials, int? WarehouseId)
        {
            //var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            //// Get the claims values
            //int warehouse = Int16.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Actor)
            //                   .Select(c => c.Value).SingleOrDefault());
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            var grvitems = db.GRVMaterials.Where(q => q.GRVId == id).ToArray();
            try
            {
                if (ModelState.IsValid)
                {

                    ObjGRV.Warehouse = AddedBy;
                    ObjGRV.ValidUntil = DateTime.Now;
                    ObjGRV.approved = approved;
                    db.Entry(ObjGRV).State = EntityState.Modified;
                    db.SaveChanges();
                    if (ObjGRV.approved == true)
                    {
                        foreach (var items in grvitems)
                        {

                            var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == items.ProductId && i.WarehouseId == warehouse);
                            var SelectedProduct = db.Products.FirstOrDefault(i => i.Id == items.ProductId);
                            if (SelectedProduct.RemainingQuantity < items.Quantity)
                            {
                                return Content(sb.ToString());
                            }
                            else
                            {
                                GRVMaterialsDto itemDto = new GRVMaterialsDto();
                            itemDto.Name = db.Products.FirstOrDefault(i => i.Id == items.ProductId).Name;
                            itemDto.Quantity = items.Quantity;
                            itemDto.Description = db.Products.FirstOrDefault(i => i.Id == items.ProductId).ProductDescription;
                            itemDto.Status = items.Status;

                            SelectedProduct.RemainingQuantity = SelectedProduct.RemainingQuantity - items.Quantity;//dispatch
                            ObjWarehouseStock.RemainingQuantity = ObjWarehouseStock.RemainingQuantity + items.Quantity;// other warestocks
                            db.Entry(ObjWarehouseStock).State = EntityState.Modified;
                            db.SaveChanges();
                           
                            }

                           
                        }

                        sb.Append("Sumitted");

                        return Content(sb.ToString());
                    }
                    else
                    {

                    }

                    ObjGRV.approved = true;
                    db.SaveChanges();
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

        public ActionResult print(int id)
        {
            GRV gnote = db.GRVs.Find(id);
            var grvitems = db.GRVMaterials.Where(q => q.GRVId == id).ToArray();

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            if (gnote == null)
            {
                return HttpNotFound();
            }

            GRVDto dto = new GRVDto();
            dto.receivedby = gnote.receivedby;
            dto.Warehouse = db.Warehouses.FirstOrDefault(i => i.Id == gnote.Warehouse).Name;
            //dto.OrderNumber = dnote.Id;
            //dto.Date = DateTime.Now;

            List<GRVMaterialsDto> itemsList = new List<GRVMaterialsDto>();

            foreach (var items in grvitems)
            {
                GRVMaterialsDto itemDto = new GRVMaterialsDto();
                itemDto.Name = db.Products.FirstOrDefault(i => i.Id == items.ProductId).Name;
                itemDto.Quantity = items.Quantity;
                itemDto.Description = db.Products.FirstOrDefault(i => i.Id == items.ProductId).ProductDescription;
                itemDto.Status = items.Status;

                itemsList.Add(itemDto);
            }

            dto.items = itemsList;

            return View(dto);

        }
        // GET: GRV/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GRV ObjGRV = db.GRVs.Find(id);
            var materials = db.GRVMaterials.Where(grv => grv.GRVId == id);
            if (ObjGRV == null)
            {
                return HttpNotFound();
            }

            GRVDto dto = new GRVDto();
            dto.Id = ObjGRV.Id;
            //dto.OrderNumber = (int)ObjGRV.OrderNumber;
            dto.purchasedate = ObjGRV.purchasedate;
            dto.receivedby = ObjGRV.receivedby;
            dto.supplier = ObjGRV.supplier;

            List<GRVMaterialsDto> materialsDtos = new List<GRVMaterialsDto>();

            foreach (var item in materials)
            {
                GRVMaterialsDto gRV = new GRVMaterialsDto();
                gRV.Description = item.Description;
                gRV.Quantity = item.Quantity;
                gRV.Id = item.Id;

                materialsDtos.Add(gRV);
            }

            dto.GRVMaterials = materialsDtos;

            return View(dto);
        }

        // POST: GRV/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                GRV ObjGRV = db.GRVs.Find(id);
                db.GRVs.Remove(ObjGRV);
                db.SaveChanges();

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

        // GET: /GRV/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            GRV ObjGRV = db.GRVs.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;

            }

            return View(ObjGRV);
        }

        // POST: /GRV/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(GRV ObjGRV)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjGRV).State = EntityState.Modified;
                    db.SaveChanges();

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
