using WebLogistica.Web.Filters;
using WebLogistica.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebLogistica.Data;
using static WebLogistica.Data.ApiAccess;
using WebLogistica.Domain;
using WebLogistica.Models;
using System.Web.UI;

namespace WebLogistica.Web.Controllers
{
		public class NameValue
		{
			public string name { get; set; }
			public string value { get; set; }
		}

		[AuthActionFilter]
    public class EnviosController : Controller
    {
			[CustomAuthorize(1)] //Admin
			public ActionResult Pendientes()
			{
				if (Session["EnviosHomeData"] == null)
					{
						EnviosHomeData viewModel = new EnviosHomeData();

						ApiAccess Api = new ApiAccess();

						UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
						WebLogistica.Data.Estadisticas _EstadisticasData = Api.GetEstadisticas(Usd.Token);

						WebLogistica.Models.Estadisticas _EstadisticasModel = new WebLogistica.Models.Estadisticas();

						_EstadisticasModel.Estadistica1 = _EstadisticasData.Estadistica1;
						_EstadisticasModel.Estadistica2 = _EstadisticasData.Estadistica2;
						_EstadisticasModel.Estadistica3 = _EstadisticasData.Estadistica3;

						viewModel._Estadisticas = _EstadisticasModel;
						Session["EnviosHomeData"] = viewModel;
					}
				return View((EnviosHomeData)Session["EnviosHomeData"]);
			}

			[CustomAuthorize(3)] //Clientes
			public ActionResult Consultas()
			{
				if (Session["EnviosHomeData"] == null)
				{
					EnviosHomeData viewModel = new EnviosHomeData();

					ApiAccess Api = new ApiAccess();

					UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
					WebLogistica.Data.Estadisticas _EstadisticasData = Api.GetEstadisticas(Usd.Token);

					WebLogistica.Models.Estadisticas _EstadisticasModel = new WebLogistica.Models.Estadisticas();

					_EstadisticasModel.Estadistica1 = _EstadisticasData.Estadistica1;
					_EstadisticasModel.Estadistica2 = _EstadisticasData.Estadistica2;
					_EstadisticasModel.Estadistica3 = _EstadisticasData.Estadistica3;

					viewModel._Estadisticas = _EstadisticasModel;
					Session["EnviosHomeData"] = viewModel;
				}
				return View((EnviosHomeData)Session["EnviosHomeData"]);
			}

			[CustomAuthorize(1)] //Admin
			public ActionResult NuevoEnvioView()
			{
				TempData["ClientesDDLId"] = new SelectList(ObtenerClinetesDDLId(), "Value", "Text");

				return PartialView("~/Views/Envios/_NuevoEnvioPartial.cshtml");
			}

			[CustomAuthorize(1)] //Admin
			public ActionResult Completados()
			{
				return View();
			}

			private List<SelectListItem> ObtenerTransportistasDDLText()
			{
				if (Session["TransportistasDDLText"] != null)
				{
					return (List<SelectListItem>)Session["TransportistasDDLText"];
				}
				else
				{
					List<SelectListItem> OutTransportistas = new List<SelectListItem>();
					ApiAccess Api = new ApiAccess();

					UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
					var TransportistasApi = Api.GetTransportistas(Usd.Token);

					foreach (Transportista tr in TransportistasApi)
					{
						OutTransportistas.Add(new SelectListItem() { Text = tr.DescTransportista, Value = tr.DescTransportista });
					}
					Session["TransportistasDDLText"] = OutTransportistas;
					return OutTransportistas;
				}
			}

			private IList<ClientesTransportistasTable> GetClientesTransportistas()
			{
				List<ClientesTransportistasTable> _ClientesTransportistasle = new List<ClientesTransportistasTable>();

				if (Session["ClientesTransportistsListData"] != null)
				{
					return (List<ClientesTransportistasTable>)Session["ClientesTransportistsListData"];
				}
				else
				{
					_ClientesTransportistasle = ObtenerClienteTranportistaList();
					Session["ClientesTransportistsListData"] = _ClientesTransportistasle;
					return _ClientesTransportistasle;
				}
			}

