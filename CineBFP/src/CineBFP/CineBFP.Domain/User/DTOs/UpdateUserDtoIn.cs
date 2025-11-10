using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineBFP.Domain.User.DTOs
{
    public class UpdateUserDtoIn
    {
        [JsonPropertyName("idUsuario")]
        public int IdUsuario { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;
        public bool Activo { get; set; }

        public string UsuarioModificacion { get; set; } = string.Empty;
    }
}
