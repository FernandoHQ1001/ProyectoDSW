using Bodega.SolProyectoWeb.Entidades.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodega.SolProyectoWeb.AccesoDatos.Core
{
    public class VentaDA
    {
        public List<dynamic> ObtenerProductosPorVenta(int idVenta)
        {
            var productos = new List<dynamic>();

            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();
                string query = @"SELECT p.id_producto, p.nombre, dp.cantidad, p.precio, (dp.cantidad * p.precio) as monto
                         FROM Pedido dp
                         INNER JOIN Producto p ON dp.id_producto = p.id_producto
                         WHERE dp.id_venta = @idVenta";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idVenta", idVenta);

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dynamic producto = new ExpandoObject();
                            producto.id_producto = reader.GetInt32(0);
                            producto.nombre = reader.GetString(1);
                            producto.cantidad = reader.GetInt32(2);
                            producto.precio = reader.GetDecimal(3);
                            producto.monto = reader.GetDecimal(4);

                            productos.Add(producto);
                        }
                    }
                }
            }
            return productos;
        }

        public List<Venta> ObtenerTodasLasVentas()
        {
            var ventas = new List<Venta>();

            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();
                string query = "SELECT venta.id_venta, venta.dni_cliente, venta.fecha, venta.id_metodo_pago, MetodoDePago.tipo_metodo, venta.monto_total FROM Venta INNER JOIN MetodoDePago ON venta.id_metodo_pago=MetodoDePago.id_metodo_pago";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new Venta
                            {
                                id_venta = reader.GetInt32(0),
                                dni_cliente = reader.GetString(1),
                                fecha = reader.GetDateTime(2),
                                id_metodo_pago = reader.GetInt32(3),
                                nombre_metodo_pago = reader.GetString(4),
                                monto_total = reader.GetDecimal(5)
                            });
                        }
                    }
                }
            }
            return ventas;
        }

        public List<Venta> ObtenerVentasPorCriterio(string criterio, string valor)
        {
            var ventas = new List<Venta>();
            string query = "";

            // Generar la consulta SQL en función del criterio seleccionado
            switch (criterio)
            {
                case "DNI":
                    query = "SELECT venta.id_venta, venta.dni_cliente, venta.fecha, venta.id_metodo_pago, MetodoDePago.tipo_metodo, venta.monto_total FROM Venta INNER JOIN MetodoDePago ON venta.id_metodo_pago=MetodoDePago.id_metodo_pago WHERE TRY_CAST(dni_cliente AS INT) = @valor";
                    break;
                case "Fecha":
                    query = "SELECT venta.id_venta, venta.dni_cliente, venta.fecha, venta.id_metodo_pago, MetodoDePago.tipo_metodo, venta.monto_total FROM Venta INNER JOIN MetodoDePago ON venta.id_metodo_pago=MetodoDePago.id_metodo_pago WHERE CONVERT(DATE, fecha) = @valor";
                    break;
                case "MetodoPago":
                    query = "SELECT venta.id_venta, venta.dni_cliente, venta.fecha, venta.id_metodo_pago, MetodoDePago.tipo_metodo, venta.monto_total FROM Venta INNER JOIN MetodoDePago ON venta.id_metodo_pago=MetodoDePago.id_metodo_pago WHERE MetodoDePago.tipo_metodo = @valor";
                    break;
                default:
                    throw new ArgumentException("Criterio de búsqueda no válido");
            }

            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    // Convertir valor a fecha si el criterio es fecha y pasarlo como DateTime
                    if (criterio == "Fecha" && DateTime.TryParse(valor, out DateTime fechaBusqueda))
                    {
                        comando.Parameters.AddWithValue("@valor", fechaBusqueda);
                    }
                    else
                    {
                        comando.Parameters.AddWithValue("@valor", valor);
                    }

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ventas.Add(new Venta
                            {
                                id_venta = reader.GetInt32(0),
                                dni_cliente = reader.GetString(1),
                                fecha = reader.GetDateTime(2),
                                id_metodo_pago = reader.GetInt32(3),
                                nombre_metodo_pago = reader.GetString(4),
                                monto_total = reader.GetDecimal(5)
                            });
                        }
                    }
                }
            }
            return ventas;
        }

        public List<MetodoDePago> ObtenerMetodoDePago()
        {
            var metodosDePago = new List<MetodoDePago>();

            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();
                using (SqlCommand comando = new SqlCommand("SELECT id_metodo_pago, tipo_metodo FROM MetodoDePago", conexion))
                {
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metodosDePago.Add(new MetodoDePago
                            {
                                id_metodo_pago = reader.GetInt32(0),
                                tipo_metodo = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return metodosDePago;
        }

        public List<Producto> ObtenerProductos()
        {
            List<Producto> productos = new List<Producto>();

            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();

                using (SqlCommand comando = new SqlCommand("SELECT id_producto, nombre, precio FROM Producto", conexion))
                {
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Producto producto = new Producto
                            {
                                id_producto = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                precio = reader.GetDecimal(2)
                            };
                            productos.Add(producto);
                        }
                    }
                }
            }
            return productos;
        }

        public void InicializarStockTemporal()
        {
            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();
                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexion;
                    // Limpiar la tabla StockTemporal y llenarla con los datos actuales de Producto
                    comando.CommandText = "DELETE FROM StockTemporal; " +
                                          "INSERT INTO StockTemporal (id_producto, stock) " +
                                          "SELECT id_producto, stock FROM Producto;";
                    comando.ExecuteNonQuery();
                }
            }
        }

        public void RestaurarStockTemporal(int idProducto, int cantidad)
        {
            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();
                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexion;
                    // Incrementa el stock en la tabla StockTemporal
                    comando.CommandText = "UPDATE StockTemporal SET stock = stock + @cantidad WHERE id_producto = @id_producto";
                    comando.Parameters.AddWithValue("@id_producto", idProducto);
                    comando.Parameters.AddWithValue("@cantidad", cantidad);
                    comando.ExecuteNonQuery();
                }
            }
        }

        public int RegistrarVenta(Venta venta)
        {
            int idVentaGenerado;
            Console.WriteLine("ID Usuario antes de ejecutar el procedimiento: " + venta.id_usuario);
            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();

                using (SqlCommand comando = new SqlCommand("paInsertarVenta", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@dni_cliente", venta.dni_cliente);
                    comando.Parameters.AddWithValue("@id_metodo_pago", venta.id_metodo_pago);
                    comando.Parameters.AddWithValue("@fecha", venta.fecha);
                    comando.Parameters.AddWithValue("@monto_total", venta.monto_total);
                    comando.Parameters.AddWithValue("@id_usuario", venta.id_usuario);


                    SqlParameter idVentaParam = new SqlParameter("@id_venta", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };


                    comando.Parameters.Add(idVentaParam);

                    comando.ExecuteNonQuery();
                    idVentaGenerado = (int)idVentaParam.Value;
                }
            }

            return idVentaGenerado;
        }

        public void RegistrarPedido(Pedido pedido)
        {
            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();

                using (SqlCommand comando = new SqlCommand("paInsertarPedido", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@id_venta", pedido.id_venta); // Debes tener este campo en el objeto Pedido
                    comando.Parameters.AddWithValue("@id_producto", pedido.id_producto);
                    comando.Parameters.AddWithValue("@cantidad", pedido.cantidad);
                    comando.Parameters.AddWithValue("@subtotal", pedido.subtotal);

                    comando.ExecuteNonQuery();
                }
            }
        }

        public bool VerificarYActualizarStockTemporal(int idProducto, int cantidad, out int stockDisponible)
        {
            stockDisponible = 0; // Inicializar el parámetro de salida

            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();

                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexion;

                    // Primero, verificar si hay suficiente stock en la tabla StockTemporal
                    comando.CommandText = "SELECT stock FROM StockTemporal WHERE id_producto = @id_producto";
                    comando.Parameters.AddWithValue("@id_producto", idProducto);

                    object result = comando.ExecuteScalar();
                    stockDisponible = result != null ? Convert.ToInt32(result) : 0;

                    // Si hay suficiente stock, actualizar la tabla restando la cantidad
                    if (stockDisponible >= cantidad)
                    {
                        comando.CommandText = "UPDATE StockTemporal SET stock = stock - @cantidad WHERE id_producto = @id_producto";
                        comando.Parameters.AddWithValue("@cantidad", cantidad);

                        comando.ExecuteNonQuery();
                        return true; // Indica que hay suficiente stock y se ha actualizado
                    }
                    else
                    {

                        return false; // No hay suficiente stock
                    }
                }
            }
        }

        public void ActualizarStockDesdeStockTemporal(List<int> idsProductos)
        {
            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();

                foreach (int idProducto in idsProductos)
                {
                    string query = @"
                UPDATE Producto 
                SET stock = (
                    SELECT stock 
                    FROM StockTemporal 
                    WHERE id_producto = @idProducto
                )
                WHERE id_producto = @idProducto";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@idProducto", idProducto);
                        comando.ExecuteNonQuery();
                    }
                }
            }
        }

        public void ReponerStockTemporal(int idProducto, int cantidad)
        {
            using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString))
            {
                conexion.Open();
                string query = "UPDATE StockTemporal SET stock = stock + @cantidad WHERE id_producto = @idProducto";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@cantidad", cantidad);
                    comando.Parameters.AddWithValue("@idProducto", idProducto);
                    comando.ExecuteNonQuery();
                }
            }
        }
    }
}
