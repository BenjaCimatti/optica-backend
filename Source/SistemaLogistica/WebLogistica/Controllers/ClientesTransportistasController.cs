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

namespace WebLogistica.Web.Controllers
{
    [AuthActionFilter]
    public class ClientesTransportistasController : Controller
    {
		[CustomAuthorize(1)] //Admin
			public ActionResult Listado()
      {
          return View();
      }

			[CustomAuthorize(1)] //Admin
			public ActionResult AjaxHandlerClientesTransportistas(jQueryDataTableParamModel param)
      {

					var allClientesTransportistas = GetClientesTransportistas();

					IEnumerable<ClientesTransportistasTable> filteredClientesTransportistas;
          //Check whether the companies should be filtered by keyword
          if (!string.IsNullOrEmpty(param.sSearch))
          {
              //Used if particulare columns are filtered 
              var idClienteFilter = Convert.ToString(Request["sSearch_0"]);
              var clienteFilter = Convert.ToString(Request["sSearch_1"]);
              var idTransportistaFilter = Convert.ToString(Request["sSearch_2"]);
              var transportistaFilter = Convert.ToString(Request["sSearch_3"]);

              //Optionally check whether the columns are searchable at all 
              var isIdClienteSearchable = Convert.ToBoolean(Request["bSearchable_0"]);
              var isClienteSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
              var isIdTransportistaSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
              var isTransportistaSearchable = Convert.ToBoolean(Request["bSearchable_3"]);

							filteredClientesTransportistas = GetClientesTransportistas()
                  .Where(c => isIdClienteSearchable && c.IdCliente.ToString().ToLower().Contains(param.sSearch.ToLower())
                              ||
															isClienteSearchable && c.DescCLiente.ToLower().Contains(param.sSearch.ToLower())
                              ||
															isIdTransportistaSearchable && c.IdTransportista.ToString().ToLower().Contains(param.sSearch.ToLower())
															||
															isTransportistaSearchable && c.DescTransportista.ToLower().Contains(param.sSearch.ToLower()));
					}
					else
					{
						filteredClientesTransportistas = allClientesTransportistas;
					}

					var isIdClienteSortable = Convert.ToBoolean(Request["bSortable_1"]);
					var isClienteSortable = Convert.ToBoolean(Request["bSortable_2"]);
					var isIdTransportistaSortable = Convert.ToBoolean(Request["bSortable_3"]);
					var isTransportistaSortable = Convert.ToBoolean(Request["bSortable_4"]);
					var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
					Func<ClientesTransportistasTable, string> orderingFunction = (c => sortColumnIndex == 1 && isIdClienteSortable ? IdxOrderIntString(c.IdCliente.ToString()) : 
																													sortColumnIndex == 2 && isClienteSortable ? c.DescCLiente :
																													sortColumnIndex == 3 && isIdTransportistaSortable ? IdxOrderIntString(c.IdTransportista.ToString()) :
																													sortColumnIndex == 4 && isTransportistaSortable ? c.DescTransportista :
																													"");

					var sortDirection = Request["sSortDir_0"]; // asc or desc
					if (sortDirection == "asc")
							filteredClientesTransportistas = filteredClientesTransportistas.OrderBy(orderingFunction);
					else
							filteredClientesTransportistas = filteredClientesTransportistas.OrderByDescending(orderingFunction);

					var displayedClientesTransportistas = filteredClientesTransportistas.Skip(param.iDisplayStart).Take(param.iDisplayLength);
					var result = from c in displayedClientesTransportistas select new[] { c.IdCliente.ToString(), c.DescCLiente, c.IdTransportista.ToString(), c.DescTransportista };
					return Json(new
					{
							sEcho = param.sEcho,
							iTotalRecords = allClientesTransportistas.Count(),
							iTotalDisplayRecords = filteredClientesTransportistas.Count(),
							aaData = result
					},
					JsonRequestBehavior.AllowGet);
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

			[CustomAuthorize(1)] //Admin
			public ActionResult SwitchView()
				{
					TempData["TransportistaOrigenDDLId"] = new SelectList(ObtenerTransportistasOrigenDDLId(), "Value", "Text");
					TempData["TransportistaDestinoDDLId"] = new SelectList(ObtenerTransportistasDestinoDDLId(), "Value", "Text");

					return PartialView("~/Views/ClientesTransportistas/_SwitchTransportistaPartial.cshtml");
				}

			private List<SelectListItem> ObtenerTransportistasOrigenDDLId()
			{
				if (Session["TransportistaOrigenDDLId"] != null)
				{
					return (List<SelectListItem>)Session["TransportistaOrigenDDLId"];
				}
				else
				{
					List<SelectListItem> OutTransportistas = new List<SelectListItem>();
					ApiAccess Api = new ApiAccess();

					UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
					var TransportistasApi = Api.GetTransportistas(Usd.Token);

					foreach (Transportista tr in TransportistasApi)
					{
						OutTransportistas.Add(new SelectListItem() { Text = tr.DescTransportista, Value = tr.IdTransportista.ToString() });
					}
					Session["TransportistaOrigenDDLId"] = OutTransportistas;
					return OutTransportistas;
				}
			}

			private List<SelectListItem> ObtenerTransportistasDestinoDDLId()
			{
				if (Session["TransportistaDestinoDDLId"] != null)
				{
					return (List<SelectListItem>)Session["TransportistaDestinoDDLId"];
				}
				else
				{
					List<SelectListItem> OutTransportistas = new List<SelectListItem>();
					ApiAccess Api = new ApiAccess();

					UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
					var TransportistasApi = Api.GetTransportistas(Usd.Token);

					foreach (Transportista tr in TransportistasApi)
					{
						OutTransportistas.Add(new SelectListItem() { Text = tr.DescTransportista, Value = tr.IdTransportista.ToString() });
					}
					Session["TransportistaDestinoDDLId"] = OutTransportistas;
					return OutTransportistas;
				}
			}

			[HttpPost]
			[CustomAuthorize(1)] //Admin
			public ActionResult Switch(SwitchTransportista Switch)
			{
				ApiAccess Api = new ApiAccess();

				UsuarioSessionData Usd = (UsuarioSessionData)Session["UsuarioSessionData"];
				Api.SwitchTransportistas(Usd.Token, Switch.IdTransportistaOrigen, Switch.IdTransportistaDestino);

				ModelState.Clear();
				Session.Remove("ClientesTransportistsListData");
				return RedirectToAction("Listado", "ClientesTransportistas");
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
			}
}