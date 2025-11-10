using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineBFP.Domain.User.DTOs
{
    public class LoginDtoIn
    {
        [Required, EmailAddress, MaxLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Contrasenia { get; set; } = string.Empty;
    }
}
