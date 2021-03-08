using Microsoft.Owin.Security;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Axosnet.Helpers;
using Microsoft.Owin.Security.Infrastructure;
using System.Collections.Concurrent;
using Axosnet.Models;
using Axosnet.BD;
using System.Data;
using System.Security.Claims;
using System.Web.Http.Cors;

namespace Axosnet.Providers
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            SetKeysApp(context);
            context.Validated();
        }

        private void SetKeysApp(OAuthValidateClientAuthenticationContext context)
        {
            string value = string.Empty;
            KeyValuePair<string, string[]> key_value = new KeyValuePair<string, string[]>();

            List<string> keys = new List<string> { "uuid", "Modelo", "Plataforma", "VersionSO", "Latitud", "Longitud" };
            foreach (string key in keys)
            {
                key_value = context.Parameters.Where(f => f.Key == key).FirstOrDefault();
                value = key_value.Key != null ? key_value.Value.FirstOrDefault() : "";

                context.OwinContext.Set<string>(key, value);
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            string clientAddress = HttpContext.Current.Request.UserHostAddress;

            if (context.UserName.IsValidEmail())
            {
                string uuid = context.OwinContext.Get<string>("uuid");
                var res = validaUsuario(context.UserName, context.Password, clientAddress);

                Usuario usuario = res.Objecto as Usuario;
                if (usuario == null) {
                    context.SetError("Error", res.mensaje);
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("Id_usuario", usuario.Id_usuario.ToString()));
                identity.AddClaim(new Claim("username", usuario.username.ToString()));
                identity.AddClaim(new Claim("Token", usuario.token));

                AuthenticationProperties props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {"mensaje",         res.mensaje},
                    {"nombre",          $"{usuario.nombre}"},
                    {"apellidoPaterno", $"{usuario.apellidoPaterno}"},
                    {"apellidoMaterno", $"{usuario.apellidoMaterno}"},
                });

                AuthenticationTicket ticket = new AuthenticationTicket(identity, props);
                context.Validated(ticket);
            }
        }

        private Resultado validaUsuario(string userName, string password, string clientAddress)
        {
            Resultado resultado = new Resultado();
            Usuario usuario = new Usuario();

            string token = AuthToken.RandomString();

            DataTable dt = new DataTable();
            using (var proc = new Cls_BD.Cls_Coneccion())
            {
                proc.SetCommand("admin.SP_UsuarioCON ");
                proc.CreateParameter("@username", userName);
                proc.CreateParameter("@token", token);
                dt = proc.getDataTable();
            }

            if (dt.Rows.Count > 0)
            {
                usuario = dt.AsEnumerable().Select(x => new Usuario
                {
                    Id_usuario = x.Field<int>("Id_usuario"),
                    username = x.Field<string>("username"),
                    password = x.Field<string>("password"),
                    nombre = x.Field<string>("nombre"),
                    apellidoPaterno = x.Field<string>("apellidoPaterno"),
                    apellidoMaterno = x.Field<string>("apellidoMaterno")
                }).FirstOrDefault();
            }
            else {
                usuario = null;
            }

            if (usuario == null)
            {
                resultado.mensaje = "Error al ingresar, revisa tu usuario y contraseña.";
            }
            else
            {
                if (Helpers.Hashing.validatePassword(password, usuario.password))
                {
                    usuario.token = token;
                    resultado.mensaje = "Acceso correcto.";
                }
                else
                {
                    resultado.mensaje = "Error al ingresar, revisa tu usuario y contraseña.";
                }
            }       

            resultado.exito = bool.Parse((usuario != null ? "true" : "false").ToString());
            resultado.Objecto = usuario;
            return resultado;
        }

    }

    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();

            // maybe only create a handle the first time, then re-use for same client
            // copy properties and set the desired lifetime of refresh token
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddHours(1)
            };
            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            //_refreshTokens.TryAdd(guid, context.Ticket);
            _refreshTokens.TryAdd(guid, refreshTokenTicket);

            // consider storing only the hash of the handle
            context.SetToken(guid);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}