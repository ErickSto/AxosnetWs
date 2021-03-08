using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Axosnet.Models
{
    public class Recibo
    {
        public int Id_recibo { get; set; }
        public int moneda_Id { get; set; }
        public int proveedor_Id { get; set; }
        public decimal monto { get; set; }
        public string comentario { get; set; }

    }

    public class ReciboDelete
    {
        public int Id_recibo { get; set; }
    }

    public class ReciboPag
    {

        public int totalRegistros { get; set; }
        public int Id_recibo { get; set; }
        public string folio { get; set; }
        public decimal monto { get; set; }
        public string comentario { get; set; }
        public DateTime fechaAlta { get; set; }
        public int moneda_Id { get; set; }
        public string monedaCodigo { get; set; }
        public string monedaNombre { get; set; }
        public int proveedor_Id { get; set; }
        public string proveedorNombre { get; set; }
        public int creado_Id { get; set; }
        public string usuarioCreado { get; set; }
        public int modificado_Id { get; set; }
        public string usuarioModificado { get; set; }

    }
}