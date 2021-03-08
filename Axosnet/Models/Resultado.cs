using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Axosnet.Models
{
    public class Resultado
    {
        public bool exito { get; set; }
        public string mensaje { get; set; }
        public dynamic Objecto { get; set; }
    }
}