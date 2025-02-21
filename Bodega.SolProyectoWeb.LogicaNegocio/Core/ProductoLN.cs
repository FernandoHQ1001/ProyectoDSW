using Bodega.SolProyectoWeb.Entidades.Base;
using Bodega.SolProyectoWeb.Entidades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Bodega.SolProyectoWeb.AccesoDatos.Core;

namespace Bodega.SolProyectoWeb.LogicaNegocio.Core
{
    public class ProductoLN : BaseLN
    {
        public void ModificarProducto(Producto producto)
        {
            new ProductoDA().ModificarProducto(producto);
        }

        public void EliminarProducto(int idProducto)
        {
            new ProductoDA().EliminarProducto(idProducto);
        }

        public Producto BuscarProducto(int id)
        {
            return new ProductoDA().BuscarProducto(id);
        }

        public Producto BuscarProductoPorNombre(string nombre)
        {
            return new ProductoDA().BuscarProductoPorNombre(nombre);
        }
        public bool ExisteProductoPorNombre(string nombreProducto)
        {
            // Llama al método BuscarProductoPorNombre de ProductoDA
            var producto = new ProductoDA().BuscarProductoPorNombre(nombreProducto);

            // Si el producto no es nulo, significa que ya existe
            return producto != null;
        }

        public bool RegistrarProducto(Producto producto)
        {
            // Verifica si el producto ya existe antes de registrarlo
            if (ExisteProductoPorNombre(producto.nombre))
            {
                throw new Exception("El producto ya existe. No se puede registrar duplicados.");
            }

            // Si no existe, llama a ProductoDA para registrar el producto
            return new ProductoDA().RegistrarProducto(producto);
        }

        public List<Producto> ConsultarInventario(string criterio, string valor)
        {
            if (string.IsNullOrEmpty(criterio) || string.IsNullOrEmpty(valor))
            {
                throw new ArgumentException("El criterio y el valor de búsqueda no pueden estar vacíos.");
            }

            return new ProductoDA().ObtenerProductosPorCriterio(criterio, valor);
        }

        public List<Producto> ObtenerTodosLosProductos()
        {
            return new ProductoDA().ObtenerTodosLosProductos();
        }
    }
}