			private List<ClientesTransportistasTable> ObtenerClienteTranportistaList()
				{
					List<ClientesTransportistasTable> _ClientesTransportistasle = new List<ClientesTransportistasTable>();
					ApiAccess Api = new ApiAccess();

					UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
					var AllClientesTransportistas = Api.GetClientesTransportistas(Usd.Token);

					_ClientesTransportistasle = AllClientesTransportistas.AsEnumerable().Select(m => new ClientesTransportistasTable()
					{
						IdCliente = m.IdCliente,
						DescCLiente = m.DescCLiente,
						IdTransportista = m.IdTransportista,
						DescTransportista = m.DescTransportista
					}).ToList();

					return _ClientesTransportistasle;
				}

			private List<SelectListItem> ObtenerClinetesDDLId()
			{
				if (Session["ClientesDDLId"] != null)
				{
					return (List<SelectListItem>)Session["ClientesDDLId"];
				}
				else
				{
					List<SelectListItem> OutClientes = new List<SelectListItem>();
					var _ClientesTransportistasle = GetClientesTransportistas();

					var ClinetesDistinct = _ClientesTransportistasle.Select(x => new { x.IdCliente, x.DescCLiente }).Distinct().OrderBy(x => x.DescCLiente);

					foreach (var cl in ClinetesDistinct)
					{
						OutClientes.Add(new SelectListItem() { Text = cl.DescCLiente, Value = cl.IdCliente.ToString() });
					}
					Session["ClientesDDLId"] = OutClientes;

					return OutClientes;
				}
			}

			[CustomAuthorize(1)] //Admin
			public ActionResult AjaxHandlerPendientes(jQueryDataTableParamModel param)
			{
				var allEnvios = GetEnviosPendientes();

				IEnumerable<EnvioPendienteTable> filteredEnvios;
				//Check whether the companies should be filtered by keyword
				if (!string.IsNullOrEmpty(param.sSearch))
				{
					//Used if particulare columns are filtered 
					var envioFilter = Convert.ToString(Request["sSearch_1"]);
					var clienteFilter = Convert.ToString(Request["sSearch_2"]);
					var transportistaFilter = Convert.ToString(Request["sSearch_3"]);
					var observacionesFilter = Convert.ToString(Request["sSearch_5"]);

					//Optionally check whether the columns are searchable at all 
					var isEnvioSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
					var isClienteSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
					var isTransportistaSearchable = Convert.ToBoolean(Request["bSearchable_3"]);
					var isObservacionesSearchable = Convert.ToBoolean(Request["bSearchable_5"]);

				filteredEnvios = GetEnviosPendientes()
							.Where(c => isEnvioSearchable && c.DescEnvio.ToString().ToLower().Contains(param.sSearch.ToLower())
													||
													isClienteSearchable && c.DescCLiente.ToLower().Contains(param.sSearch.ToLower())
													||
													isTransportistaSearchable && c.DescTransportista.ToLower().Contains(param.sSearch.ToLower())
													||
													isObservacionesSearchable && c.Observaciones.ToLower().Contains(param.sSearch.ToLower()));
				}
				else
				{
					filteredEnvios = allEnvios;
				}

				var isEnvioSortable = Convert.ToBoolean(Request["bSortable_1"]);
				var isClienteSortable = Convert.ToBoolean(Request["bSortable_2"]);
				var isTransportistaSortable = Convert.ToBoolean(Request["bSortable_3"]);
				var isFecCargaSortable = Convert.ToBoolean(Request["bSortable_5"]);
				var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
				Func<EnvioPendienteTable, string> orderingFunction = (c => sortColumnIndex == 1 && isEnvioSortable ? IdxOrderIntString(c.DescEnvio.ToString()) :
																												sortColumnIndex == 2 && isClienteSortable ? c.DescCLiente :
																												sortColumnIndex == 3 && isTransportistaSortable ? c.DescTransportista :
																												sortColumnIndex == 5 && isFecCargaSortable ? String.Format("{0:yyyyMMdd}", c.FechaCarga) :
																												"");

				var sortDirection = Request["sSortDir_0"]; // asc or desc
				if (sortDirection == "asc")
					filteredEnvios = filteredEnvios.OrderBy(orderingFunction);
				else
					filteredEnvios = filteredEnvios.OrderByDescending(orderingFunction);

				var displayedEnvios = filteredEnvios.Skip(param.iDisplayStart).Take(param.iDisplayLength);
				var result = from c in displayedEnvios select new[] { c.IdEnvio.ToString(), c.DescEnvio.ToString(), c.DescCLiente, c.DescTransportista, String.Format(ConfigurationManager.AppSettings["DisplayFormatDate"].ToString(), c.FechaCarga), c.Observaciones, c.IdEnvio.ToString() };
				return Json(new
				{
					sEcho = param.sEcho,
					iTotalRecords = allEnvios.Count(),
					iTotalDisplayRecords = filteredEnvios.Count(),
					aaData = result
				},
				JsonRequestBehavior.AllowGet);
			}

