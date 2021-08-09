using System;
using System.Data.SQLite;
using com.Sistema.Logistica.Seguridad;
using System.Collections.Generic;
using SistemaLogistica.Common;
using System.Reflection;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Text;
using System.Configuration;

namespace com.Sistema.Logistica
{
	public class DataAccess
	{

		public DataAccess()
		{
			CustomLogging.Initialize();
		}

		private string dbConnectionStringRO = System.Configuration.ConfigurationManager.ConnectionStrings["SistemaLogisticaRO"].ConnectionString;

		private string dbConnectionStringNoRO = System.Configuration.ConfigurationManager.ConnectionStrings["SistemaLogisticaNoRO"].ConnectionString;

		private string SQLiteDateTimeFormat = System.Configuration.ConfigurationManager.AppSettings["SQLiteDateTimeFormat"];

		public class Estadisticas
		{
			public int Estadistica1 { get; set; }
			public int Estadistica2 { get; set; }
			public int Estadistica3 { get; set; }
		}

		public class Transportista
		{
			public int IdTransportista { get; set; }
			public string DescTransportista { get; set; }
		}

		public class Cliente
		{
			public int IdCliente { get; set; }
			public string DescCliente { get; set; }
			public int IdTransportista { get; set; }
		}

		public class Usuario
		{
			public string _Usuario { get; set; }
			public int? IdTransportista { get; set; }
			public int? IdCliente { get; set; }
			public string DescUsuario { get; set; }
			public int? IdRol { get; set; }
			public DateTime? UltimoLogin { get; set; }
			public int? IdOrganizacion { get; set; }
		}

		public class Envio
		{
			public int IdEnvio { get; set; }
			public int IdCliente { get; set; }
			public string DescCliente { get; set; }
			public int IdTransportista { get; set; }
			public string DescTransportista { get; set; }
			public DateTime? FechaCarga { get; set; }
			public DateTime? FechaEnvio { get; set; }
			public DateTime? FechaCierre { get; set; }
			public int IdEstado { get; set; }
			public string DescEstado { get; set; }
			public double? GeoLatitud { get; set; }
			public double? GeoLongitud { get; set; }
			public string Observaciones { get; set; }
			public int Contactos { get; set; }
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

		public class ClienteTransportista
		{
			public int IdCliente { get; set; }
			public string DescCliente { get; set; }
			public int IdTransportista { get; set; }
			public string DescTransportista { get; set; }
		}

		public class ConfirmacionEnvio
		{
			public int IdEnvio { get; set; }
			public double? GeoLatitud { get; set; }
			public double? GeoLongitud { get; set; }
			public int IdTransportista { get; set; }
		}

		public class InformacionEnvio
		{
			public int IdEnvio { get; set; }
			public double? GeoLatitud { get; set; }
			public double? GeoLongitud { get; set; }
			public int IdTransportista { get; set; }
			public string Observaciones { get; set; }
		}

		public class NuevoEnvio
		{
			public int IdCliente { get; set; }

			public int IdTransportista { get; set; }

			public string Observaciones { get; set; }
		}

