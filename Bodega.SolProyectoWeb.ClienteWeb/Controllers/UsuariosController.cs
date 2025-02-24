using Bodega.SolProyectoWeb.LogicaNegocio.Core;
using Bodega.SolProyectoWeb.Entidades.Base;
using Bodega.SolProyectoWeb.Entidades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Bodega.SolProyectoWeb.AccesoDatos.Core;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Web.Security;

namespace Bodega.SolProyectoWeb.ClienteWeb.Controllers
{
    [AuthorizeCargo("Administrador")]
    public class UsuariosController : Controller
    {
        //string RutaApi = ConfigurationManager.AppSettings["ApiBaseUrl"];
        string RutaApi = "https://localhost:44386/api/"; //define la ruta del web api 
        string jsonMediaType = "application/json"; // define el tipo de dat 

        // GET: MantenerUsuario
        public ActionResult Index()
        {
            return View();
        }

        // GET: Usuarios/Create 
        public ActionResult RegistrarUsuario()
        {
            Usuario usuario = new Usuario();// se crea uns instancia de la clase usuario
            return View(usuario);
        }

        // POST: Usuarios/Create 
        [HttpPost]
        public ActionResult RegistrarUsuario(Usuario usuario)
        {
            System.Diagnostics.Debug.WriteLine("Usuario registrado (controlador): " + usuario.nombre + " - DNI: " + usuario.dni);
            System.Diagnostics.Debug.WriteLine("Usuario registrado (controlador): " + usuario.id_usuario + " - DNI: " + usuario.apellido);
            string controladora = "Usuarios";
            try
            {

                using (WebClient cliente = new WebClient())
                {
                    cliente.Headers.Clear(); // Borra los encabezados anteriores

                    //establece el tipo de dato de tranferencia 
                    cliente.Headers[HttpRequestHeader.ContentType] = "application/json";
                    System.Diagnostics.Debug.WriteLine("Paso el header");

                    // Tipo de codificación para caracteres especiales
                    cliente.Encoding = UTF8Encoding.UTF8;
                    System.Diagnostics.Debug.WriteLine("Paso el UTF");

                    // Convierte el objeto de tipo Usuario a una cadena JSON
                    var usuarioJson = JsonConvert.SerializeObject(usuario);
                    System.Diagnostics.Debug.WriteLine("JSON a enviar: " + usuarioJson);

                    string rutacompleta = RutaApi + controladora;

                    System.Diagnostics.Debug.WriteLine(rutacompleta);

                    var resultado = cliente.UploadString(new Uri(rutacompleta), "POST", usuarioJson);

                    System.Diagnostics.Debug.WriteLine("Paso el el var resultado");
                }
                System.Diagnostics.Debug.WriteLine("Paso el using" );
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al crear el usuario: {ex.Message}";
                return View();
            }
        }

        // GET: Usuarios/Edit/5 
        public ActionResult ModificarUsuario()
        {
            string controladora = "Usuarios";
            string metodo = "GetUserId";

            Usuario users = new Usuario();
            /*
            using (WebClient cliente = new WebClient())
            {
                cliente.Headers.Clear(); // Borra los encabezados anteriores

                // Establece el tipo de dato de transferencia
                cliente.Headers[HttpRequestHeader.ContentType] = "application/json"; 

                // Tipo de decodificador para caracteres especiales
                cliente.Encoding = UTF8Encoding.UTF8;

                // Construye la URL completa para realizar la solicitud GET a la API
                string rutacompleta = RutaApi + controladora + "?IdUsuario=" + id;

                try
                {
                    // Ejecuta la solicitud GET a la API y recibe la respuesta
                    var data = cliente.DownloadString(new Uri(rutacompleta));

                    // Convierte los datos JSON traídos por la API en un objeto Usuario
                    users = JsonConvert.DeserializeObject<Usuario>(data);
                }
                catch (Exception ex)
                {
                    // Maneja cualquier error que pueda ocurrir al hacer la solicitud
                    ViewBag.ErrorMessage = $"Error al obtener el usuario: {ex.Message}";
                    return View(); // Retorna la vista con un mensaje de error
                }
            }

            */
            return View(users);
        }

        // POST: Usuarios/Edit/5 
        [HttpPost]
        // [HttpPut] 
        public ActionResult ModificarUsuario( Usuario usuario)
        {
            System.Diagnostics.Debug.WriteLine("Usuario a modificar (controlador): " + usuario.nombre + " - DNI: " + usuario.dni);
            System.Diagnostics.Debug.WriteLine("Usuario a modificar (controlador): " + usuario.id_usuario + " - Apellido: " + usuario.apellido);

            string controladora = "Usuarios";

            string metodo = "Put";
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(RutaApi);

                    string rutacompleta = $"{RutaApi}{controladora}?IdUsuario={usuario.id_usuario}"; // Define la ruta completa para la solicitud PUT
                    System.Diagnostics.Debug.WriteLine(rutacompleta);

                    // Realiza la solicitud PUT a la API enviando el objeto usuario como JSON
                    var putTask = client.PutAsJsonAsync(rutacompleta, usuario); // Usa el objeto usuario en lugar de collection

                    putTask.Wait();

                    var result = putTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return RedirectToAction("Index");
                // return View(); 
            }
        }

