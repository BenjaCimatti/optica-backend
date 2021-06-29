using System.ComponentModel.DataAnnotations;

namespace SistemaLogistica.Dominios
{
	public class TokenRequest
	{
		[Required]
		public string NombreUsuario { get; set; }
		[Required]
		public string Clave { get; set; }
		[Required]
		public int IdOrganizacion { get; set; }
	}
}