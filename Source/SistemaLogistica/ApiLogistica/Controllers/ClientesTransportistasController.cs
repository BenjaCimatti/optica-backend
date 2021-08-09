using ApiLogistica.Filters;
using com.Sistema.Logistica;
using SistemaLogistica.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Caching;
using static com.Sistema.Logistica.DataAccess;

namespace ApiLogistica.Controllers
{
    public class ClientesTransportistasController : ApiController
    {
			[HttpGet]
			[Authorize]
			[ActionName("Get")]
			[CustomAuthorizationAttribute(ClaimType = "IdRol", ClaimValues = "1")] //Admin
			[LogAction]
			public Object Get()
			{
				DataAccess dal = new DataAccess();
				List<ClienteTransportista> res;
				try
					{
						res = dal.ObtenerClientesTransportistas();
					}
					catch (Exception ex)
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
			[ActionName("SwitchTransportistas")]
			[CustomAuthorizationAttribute(ClaimType = "IdRol", ClaimValues = "1")] //Admin
			[LogAction]
			public Object SwitchTransportistas(int IdTransportistaOrigen, int IdTransportistaDestino)
		{
				if (!ModelState.IsValid)
				{
					CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.BadRequest.ToString() + " - " + ModelState);
					throw new HttpResponseException(HttpStatusCode.BadRequest);
				}

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
					return Json(new { status = "OK" });
				}
				else
				{
					CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.NoContent.ToString());
					throw new HttpResponseException(HttpStatusCode.NoContent);
				}
			}
	}
}
