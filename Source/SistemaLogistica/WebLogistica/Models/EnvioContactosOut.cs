using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebLogistica.Models
{
	public class Contacto
	{
		public int IdEnvio { get; set; }
		public int IdTransportista { get; set; }
		public string DescTransportista { get; set; }
		public DateTime FechaContacto { get; set; }
		public string GeoLatitud { get; set; }
		public string GeoLongitud { get; set; }
		public string Observaciones { get; set; }
	}

	public class EnvioContactosOut
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
		public int CantContactos { get; set; }
		public List<Contacto> Contactos { get; set; }
	}
}