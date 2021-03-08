using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Axosnet.Models;
using Axosnet.BD;
using System.Data;
using Axosnet.Providers;
using System.Security.Claims;
using System.Web;

namespace Axosnet.Controllers
{
    public class RecibosController : ApiController
    {

        [HttpPost]
        [Route("Recibo")]
        public IHttpActionResult GuardaRecibo(Recibo recibo)
        {
            var claims = ((ClaimsPrincipal)HttpContext.Current.User);
            int.TryParse(claims.Claims.Where(c => c.Type == "Id_usuario").FirstOrDefault().Value, out int id_Usuario);
            string folio = "";
            string msg = "";

            try
            {
                bool exito = false;
                DataTable dt = new DataTable();
                using (var proc = new Cls_BD.Cls_Coneccion())
                {
                    proc.SetCommand("admin.SP_ReciboALT");
                    proc.CreateParameter("@moneda_Id", recibo.moneda_Id);
                    proc.CreateParameter("@proveedor_Id", recibo.proveedor_Id);
                    proc.CreateParameter("@monto", recibo.monto);
                    proc.CreateParameter("@comentario", recibo.comentario);
                    proc.CreateParameter("@usuario_Id", id_Usuario);

                    dt = proc.getDataTable();
                }

                exito = bool.Parse(dt.Rows[0]["exito"].ToString());
                msg = dt.Rows[0]["msg"].ToString();
                folio = dt.Rows[0]["folio"].ToString();

                if (!exito)
                    throw new ArgumentException(msg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

            return Ok(folio);
        }

        [HttpPut]
        [Route("Recibo")]
        public IHttpActionResult EditaRecibo(Recibo recibo)
        {
            var claims = ((ClaimsPrincipal)HttpContext.Current.User);
            int.TryParse(claims.Claims.Where(c => c.Type == "Id_usuario").FirstOrDefault().Value, out int id_Usuario);
            string msg = "";
            try
            {
                bool exito = false;
                DataTable dt = new DataTable();
                using (var proc = new Cls_BD.Cls_Coneccion())
                {
                    proc.SetCommand("admin.SP_ReciboACT");
                    proc.CreateParameter("@Id_Recibo", recibo.Id_recibo);
                    proc.CreateParameter("@moneda_Id", recibo.moneda_Id);
                    proc.CreateParameter("@proveedor_Id", recibo.proveedor_Id);
                    proc.CreateParameter("@monto", recibo.monto);
                    proc.CreateParameter("@comentario", recibo.comentario);
                    proc.CreateParameter("@usuario_Id", id_Usuario);

                    dt = proc.getDataTable();
                }

                exito = bool.Parse(dt.Rows[0]["exito"].ToString());
                msg = dt.Rows[0]["msg"].ToString();

                if (!exito)
                    throw new ArgumentException(msg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

            return Ok(msg);
        }

        [HttpDelete]
        [Route("Recibo/{id}")]
        public IHttpActionResult EliminaRecibo(int id)
        {
            var claims = ((ClaimsPrincipal)HttpContext.Current.User);
            int.TryParse(claims.Claims.Where(c => c.Type == "Id_usuario").FirstOrDefault().Value, out int id_Usuario);
            string msg = "";
            try
            {
                bool exito = false;
                DataTable dt = new DataTable();
                using (var proc = new Cls_BD.Cls_Coneccion())
                {
                    proc.SetCommand("admin.SP_ReciboDEL");
                    proc.CreateParameter("@Id_Recibo", id);
                    proc.CreateParameter("@usuario_Id", id_Usuario);
                    dt = proc.getDataTable();
                }

                exito = bool.Parse(dt.Rows[0]["exito"].ToString());
                msg = dt.Rows[0]["msg"].ToString();

                if (!exito)
                    throw new ArgumentException(msg);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

            return Ok(msg);
        }

        [HttpGet]
        [Route("Recibo")]
        [AuthToken]
        public List<ReciboPag> ConsultaRecibos(DateTime? fechaInicio, DateTime? fechaFin, int moneda_Id = 0, int proveedor_Id = 0, int paginaActual = 1, int regPag = 50)
        {

            List<ReciboPag> recibos = new List<ReciboPag>();
            try
            {
                DataTable dt = new DataTable();
                using (var proc = new Cls_BD.Cls_Coneccion())
                {
                    proc.SetCommand("admin.SP_RecibosCON");
                    proc.CreateParameter("@paginaActual", paginaActual);
                    proc.CreateParameter("@regPag", regPag);
                    proc.CreateParameter("@moneda_Id", moneda_Id);
                    proc.CreateParameter("@proveedor_Id", proveedor_Id);

                    if (fechaInicio.HasValue && fechaInicio.Value != DateTime.Parse("1900-01-01"))
                        proc.CreateParameter("@fechaInicio", fechaInicio.Value);

                    if (fechaFin.HasValue && fechaFin.Value != DateTime.Parse("1900-01-01"))
                        proc.CreateParameter("@fechaFin", fechaFin.Value);

                    dt = proc.getDataTable();
                }

                recibos = dt.AsEnumerable().Select(x => new ReciboPag
                {
                    totalRegistros = x.Field<int>("totalRegistros"),
                    Id_recibo = x.Field<int>("Id_recibo"),
                    folio = x.Field<string>("folio"),
                    monto = x.Field<decimal>("monto"),
                    comentario = x.Field<string>("comentario"),
                    fechaAlta = x.Field<DateTime>("fechaAlta"),
                    moneda_Id = x.Field<int>("moneda_Id"),
                    monedaCodigo = x.Field<string>("monedaCodigo"),
                    monedaNombre = x.Field<string>("monedaNombre"),
                    proveedor_Id = x.Field<int>("proveedor_Id"),
                    proveedorNombre = x.Field<string>("proveedorNombre"),
                    creado_Id = x.Field<int>("creado_Id"),
                    usuarioCreado = x.Field<string>("usuarioCreado"),
                    modificado_Id = x.Field<int>("modificado_Id"),
                    usuarioModificado = x.Field<string>("usuarioModificado")
                }).ToList();
            }
            catch (Exception ex)
            {
                return recibos = new List<ReciboPag>();
            }
            return recibos;
        }

        [HttpGet]
        [Route("Recibo/{id}")]
        [AuthToken]
        public ReciboPag ConsultaRecibo(int id) {

            ReciboPag recibo = new ReciboPag();
            try
            {
                DataTable dt = new DataTable();
                using (var proc = new Cls_BD.Cls_Coneccion())
                {
                    proc.SetCommand("admin.SP_ReciboCON");
                    proc.CreateParameter("@Id_recibo", id);
                    dt = proc.getDataTable();
                }

                if (dt.Rows.Count <= 0)
                    throw new ArgumentException("No se encontro el registro");

                recibo = dt.AsEnumerable().Select(x => new ReciboPag
                {
                    Id_recibo = x.Field<int>("Id_recibo"),
                    folio = x.Field<string>("folio"),
                    monto = x.Field<decimal>("monto"),
                    comentario = x.Field<string>("comentario"),
                    moneda_Id = x.Field<int>("moneda_Id"),
                    monedaNombre = x.Field<string>("moneda"),
                    proveedor_Id = x.Field<int>("proveedor_Id"),
                    proveedorNombre = x.Field<string>("proveedorNombre")
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message.ToString());
            }

            return recibo;

        }


        [HttpGet]
        [Route("Proveedores")]
        [AuthToken]
        public List<Proveedor> ConsultaProveedores() {

            List<Proveedor> proveedores = new List<Proveedor>();

            try
            {
                DataTable dt = new DataTable();
                using (var proc = new Cls_BD.Cls_Coneccion())
                {
                    proc.SetCommand("admin.SP_ProveedoresCON");
                    dt = proc.getDataTable();
                }
                if (dt.Rows.Count > 0)
                    new ArgumentException("No hay datos");

                proveedores = dt.AsEnumerable().Select(x => new Proveedor
                {
                    Id_proveedor = x.Field<int>("Id_proveedor"),
                    nombre = x.Field<string>("nombre")
                }).ToList();

            }
            catch (Exception ex)
            {
                return proveedores = new List<Proveedor>();
            }

            return proveedores;
        }

        [HttpGet]
        [Route("Monedas")]
        [AuthToken]
        public List<Moneda> ConsultaMonedas()
        {

            List<Moneda> monedas = new List<Moneda>();

            try
            {
                DataTable dt = new DataTable();
                using (var proc = new Cls_BD.Cls_Coneccion())
                {
                    proc.SetCommand("admin.SP_MonedasCON");
                    dt = proc.getDataTable();
                }
                if (dt.Rows.Count > 0)
                    new ArgumentException("No hay datos");

                monedas = dt.AsEnumerable().Select(x => new Moneda
                {
                    Id_moneda = x.Field<int>("Id_moneda"),
                    codigo = x.Field<string>("codigo"),
                    nombre = x.Field<string>("nombre")
                }).ToList();

            }
            catch (Exception ex)
            {
                return monedas = new List<Moneda>();
            }

            return monedas;
        }

    }
}

