using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using ShopMate.Models;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class BaseController : Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            try
            {
                if (Env.GetUserInfo("name").Length <= 0)
                {
                    requestContext.HttpContext.Response.Clear();
                    requestContext.HttpContext.Response.Redirect("~/Account/login");
                    requestContext.HttpContext.Response.End();
                }
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
            }

        }


        /// <summary>
        /// Used for menu access restriction
        /// Menu create just with controller name
        /// Root menu which have no Menu Url there insert 'root'
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            try
            {
                int roleid = int.Parse(Env.GetUserInfo("roleid"));
                int userid = int.Parse(Env.GetUserInfo("userid"));
                var descriptor = context.ActionDescriptor;
                var actionName = descriptor.ActionName.ToLower();
                var controllerName = descriptor.ControllerDescriptor.ControllerName.ToLower();

                var GetOrPost = context.HttpContext.Request.HttpMethod.ToString();
                var checkAreaName = context.HttpContext.Request.RequestContext.RouteData.DataTokens["area"];
                string AreaName = "";
                if (checkAreaName != null)
                {
                    AreaName = checkAreaName.ToString().ToLower() + "/";
                }

                var cacheItemKey = "AllMenuBar";

                var globle = HttpRuntime.Cache.Get(cacheItemKey);

                if (GetOrPost == "POST")
                {
                    ///if menupermission create,edit,delete then update value "true" in IsMenuChange file
                    if (controllerName == "menupermission" && (actionName == "create" || actionName == "edit" || actionName == "delete" || actionName == "multiviewindex"))
                    {
                        globle = MenuBarCache(cacheItemKey, globle, "shortcache");
                    }
                }

                if (globle == null)//if cashe is null
                {
                    globle = MenuBarCache(cacheItemKey, globle, "60mincache");//make cache from db
                }


                var menuaccess = (MenuOfRole[])globle;

                string menuUrl = AreaName + controllerName + "/" + actionName;

                if (IsActionNameEqualToCrudPageName(actionName))
                {
                    menuUrl = AreaName + controllerName;
                }


                var checkUrl = menuaccess.FirstOrDefault(i => (i.MenuURL == AreaName + controllerName+ "/" + actionName) || i.MenuURL == menuUrl);
                ///checkUrl: check if menu url Exists in MenuPermission if not exists then will be run
                if (checkUrl != null)
                {
                    var checkControllerActionRoleUserId = menuaccess.FirstOrDefault(i => i.MenuURL == menuUrl && i.RoleId == roleid && i.UserId == userid);
                    ///check menu  && roleid && userid
                    if (checkControllerActionRoleUserId != null)
                    {
                        if (IsActionNameEqualToCrudPageName(actionName))
                        {
                            CheckAccessOfPageAction(context, actionName, checkControllerActionRoleUserId);
                        }
                        else
                        {
                            if (checkControllerActionRoleUserId.IsRead == false || checkControllerActionRoleUserId.IsDelete == false || checkControllerActionRoleUserId.IsCreate == false || checkControllerActionRoleUserId.IsUpdate == false)//if userid !=null && Check Crud
                            {
                                UnAuthoRedirect(context);
                            }
                        }
                    }
                    else
                    {

                        var checkControllerActionRole = menuaccess.FirstOrDefault(i => i.MenuURL == menuUrl && i.RoleId == roleid && i.UserId == null);
                        if (checkControllerActionRole != null)
                        {

                            if (IsActionNameEqualToCrudPageName(actionName))
                            {
                                CheckAccessOfPageAction(context, actionName, checkControllerActionRole);
                            }
                            else
                            {
                                if (checkControllerActionRole.IsRead == false || checkControllerActionRole.IsDelete == false || checkControllerActionRole.IsCreate == false || checkControllerActionRole.IsUpdate == false)//if userid !=null && Check Crud
                                {
                                    UnAuthoRedirect(context);
                                }
                            }
                        }
                        else
                        {
                            if (IsThisAjaxRequest() == false)//if userid !=null && Check Crud
                            {
                                UnAuthoRedirect(context);
                            }

                        }


                    }


                }


            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
            }
        }

        private bool IsActionNameEqualToCrudPageName(string actionName)
        {
            bool ActionIsCrud = false;
            switch (actionName)
            {
                case "create":
                    ActionIsCrud = true;
                    break;
                case "index":
                    ActionIsCrud = true;
                    break;
                case "details":
                    ActionIsCrud = true;
                    break;
                case "edit":
                    ActionIsCrud = true;
                    break;
                case "multiviewindex":
                    ActionIsCrud = true;
                    break;
                case "delete":
                    ActionIsCrud = true;
                    break;
                default:
                    ActionIsCrud = false;
                    break;
            }

            return ActionIsCrud;
        }

        private void CheckAccessOfPageAction(ActionExecutingContext context, string actionName, MenuOfRole checkRoleUrlCrud)
        {
            switch (actionName)
            {
                case "create":
                    if (checkRoleUrlCrud.IsCreate == false)//Check Crud
                    {
                        UnAuthoRedirect(context);
                    }
                    break;
                case "index":
                    if (checkRoleUrlCrud.IsRead == false)//Check Crud
                    {
                        UnAuthoRedirect(context);
                    }
                    break;
                case "details":
                    if (checkRoleUrlCrud.IsRead == false)//Check Crud
                    {
                        UnAuthoRedirect(context);
                    }
                    break;
                case "edit":
                    if (checkRoleUrlCrud.IsUpdate == false)//Check Crud
                    {
                        UnAuthoRedirect(context);
                    }
                    break;
                case "multiviewindex":
                    if (checkRoleUrlCrud.IsUpdate == false)//Check Crud
                    {
                        UnAuthoRedirect(context);
                    }
                    break;
                case "delete":
                    if (checkRoleUrlCrud.IsDelete == false)//Check Crud
                    {
                        UnAuthoRedirect(context);
                    }
                    break;

                default:
                    break;
            }
        }

        private dynamic MenuBarCache(string cacheItemKey, dynamic globle, string cachecaption)
        {
            SIContext db = new SIContext();

            var mp = db.MenuPermissions.Select(m => new MenuOfRole
            {
                MenuURL = m.Menu_MenuId.MenuURL.ToLower(),
                RoleId = m.RoleId.Value,
                UserId = m.UserId,
                IsCreate = m.IsCreate,
                IsDelete = m.IsDelete,
                IsRead = m.IsRead,
                IsUpdate = m.IsUpdate
            }).Where(i => i.MenuURL != "root").ToArray();


            globle = mp;
            if (cachecaption == "shortcache")
            {
                HttpRuntime.Cache.Insert(cacheItemKey, mp, null, DateTime.Now.AddMilliseconds(2), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
            {
                HttpRuntime.Cache.Insert(cacheItemKey, mp, null, DateTime.Now.AddMinutes(60), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            return globle;
        }

        private void UnAuthoRedirect(ActionExecutingContext context)
        {
            //context.HttpContext.Response.Redirect(Url.Action("unauthorized", "Account"));
            context.HttpContext.Response.Redirect("~/Account/unauthorized");
        }

        private class MenuOfRole
        {
            public string MenuURL { get; set; }
            public int RoleId { get; set; }
            public Nullable<int> UserId { get; set; }
            public bool IsCreate { get; set; }
            public bool IsRead { get; set; }
            public bool IsUpdate { get; set; }
            public bool IsDelete { get; set; }
        }


        private bool IsThisAjaxRequest()
        {
            bool result = false;
            var currentContext = new HttpContextWrapper(System.Web.HttpContext.Current);
            if (currentContext.Request.Headers["X-Requested-With"] != null
                && currentContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                result = true;
            }

            return result;
        }

    }
}
