using Bodega.SolProyectoWeb.Entidades.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bodega.SolProyectoWeb.AccesoDatos.Core
{
    public class ProductoDA
    {
        public Producto BuscarProducto(int id_producto)
        {
            Producto producto = null;
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("buscarProducto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_producto", id_producto);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    producto = new Producto
                    {
                        id_producto = Convert.ToInt32(dr["id_producto"]),
                        nombre = dr["nombre"].ToString(),
                        stock = Convert.ToInt32(dr["stock"]),
                        precio = Convert.ToDecimal(dr["precio"]),
                        descripcion = dr["descripcion"].ToString(),
                        id_categoria = Convert.ToInt32(dr["id_categoria"]),
                        id_usuario = Convert.ToInt32(dr["id_usuario"])
                    };
                }
            }
            return producto;
        }
        public Producto BuscarProductoPorNombre(string nombre)
        {
            Producto producto = null;
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("buscarProductoPorNombre", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombre", nombre);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    producto = new Producto
                    {
                        id_producto = Convert.ToInt32(dr["id_producto"]),
                        nombre = dr["nombre"].ToString(),
                        stock = Convert.ToInt32(dr["stock"]),
                        precio = Convert.ToDecimal(dr["precio"]),
                        descripcion = dr["descripcion"].ToString(),
                        id_categoria = Convert.ToInt32(dr["id_categoria"]),
                        id_usuario = Convert.ToInt32(dr["id_usuario"]),
                        // Aquí agregamos solo la propiedad nombre_categoria
                        nombre_categoria = dr["nombre_categoria"].ToString()  // Nombre de la categoría
                    };
                }
            }
            return producto;
        }

        public bool RegistrarProducto(Producto producto)
        {
            bool resultado = false;
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;

            try
            {
                using (SqlConnection conexion = new SqlConnection(connectionString))
                {
                    // Abrir la conexión
                    conexion.Open();

                    // Procedimiento para registrar el producto
                    SqlCommand cmdRegistrar = new SqlCommand("RegistrarProducto", conexion);
                    cmdRegistrar.CommandType = CommandType.StoredProcedure;

                    // Pasar los parámetros necesarios
                    cmdRegistrar.Parameters.AddWithValue("@nombre", producto.nombre);
                    cmdRegistrar.Parameters.AddWithValue("@stock", producto.stock);
                    cmdRegistrar.Parameters.AddWithValue("@precio", producto.precio);
                    cmdRegistrar.Parameters.AddWithValue("@descripcion", producto.descripcion);
                    cmdRegistrar.Parameters.AddWithValue("@id_categoria", producto.id_categoria);
                    cmdRegistrar.Parameters.AddWithValue("@id_usuario", producto.id_usuario); // Usar directamente el id_usuario almacenado en producto

                    // Ejecutar el comando para registrar el producto
                    int filasAfectadas = cmdRegistrar.ExecuteNonQuery();
                    resultado = filasAfectadas > 0;
                }
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error SQL: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Número de error: {ex.Number}");
                System.Diagnostics.Debug.WriteLine($"Línea: {ex.LineNumber}");
                throw; // Re-lanza la excepción para manejarla en niveles superiores
            }

            return resultado;
        }

        //para consultar producto
        public List<Producto> ObtenerProductosPorCriterio(string criterio, object valor)
        {
            var productos = new List<Producto>();

            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();

                // Base query
                string query = @"
            SELECT 
                p.id_producto, 
                p.nombre, 
                p.stock, 
                p.precio, 
                p.descripcion, 
                p.id_categoria, 
                p.id_usuario, 
                c.nombre_categoria
            FROM Producto p
            INNER JOIN Categoria c ON p.id_categoria = c.id_categoria";

                // consulta segun el criterio
                switch (criterio)
                {
                    case "Categoria":
                        query += " WHERE p.id_categoria = @valor";
                        break;

                    case "Precio":
                        query += " WHERE p.precio = @valor";
                        break;

                    default:
                        throw new ArgumentException("Criterio no válido. Usa 'Categoria' o 'Precio'.");
                }

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    // parametro para los casos validos
                    comando.Parameters.AddWithValue("@valor", valor);

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                id_producto = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                stock = reader.GetInt32(2),
                                precio = reader.GetDecimal(3),
                                descripcion = reader.GetString(4),
                                id_categoria = reader.GetInt32(5),
                                id_usuario = reader.GetInt32(6),
                                nombre_categoria = reader.GetString(7)
                            });
                        }
                    }
                }
            }

            return productos;
        }

        public List<Producto> ObtenerTodosLosProductos()
        {
            var productos = new List<Producto>();

            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();

                string query = @"
            SELECT 
                p.id_producto, 
                p.nombre, 
                c.nombre_categoria, 
                p.precio, 
                p.stock, 
                p.descripcion
            FROM Producto p
            INNER JOIN Categoria c ON p.id_categoria = c.id_categoria";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                id_producto = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                nombre_categoria = reader.GetString(2),
                                precio = reader.GetDecimal(3),
                                stock = reader.GetInt32(4),
                                descripcion = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return productos;
        }
    }
}
