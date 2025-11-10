using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain.User.Entities
{
    public class GetUserEntities
    {
        public int IdUsuario { get; set; }
        public string? FullName { get; set; }

        public string? Correo { get; set; }
        public bool? Activo { get; set; }
    }
}
