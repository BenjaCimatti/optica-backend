using Newtonsoft.Json;
using RestSharp;
using SistemaLogistica.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using WebLogistica.Web.Models;

namespace WebLogistica.Data
{
	public class Estadisticas
	{
		public int Estadistica1 { get; set; }
		public int Estadistica2 { get; set; }
		public int Estadistica3 { get; set; }
	}

	public class ApiAccess
	{
		public class Token
		{
			public string Usuario { get; set; }
			public int? IdTransportista { get; set; }
			public int? IdCliente { get; set; }
			public int? IdRol { get; set; }
			public DateTime? UltimoLogin { get; set; }
			public int? IdOrganizacion { get; set; }
			public string Data { get; set; }
		}

		public class Envio
		{
			public int IdEnvio { get; set; }
			public int IdCliente { get; set; }
			public string DescCLiente { get; set; }
			public int IdTransportista { get; set; }
			public string DescTransportista { get; set; }
			public DateTime? FechaCarga { get; set; }
			public DateTime? FechaEnvio { get; set; }
			public int IdEstado { get; set; }
			public string DescEstado { get; set; }
			public double? GeoLatitud { get; set; }
			public double? GeoLongitud { get; set; }
			public string Observaciones { get; set; }
			public int Contactos { get; set; }
		}

		public class NuevoEnvio
		{
			public int IdCliente { get; set; }
			public int IdTransportista { get; set; }
			public string Observaciones { get; set; }
		}

		public class Contacto
		{
			public int IdEnvio { get; set; }
			public int IdTransportista { get; set; }
			public string DescTransportista { get; set; }
			public DateTime FechaContacto { get; set; }
			public double GeoLatitud { get; set; }
			public double GeoLongitud { get; set; }
			public string Observaciones { get; set; }
		}

		public class InfoEnvio
		{
			public Envio _Envio { get; set; }
			public List<Contacto> _Contactos { get; set; }
		}

		public class EnvioCompletado
		{
			public Envio _Envio { get; set; }
			public List<Contacto> _Contactos { get; set; }
		}

		public class ClienteTransportista
		{
			public int IdCliente { get; set; }
			public string DescCLiente { get; set; }
			public int IdTransportista { get; set; }
			public string DescTransportista { get; set; }
		}

		public class Transportista
		{
			public int IdTransportista { get; set; }
			public string DescTransportista { get; set; }
		}

