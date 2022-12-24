using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AterrizajesApp.Models
{
    public class Aterrizajes
    {
        public int Id { get; set; }
        public DateTime Tiempo { get; set; }
        public string Destino { get; set; } = "";
        
        public string? Vuelo { get; set; }
        public string? Puerta { get; set; }
        public string? Status { get; set; }
    }
}
