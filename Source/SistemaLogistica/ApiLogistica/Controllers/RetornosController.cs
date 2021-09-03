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
    public class RetornosController : ApiController
    {

		[HttpPost]
		[Authorize]
		[ActionName("Ingresar")]
		[CustomAuthorizationAttribute(ClaimType = "IdRol", ClaimValues = "2")] //Transportista
		[LogAction]
		public Object Ingresar(NuevoRetornoRequest Nr)
		{
			var identity = User.Identity as ClaimsIdentity;

			if (!ModelState.IsValid)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.BadRequest.ToString() + " - " + ModelState);
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			IEnumerable<Claim> claims = identity.Claims;

			long res;

			NuevoRetorno DNr = new NuevoRetorno();
			DNr.IdCliente = Nr.IdCliente;
			DNr.IdTransportista = int.Parse(claims.Where(p => p.Type == "IdTransportista").FirstOrDefault()?.Value);
			DNr.GeoLatitud = Nr.GeoLatitud;
			DNr.GeoLongitud = Nr.GeoLongitud;

			DataAccess dal = new DataAccess();
			try
			{
				res = dal.IngresarRetorno(DNr);
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				throw new HttpResponseException(HttpStatusCode.InternalServerError);
			}

			if (res > 0)
			{
				return Json(new { idRetorno = res });
			}
			else
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.NoContent.ToString());
				throw new HttpResponseException(HttpStatusCode.NoContent);
			}
		}

		[HttpGet]
		[Authorize]
		[ActionName("Get")]
		[CustomAuthorizationAttribute(ClaimType = "IdRol", ClaimValues = "1")] //Admin
		[LogAction]
		public Object Get()
		{
			var identity = User.Identity as ClaimsIdentity;
			List<Envio> res;

			if (identity != null)
			{
				int? IdTransportista = null;
				int? IdCliente = null;
				int? IdRol = null;
				int _Estado;
				string Usuario;

				IEnumerable<Claim> claims = identity.Claims;
				Usuario = claims.Where(p => p.Type == "Usuario").FirstOrDefault()?.Value;
				IdRol = int.Parse(claims.Where(p => p.Type == "IdRol").FirstOrDefault()?.Value);

				int IdOrganizacion = int.Parse(claims.Where(p => p.Type == "IdOrganizacion").FirstOrDefault()?.Value);
				_Estado = int.Parse(ConfigurationManager.AppSettings["IdEstadoEnviado"]);
				int idRecorrido = int.Parse(System.Configuration.ConfigurationManager.AppSettings["IdRecorridoRetorno"]);

				DataAccess dal = new DataAccess();
				try
				{
					res = dal.ObtenerEnvios(IdTransportista, IdCliente, IdRol, IdOrganizacion, _Estado, idRecorrido);
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
			else
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.Unauthorized.ToString());
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
			}
		}

		[HttpPost]
		[Authorize]
		[ActionName("Recibir")]
		[CustomAuthorizationAttribute(ClaimType = "IdRol", ClaimValues = "1")] //Admin
		[LogAction]
		public Object Recibir(List<long> Le)
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
				res = dal.RecibirRetornos(Le);
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
