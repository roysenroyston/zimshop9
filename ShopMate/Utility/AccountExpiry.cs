using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShopMate
{
    public static class AccountExpiry
    {
        public const int ValidityDays = 365;
        public const string WebMessage = "Your Account Expired, Contact Support for Assistance";
        public const string AppMessage = "Your Account Expired, Contact 0783 284 440";

        public static bool IsExpired(User user)
        {
            if (user == null)
            {
                return false;
            }

            // Support accounts must stay able to log in and Activate shops.
            if (user.RoleId == 1)
            {
                return false;
            }

            if (!user.JoinDate.HasValue)
            {
                return false;
            }

            return (DateTime.Now - user.JoinDate.Value).TotalDays >= ValidityDays;
        }

        public static bool IsWarehouseExpired(SIContext db, int warehouseId)
        {
            var users = db.Users.Where(u => u.WarehouseId == warehouseId && u.CanLogin).ToList();
            if (!users.Any())
            {
                return false;
            }

            return users.All(IsExpired);
        }

        public static void SignOutWeb(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return;
            }

            IAuthenticationManager authenticationManager = httpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();

            HttpCookie authCookie = new HttpCookie(".AspNet.ApplicationCookie");
            authCookie.Expires = DateTime.Now.AddDays(-1);
            httpContext.Response.Cookies.Add(authCookie);

            HttpCookie tokenCookie = new HttpCookie("__RequestVerificationToken");
            tokenCookie.Expires = DateTime.Now.AddDays(-1);
            httpContext.Response.Cookies.Add(tokenCookie);

            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            httpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), new string[0]);
        }
    }

    public class AccountExpiryMvcAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            if (string.Equals(controller, "Account", StringComparison.OrdinalIgnoreCase)
                && string.Equals(action, "signout", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string userIdValue = Env.GetUserInfo("userid");
            int userId;
            if (string.IsNullOrWhiteSpace(userIdValue) || !int.TryParse(userIdValue, out userId))
            {
                return;
            }

            using (SIContext db = new SIContext())
            {
                User user = db.Users.Find(userId);
                if (!AccountExpiry.IsExpired(user))
                {
                    return;
                }
            }

            AccountExpiry.SignOutWeb(filterContext.HttpContext);

            if (string.Equals(controller, "Account", StringComparison.OrdinalIgnoreCase)
                && string.Equals(action, "login", StringComparison.OrdinalIgnoreCase))
            {
                filterContext.Controller.ViewBag.Msg = AccountExpiry.WebMessage;
                return;
            }

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            {
                controller = "Account",
                action = "login",
                expired = 1
            }));
        }
    }

    public class AccountExpiryApiAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string actionName = actionContext.ActionDescriptor.ActionName;
            if (string.Equals(actionName, "login", StringComparison.OrdinalIgnoreCase)
                || string.Equals(actionName, "test", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            using (SIContext db = new SIContext())
            {
                int userId;
                if (TryGetIntArgument(actionContext, "user", out userId))
                {
                    if (AccountExpiry.IsExpired(db.Users.Find(userId)))
                    {
                        SetExpired(actionContext);
                        return;
                    }
                }

                int warehouseId;
                if (TryGetIntArgument(actionContext, "userWarehouse", out warehouseId))
                {
                    if (AccountExpiry.IsWarehouseExpired(db, warehouseId))
                    {
                        SetExpired(actionContext);
                        return;
                    }
                }

                object requestObj;
                if (actionContext.ActionArguments.TryGetValue("request", out requestObj))
                {
                    var saleOrder = requestObj as ShopMate.Controllers.AppController.SaleOrderRequest;
                    int parsedUserId;
                    if (saleOrder != null && int.TryParse(saleOrder.userId, out parsedUserId))
                    {
                        if (AccountExpiry.IsExpired(db.Users.Find(parsedUserId)))
                        {
                            SetExpired(actionContext);
                            return;
                        }
                    }
                }

                object sellObj;
                if (actionContext.ActionArguments.TryGetValue("sell", out sellObj))
                {
                    JObject sell = sellObj as JObject;
                    if (sell != null && IsExpiredSellPayload(db, sell))
                    {
                        SetExpired(actionContext);
                        return;
                    }
                }

                object salesDataObj;
                if (actionContext.ActionArguments.TryGetValue("salesData", out salesDataObj))
                {
                    JToken salesData = salesDataObj as JToken;
                    int dsalesUserId;
                    if (TryGetDsalesUserId(salesData, out dsalesUserId)
                        && AccountExpiry.IsExpired(db.Users.Find(dsalesUserId)))
                    {
                        SetExpired(actionContext);
                        return;
                    }
                }
            }
        }

        private static bool IsExpiredSellPayload(SIContext db, JObject sell)
        {
            if (sell["w"] != null)
            {
                int warehouseId;
                if (int.TryParse(Convert.ToString(sell["w"]), out warehouseId)
                    && AccountExpiry.IsWarehouseExpired(db, warehouseId))
                {
                    return true;
                }
            }

            if (sell["sell"] == null)
            {
                return false;
            }

            try
            {
                List<MySell> sales = JsonConvert.DeserializeObject<List<MySell>>(sell["sell"].ToString()) ?? new List<MySell>();
                MySell first = sales.FirstOrDefault();
                if (first == null)
                {
                    return false;
                }

                return AccountExpiry.IsExpired(db.Users.Find(first.userId));
            }
            catch
            {
                return false;
            }
        }

        private static bool TryGetDsalesUserId(JToken salesData, out int userId)
        {
            userId = 0;
            if (salesData == null)
            {
                return false;
            }

            JToken first = salesData;
            if (first is JArray)
            {
                JArray array = (JArray)first;
                if (array.Count == 0)
                {
                    return false;
                }

                first = array[0];
                if (first is JArray && ((JArray)first).Count > 0)
                {
                    first = ((JArray)first)[0];
                }
            }

            JObject sale = first as JObject;
            if (sale == null)
            {
                return false;
            }

            string raw = Convert.ToString(sale["UserId"] ?? sale["userId"]);
            return int.TryParse(raw, out userId);
        }

        private static bool TryGetIntArgument(HttpActionContext actionContext, string name, out int value)
        {
            value = 0;
            object raw;
            if (!actionContext.ActionArguments.TryGetValue(name, out raw) || raw == null)
            {
                return false;
            }

            return int.TryParse(Convert.ToString(raw), out value);
        }

        private static void SetExpired(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, AccountExpiry.AppMessage);
        }
    }
}
