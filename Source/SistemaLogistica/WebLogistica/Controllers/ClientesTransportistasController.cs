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

      private IList<ClientesTransportistasTable> GetClientesTransportistas()
      {
          List<ClientesTransportistasTable> _ClientesTransportistasle = new List<ClientesTransportistasTable>();

          if (Session["ClientesTransportistsListData"] != null)
          {
							_ClientesTransportistasle = (List<ClientesTransportistasTable>)Session["ClientesTransportistsListData"];
          }
          else
          {
              if (Session["UsuarioSessionData"] != null)
              {
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

								Session["ClientesTransportistsListData"] = _ClientesTransportistasle;
							}
          }

          return _ClientesTransportistasle;
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