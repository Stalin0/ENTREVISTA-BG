using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineBFP.Application.User.Models
{
    public class UserInput
    {
        [Required, MaxLength(100)]
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        [JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        [JsonPropertyName("contrasenia")]
        public string Contrasenia { get; set; } = string.Empty;

        public int? IdRol { get; set; }

        [Required, MaxLength(100)]
        [JsonPropertyName("usuarioCreacion")]
        public string UsuarioCreacion { get; set; } = string.Empty;
    }
}
