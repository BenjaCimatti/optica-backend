using com.Sistema.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using SistemaLogistica.Dominios;
using static com.Sistema.Logistica.DataAccess;
using SistemaLogistica.Common;
using System.Reflection;
using System.Configuration;
using ApiLogistica.Dominios;
using ApiLogistica.Filters;

namespace SistemaLogistica.Controllers
{
    public class TransportistasController : ApiController
    {
		[HttpGet]
		[Authorize]
		[ActionName("Get")]
		[CustomAuthorizationAttribute(ClaimType = "IdRol", ClaimValues = "1")] //Admin
		[LogAction]
		public Object Get()
		{
				var identity = User.Identity as ClaimsIdentity;
				List<Transportista> res;

					DataAccess dal = new DataAccess();
					try
					{
						res = dal.ObtenerTransportistas();
					}
					catch(Exception ex)
					{
						CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
						throw new HttpResponseException(HttpStatusCode.InternalServerError);
					}

					if (res.Count > 0)
					{
						return res;
					}
					else
					{
					CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.NoContent.ToString());
					throw new HttpResponseException(HttpStatusCode.NoContent);
					}
		}

		[HttpGet]
		[Authorize]
		[ActionName("Switch")]
		[CustomAuthorizationAttribute(ClaimType = "IdRol", ClaimValues = "1")] //Admin
		[LogAction]
		public Object Switch(int IdTransportistaOrigen, int IdTransportistaDestino)
		{
			var identity = User.Identity as ClaimsIdentity;
			bool res;

			DataAccess dal = new DataAccess();
			try
			{
				res = dal.SwitchTransportista(IdTransportistaOrigen, IdTransportistaDestino);
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				throw new HttpResponseException(HttpStatusCode.InternalServerError);
			}

			if (res)
			{
				return res;
			}
			else
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.NoContent.ToString());
				throw new HttpResponseException(HttpStatusCode.NoContent);
			}
		}
	}
}
