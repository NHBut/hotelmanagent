    using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QuanLyKhachSan.Controllers.Auth
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly int[] allowedRoles;

        public CustomAuthorizeAttribute(params int[] roles)
        {
            this.allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (authTicket != null)
                    {
                        var userData = authTicket.UserData;
                        var userRole = new QuanLyKhachSan.Daos.UserDao().getInfor(int.Parse(userData)); // Assuming userData contains idUser;idRole
                        if ( allowedRoles.Contains(userRole.idRole))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/PublicAuthentication/Unauthorize");
        }
    }
}