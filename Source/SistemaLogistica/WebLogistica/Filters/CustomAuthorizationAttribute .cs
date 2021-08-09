using System.Web;
using System.Web.Mvc;
using WebLogistica.Web.Models;

namespace WebLogistica.Web.Filters
{
	public class CustomAuthorizeAttribute : AuthorizeAttribute
	{
		private readonly int[] allowedroles;
		public CustomAuthorizeAttribute(params int[] roles)
		{
			this.allowedroles = roles;
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			bool authorize = false;
			UsuarioSessionData UsuarioSessionData = (UsuarioSessionData)httpContext.Session["UsuarioSessionData"];
			foreach(int i in allowedroles)
			{
				if (UsuarioSessionData != null)
				{
					if (i == UsuarioSessionData.IdRol)
					{ authorize = true; }
				}
			}
			return authorize;
		}
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new HttpUnauthorizedResult();
		}
	}
}