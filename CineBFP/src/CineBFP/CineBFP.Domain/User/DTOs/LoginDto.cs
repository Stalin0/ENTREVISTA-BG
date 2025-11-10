using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineBFP.Domain.User.DTOs
{
    public class LoginDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        [JsonPropertyName("access_token")]
        public string? Token { get; set; }
    }
}
