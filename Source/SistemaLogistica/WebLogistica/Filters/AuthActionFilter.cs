using WebLogistica.Web.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace WebLogistica.Web.Filters
{
	public class AuthActionFilter : ActionFilterAttribute, IAuthenticationFilter
	{

		public void OnAuthentication(AuthenticationContext filterContext)
		{
		}

		public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
		{
			var user = filterContext.HttpContext.User;
			if (user == null || !user.Identity.IsAuthenticated)
			{
				filterContext.Result = new HttpUnauthorizedResult();
			}
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			string userName = null;
			if (filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				userName = filterContext.HttpContext.User.Identity.Name;
			}

			try
			{
				if (!Access(filterContext.RouteData, userName))
					filterContext.Result = new HttpUnauthorizedResult();

				base.OnActionExecuting(filterContext);
			}
			catch
			{
				filterContext.Result = new HttpUnauthorizedResult();
			}

		}

		private bool Access(RouteData routeData, string userName)
		{
			var controllerName = routeData.Values["controller"].ToString();
			var actionName = routeData.Values["action"].ToString();

			var data = new NavbarData();
			var items = data.NavbarItems();

			var context = new ActionExecutingContext();

			return true;
		}
	}
}