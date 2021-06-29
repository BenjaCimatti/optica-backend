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
    public class EnviosController : ApiController
    {
		[HttpGet]
		[Authorize]
		[ActionName("Get")]
		//[CustomAuthorizationAttribute(ClaimType = "IdRol", ClaimValue = "1")] //Admin arreglar para Transportistas
		[LogAction]
		public Object Get(int? Estado = null)
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
					
					if (claims.Where(p => p.Type == "IdTransportista").Count() > 0)
					{
						IdTransportista = int.Parse(claims.Where(p => p.Type == "IdTransportista").FirstOrDefault()?.Value);
					}

				  if (claims.Where(p => p.Type == "IdCliente").Count() > 0)
					{
						IdCliente = int.Parse(claims.Where(p => p.Type == "IdCliente").FirstOrDefault()?.Value);
					}

					int IdOrganizacion = int.Parse(claims.Where(p => p.Type == "IdOrganizacion").FirstOrDefault()?.Value);

					if(Estado == null)
					{ _Estado = int.Parse(ConfigurationManager.AppSettings["IdEstadoIngresado"]); }
					else
					{ _Estado = (int)Estado; }

					DataAccess dal = new DataAccess();
					try
					{
						res = dal.ObtenerEnvios(IdTransportista, IdCliente, IdRol, IdOrganizacion, _Estado);
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
				else
				{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.Unauthorized.ToString());
				throw new HttpResponseException(HttpStatusCode.Unauthorized);
				}
		}

		[HttpPost]
		[Authorize]
		[ActionName("Confirmar")]
		[CustomAuthorizationAttribute(ClaimType = "IdRol", ClaimValue = "2")] //Transportista
		[LogAction]
		public Object Confirmar(ConfirmacionEnvioRequest Ce)
		{
			if (!ModelState.IsValid)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.BadRequest.ToString() + " - " + ModelState);
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			var identity = User.Identity as ClaimsIdentity;
			bool res;
					 
			if (identity != null)
			{
				int IdTransportista = 0;

				IEnumerable<Claim> claims = identity.Claims;

				if (claims.Where(p => p.Type == "IdTransportista").Count() > 0)
				{
					IdTransportista = int.Parse(claims.Where(p => p.Type == "IdTransportista").FirstOrDefault()?.Value);

					ConfirmacionEnvio DCe = new ConfirmacionEnvio();
					DCe.IdEnvio = Ce.IdEnvio;
					DCe.GeoLongitud = Ce.GeoLongitud;
					DCe.GeoLatitud = Ce.GeoLatitud;
					DCe.IdTransportista = IdTransportista;

					DataAccess dal = new DataAccess();
					try
					{
						res = dal.ConfirmarEnvio(DCe);
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
				else
				{
					CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.BadRequest.ToString());
					throw new HttpResponseException(HttpStatusCode.BadRequest);
				}
			}
			else
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + HttpStatusCode.BadRequest.ToString());
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}
		}

		[HttpGet]
		[Authorize]
		[ActionName("EliminarDisponible")]
		[CustomAuthorizationAttribute(ClaimType = "IdRol", ClaimValue = "1")] //Admin
		[LogAction]
		public Object Eliminar(int IdEnvio)
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
				res = dal.EliminarEnvioDisponible(IdEnvio);
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
