using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain.User.DTOs
{
    public class UserDtoIn
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Contrasenia { get; set; } = string.Empty;

        public int? IdRol { get; set; }

        [Required, MaxLength(100)]
        public string UsuarioCreacion { get; set; } = string.Empty;
    }
}

