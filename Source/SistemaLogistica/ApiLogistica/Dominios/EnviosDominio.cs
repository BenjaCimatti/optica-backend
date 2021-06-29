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
}