using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineBFP.Application.User.Models
{
    public class LoginInput
    {
        [Required, EmailAddress, MaxLength(100)]
        [JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        [JsonPropertyName("contrasenia")]
        public string Contrasenia { get; set; } = string.Empty;
    }
}
