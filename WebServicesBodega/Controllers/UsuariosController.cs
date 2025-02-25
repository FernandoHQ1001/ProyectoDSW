using Bodega.SolProyectoWeb.Entidades.Core;
using Bodega.SolProyectoWeb.LogicaNegocio.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebServicesBodega.Controllers
{
    public class UsuariosController : ApiController
    {
        // GET: api/Usuarios
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Usuarios/5
        [HttpGet]
        public IHttpActionResult GetUserId([FromUri] int IdUsuario)
        {
            if (IdUsuario <= 0)
            {
                return BadRequest("el Id debe ser mayor que 0");
            }

            try
            {

                Usuario usu = new Usuario();
                UsuarioLN usuario = new UsuarioLN();
                usu = usuario.BuscarUsuario(IdUsuario);
                return Ok(usu);
            }
            catch (Exception ex)
            {
                string innerException = (ex.InnerException == null) ? "" : ex.InnerException.ToString();
                throw;
            }
        }


        // GET: api/Usuarios/5
        [HttpGet]
        public IHttpActionResult GetUserDNI([FromUri] string dni)
        {
            if (dni == null)
            {
                return BadRequest("El DNI no es correcto");
            }

            try
            {
                Usuario usu = new Usuario();
                UsuarioLN usuario = new UsuarioLN();
                usu = usuario.BuscarUsuarioPorDni(dni);
                return Ok(usu);
            }
            catch (Exception ex)
            {
                string innerException = (ex.InnerException == null) ? "" : ex.InnerException.ToString();
                throw;
            }
        }

        // POST: api/Usuarios -- Para insertar informacion
        [HttpPost]
        public IHttpActionResult Post([FromBody] Usuario value)
        {
            // Validar que el objeto value no sea nulo
            if (value == null)
            {
                return BadRequest("El objeto Usuario no puede ser nulo.");
            }


            if (value.contrasena == null)
            {
                return BadRequest("ClaveTxt es nulo");
            }
            if (value.nombre == null)
            {
                return BadRequest("Nombres es nulo");
            }
            if (value.apellido == null)
            {
                return BadRequest("Apellidos es nulo");
            }
            if (value.telefono == null)
            {
                return BadRequest("Telefono es nulo");
            }
            if (value.dni == null)
            {
                return BadRequest("DNI es nulo");
            }
            Console.WriteLine($"DNI recibido: {value.dni}");
            if (value.correo == null)
            {
                return BadRequest("Correo es nulo");
            }
            if (value.cargo == null)
            {
                return BadRequest("Cargo es nulo");
            }

            UsuarioLN usuario = new UsuarioLN();
            usuario.InsertarUsuario(value);
            return Ok(value);
        }

        // PUT: api/Usuarios/5 -- Para modificar informacion
        [HttpPut]
        public IHttpActionResult Put(int IdUsuario, [FromBody] Usuario value)
        {

            // Validar que el objeto no sea nulo
            if (value == null)
            {
                return BadRequest("El objeto de usuario no puede ser nulo.");
            }


            try
            {
                Usuario usu = new Usuario();
                UsuarioLN usuario = new UsuarioLN();
                usuario.ModificarUsuario(IdUsuario,value);

                if (value == null)
                {
                    // Si no se encuentra el usuario para actualizar
                    return NotFound();
                }

                // Retornar el usuario actualizado
                return Ok(value);
            }
            catch (Exception ex)
            {
                // Capturar errores y retornar una respuesta de error interno
                string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;

                return InternalServerError(new Exception($"Error al actualizar el usuario: {ex.Message}. Detalle: {innerException}"));
            }
        }

        // DELETE: api/Usuarios/5 -- Para borrar informacion
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                UsuarioLN usuarioLN = new UsuarioLN();
                usuarioLN.EliminarUsuario(id);


                return StatusCode(HttpStatusCode.NoContent); // Devuelve 204 si se elimina correctamente
            }
            catch (Exception ex)
            {
                string innerException = ex.InnerException == null ? "" : ex.InnerException.ToString();
                // Loggear el error (opcional)
                return InternalServerError(new Exception($"Error al eliminar usuario: {ex.Message}. {innerException}"));
            }
        }
    }
}
