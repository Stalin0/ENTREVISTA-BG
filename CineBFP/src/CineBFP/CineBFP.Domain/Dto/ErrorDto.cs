using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineBFP.Domain.Dto
{
    public class Error
    {
        /// <summary>
        /// Código http.
        /// </summary>
        /// <example>400</example>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// Mensaje de error.
        /// </summary>
        /// <example>Error Aplicativo</example>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
