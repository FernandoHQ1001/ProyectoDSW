using Bodega.SolProyectoWeb.Entidades.Base;
using Bodega.SolProyectoWeb.Entidades.Core;
using Bodega.SolProyectoWeb.AccesoDatos.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Bodega.SolProyectoWeb.LogicaNegocio.Core
{
    public class UsuarioLN : BaseLN
    {
        public void InsertarUsuario(Usuario usuario)
        {
            if (usuario.dni.Length != 8 || !usuario.dni.All(char.IsDigit))
            {
                throw new ArgumentException("El DNI debe tener exactamente 8 dígitos numéricos.");
            }
            new UsuarioDA().InsertarUsuario(usuario);
        }

        public void ModificarUsuario(Usuario usuario)
        {
            if (usuario.dni.Length != 8 || !usuario.dni.All(char.IsDigit))
            {
                throw new ArgumentException("El DNI debe tener exactamente 8 dígitos numéricos.");
            }
            new UsuarioDA().ModificarUsuario(usuario);
        }

        public void EliminarUsuario(int idUsuario)
        {
            new UsuarioDA().EliminarUsuario(idUsuario);
        }

        public Usuario BuscarUsuario(int id)
        {
            return new UsuarioDA().BuscarUsuario(id);
        }

        public bool VerificarUsuarioExistente(string dni)
        {
            return new UsuarioDA().VerificarUsuarioExistente(dni);
        }

        public Usuario BuscarUsuarioPorDni(string dni)
        {
            return new UsuarioDA().BuscarUsuarioPorDni(dni);
        }

        public Usuario ValidarUsuario(string correo, string contrasena)
        {
            return new UsuarioDA().ValidarUsuario(correo, contrasena);
        }

        public void CambiarContrasena(int id_usuario, string nuevaContrasena)
        {
            new UsuarioDA().CambiarContrasena(id_usuario, nuevaContrasena);
        }
    }
}
