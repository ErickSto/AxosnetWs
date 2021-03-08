using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using Axosnet.BD;
using System.Data;

namespace Axosnet.Providers
{
    public class AuthToken : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {

            ClaimsPrincipal principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
            int Id_usuario = 0;

            string token = ObtieneValorDesdeToken("Token", principal);
            if (int.TryParse(ObtieneIdUsuarioDesdeToken("Id_usuario", principal), out Id_usuario))
            {
                DataTable dt = new DataTable();
                using (var proc = new Cls_BD.Cls_Coneccion())
                {
                    proc.SetCommand("admin.SP_TokenCON");
                    proc.CreateParameter("@Id_usuario", Id_usuario);
                    proc.CreateParameter("@token", token);
                    dt = proc.getDataTable();
                }
                if (dt.Rows.Count <= 0)
                {
                    actionContext.Response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Content = new StringContent("{\"Exito\":0, \"Err_Mensaje\":\"Su sesión ha expirado.\", \"Logout\":1}", System.Text.Encoding.UTF8, "application/json")
                    };
                    return false;
                }

            }
            else {
                actionContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("{\"Exito\":0, \"Err_Mensaje\":\"Inicie sesión.\", \"Logout\":1}", System.Text.Encoding.UTF8, "application/json")
                };
                return false;
            }

            return base.IsAuthorized(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext.Response.StatusCode != HttpStatusCode.Unauthorized)
            {
                actionContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("{\"mensaje\":\"Sesión caducada\"}", System.Text.Encoding.UTF8, "application/json")
                };
            }
        }

        public static string ObtieneValorDesdeToken(string Tipo, ClaimsPrincipal principal)
        {
            Claim valor = principal.Claims.Where(c => c.Type == Tipo).FirstOrDefault();
            if (valor == null)
                throw new UnauthorizedAccessException("Inicie sesión.");

            return valor.Value;
        }


        public static string ObtieneIdUsuarioDesdeToken(string Tipo, ClaimsPrincipal principal)
        {
            Claim valor = principal.Claims.Where(c => c.Type == Tipo).FirstOrDefault();
            if (valor == null)
                return "";

            return valor.Value;
        }

        public static string RandomString(int length = 4)
        {
            System.Random r = new System.Random();
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length).Select(x => pool[r.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }

    }
}