			[CustomAuthorize(1)] //Admin
			public ActionResult AjaxHandlerCompletados(jQueryDataTableParamModel param)
			{

			var allEnvios = GetEnviosCompletados();

			IEnumerable<EnvioCompletadoTable> filteredEnvios;
			//Check whether the companies should be filtered by keyword
			if (!string.IsNullOrEmpty(param.sSearch))
			{
				//Used if particulare columns are filtered 
				var envioFilter = Convert.ToString(Request["sSearch_1"]);
				var clienteFilter = Convert.ToString(Request["sSearch_2"]);
				var transportistaFilter = Convert.ToString(Request["sSearch_3"]);
				var observacionesoFilter = Convert.ToString(Request["sSearch_6"]);

				//Optionally check whether the columns are searchable at all 
				var isEnvioSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
				var isClienteSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
				var isTransportistaSearchable = Convert.ToBoolean(Request["bSearchable_3"]);
				var isObservacionesSearchable = Convert.ToBoolean(Request["bSearchable_6"]);

				filteredEnvios = GetEnviosCompletados()
						.Where(c => isEnvioSearchable && c.DescEnvio.ToString().ToLower().Contains(param.sSearch.ToLower())
												||
												isClienteSearchable && c.DescCLiente.ToLower().Contains(param.sSearch.ToLower())
												||
												isTransportistaSearchable && c.DescTransportista.ToLower().Contains(param.sSearch.ToLower())
												||
												isObservacionesSearchable && c.Observaciones.ToLower().Contains(param.sSearch.ToLower()));
			}
			else
			{
				filteredEnvios = allEnvios;
			}

			var isEnvioSortable = Convert.ToBoolean(Request["bSortable_1"]);
			var isClienteSortable = Convert.ToBoolean(Request["bSortable_2"]);
			var isTransportistaSortable = Convert.ToBoolean(Request["bSortable_3"]);
			var isFecCargaSortable = Convert.ToBoolean(Request["bSortable_5"]);
			var isObservacionesSortable = Convert.ToBoolean(Request["bSortable_6"]);
			var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
			Func<EnvioCompletadoTable, string> orderingFunction = (c => sortColumnIndex == 1 && isEnvioSortable ? IdxOrderIntString(c.DescEnvio.ToString()) :
																											sortColumnIndex == 2 && isClienteSortable ? c.DescCLiente :
																											sortColumnIndex == 3 && isTransportistaSortable ? c.DescTransportista :
																											sortColumnIndex == 5 && isFecCargaSortable ? String.Format("{0:yyyyMMdd}", c.FechaCarga) :
																											sortColumnIndex == 6 && isObservacionesSortable ? c.Observaciones :
																											"");

			var sortDirection = Request["sSortDir_0"]; // asc or desc
			if (sortDirection == "asc")
				filteredEnvios = filteredEnvios.OrderBy(orderingFunction);
			else
				filteredEnvios = filteredEnvios.OrderByDescending(orderingFunction);

			var displayedEnvios = filteredEnvios.Skip(param.iDisplayStart).Take(param.iDisplayLength);
			var result = from c in displayedEnvios select new[] { c.IdEnvio.ToString(), c.DescEnvio.ToString(), c.DescCLiente, c.DescTransportista, String.Format(ConfigurationManager.AppSettings["DisplayFormatDate"].ToString(), c.FechaCarga), String.Format(ConfigurationManager.AppSettings["DisplayFormatDate"].ToString(), c.FechaEnvio), c.Observaciones };
			return Json(new
			{
				sEcho = param.sEcho,
				iTotalRecords = allEnvios.Count(),
				iTotalDisplayRecords = filteredEnvios.Count(),
				aaData = result
			},
			JsonRequestBehavior.AllowGet);
		}

