using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Axosnet.Models
{
    public class Usuario
    {
        public int Id_usuario { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string token { get; set; }

    }
}