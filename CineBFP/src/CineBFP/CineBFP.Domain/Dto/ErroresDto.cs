using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CineBFP.Domain.Dto
{
    public class ErroresDto
    {
        /// <summary>
        /// Código http.
        /// </summary>
        /// <example>400</example>
        public string code { get; set; }

        /// <summary>
        /// Identificador de trazabilidad.
        /// </summary>
        /// <example>6ee1b7a7bcd2c7c9</example>
        public string traceid { get; set; }

        /// <summary>
        /// Mensaje de error.
        /// </summary>
        /// <example>Error Aplicativo</example>
        public string message { get; set; }

        public List<Error> errors { get; set; }

    }
}