			private IList<EnvioPendienteTable> GetEnviosPendientes()
			{
				List<EnvioPendienteTable> _Envios = new List<EnvioPendienteTable>();

				if (Session["EnviosPendientesListData"] != null)
				{
					_Envios = (List<EnvioPendienteTable>)Session["EnviosPendientesListData"];
				}
				else
				{
					if (Session["UsuarioSessionData"] != null)
					{
						ApiAccess Api = new ApiAccess();

						UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
						var AllEnvios = Api.GetEnvios(Usd.Token, int.Parse(ConfigurationManager.AppSettings["IdEstadoIngresado"].ToString()));

						_Envios = AllEnvios.AsEnumerable().Select(m => new EnvioPendienteTable()
						{
							IdEnvio = m.IdEnvio,
							DescEnvio = m.IdEnvio,
							IdCliente = m.IdCliente,
							DescCLiente = m.DescCLiente,
							IdTransportista = m.IdTransportista,
							DescTransportista = m.DescTransportista,
							IdEstado = m.IdEstado,
							DescEstado = m.DescEstado,
							FechaCarga = m.FechaCarga,
							Observaciones = m.Observaciones,
						}).ToList();

						Session["EnviosPendientesListData"] = _Envios;
					}
				}

				return _Envios;
			}

			private IList<EnvioPendienteTable> GetEnviosSalientes()
			{
				List<EnvioPendienteTable> _Envios = new List<EnvioPendienteTable>();

				if (Session["UsuarioSessionData"] != null)
				{
					ApiAccess Api = new ApiAccess();

					UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
					var AllEnvios = Api.GetEnvios(Usd.Token, int.Parse(ConfigurationManager.AppSettings["IdEstadoEnviado"].ToString()));

					_Envios = AllEnvios.AsEnumerable().Select(m => new EnvioPendienteTable()
					{
						IdEnvio = m.IdEnvio,
						DescEnvio = m.IdEnvio,
						IdCliente = m.IdCliente,
						DescCLiente = m.DescCLiente,
						IdTransportista = m.IdTransportista,
						DescTransportista = m.DescTransportista,
						IdEstado = m.IdEstado,
						DescEstado = m.DescEstado,
						FechaCarga = m.FechaCarga,
						FechaEnvio = m.FechaEnvio,
						Observaciones = m.Observaciones,
						Contactos = m.Contactos
					}).ToList();
				}

				return _Envios;
			}