		public IList<Envio> GetEnvios(string Token, int Estado)
		{
			Token = SyncroToken(Token);

			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Envios/Get", Method.GET);
			request.AddHeader("Authorization", "Bearer " + Token);
			request.AddParameter("Estado", Estado);

			try
			{
				var response = client.Get(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						List<Envio> Envios = JsonConvert.DeserializeObject<List<Envio>>(response.Content.ToString());
						return Envios;
					}
					else
					{
						return new List<Envio>();
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return null;
			}
		}

		public IList<Envio> GetRetornos(string Token)
		{
			Token = SyncroToken(Token);

			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Retornos/Get", Method.GET);
			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Get(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						List<Envio> Envios = JsonConvert.DeserializeObject<List<Envio>>(response.Content.ToString());
						return Envios;
					}
					else
					{
						return new List<Envio>();
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return null;
			}
		}

		public InfoEnvio GetDetalleEnvio(string Token, int IdEnvio)
		{
			Token = SyncroToken(Token);

			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			InfoEnvio _InfoEnvio = new InfoEnvio();

			var requestEnvio = new RestRequest("/api/Envios/Get", Method.GET);
			requestEnvio.AddHeader("Authorization", "Bearer " + Token);
			requestEnvio.AddParameter("IdEnvio", IdEnvio);

			var requestContactos = new RestRequest("/api/Envios/Contactos", Method.GET);
			requestContactos.AddHeader("Authorization", "Bearer " + Token);
			requestContactos.AddParameter("IdEnvio", IdEnvio);

			try
			{
				var responseEnvio = client.Get(requestEnvio);

				if (responseEnvio.StatusCode == HttpStatusCode.OK)
				{
					_InfoEnvio._Envio = JsonConvert.DeserializeObject<Envio>(responseEnvio.Content.ToString());
				}

				if (_InfoEnvio._Envio.Contactos > 0)
				{
					var responseContactos = client.Get(requestContactos);

					if (responseContactos.StatusCode == HttpStatusCode.OK)
					{
						_InfoEnvio._Contactos = JsonConvert.DeserializeObject<List<Contacto>>(responseContactos.Content.ToString());
					}
				}

				return _InfoEnvio;

			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return null;
			}
		}

		public Estadisticas GetEstadisticas(string Token)
		{
			Token = SyncroToken(Token);

			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Envios/Estadisticas", Method.GET);
			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Get(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						Estadisticas _Estadisticas = JsonConvert.DeserializeObject<Estadisticas>(response.Content.ToString());
						return _Estadisticas;
					}
					else
					{
						return new Estadisticas { Estadistica1 = 0, Estadistica2 = 0, Estadistica3 = 0 };
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return null;
			}
		}

		public IList<ClienteTransportista> GetClientesTransportistas(string Token)
		{
			Token = SyncroToken(Token);

			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/ClientesTransportistas/Get", Method.GET);
			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Get(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						List<ClienteTransportista> ClientesTransportistas = JsonConvert.DeserializeObject<List<ClienteTransportista>>(response.Content.ToString());
						return ClientesTransportistas;
					}
					else
					{
						return new List<ClienteTransportista>();
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return null;
			}
		}

		public IList<Transportista> GetTransportistas(string Token)
		{
			Token = SyncroToken(Token);

			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Transportistas/Get", Method.GET);
			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Get(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						List<Transportista> Transportistas = JsonConvert.DeserializeObject<List<Transportista>>(response.Content.ToString());
						return Transportistas;
					}
					else
					{
						return new List<Transportista>();
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return null;
			}
		}

		public string Renew(string Token)
		{
			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Token/Renew", Method.GET);
			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Post(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						Token _Token = JsonConvert.DeserializeObject<Token>(response.Content.ToString());
						return _Token.Data;
					}
					else
					{
						return null;
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return null;
			}
		}

		public bool EnviarEnviosDisponibles(string Token, List<long> Le)
		{
			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Envios/Enviar", Method.POST);
			// Json to post.
			string jsonToSend = JsonConvert.SerializeObject(Le);

			request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
			request.RequestFormat = DataFormat.Json;

			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Post(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return false;
			}
		}

		public bool RecibirRetornos(string Token, List<long> Le)
		{
			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Retornos/Recibir", Method.POST);
			// Json to post.
			string jsonToSend = JsonConvert.SerializeObject(Le);

			request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
			request.RequestFormat = DataFormat.Json;

			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Post(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return false;
			}
		}

		public bool EliminarEnvioDisponible(string Token, int IdEnvio)
		{
			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Envios/EliminarDisponible", Method.GET);
			request.AddParameter("IdEnvio", IdEnvio.ToString());
			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Get(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return false;
			}
		}

		public bool SwitchTransportistas(string Token, int IdTransportistaOrigen, int IdTransportistaDestino)
		{
			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/ClientesTransportistas/SwitchTransportistas", Method.GET);
			request.AddParameter("IdTransportistaOrigen", IdTransportistaOrigen.ToString());
			request.AddParameter("IdTransportistaDestino", IdTransportistaDestino.ToString());
			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Get(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return false;
			}
		}

		public bool EditarEnvio(string Token, int IdEnvio, string Observaciones)
		{
			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Envios/Editar", Method.GET);
			request.AddParameter("IdEnvio", IdEnvio.ToString());
			request.AddParameter("Observaciones", Observaciones);
			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Get(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return false;
			}
		}

		public Token LogIn(string UserName, string Password)
		{
			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Token/Get", Method.POST);

			// Json to post.
			string jsonToSend = JsonConvert.SerializeObject(new { NombreUsuario = UserName, Clave  = Password, IdOrganizacion = ConfigurationManager.AppSettings["IdOrganizacion"] });

			request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
			request.RequestFormat = DataFormat.Json;

			try
			{
				var response = client.Post(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						Token _Token = JsonConvert.DeserializeObject<Token>(response.Content.ToString());
						return _Token;
					}
					else
					{
						return null;
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return null;
			}
		}

		private string SyncroToken(string Token)
		{
			if (!VerifyToken(Token))
			{
				string NewToken = Renew(Token);
				Token = NewToken;
				UsuarioSessionData Usd = (UsuarioSessionData)System.Web.HttpContext.Current.Session["UsuarioSessionData"];
				Usd.Token = NewToken;
				System.Web.HttpContext.Current.Session["UsuarioSessionData"] = Usd;
				return NewToken;
			}
			else
			{
				return Token;
			}
		}

		private bool VerifyToken(string Token)
		{
			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Token/Verify", Method.GET);
			request.AddHeader("Authorization", "Bearer " + Token);

			try
			{
				var response = client.Get(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return false;
			}
		}

		public bool IngresarEnvio(string Token, NuevoEnvio Envio)
		{
			var client = new RestClient(ConfigurationManager.AppSettings["ApiBase"]);

			var request = new RestRequest("/api/Envios/Ingresar", Method.POST);
			request.AddHeader("Authorization", "Bearer " + Token);

			string jsonToSend = JsonConvert.SerializeObject(new { IdCliente = Envio.IdCliente, IdTransportista = Envio.IdTransportista, Observaciones = Envio.Observaciones });

			request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
			request.RequestFormat = DataFormat.Json;

			try
			{
				var response = client.Post(request);
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, MethodBase.GetCurrentMethod().Name + " - " + ex.StackTrace);
				return false;
			}
		}
	}
}