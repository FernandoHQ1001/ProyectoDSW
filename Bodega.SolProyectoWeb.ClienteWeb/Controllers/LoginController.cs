using Bodega.SolProyectoWeb.Entidades.Core;
using Bodega.SolProyectoWeb.LogicaNegocio.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Bodega.SolProyectoWeb.ClienteWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login(string mensaje = null)
        {
            if (mensaje != null)
            {
                ViewData["Mensaje"] = "No tienes permiso para acceder a esa sección.";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario usuario)
        {
            usuario.contrasena = ConvertirSha256(usuario.contrasena);

            // Verificar si el correo o la contraseña están vacíos
            if (string.IsNullOrEmpty(usuario.correo) || string.IsNullOrEmpty(usuario.contrasena))
            {
                ViewData["Mensaje"] = "El correo y la contraseña son obligatorios.";
                return View();
            }

            UsuarioLN usuarioLN = new UsuarioLN();
            Usuario usuarioValido = usuarioLN.ValidarUsuario(usuario.correo, usuario.contrasena);

            if (usuarioValido != null)
            {
                // Credenciales válidas, redirigir a la página de inicio
                Session["Usuario"] = usuarioValido;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Credenciales incorrectas, mostrar mensaje de error
                ViewData["Mensaje"] = "Usuario no encontrado.";
                return View();
            }
        }

        // para cerrar sesión
        public ActionResult Logout()
        {
            // Destruir la sesión del usuario
            Session.Abandon();

            // limpiar las cookies 
            Response.Cookies.Clear();

            // Redirigir al login
            return RedirectToAction("Login", "Login");
        }


        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
    }
}