		public bool EliminarEnvioDisponible(int IdEnvio)
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();
			try
			{
				query = "DELETE FROM envios WHERE idEnvio = @IDENVIO AND idEstado = @ESTADOCARGADO;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDENVIO", IdEnvio));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADOCARGADO", System.Configuration.ConfigurationManager.AppSettings["IdEstadoIngresado"]));
				int Eliminados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + Eliminados + " Registros");
				if (Eliminados > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				throw ex;
			}
		}

		public bool EditarEnvio(int IdEnvio, string Observaciones)
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();
			try
			{
				query = "UPDATE envios SET observaciones = @OBSERVACIONES WHERE idEnvio = @IDENVIO;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDENVIO", IdEnvio));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@OBSERVACIONES", Observaciones));
				int Actualizados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + Actualizados + " Registros");
				if (Actualizados > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				throw ex;
			}
		}

		public bool SwitchTransportista(int IdTransportistaOrigen, int IdTransportistaDestino)
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();
			try
			{
				query = "UPDATE rel_clientes_transportistas SET idTransportista = @IDTRANSDESTINO WHERE idTransportista = @IDTRANSORIGEN;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDTRANSORIGEN", IdTransportistaOrigen));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDTRANSDESTINO", IdTransportistaDestino));
				int Actualizados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + Actualizados + " Registros");
				if (Actualizados > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				throw ex;
			}
		}

		public bool EnviarEnvios(List<long> Le)
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();

			string strEnvios = "";
			foreach (long l in Le)
			{
				strEnvios = strEnvios + l.ToString() + ",";
			}
			strEnvios = strEnvios.Remove(strEnvios.Length - 1);

			try
			{
				query = "UPDATE envios SET idEstado = @ESTADOENVIADO, fechaEnvio = JULIANDAY('now','localtime') WHERE idEnvio IN ({0});";
				query = query.Replace("{0}", strEnvios);
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADOENVIADO", System.Configuration.ConfigurationManager.AppSettings["IdEstadoEnviado"]));

				int Actualizados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + Actualizados + " Registros");
				if (Actualizados > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				throw ex;
			}
		}

		public bool ConfirmarEnvio(ConfirmacionEnvio Ce)
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();
			try
			{
				query = "UPDATE envios SET idEstado = @ESTADOCONFIRMADO, fechaCierre = JULIANDAY('now','localtime'), geoLatitud = @GEOLATITUD, geoLongitud = @GEOLONGITUD WHERE idEnvio = @IDENVIO AND idEstado = @ESTADOENVIADO AND idTransportista = @IDTRANSPORTISTA;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADOCONFIRMADO", System.Configuration.ConfigurationManager.AppSettings["IdEstadoCompletado"]));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@GEOLATITUD", Ce.GeoLatitud));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@GEOLONGITUD", Ce.GeoLongitud));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDENVIO", Ce.IdEnvio));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADOENVIADO", System.Configuration.ConfigurationManager.AppSettings["IdEstadoEnviado"]));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDTRANSPORTISTA", Ce.IdTransportista));
				int Actualizados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + Actualizados + " Registros");
				if (Actualizados > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				throw ex;
			}
		}

		public bool InformarEnvio(InformacionEnvio Ce)
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();
			try
			{
				query = "INSERT INTO contactos_envios (idEnvio,idTransportista,fechaContacto,geoLatitud,geoLongitud,observaciones) VALUES (@IDENVIO,@IDTRANSPORTISTA,JULIANDAY('now','localtime'),@GEOLATITUD,@GEOLONGITUD,@OBSERVACIONES);";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@GEOLATITUD", Ce.GeoLatitud));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@GEOLONGITUD", Ce.GeoLongitud));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDENVIO", Ce.IdEnvio));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDTRANSPORTISTA", Ce.IdTransportista));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@OBSERVACIONES", Ce.Observaciones));
				int Actualizados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + Actualizados + " Registros");
				if (Actualizados > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				throw ex;
			}
		}

		public bool IngresarEnvio(NuevoEnvio Ne)
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();
			try
			{
				query = "INSERT INTO envios (idEnvio, idCliente, idTransportista, idEstado, fechaCarga, observaciones, idOrganization) VALUES(NULL, @IDCLIENTE, @IDTRANSPORTISTA, @ESTADOINGRESADO, JULIANDAY('now','localtime'), @OBSERVACIONES, @ORGANIZACION);";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADOINGRESADO", System.Configuration.ConfigurationManager.AppSettings["IdEstadoIngresado"]));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDCLIENTE", Ne.IdCliente));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDTRANSPORTISTA", Ne.IdTransportista));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@OBSERVACIONES", Ne.Observaciones));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", System.Configuration.ConfigurationManager.AppSettings["IdOrganizacion"]));
				int Actualizados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + Actualizados + " Registros");
				if (Actualizados > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				throw ex;
			}
		}

		public bool VerificarConeccion()
		{
			try
			{
				SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringRO);
				sqlite_con.Open();
				string query = "SELECT 'Hello world';";
				SQLiteCommand sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				SQLiteDataReader dr = sqlite_cmd.ExecuteReader();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + dr.HasRows + " Registros");
				sqlite_con.Close();
				return true;
			}
			catch (Exception ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				return false;
			}
		}

		private string Parameters(SQLiteParameterCollection Parameters)
		{
			string StrParameters = "";
			foreach (SQLiteParameter Parameter in Parameters)
			{
				StrParameters = StrParameters + Parameter.ParameterName + ":" + Parameter.Value.ToString() + "|";
			}

			return StrParameters;
		}

		public Usuario VerificarUsuarioEncrptado(string UsuarioLogin, string ClaveEncriptadaLogin, int IdOrganizacion)
		{
			SQLiteCommand sqlite_cmd = null;
			try
			{
				string Usuario = "";
				string ClaveEncriptada = "";
				string _DescUsuario = "";
				int? IdTransportista = null;
				int? IdCliente = null;
				DateTime? UltimoLogin = null;
				int? _IdOrganizacion = null;
				int? IdRol = null;

				SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringRO);
				sqlite_con.Open();
				string query = "SELECT usr.usuario, usr.claveEncriptada, usr.idTransportista, usr.idCliente, usr.idRol, date(usr.ultimologin) as dateUltimoLogin, time(usr.ultimologin) as timeUltimoLogin, usr.idOrganization , tra.descTransportista, cli.descCliente FROM usuarios usr LEFT JOIN transportistas tra ON (tra.idTransportista = usr.idTransportista AND tra.idOrganization = usr.idOrganization) LEFT JOIN clientes cli ON (cli.idCliente = usr.idCliente AND cli.idOrganization = usr.idOrganization) WHERE usr.usuario = @USUARIO AND usr.claveEncriptada = @CLAVEENCRIPTADA AND  usr.idOrganization = @ORGANIZACION;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@USUARIO", UsuarioLogin));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", IdOrganizacion));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@CLAVEENCRIPTADA", ClaveEncriptadaLogin));
				SQLiteDataReader dr = sqlite_cmd.ExecuteReader();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + dr.HasRows + " Registros");
				while (dr.Read())
				{
					Usuario = dr.GetString(0);
					ClaveEncriptada = dr.GetString(1);
					_IdOrganizacion = dr.GetInt32(7);
					IdRol = dr.GetInt32(4);

					switch (IdRol)
					{
						case 1: //Admin
							IdTransportista = null;
							IdCliente= null;
							break;
						case 2: //Transportista
							IdTransportista = dr.GetInt32(2);
							_DescUsuario = dr.GetString(8);
							IdCliente = null;
							break;
						default: //Cliente
							IdTransportista = null;
							_DescUsuario = dr.GetString(9);
							IdCliente = dr.GetInt32(3);
							break;
					}

					if (dr[5] == System.DBNull.Value)
					{ UltimoLogin = null; }
					else
					{
						UltimoLogin = DateTime.ParseExact(dr.GetString(5) + " " + dr.GetString(6), SQLiteDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
					}
				}
				sqlite_con.Close();

				if ((dr != null) && (ClaveEncriptada.Equals(ClaveEncriptadaLogin)))
				{ return new DataAccess.Usuario { _Usuario = Usuario, IdTransportista = IdTransportista, IdCliente = IdCliente, IdRol= IdRol, UltimoLogin = UltimoLogin, IdOrganizacion = _IdOrganizacion, DescUsuario = _DescUsuario }; }
				else
				{ return null; }
			}
			catch (Exception ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex, UsuarioLogin, ClaveEncriptadaLogin, IdOrganizacion);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				return null;
			}
		}

		public string VerificarVersionComponente(string Componente)
		{
			SQLiteCommand sqlite_cmd = null;
			try
			{
				string Version = "";

				SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringRO);
				sqlite_con.Open();
				string query = "SELECT version FROM versiones WHERE componente = @COMPONENTE;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@COMPONENTE", Componente));
				SQLiteDataReader dr = sqlite_cmd.ExecuteReader();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + dr.HasRows + " Registros");
				while (dr.Read())
				{
					if (dr[0] != System.DBNull.Value)
					{ Version = dr.GetString(0); }
					else
					{
						Version = "";
					}
				}
				sqlite_con.Close();

				return Version;
			}
			catch (Exception ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex, Componente);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				return "";
			}
		}

		public List<Envio> ObtenerEnvios(int? idTransportista, int? IdCliente, int? IdRol,int idOrganization, int Estado)
		{
			SQLiteCommand sqlite_cmd = null;
			List<Envio> Result = new List<Envio>();
			try
			{
				SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringRO);
				sqlite_con.Open();

				string query = "";

				switch (IdRol)
				{
					case 1: //Admin
						query = "SELECT env.idEnvio,env.idCliente,env.idEstado,date(env.fechaCarga) as dateFechaCarga,time(env.fechaCarga) as timeFechaCarga,date(env.fechaEnvio) as dateFechaEnvio, time(env.fechaEnvio) as timeFechaEnvio,date(env.fechaCierre) as dateFechaCierre, time(env.fechaCierre) as timeFechaCierre,env.geoLatitud, env.geoLongitud,env.idTransportista,cli.descCliente,tra.descTransportista,cli.descCliente,est.descEstado,env.observaciones, (SELECT COUNT(cctos.idEnvio) FROM contactos_envios cctos WHERE cctos.idEnvio = env.idEnvio) as contactos FROM envios env, transportistas tra, clientes cli, estados est  WHERE tra.idTransportista = env.idTransportista AND cli.idCliente = env.idCliente AND est.idEstado = env.idEstado AND env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO ORDER BY env.fechaCarga DESC;";
						sqlite_cmd = new SQLiteCommand(query, sqlite_con);
						sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
						sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
						break;
					case 2: //Transportista
						query = "SELECT env.idEnvio,env.idCliente,env.idEstado,date(env.fechaCarga) as dateFechaCarga,time(env.fechaCarga) as timeFechaCarga,date(env.fechaEnvio) as dateFechaEnvio, time(env.fechaEnvio) as timeFechaEnvio,date(env.fechaCierre) as dateFechaCierre, time(env.fechaCierre) as timeFechaCierre,env.geoLatitud, env.geoLongitud,env.idTransportista,cli.descCliente,tra.descTransportista,cli.descCliente,est.descEstado,env.observaciones, (SELECT COUNT(cctos.idEnvio) FROM contactos_envios cctos WHERE cctos.idEnvio = env.idEnvio) as contactos FROM envios env, transportistas tra, clientes cli, estados est WHERE tra.idTransportista = env.idTransportista AND cli.idCliente = env.idCliente AND est.idEstado = env.idEstado AND env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO AND env.idTransportista = @TRANSPORTISTA ORDER BY env.fechaCarga DESC;";
						sqlite_cmd = new SQLiteCommand(query, sqlite_con);
						sqlite_cmd.Parameters.Add(new SQLiteParameter("@TRANSPORTISTA", idTransportista));
						sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
						sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
						break;
					default: //Cliente
						query = "SELECT env.idEnvio,env.idCliente,env.idEstado,date(env.fechaCarga) as dateFechaCarga,time(env.fechaCarga) as timeFechaCarga,date(env.fechaEnvio) as dateFechaEnvio, time(env.fechaEnvio) as timeFechaEnvio,date(env.fechaCierre) as dateFechaCierre, time(env.fechaCierre) as timeFechaCierre,env.geoLatitud, env.geoLongitud,env.idTransportista,cli.descCliente,tra.descTransportista,cli.descCliente,est.descEstado,env.observaciones, (SELECT COUNT(cctos.idEnvio) FROM contactos_envios cctos WHERE cctos.idEnvio = env.idEnvio) as contactos FROM envios env, transportistas tra, clientes cli, estados est WHERE tra.idTransportista = env.idTransportista AND cli.idCliente = env.idCliente AND est.idEstado = env.idEstado AND env.idOrganization = @ORGANIZACION AND env.idEstado >= @ESTADO AND env.idCliente = @CLIENTE ORDER BY env.fechaCarga DESC;";
						sqlite_cmd = new SQLiteCommand(query, sqlite_con);
						sqlite_cmd.Parameters.Add(new SQLiteParameter("@CLIENTE", IdCliente));
						sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
						sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
						break;
				}

				SQLiteDataReader dr = sqlite_cmd.ExecuteReader();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + dr.HasRows + " Registros");

				while (dr.Read())
				{
					DateTime? tempFechaEnvio;
					DateTime? tempFechaCierre;
					double? tempGeoLatitud;
					double? tempGeoLongitud;

					if (dr["dateFechaEnvio"] == System.DBNull.Value)
					{ tempFechaEnvio = null; }
					else
					{
						tempFechaEnvio = DateTime.ParseExact(dr["dateFechaEnvio"].ToString() + " " + dr["timeFechaEnvio"].ToString(), SQLiteDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
					}

					if (dr["dateFechaCierre"] == System.DBNull.Value)
					{ tempFechaCierre = null; }
					else
					{
						tempFechaCierre = DateTime.ParseExact(dr["dateFechaCierre"].ToString() + " " + dr["timeFechaCierre"].ToString(), SQLiteDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
					}

					if (dr["geoLatitud"] == System.DBNull.Value)
					{ tempGeoLatitud = null; }
					else
					{
						tempGeoLatitud = Convert.ToDouble(dr["geoLatitud"]);
					}

					if (dr["geoLongitud"] == System.DBNull.Value)
					{ tempGeoLongitud = null; }
					else
					{
						tempGeoLongitud = Convert.ToDouble(dr["geoLongitud"]);
					}

					Result.Add(new Envio
					{
						IdEnvio = Convert.ToInt32(dr["idEnvio"]),
						IdCliente = Convert.ToInt32(dr["idCliente"]),
						IdTransportista = Convert.ToInt32(dr["idTransportista"]),
						FechaCarga = DateTime.ParseExact(dr["dateFechaCarga"].ToString() + " " + dr["timeFechaCarga"].ToString(), SQLiteDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture),
						FechaEnvio = tempFechaEnvio,
						FechaCierre = tempFechaCierre,
						IdEstado = Convert.ToInt32(dr["idEstado"].ToString()),
						GeoLatitud = tempGeoLatitud,
						GeoLongitud = tempGeoLongitud,
						DescCliente = dr["descCliente"].ToString(),
						DescTransportista = dr["descTransportista"].ToString(),
						DescEstado = dr["descEstado"].ToString(),
						Observaciones = dr["observaciones"].ToString(),
						Contactos = Convert.ToInt32(dr["contactos"].ToString())
					});
				}
				return Result;
			}
			catch (Exception ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex, idTransportista, idOrganization, Estado);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				return Result;
			}
		}

		private int ObtenerEstadistica(int? idTransportista, int? IdCliente, int? IdRol, int idOrganization, int Estado, int Dias, int Estadistica)
		{
			SQLiteCommand sqlite_cmd = null;
			List<Envio> Result = new List<Envio>();
			int Cantidad = 0;

			try
			{
				SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringRO);
				sqlite_con.Open();

				string query = "";

				switch (Estadistica)
				{
					case 1: //Nuevos Envios Cargados en X Dias!
						switch (IdRol)
						{
							case 1: //Admin
								query = "SELECT COUNT(*) as Cantidad FROM envios env WHERE env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO AND env.fechaCarga > JULIANDAY('now','-{0} day','localtime');";
								query = string.Format(query, Dias.ToString());
								sqlite_cmd = new SQLiteCommand(query, sqlite_con);
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
								break;
							case 2: //Transportista
								query = "SELECT COUNT(*) as Cantidad FROM envios env WHERE env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO AND env.fechaCarga > JULIANDAY('now','-{0} day','localtime') AND env.idTransportista = @TRANSPORTISTA;";
								query = string.Format(query, Dias.ToString());
								sqlite_cmd = new SQLiteCommand(query, sqlite_con);
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@TRANSPORTISTA", idTransportista));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
								break;
							default: //Cliente
								query = "SELECT COUNT(*) as Cantidad FROM envios env WHERE env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO AND env.fechaCarga > JULIANDAY('now','-{0} day','localtime') AND env.idCliente = @CLIENTE;";
								query = string.Format(query, Dias.ToString());
								sqlite_cmd = new SQLiteCommand(query, sqlite_con);
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@CLIENTE", IdCliente));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
								break;
						}
						break;
						case 2: //Promedio Entrega Ult. X dias
						switch (IdRol)
						{
							case 1: //Admin
								query = "SELECT CAST(AVG(env.fechaCierre - env.fechaEnvio) AS INTEGER) AS Cantidad FROM envios env WHERE env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO AND env.fechaCarga > JULIANDAY('now','-{0} day','localtime');";
								query = string.Format(query, Dias.ToString());
								sqlite_cmd = new SQLiteCommand(query, sqlite_con);
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
								break;
							case 2: //Transportista
								query = "SELECT CAST(AVG(env.fechaCierre - env.fechaEnvio) AS INTEGER) AS Cantidad FROM envios env WHERE env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO AND env.fechaCarga > JULIANDAY('now','-{0} day','localtime') AND env.idTransportista = @TRANSPORTISTA;";
								query = string.Format(query, Dias.ToString());
								sqlite_cmd = new SQLiteCommand(query, sqlite_con);
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@TRANSPORTISTA", idTransportista));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
								break;
							default: //Cliente
								query = "SELECT CAST(AVG(env.fechaCierre - env.fechaEnvio) AS INTEGER) AS Cantidad FROM envios env WHERE env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO AND env.fechaCarga > JULIANDAY('now','-{0} day','localtime') AND env.idCliente = @CLIENTE;";
								query = string.Format(query, Dias.ToString());
								sqlite_cmd = new SQLiteCommand(query, sqlite_con);
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@CLIENTE", IdCliente));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
								break;
						}
						break;
						case 3: //Nuevos Envios Cargados en X Dias!
						switch (IdRol)
						{
							case 1: //Admin
								query = "SELECT COUNT(*) as Cantidad FROM envios env WHERE env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO;";
								query = string.Format(query, Dias.ToString());
								sqlite_cmd = new SQLiteCommand(query, sqlite_con);
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
								break;
							case 2: //Transportista
								query = "SELECT COUNT(*) as Cantidad FROM envios env WHERE env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO AND env.idTransportista = @TRANSPORTISTA;";
								query = string.Format(query, Dias.ToString());
								sqlite_cmd = new SQLiteCommand(query, sqlite_con);
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@TRANSPORTISTA", idTransportista));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
								break;
							default: //Cliente
								query = "SELECT COUNT(*) as Cantidad FROM envios env WHERE env.idOrganization = @ORGANIZACION AND env.idEstado = @ESTADO AND env.idCliente = @CLIENTE;";
								query = string.Format(query, Dias.ToString());
								sqlite_cmd = new SQLiteCommand(query, sqlite_con);
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@CLIENTE", IdCliente));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", idOrganization));
								sqlite_cmd.Parameters.Add(new SQLiteParameter("@ESTADO", Estado));
								break;
						}
						break;
				}

				SQLiteDataReader dr = sqlite_cmd.ExecuteReader();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + dr.HasRows + " Registros");

				while (dr.Read())
				{
					if (dr["Cantidad"] != System.DBNull.Value)
					{
						Cantidad = Convert.ToInt32(dr["Cantidad"]);
					}
					else Cantidad = 0;
				}
				return Cantidad;
			}
			catch (Exception ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex, idTransportista, idOrganization, Estado);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				return Cantidad;
			}
		}

		public Estadisticas ObtenerEstadisticas(int? idTransportista, int? IdCliente, int? IdRol, int idOrganization, int Dias)
		{
			Estadisticas _Estadisticas = new Estadisticas() { Estadistica1 = 0, Estadistica2 = 0, Estadistica3 = 0 };

			_Estadisticas.Estadistica1 = ObtenerEstadistica(idTransportista, IdCliente, IdRol, idOrganization, int.Parse(System.Configuration.ConfigurationManager.AppSettings["IdEstadoEnviado"]), Dias, 1);

			_Estadisticas.Estadistica2 = ObtenerEstadistica(idTransportista, IdCliente, IdRol, idOrganization, int.Parse(System.Configuration.ConfigurationManager.AppSettings["IdEstadoCompletado"]), Dias, 2);

			_Estadisticas.Estadistica3 = ObtenerEstadistica(idTransportista, IdCliente, IdRol, idOrganization, int.Parse(System.Configuration.ConfigurationManager.AppSettings["IdEstadoIngresado"]), Dias, 3);

			return _Estadisticas;
		}

		public Envio ObtenerDetalleEnvio(int IdEnvio, int IdOrganization)
		{
			SQLiteCommand sqlite_cmd = null;
			List<Envio> Result = new List<Envio>();
			Envio _Envio = new Envio();

			try
			{
				SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringRO);
				sqlite_con.Open();

				string query = "";

				query = "SELECT env.idEnvio,env.idCliente,env.idEstado,date(env.fechaCarga) as dateFechaCarga,time(env.fechaCarga) as timeFechaCarga,date(env.fechaEnvio) as dateFechaEnvio, time(env.fechaEnvio) as timeFechaEnvio,env.geoLatitud, env.geoLongitud,env.idTransportista,cli.descCliente,tra.descTransportista,cli.descCliente,est.descEstado,env.observaciones, (SELECT COUNT(cctos.idEnvio) FROM contactos_envios cctos WHERE cctos.idEnvio = env.IdEnvio) as contactos FROM envios env, transportistas tra, clientes cli, estados est WHERE tra.idTransportista = env.idTransportista AND cli.idCliente = env.idCliente AND est.idEstado = env.idEstado AND env.idOrganization = @ORGANIZACION AND env.idEnvio = @ENVIO;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", IdOrganization));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ENVIO", IdEnvio));

				SQLiteDataReader dr = sqlite_cmd.ExecuteReader();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + dr.HasRows + " Registros");

				while (dr.Read())
				{
					DateTime? tempFechaEnvio;
					double? tempGeoLatitud;
					double? tempGeoLongitud;

					if (dr["dateFechaEnvio"] == System.DBNull.Value)
					{ tempFechaEnvio = null; }
					else
					{
						tempFechaEnvio = DateTime.ParseExact(dr["dateFechaEnvio"].ToString() + " " + dr["timeFechaEnvio"].ToString(), SQLiteDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
					}

					if (dr["geoLatitud"] == System.DBNull.Value)
					{ tempGeoLatitud = null; }
					else
					{
						tempGeoLatitud = Convert.ToDouble(dr["geoLatitud"]);
					}

					if (dr["geoLongitud"] == System.DBNull.Value)
					{ tempGeoLongitud = null; }
					else
					{
						tempGeoLongitud = Convert.ToDouble(dr["geoLongitud"]);
					}

					_Envio = new Envio
					{
						IdEnvio = Convert.ToInt32(dr["idEnvio"]),
						IdCliente = Convert.ToInt32(dr["idCliente"]),
						IdTransportista = Convert.ToInt32(dr["idTransportista"]),
						FechaCarga = DateTime.ParseExact(dr["dateFechaCarga"].ToString() + " " + dr["timeFechaCarga"].ToString(), SQLiteDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture),
						FechaEnvio = tempFechaEnvio,
						IdEstado = Convert.ToInt32(dr["idEstado"].ToString()),
						GeoLatitud = tempGeoLatitud,
						GeoLongitud = tempGeoLongitud,
						DescCliente = dr["descCliente"].ToString(),
						DescTransportista = dr["descTransportista"].ToString(),
						DescEstado = dr["descEstado"].ToString(),
						Observaciones = dr["observaciones"].ToString(),
						Contactos = Convert.ToInt32(dr["contactos"].ToString())
					};
				}
				return _Envio;
			}
			catch (Exception ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex, IdEnvio, IdOrganization);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				return _Envio;
			}
		}

		public List<Contacto> ObtenerContactosEnvio(int IdEnvio, int IdOrganization)
		{
			SQLiteCommand sqlite_cmd = null;
			List<Contacto> Result = new List<Contacto>();
			Contacto _Contacto = new Contacto();

			try
			{
				SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringRO);
				sqlite_con.Open();

				string query = "";

				query = "SELECT ccto.idEnvio, date(ccto.fechaContacto) as dateFechaContacto, time(ccto.fechaContacto) as timeFechaContacto, ccto.geoLatitud, ccto.geoLongitud, ccto.idTransportista,tra.descTransportista, ccto.observaciones FROM transportistas tra, contactos_envios ccto, envios env WHERE ccto.idEnvio = @ENVIO AND env.idEnvio = ccto.IdEnvio AND tra.idTransportista = ccto.idTransportista AND env.idOrganization = @ORGANIZACION;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ORGANIZACION", IdOrganization));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@ENVIO", IdEnvio));

				SQLiteDataReader dr = sqlite_cmd.ExecuteReader();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + dr.HasRows + " Registros");

				while (dr.Read())
				{
					_Contacto = new Contacto
					{
						IdEnvio = Convert.ToInt32(dr["idEnvio"]),
						IdTransportista = Convert.ToInt32(dr["idTransportista"]),
						FechaContacto = DateTime.ParseExact(dr["dateFechaContacto"].ToString() + " " + dr["timeFechaContacto"].ToString(), SQLiteDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture),
						GeoLatitud = Convert.ToDouble(dr["geoLatitud"]),
						GeoLongitud = Convert.ToDouble(dr["geoLongitud"]),
						DescTransportista = dr["descTransportista"].ToString(),
						Observaciones = dr["observaciones"].ToString()
					};
					Result.Add(_Contacto);
				}
				return Result;
			}
			catch (Exception ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex, IdEnvio);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				return Result;
			}
		}

		public List<Cliente> ObtenerClientesDBF()
		{
			List<Cliente> Result = new List<Cliente>();
			try
			{
				string constr = string.Format(ConfigurationManager.AppSettings["StrConnDBF"], Path.GetDirectoryName(ConfigurationManager.AppSettings["ClientesDBFPath"]));
				using (OleDbConnection dbf_con = new OleDbConnection(constr))
				{
					var dbf_sql = "select * from " + Path.GetFileName(ConfigurationManager.AppSettings["ClientesDBFPath"]);
					OleDbCommand dbf_cmd = new OleDbCommand(dbf_sql, dbf_con);
					dbf_con.Open();
					DataSet dbf_ds = new DataSet();
					OleDbDataAdapter dbf_da = new OleDbDataAdapter(dbf_cmd);
					dbf_da.Fill(dbf_ds);
					dbf_con.Close();
					CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, dbf_sql + " - " + dbf_ds.Tables[0].Rows.Count + " Registros");

					foreach (DataRow dbf_dr in dbf_ds.Tables[0].Rows)
					{
						if (dbf_dr["ACTIVO"].ToString().Trim() == "S" && !dbf_dr["TRANSPORTE"].ToString().Trim().Equals(""))
						{
							Result.Add(new Cliente { IdCliente = Convert.ToInt32(dbf_dr["CODIGO"]), DescCliente = dbf_dr["NOMBRE"].ToString().ToUpper(), IdTransportista = Convert.ToInt32(dbf_dr["TRANSPORTE"]) });
						}
					}
				}

				return Result;

			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, ex.StackTrace);
				return Result;
			}
		}

		public List<Transportista> ObtenerTransportistasDBF()
		{
			List<Transportista> Result = new List<Transportista>();
			try
			{
				string constr = string.Format(ConfigurationManager.AppSettings["StrConnDBF"], Path.GetDirectoryName(ConfigurationManager.AppSettings["TransportDBFPath"]));
				using (OleDbConnection dbf_con = new OleDbConnection(constr))
				{
					var dbf_sql = "select * from " + Path.GetFileName(ConfigurationManager.AppSettings["TransportDBFPath"]);
					OleDbCommand dbf_cmd = new OleDbCommand(dbf_sql, dbf_con);
					dbf_con.Open();
					DataSet dbf_ds = new DataSet();
					OleDbDataAdapter dbf_da = new OleDbDataAdapter(dbf_cmd);
					dbf_da.Fill(dbf_ds);
					dbf_con.Close();
					CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, dbf_sql + " - " + dbf_ds.Tables[0].Rows.Count + " Registros");

					foreach (DataRow dbf_dr in dbf_ds.Tables[0].Rows)
					{
						Result.Add(new Transportista { IdTransportista = Convert.ToInt32(dbf_dr["CODIGO"]),DescTransportista = dbf_dr["NOMBRE"].ToString().ToUpper() });
					}
				}

				return Result;

			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, ex.StackTrace);
				return Result;
			}
		}

		public bool SyncroTransportistas()
		{
			try
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, "Inicio Syncro Transportistas");
				List<Transportista> Transportistas = ObtenerTransportistasDBF();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, "ObtenerTransportistas - " + Transportistas.Count + " Objetos");

				foreach (Transportista Tr in Transportistas)
				{
					SyncroAnalizarTransportista(Tr);
				}

				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, "Fin Syncro Transportistas");
				return true;
			}
			catch
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, "Syncro Transportistas");
				return false;
			}
		}

		public bool InsertarTransportista(Transportista Tr)
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();
			try
			{
				query = "INSERT INTO transportistas (idTransportista,descTransportista,idOrganization) VALUES (@IDTRANSPORTISTA,@DESCTRANSPORTISTA,@IDORGANIZACION);";
			  sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDTRANSPORTISTA", Tr.IdTransportista));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@DESCTRANSPORTISTA", Tr.DescTransportista.Trim()));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDORGANIZACION", System.Configuration.ConfigurationManager.AppSettings["IdOrganizacion"]));
				int Insertados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + Insertados + " Registros");
				if (Insertados  > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{

				if (ex.ErrorCode == 19) //Constraint
				{ return false; }
				else
				{
					CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
					CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
					throw ex;
				}
			}
		}

		public bool InsertarCliente(Cliente Cl)
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();
			try
			{
				query = "INSERT INTO Clientes (idCliente,descCliente,idOrganization) VALUES (@IDCLIENTE,@DESCCLIENTE,@IDORGANIZACION);";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDCLIENTE", Cl.IdCliente));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@DESCCLIENTE", Cl.DescCliente.Trim()));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDORGANIZACION", System.Configuration.ConfigurationManager.AppSettings["IdOrganizacion"]));
				int Insertados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + Insertados + " Registros");
				if (Insertados > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{

				if (ex.ErrorCode == 19) //Constraint
				{ return false; }
				else
				{
					CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
					CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
					throw ex;
				}
			}
		}

		private void SyncroAnalizarTransportista(Transportista _Transportista)
		{
			try
			{
				bool Insertado = InsertarTransportista(_Transportista);
				if (Insertado)
				{
					CustomLogging.LogMessage(CustomLogging.TracingLevel.INFO, "Insertado Transportista " + _Transportista.IdTransportista + " - " + _Transportista.DescTransportista);
				}
				else
				{
					CustomLogging.LogMessage(CustomLogging.TracingLevel.INFO, "Esistente Transportista " + _Transportista.IdTransportista + " - " + _Transportista.DescTransportista);
				}
			}
			catch(Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, "Analizando Transportista " + _Transportista.IdTransportista + " - " + _Transportista.DescTransportista + " - " + ex.StackTrace);
			}
		}

		public bool SyncroClientes()
		{
			try
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, "Inicio Syncro Clientes");
				List<Cliente> Clientes = ObtenerClientesDBF();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, "ObtenerClientes - " + Clientes.Count + " Objetos");

				foreach (Cliente Cl in Clientes)
				{
					SyncroAnalizarCliente(Cl);
				}

				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, "Fin Syncro Clientes");
				return true;
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, "Syncro Transportistas" + " - " + ex.StackTrace);
				return false;
			}
		}

		private void SyncroAnalizarCliente(Cliente _Cliente)
		{
			try
			{
				bool Insertado = InsertarCliente(_Cliente);
				if (Insertado)
				{
					CustomLogging.LogMessage(CustomLogging.TracingLevel.INFO, "Insertado Cliente " + _Cliente.IdCliente + " - " + _Cliente.DescCliente);
				}
				else
				{
					CustomLogging.LogMessage(CustomLogging.TracingLevel.INFO, "Esistente Cliente " + _Cliente.IdCliente + " - " + _Cliente.DescCliente);
				}
			}
			catch (Exception ex)
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, "Analizando Cliente " + _Cliente.IdCliente + " - " + _Cliente.DescCliente + " - " + ex.StackTrace);
			}
		}

		public bool SyncroRelTransportistaClientes()
		{
			try
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, "Inicio Syncro Relacion Tranportista Clientes");
				List<Cliente> Clientes = ObtenerClientesDBF();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, "ObtenerClientes - " + Clientes.Count + " Objetos");

				foreach (Cliente Cl in Clientes)
				{
					SyncroAnalizarRelTransportistaClientes(Cl);
				}

				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, "Fin Syncro Relacion Tranportista Clientes");
				return true;
			}
			catch
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, "Syncro Syncro Relacion Tranportista Clientes");
				return false;
			}
		}

		private void SyncroAnalizarRelTransportistaClientes(Cliente _Cliente)
		{
			try
			{
				bool Insertado = InsertarRelTransportistaClientes(_Cliente);
				if (Insertado)
				{
					CustomLogging.LogMessage(CustomLogging.TracingLevel.INFO, "Insertado Relacion Tranportista Clientes " + _Cliente.IdCliente + " - " + _Cliente.DescCliente);
				}
				else
				{
					CustomLogging.LogMessage(CustomLogging.TracingLevel.INFO, "Esistente Relacion Tranportista Clientes " + _Cliente.IdCliente + " - " + _Cliente.DescCliente);
				}
			}
			catch
			{
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, "Analizando Relacion Tranportista Clientes " + _Cliente.IdCliente + " - " + _Cliente.DescCliente);
			}
		}

		private bool InsertarRelTransportistaClientes(Cliente Cl)
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();
			try
			{
				query = "INSERT INTO rel_clientes_transportistas (idCliente,idTransportista) VALUES (@IDCLIENTE,@IDTRANSPORTISTA);";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDCLIENTE", Cl.IdCliente));
				sqlite_cmd.Parameters.Add(new SQLiteParameter("@IDTRANSPORTISTA",Cl.IdTransportista));
				int Insertados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + Insertados + " Registros");
				if (Insertados > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{

				if (ex.ErrorCode == 19) //Constraint
				{ return false; }
				else
				{
					CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
					CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
					throw ex;
				}
			}
		}

		public bool RestartDataClienteTransportistaRel()
		{
			SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringNoRO);
			sqlite_con.Open();
			string query = "";
			SQLiteCommand sqlite_cmd = new SQLiteCommand();
			try
			{
				query = "BEGIN TRANSACTION;DELETE FROM clientes;DELETE FROM transportistas;DELETE FROM rel_clientes_transportistas;COMMIT;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);
				int Eliminados = sqlite_cmd.ExecuteNonQuery();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Eliminados + " Registros");
				if (Eliminados > 0)
				{ return true; }
				else
				{ return false; }
			}
			catch (SQLiteException ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				throw ex;
			}
		}

		public List<ClienteTransportista> ObtenerClientesTransportistas()
		{
			SQLiteCommand sqlite_cmd = null;
			List<ClienteTransportista> Result = new List<ClienteTransportista>();
			try
			{
				SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringRO);
				sqlite_con.Open();

				string query = "";

				query = "SELECT cli.idCliente, cli.descCliente, tra.idTransportista, tra.descTransportista FROM clientes cli, transportistas tra, rel_clientes_transportistas rel WHERE cli.idCliente = rel.idCliente AND tra.idTransportista = rel.idTransportista ORDER BY cli.idCliente ASC;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);

				SQLiteDataReader dr = sqlite_cmd.ExecuteReader();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + dr.HasRows + " Registros");

				while (dr.Read())
				{
					Result.Add(new ClienteTransportista
					{
						IdCliente = Convert.ToInt32(dr["idCliente"]),
						IdTransportista = Convert.ToInt32(dr["idTransportista"]),
						DescCliente = dr["descCliente"].ToString(),
						DescTransportista = dr["descTransportista"].ToString()
					});
				}
				return Result;
			}
			catch (Exception ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				return Result;
			}
		}

		public List<Transportista> ObtenerTransportistas()
		{
			SQLiteCommand sqlite_cmd = null;
			List<Transportista> Result = new List<Transportista>();
			try
			{
				SQLiteConnection sqlite_con = new SQLiteConnection(dbConnectionStringRO);
				sqlite_con.Open();

				string query = "";

				query = "SELECT idTransportista, descTransportista FROM transportistas ORDER BY descTransportista ASC;";
				sqlite_cmd = new SQLiteCommand(query, sqlite_con);

				SQLiteDataReader dr = sqlite_cmd.ExecuteReader();
				CustomLogging.LogMessage(CustomLogging.TracingLevel.DEBUG, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters) + " - " + dr.HasRows + " Registros");

				while (dr.Read())
				{
					Result.Add(new Transportista
					{
						IdTransportista = Convert.ToInt32(dr["idTransportista"]),
						DescTransportista = dr["descTransportista"].ToString()
					});
				}
				return Result;
			}
			catch (Exception ex)
			{
				CustomLogging.LogError(MethodBase.GetCurrentMethod(), ex);
				CustomLogging.LogMessage(CustomLogging.TracingLevel.ERROR, sqlite_cmd.CommandText + " - " + Parameters(sqlite_cmd.Parameters));
				return Result;
			}
		}
	}
}
