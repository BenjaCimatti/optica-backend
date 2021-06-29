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

namespace WebLogistica.Web.Controllers
{
    [AuthActionFilter]
    public class EnviosController : Controller
    {
			[CustomAuthorize(1)] //Admin
			public ActionResult Pendientes()
			{
				return View();
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
					var estadoFilter = Convert.ToString(Request["sSearch_4"]);

					//Optionally check whether the columns are searchable at all 
					var isEnvioSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
					var isClienteSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
					var isTransportistaSearchable = Convert.ToBoolean(Request["bSearchable_3"]);
					var isEstadoSearchable = Convert.ToBoolean(Request["bSearchable_4"]);

					filteredEnvios = GetEnviosPendientes()
							.Where(c => isEnvioSearchable && c.DescEnvio.ToString().ToLower().Contains(param.sSearch.ToLower())
													||
													isClienteSearchable && c.DescCLiente.ToLower().Contains(param.sSearch.ToLower())
													||
													isTransportistaSearchable && c.DescTransportista.ToLower().Contains(param.sSearch.ToLower())
													||
													isEstadoSearchable && c.DescEstado.ToLower().Contains(param.sSearch.ToLower()));
				}
				else
				{
					filteredEnvios = allEnvios;
				}

				var isEnvioSortable = Convert.ToBoolean(Request["bSortable_1"]);
				var isClienteSortable = Convert.ToBoolean(Request["bSortable_2"]);
				var isTransportistaSortable = Convert.ToBoolean(Request["bSortable_3"]);
				var isEstadoSortable = Convert.ToBoolean(Request["bSortable_4"]);
				var isFecCargaSortable = Convert.ToBoolean(Request["bSortable_5"]);
				var isFecEnvioSortable = Convert.ToBoolean(Request["bSortable_6"]);
				var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
				Func<EnvioPendienteTable, string> orderingFunction = (c => sortColumnIndex == 1 && isEnvioSortable ? IdxOrderIntString(c.DescEnvio.ToString()) :
																												sortColumnIndex == 2 && isClienteSortable ? c.DescCLiente :
																												sortColumnIndex == 3 && isTransportistaSortable ? c.DescTransportista :
																												sortColumnIndex == 4 && isEstadoSortable ? c.DescEstado :
																												sortColumnIndex == 5 && isFecCargaSortable ? String.Format("{0:yyyyMMdd}", c.FechaCarga) :
																												sortColumnIndex == 6 && isFecEnvioSortable ? String.Format("{0:yyyyMMdd}", c.FechaEnvio) :
																												"");

				var sortDirection = Request["sSortDir_0"]; // asc or desc
				if (sortDirection == "asc")
					filteredEnvios = filteredEnvios.OrderBy(orderingFunction);
				else
					filteredEnvios = filteredEnvios.OrderByDescending(orderingFunction);

				var displayedEnvios = filteredEnvios.Skip(param.iDisplayStart).Take(param.iDisplayLength);
				var result = from c in displayedEnvios select new[] { c.IdEnvio.ToString(), c.DescEnvio.ToString(), c.DescCLiente, c.DescTransportista, c.DescEstado, String.Format(ConfigurationManager.AppSettings["DisplayFormatDate"].ToString(), c.FechaCarga), c.Observaciones };
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
						var AllEnvios = Api.GetEnvios(Usd.Token, 1);

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
						}).ToList();

						Session["EnviosPendientesListData"] = _Envios;
					}
				}

				return _Envios;
			}

			[HttpPost]
			[CustomAuthorize(1)] //Admin
			public JsonResult DeleteEnvio(int ID)
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

			[CustomAuthorize(1)] //Admin
			public ActionResult Salientes()
      {
				List<SelectListItem> OutTransportistas = new List<SelectListItem>();
				ApiAccess Api = new ApiAccess();

				UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
				var TransportistasApi = Api.GetTransportistas(Usd.Token);
				
				foreach (Transportista tr in TransportistasApi)
				{
					OutTransportistas.Add(new SelectListItem() { Text = tr.DescTransportista, Value = tr.DescTransportista });
				}

				TempData["Transportistas"] = new SelectList(OutTransportistas, "Value", "Text");

				return View();
      }

		//[CustomAuthorize(1)] //Admin
		//public ActionResult AjaxHandlerSalientes(jQueryDataTableParamModel param)
		//{
		//	var allEnvios = GetEnviosPendientes();

		//	IEnumerable<EnvioPendienteTable> filteredEnvios;
		//	//Check whether the companies should be filtered by keyword
		//	if (!string.IsNullOrEmpty(param.sSearch))
		//	{
		//		//Used if particulare columns are filtered 
		//		var transportistaFilter = Convert.ToString(Request["sSearch_1"]);

		//		//Optionally check whether the columns are searchable at all 
		//		var isTransportistaSearchable = Convert.ToBoolean(Request["bSearchable_1"]);

		//		filteredEnvios = GetEnviosPendientes()
		//				.Where(c => isTransportistaSearchable && c.DescTransportista.ToLower().Contains(param.sSearch.ToLower()));
		//	}
		//	else
		//	{
		//		filteredEnvios = allEnvios;
		//	}

		//	var isEnvioSortable = Convert.ToBoolean(Request["bSortable_0"]);
		//	var isTransportistaSortable = Convert.ToBoolean(Request["bSortable_1"]);
		//	var isFecCargaSortable = Convert.ToBoolean(Request["bSortable_3"]);
		//	var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
		//	Func<EnvioPendienteTable, string> orderingFunction = (c => 
		//																									sortColumnIndex == 0 && isEnvioSortable ? IdxOrderIntString(c.IdEnvio.ToString()) :
		//																									sortColumnIndex == 1 && isTransportistaSortable ? c.DescTransportista :
		//																									sortColumnIndex == 3 && isFecCargaSortable ? String.Format("{0:yyyyMMdd}", c.FechaCarga) :
		//																									"");

		//	var sortDirection = Request["sSortDir_0"]; // asc or desc
		//	if (sortDirection == "asc")
		//		filteredEnvios = filteredEnvios.OrderBy(orderingFunction);
		//	else
		//		filteredEnvios = filteredEnvios.OrderByDescending(orderingFunction);

		//	var displayedEnvios = filteredEnvios.Skip(param.iDisplayStart).Take(param.iDisplayLength);
		//	var result = from c in displayedEnvios select new[] { c.IdEnvio.ToString(), c.DescTransportista, c.DescCLiente, String.Format(ConfigurationManager.AppSettings["DisplayFormatDate"].ToString(), c.FechaCarga), c.Observaciones };
		//	return Json(new
		//	{
		//		sEcho = param.sEcho,
		//		iTotalRecords = allEnvios.Count(),
		//		iTotalDisplayRecords = filteredEnvios.Count(),
		//		aaData = result
		//	},
		//	JsonRequestBehavior.AllowGet);
		//}

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
			  IEnumerable<EnvioPendienteTable> allTransportistas;

				allTransportistas = GetEnviosPendientes();

				var result = from c in allTransportistas select new[] { c.IdEnvio.ToString(), c.DescTransportista, c.DescCLiente, String.Format(ConfigurationManager.AppSettings["DisplayFormatDate"].ToString(), c.FechaCarga), c.Observaciones };

				return Json(new
				{
					sEcho = param.sEcho,
					iTotalRecords = allTransportistas.Count(),
					iTotalDisplayRecords = allTransportistas.Count(),
					aaData = result
				},
				JsonRequestBehavior.AllowGet);
		}
		}
}