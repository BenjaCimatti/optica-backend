using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebLogistica.Models
{
	public class EnvioIn
	{
		[Required()]
		public int IdCliente { get; set; }

		[Display(Name = "Observaciones")]
		[StringLength(250)]
		public string Observaciones { get; set; }
	}

	public class EnvioEdit
	{
		[Required()]
		public int IdEnvio { get; set; }

		[Display(Name = "Observaciones")]
		[StringLength(250)]
		public string Observaciones { get; set; }
	}
}