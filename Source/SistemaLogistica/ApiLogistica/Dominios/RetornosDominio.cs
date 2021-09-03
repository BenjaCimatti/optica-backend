using System;
using System.ComponentModel.DataAnnotations;


namespace ApiLogistica.Dominios
{
	public class NuevoRetornoRequest
	{
		[Required]
		public int IdCliente { get; set; }

		[Required]
		public double? GeoLatitud { get; set; }

		[Required]
		public double? GeoLongitud { get; set; }
	}
}