using Bodega.SolProyectoWeb.Entidades.Core;
using Bodega.SolProyectoWeb.LogicaNegocio.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bodega.SolProyectoWeb.ClienteWeb.Controllers
{
    public class CambiarContrasenaController : Controller
    {
        // GET: CambiarConstrasena
        public ActionResult Index()
        {
            Usuario usuario = new Usuario();
            return View(usuario);
        }

        // POST: CambiarContrasena/CargarUsuario
        [HttpPost]
        public ActionResult CargarUsuario(string dni)
        {
            if (!string.IsNullOrWhiteSpace(dni)) // Verifica si el parámetro tiene un valor
            {
                Usuario usuario = new UsuarioLN().BuscarUsuarioPorDni(dni); // Usa el valor del id_usuario

                if (usuario != null)
                {
                    return View("Index", usuario);
                }
                else
                {
                    TempData["Message"] = "No se encontró un personal que coincida con el DNI ingresado.";
                    TempData["MessageType"] = "error";  // Tipo de mensaje: error
                    return View("Index", new Usuario());
                }
            }
            else
            {
                TempData["Message"] = "El DNI ingresado no es valido.";
                TempData["MessageType"] = "warning";  // Tipo de mensaje: error
                return View("Index", new Usuario());
            }
        }

        // POST: CambiarContrasena
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarContrasena(int? id_usuario, string nuevaContrasena)
        {


            try
            {
                System.Diagnostics.Debug.WriteLine($"ID de usuario recibido: {id_usuario.Value}");
                System.Diagnostics.Debug.WriteLine($"newpassword de usuario recibido: {nuevaContrasena}");

                if (!id_usuario.HasValue)
                {
                    TempData["Message"] = "Debe ingresar un ID de usuario.";
                    TempData["MessageType"] = "warning";  // Tipo de mensaje: warning
                    return View("Index", new Usuario());
                }

                Usuario usuario = new UsuarioLN().BuscarUsuario(id_usuario.Value);

                if (usuario == null)
                {
                    TempData["Message"] = "No se encontró un personal que coincida con el DNI ingresado.";
                    TempData["MessageType"] = "error";  // Tipo de mensaje: error
                    return View("Index", new Usuario());
                }

                if (string.IsNullOrWhiteSpace(nuevaContrasena))
                {
                    TempData["Message"] = "Debe ingresar una nueva contraseña.";
                    TempData["MessageType"] = "info";  // Tipo de mensaje: info
                    return View("Index", usuario);
                }

                nuevaContrasena = LoginController.ConvertirSha256(nuevaContrasena);
                new UsuarioLN().CambiarContrasena(id_usuario.Value, nuevaContrasena);

                TempData["Message"] = "El cambio de contraseña se realizo con éxito.";
                TempData["MessageType"] = "success";  // Tipo de mensaje: success
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error al cambiar la contraseña del usuario: " + ex.Message;
                TempData["MessageType"] = "error";  // Tipo de mensaje: error
                return View("Index", new Usuario());
            }
        }

    }
}
