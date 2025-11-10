using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineBFP.Application.User.Models
{
    public class UpdateUserInput
    {
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;
        [JsonPropertyName("activo")]
        public bool Activo { get; set; }
    }
}
