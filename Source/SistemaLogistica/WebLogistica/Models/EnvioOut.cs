using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebLogistica.Models
{
	public class EnvioOut
	{
		public int IdEnvio { get; set; }
		public string DescCliente { get; set; }
		public string DescTransportista { get; set; }
		public DateTime? FechaCarga { get; set; }
		public DateTime? FechaEnvio { get; set; }
		public string DescEstado { get; set; }
		public string GeoLatitud { get; set; }
		public string GeoLongitud { get; set; }
		public string Observaciones { get; set; }
	}
}