        // POST: Usuarios/BuscarUsuario
        [HttpPost]
        public ActionResult BuscarUsuarioModificar(string dni)
        {
            string controladora = "Usuarios";
            Usuario usuario = null;

            using (WebClient cliente = new WebClient())
            {
                cliente.Headers.Clear();
                cliente.Headers[HttpRequestHeader.ContentType] = "application/json";
                cliente.Encoding = UTF8Encoding.UTF8;

                string rutacompleta = $"{RutaApi}{controladora}?dni={dni}"; 

                try
                {
                    var data = cliente.DownloadString(new Uri(rutacompleta));
                    usuario = JsonConvert.DeserializeObject<Usuario>(data);

                    if (usuario == null)
                    {
                        TempData["Error"] = "No se encontró un usuario con el DNI especificado.";
                        return RedirectToAction("ModificarUsuario");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al obtener el usuario: {ex.Message}";
                    return RedirectToAction("ModificarUsuario");
                }
            }

            return View("ModificarUsuario", usuario);
        }


        // GET: Usuarios/Delete/5 
        public ActionResult EliminarUsuario()
        {
            string controladora = "Usuarios";
            Usuario users = new Usuario();

            /*
            using (WebClient cliente = new WebClient())
            {
                cliente.Headers[HttpRequestHeader.ContentType] = "application/json"; // Establece el tipo de contenido como JSON
                cliente.Encoding = UTF8Encoding.UTF8;

                // Construye la URL para obtener los detalles del usuario
                string url = $"{RutaApi}{controladora}/GetUserId?IdUsuario={id}";

                // Realiza la solicitud GET para obtener los datos del usuario
                var data = cliente.DownloadString(url);

                // Deserializa el JSON recibido en un objeto Usuario
                usuario = JsonConvert.DeserializeObject<Usuario>(data);
            }
            */

            return View(users);
        }

        // POST: Usuarios/Delete/5 
        [HttpPost]
        public ActionResult EliminarUsuario( Usuario usuario)
        {
            System.Diagnostics.Debug.WriteLine("Usuario a eliminar (controlador): " + usuario.nombre + " - DNI: " + usuario.dni);
            System.Diagnostics.Debug.WriteLine("Usuario a eliminar (controlador): " + usuario.id_usuario + " - Apellido: " + usuario.apellido);

            string controladora = "Usuarios";

            try
            {
                using (WebClient cliente = new WebClient())
                {
                    cliente.Headers[HttpRequestHeader.ContentType] = "application/json";  // Establece el tipo de contenido como JSON

                    // Construye la URL para la solicitud DELETE
                    string url = $"{RutaApi}{controladora}/{usuario.id_usuario}";
                    System.Diagnostics.Debug.WriteLine(url);


                    // Realiza la solicitud DELETE a la API para eliminar   al usuario
                    cliente.UploadString(url, "DELETE", string.Empty);  // Utiliza el método "DELETE" con cuerpo vacío

                    // Si la solicitud fue exitosa, redirige con un mensaje de éxito
                    TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                // Log o manejo de errores según sea necesario
                ModelState.AddModelError("", $"Error al eliminar el usuario: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        // POST: Usuarios/BuscarUsuarioEliminar
        [HttpPost]
        public ActionResult BuscarUsuarioEliminar(string dni)
        {
            string controladora = "Usuarios";
            Usuario usuario = null;

            using (WebClient cliente = new WebClient())
            {
                cliente.Headers.Clear();
                cliente.Headers[HttpRequestHeader.ContentType] = "application/json";
                cliente.Encoding = UTF8Encoding.UTF8;

                string rutacompleta = $"{RutaApi}{controladora}?dni={dni}";

                try
                {
                    var data = cliente.DownloadString(new Uri(rutacompleta));
                    System.Diagnostics.Debug.WriteLine("Data: " + data);
                    usuario = JsonConvert.DeserializeObject<Usuario>(data);
                    System.Diagnostics.Debug.WriteLine("id usuario deserializado: " + usuario.id_usuario);


                    if (usuario == null)
                    {
                        TempData["Error"] = "No se encontró un usuario con el DNI especificado.";
                        return RedirectToAction("EliminarUsuario");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al obtener el usuario: {ex.Message}";
                    return RedirectToAction("EliminarUsuario");
                }
            }

            return View("EliminarUsuario", usuario);
        }


    }
}
