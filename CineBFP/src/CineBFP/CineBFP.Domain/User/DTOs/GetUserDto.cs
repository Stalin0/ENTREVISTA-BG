using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain.User.DTOs
{
    public class GetUserDto
    {
        public int IdUsuario { get; set; }
        [Required, MaxLength(100)]
        public string? FullName { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string? Correo { get; set; }
        public string? Activo {  get; set; }
    }
}
