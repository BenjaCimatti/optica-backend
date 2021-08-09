using System;
using System.ComponentModel.DataAnnotations;


namespace ApiLogistica.Dominios
{
	public class ConfirmacionEnvioRequest
	{
		[Required]
		public int IdEnvio { get; set; }

		[Required]
		public double? GeoLatitud { get; set; }

		[Required]
		public double? GeoLongitud { get; set; }
	}

	public class InformacionEnvioRequest
	{
		[Required]
		public int IdEnvio { get; set; }
		[Required]
		public double? GeoLatitud { get; set; }
		[Required]
		public double? GeoLongitud { get; set; }
		[Required]
		public string Observaciones { get; set; }
	}

	public class NuevoEnvioRequest
	{
		[Required]
		public int IdCliente { get; set; }

		[Required]
		public int IdTransportista { get; set; }

		[Required]
		public string Observaciones { get; set; }
	}
}