			private IList<EnvioCompletadoTable> GetEnviosCompletados()
				{
				List<EnvioCompletadoTable> _Envios = new List<EnvioCompletadoTable>();

				if (Session["EnviosCompletadosListData"] != null)
				{
					_Envios = (List<EnvioCompletadoTable>)Session["EnviosCompletadosListData"];
				}
				else
				{
					if (Session["UsuarioSessionData"] != null)
					{
						ApiAccess Api = new ApiAccess();

						UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
						var AllEnvios = Api.GetEnvios(Usd.Token, int.Parse(ConfigurationManager.AppSettings["IdEstadoCompletado"].ToString()));

						_Envios = AllEnvios.AsEnumerable().Select(m => new EnvioCompletadoTable()
						{
							IdEnvio = m.IdEnvio,
							DescEnvio = m.IdEnvio,
							IdCliente = m.IdCliente,
							DescCLiente = m.DescCLiente,
							IdTransportista = m.IdTransportista,
							DescTransportista = m.DescTransportista,
							IdEstado = m.IdEstado,
							DescEstado = m.DescEstado,
							FechaCarga = m.FechaCarga,
							FechaEnvio = m.FechaEnvio,
							Observaciones = m.Observaciones,
						}).ToList();

						Session["EnviosCompletadosListData"] = _Envios;
					}
				}

				return _Envios;
			}

			[HttpPost]
			[CustomAuthorize(1)] //Admin
			public JsonResult Delete(int ID)
			{
				bool res = false;
				try
				{
					ApiAccess Api = new ApiAccess();

					UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
					res = Api.EliminarEnvioDisponible(Usd.Token, ID);
				}
				catch
				{
					res = false;
				}

				if (res)
				{
					ModelState.Clear();
					Session.Remove("EnviosPendientesListData");
					return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
				}
			}

			[HttpPost]
			[CustomAuthorize(1)] //Admin
			public JsonResult GetTransportistaFromCliente(int ID)
			{
				List<ClientesTransportistasTable> DataCliente = (List<ClientesTransportistasTable>)Session["ClientesTransportistsListData"];
				var OutDataCliente = DataCliente.Select(x => new { x.DescTransportista, x.IdCliente }).Where(x => x.IdCliente == ID).Distinct().First();

				return Json(new { data = "Success",  transportista = OutDataCliente.DescTransportista});
			}

			[HttpPost]
			[CustomAuthorize(1)] //Admin
			public ActionResult Nuevo(EnvioIn Envio)
			{
				List<ClientesTransportistasTable> DataTransportista= (List<ClientesTransportistasTable>)Session["ClientesTransportistsListData"];
				var OutDataCliente = DataTransportista.Select(x => new { x.IdTransportista, x.IdCliente }).Where(x => x.IdCliente == Envio.IdCliente).Distinct().First();
				NuevoEnvio NuevoEnvioApi = new NuevoEnvio();

				NuevoEnvioApi.IdCliente = Envio.IdCliente;
				NuevoEnvioApi.Observaciones = Envio.Observaciones;
				NuevoEnvioApi.IdTransportista = OutDataCliente.IdTransportista;

				ApiAccess Api = new ApiAccess();

				UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
				Api.IngresarEnvio(Usd.Token, NuevoEnvioApi);

				ModelState.Clear();
				Session.Remove("EnviosPendientesListData");
				return RedirectToAction("Pendientes", "Envios");
			}

			[HttpPost]
			[CustomAuthorize(1)] //Admin
			public ActionResult Editar(EnvioEdit Envio)
			{
				ApiAccess Api = new ApiAccess();

				UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
				Api.EditarEnvio(Usd.Token, Envio.IdEnvio, Envio.Observaciones);

				ModelState.Clear();
				Session.Remove("EnviosPendientesListData");
				return RedirectToAction("Pendientes", "Envios");
			}

			[CustomAuthorize(1)] //Admin
			public ActionResult Salientes()
      {
				TempData["TransportistasDDLText"] = new SelectList(ObtenerTransportistasDDLText(), "Value", "Text");

				return View();
      }

			private string IdxOrderIntString(string Id)
				{
					char[] ChrArray = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k' };
					string Out = "";
					foreach(char c in Id)
					{
						Out = Out + ChrArray[int.Parse(c.ToString())];
					}
					return Out.PadLeft(10,'a');
				}

