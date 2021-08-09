using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebLogistica.Models
{
	public class Estadisticas
	{
		public int Estadistica1 { get; set; }
		public int Estadistica2 { get; set; }
		public int Estadistica3 { get; set; }
	}

	public class EnviosHomeData
	{
		public Estadisticas _Estadisticas { get; set; }
	}
}