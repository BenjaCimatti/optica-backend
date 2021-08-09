using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebLogistica.Models
{
	public class SwitchTransportista
	{
		[Required()]
		[Display(Name = "Transportista Origen")]
		public int IdTransportistaOrigen { get; set; }

		[Required()]
		[Display(Name = "Transportista Destino")]
		public int IdTransportistaDestino { get; set; }
	}
}