			[CustomAuthorize(1)] //Admin
			public ActionResult AjaxHandlerSalientes(jQueryDataTableParamModel param)
			{
				IEnumerable<EnvioPendienteTable> filteredEnvios;

				var allEnvios = GetEnviosSalientes();

				if (!string.IsNullOrEmpty(param.sSearch))
				{
					filteredEnvios = allEnvios.Where(c =>  c.DescTransportista.ToLower().Contains(param.sSearch.ToLower()));
				}
				else
				{
					filteredEnvios = allEnvios;
				}

				var result = from c in filteredEnvios select new[] { c.IdEnvio.ToString(), c.DescTransportista, c.DescCLiente, String.Format(ConfigurationManager.AppSettings["DisplayFormatDate"].ToString(), c.FechaCarga), c.Observaciones, c.Contactos.ToString() };

				return Json(new
				{
					sEcho = param.sEcho,
					iTotalRecords = filteredEnvios.Count(),
					iTotalDisplayRecords = filteredEnvios.Count(),
					aaData = result
				},
				JsonRequestBehavior.AllowGet);
			}

			[CustomAuthorize(3)] //Clientes
			public ActionResult AjaxHandlerConsultas(jQueryDataTableParamModel param)
			{
				IEnumerable<EnvioPendienteTable> filteredEnvios;

				var allEnvios = GetEnviosSalientes();

				if (!string.IsNullOrEmpty(param.sSearch))
				{
					filteredEnvios = allEnvios.Where(c => c.DescTransportista.ToLower().Contains(param.sSearch.ToLower()));
				}
				else
				{
					filteredEnvios = allEnvios;
				}

				var result = from c in filteredEnvios select new[] { c.IdEnvio.ToString(), c.DescTransportista, c.DescCLiente, String.Format(ConfigurationManager.AppSettings["DisplayFormatDate"].ToString(), c.FechaCarga), c.Observaciones, c.Contactos.ToString() };

				return Json(new
				{
					sEcho = param.sEcho,
					iTotalRecords = filteredEnvios.Count(),
					iTotalDisplayRecords = filteredEnvios.Count(),
					aaData = result
				},
				JsonRequestBehavior.AllowGet);
			}

			[HttpGet] // this action result returns the partial containing the modal
			[CustomAuthorize(1)] //Admin
			public ActionResult EditarView(int id)
			{
				var viewModel = new EnvioEdit();

				ApiAccess Api = new ApiAccess();

				UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
				InfoEnvio _InfoEnvio = Api.GetDetalleEnvio(Usd.Token, id);

				viewModel.IdEnvio = _InfoEnvio._Envio.IdEnvio;
				viewModel.Observaciones = _InfoEnvio._Envio.Observaciones;
				return PartialView("~/Views/Envios/_EditEnvioPartial.cshtml", viewModel);
			}

