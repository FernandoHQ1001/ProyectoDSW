using Bodega.SolProyectoWeb.Entidades.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Bodega.SolProyectoWeb.AccesoDatos.Core
{
    public class UsuarioDA
    {
        public void InsertarUsuario(Usuario usuario)
        {
            // Codificar la contraseña antes de insertarla en la base de datos
            usuario.contrasena = ConvertirSha256(usuario.contrasena);

            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("insertarUsuario", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@apellido", usuario.apellido);
                    cmd.Parameters.AddWithValue("@telefono", usuario.telefono);
                    cmd.Parameters.AddWithValue("@dni", usuario.dni);
                    cmd.Parameters.AddWithValue("@correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@cargo", usuario.cargo);
                    cmd.Parameters.AddWithValue("@contrasena", usuario.contrasena);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void ModificarUsuario(int IdUsuario, Usuario usuario)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("modificarUsuario", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_usuario", IdUsuario);
                    cmd.Parameters.AddWithValue("@nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@apellido", usuario.apellido);
                    cmd.Parameters.AddWithValue("@telefono", usuario.telefono);
                    cmd.Parameters.AddWithValue("@dni", usuario.dni);
                    cmd.Parameters.AddWithValue("@correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@cargo", usuario.cargo);


                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public Usuario BuscarUsuario(int id_usuario)
        {
            Usuario usuario = null;
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("buscarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_usuario", id_usuario);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    usuario = new Usuario
                    {
                        id_usuario = Convert.ToInt32(dr["id_usuario"]),
                        nombre = dr["nombre"].ToString(),
                        apellido = dr["apellido"].ToString(),
                        telefono = dr["telefono"].ToString(),
                        dni = dr["dni"].ToString(),
                        correo = dr["correo"].ToString(),
                        contrasena = dr["contrasena"].ToString(),
                        cargo = dr["cargo"].ToString(),

                    };
                }
            }
            return usuario;
        }

        public void EliminarUsuario(int id_usuario)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("eliminarUsuario", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_usuario", id_usuario);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public bool VerificarUsuarioExistente(string dni)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;

            bool dniExiste = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("verificarUsuarioExistente", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@dni", dni);

                        var result = command.ExecuteScalar();

                        dniExiste = (result != null && Convert.ToInt32(result) == 1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al verificar el DNI: " + ex.Message);
                }
            }
            return dniExiste;
        }

        public Usuario BuscarUsuarioPorDni(string dni)
        {
            Usuario usuario = null;
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("buscarUsuarioPorDni", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@dni", dni);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    usuario = new Usuario
                    {
                        id_usuario = Convert.ToInt32(dr["id_usuario"]),
                        nombre = dr["nombre"].ToString(),
                        apellido = dr["apellido"].ToString(),
                        telefono = dr["telefono"].ToString(),
                        dni = dr["dni"].ToString(),
                        correo = dr["correo"].ToString(),
                        contrasena = dr["contrasena"].ToString(),
                        cargo = dr["cargo"].ToString(),

                    };
                }
            }
            return usuario;
        }

        public Usuario ValidarUsuario(string correo, string contrasena)
        {
            Usuario usuario = null;
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("validarUsuario", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@correo", correo);
                cmd.Parameters.AddWithValue("@contrasena", contrasena);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    usuario = new Usuario
                    {
                        id_usuario = Convert.ToInt32(dr["id_usuario"]),
                        nombre = dr["nombre"].ToString(),
                        apellido = dr["apellido"].ToString(),
                        telefono = dr["telefono"].ToString(),
                        dni = dr["dni"].ToString(),
                        correo = dr["correo"].ToString(),
                        cargo = dr["cargo"].ToString(),
                    };
                }
            }
            return usuario;
        }

        public void CambiarContrasena(int id_usuario, string nuevaContrasena)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("entro");
                    SqlCommand cmd = new SqlCommand("cambiarContrasena", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_usuario", id_usuario);
                    cmd.Parameters.AddWithValue("@nueva_contrasena", nuevaContrasena);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("salio");

                }
                catch (Exception ex)
                {
                    throw new Exception("Error al cambiar la contraseña", ex);
                }
            }
        }

        // Método para codificar la contraseña en SHA256
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
