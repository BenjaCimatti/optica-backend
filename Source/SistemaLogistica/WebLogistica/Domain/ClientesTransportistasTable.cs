using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebLogistica.Domain
{
	public class ClientesTransportistasTable
	{
		public int IdCliente { get; set; }
		public string DescCLiente { get; set; }
		public int IdTransportista { get; set; }
		public string DescTransportista { get; set; }
	}
}