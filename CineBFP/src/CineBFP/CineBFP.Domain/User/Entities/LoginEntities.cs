using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain.User.Entities
{
    public class LoginEntities
    {
        public int IdUsuario { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasenia { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public string? UsuarioCreacion { get; set; }
    }
}