			[OutputCache(Duration = 0)]
			[CustomAuthorize(1)] //Admin
			public ActionResult Completado(int id)
			{
					ApiAccess Api = new ApiAccess();

					UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
					InfoEnvio _InfoEnvio = Api.GetDetalleEnvio(Usd.Token, id);

					var viewModel = new EnvioContactosOut();
					viewModel.IdEnvio = _InfoEnvio._Envio.IdEnvio;
					viewModel.Observaciones = _InfoEnvio._Envio.Observaciones;
					viewModel.DescCliente = _InfoEnvio._Envio.DescCLiente;
					viewModel.DescEstado = _InfoEnvio._Envio.DescEstado;
					viewModel.DescTransportista = _InfoEnvio._Envio.DescTransportista;
					viewModel.FechaCarga = _InfoEnvio._Envio.FechaCarga;
					viewModel.FechaEnvio = _InfoEnvio._Envio.FechaEnvio;
					viewModel.GeoLatitud = _InfoEnvio._Envio.GeoLatitud.ToString().Replace(",", ".");
					viewModel.GeoLongitud = _InfoEnvio._Envio.GeoLongitud.ToString().Replace(",", ".");
					viewModel.CantContactos = _InfoEnvio._Envio.Contactos;

					if (_InfoEnvio._Contactos != null)
					{
						if (_InfoEnvio._Contactos.Count > 0)
						{
							viewModel.Contactos = new List<WebLogistica.Models.Contacto>();
							foreach (ApiAccess.Contacto c in _InfoEnvio._Contactos)
							{
								viewModel.Contactos.Add(new WebLogistica.Models.Contacto { IdEnvio = c.IdEnvio, IdTransportista = c.IdTransportista, DescTransportista = c.DescTransportista, FechaContacto = c.FechaContacto, GeoLatitud = c.GeoLatitud.ToString().Replace(",", "."), GeoLongitud = c.GeoLongitud.ToString().Replace(",", "."), Observaciones = c.Observaciones });
							}
						}
				}

				return PartialView("~/Views/Envios/_OutEnvioPartial.cshtml", viewModel);
			}

			[OutputCache(Duration = 0)]
			[CustomAuthorize(1,3)] //Admin Clientes
			public ActionResult Contactado(int id)
			{
				ApiAccess Api = new ApiAccess();

				UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
				InfoEnvio _EnvioSaliente = Api.GetDetalleEnvio(Usd.Token,id);

				var viewModel = new EnvioContactosOut();
				viewModel.IdEnvio = _EnvioSaliente._Envio.IdEnvio;
				viewModel.Observaciones = _EnvioSaliente._Envio.Observaciones;
				viewModel.DescCliente = _EnvioSaliente._Envio.DescCLiente;
				viewModel.DescEstado = _EnvioSaliente._Envio.DescEstado;
				viewModel.DescTransportista = _EnvioSaliente._Envio.DescTransportista;
				viewModel.FechaCarga = _EnvioSaliente._Envio.FechaCarga;
				viewModel.FechaEnvio = _EnvioSaliente._Envio.FechaEnvio;
				viewModel.CantContactos = _EnvioSaliente._Envio.Contactos;

				if (_EnvioSaliente._Contactos != null)
				{
					if (_EnvioSaliente._Contactos.Count > 0)
					{
						viewModel.Contactos = new List<WebLogistica.Models.Contacto>();
						foreach (ApiAccess.Contacto c in _EnvioSaliente._Contactos)
						{
							viewModel.Contactos.Add(new WebLogistica.Models.Contacto { IdEnvio = c.IdEnvio, IdTransportista = c.IdTransportista, DescTransportista = c.DescTransportista, FechaContacto = c.FechaContacto, GeoLatitud = c.GeoLatitud.ToString().Replace(",", "."), GeoLongitud = c.GeoLongitud.ToString().Replace(",", "."), Observaciones = c.Observaciones });
						}
					}
				}

				return PartialView("~/Views/Envios/_OutEnvioContactosPartial.cshtml", viewModel);
			}

			[CustomAuthorize(1)] //Admin
			public JsonResult Liberar(List<NameValue> jsonInput)
			{
				if (jsonInput != null)
				{
				bool res = false;

				List<long> Le = new List<long>();
				foreach (NameValue Nv in jsonInput)
				{
					Le.Add(long.Parse(Nv.value));
				}

				try
				{
					ApiAccess Api = new ApiAccess();

					UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
					res = Api.EnviarEnviosDisponibles(Usd.Token, Le);
				}
				catch
				{
					res = false;
				}

				if (res)
				{
					ModelState.Clear();
					Session.Remove("EnviosPendientesListData");
					return Json(data: "Liberados", behavior: JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
				}
			}
				else
				{
					return Json(data: "NoData", behavior: JsonRequestBehavior.AllowGet);
				}
			}
		}
}