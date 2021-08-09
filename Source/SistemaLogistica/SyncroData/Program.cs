using com.Sistema.Logistica;
using SistemaLogistica.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroData
{
	class Program
	{
		static void Main(string[] args)
		{
			bool res = false;
		
			CustomLogging.Initialize();
			DataAccess dal = new DataAccess();

			if (args.Length > 0 && args[0].ToString().ToUpper() == "REFRESH")
			{
				res = dal.RestartDataClienteTransportistaRel();
			}

			res = dal.SyncroTransportistas();
			res = dal.SyncroClientes();
			res = dal.SyncroRelTransportistaClientes();
		}
	}
}
