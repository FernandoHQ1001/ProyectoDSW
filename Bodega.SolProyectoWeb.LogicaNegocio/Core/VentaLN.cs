using Bodega.SolProyectoWeb.AccesoDatos.Core;
using Bodega.SolProyectoWeb.Entidades.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodega.SolProyectoWeb.LogicaNegocio.Core
{
    public class VentaLN
    {
        private VentaDA ventaDA = new VentaDA();

        public List<Venta> ConsultarVentas(string criterio, string valor)
        {
            if (string.IsNullOrEmpty(criterio) || string.IsNullOrEmpty(valor))
            {
                throw new ArgumentException("El criterio y el valor de búsqueda no pueden estar vacíos.");
            }

            return ventaDA.ObtenerVentasPorCriterio(criterio, valor);
        }
        public List<Venta> ObtenerTodasLasVentas()
        {
            return ventaDA.ObtenerTodasLasVentas();
        }
        public List<Producto> ObtenerProductos()
        {
            return ventaDA.ObtenerProductos();
        }

        public List<MetodoDePago> ObtenerMetodoDePago()
        {
            return ventaDA.ObtenerMetodoDePago();
        }

        public void InicializarStockTemporal()
        {
            ventaDA.InicializarStockTemporal();
        }
        public int RegistrarVenta(Venta venta, List<Pedido> pedidos)
        {
            venta.monto_total = pedidos.Sum(p => p.subtotal);
            int idVenta = ventaDA.RegistrarVenta(venta);

            foreach (var pedido in pedidos)
            {
                pedido.id_venta = idVenta; // Asigna el id de la venta generada a cada pedido
                ventaDA.RegistrarPedido(pedido);
            }

            return idVenta;
        }

        public bool VerificarStockTemporal(int idProducto, int cantidad, out int stockDisponible)
        {
            // Llamada a la capa de datos para verificar el stock
            return ventaDA.VerificarYActualizarStockTemporal(idProducto, cantidad, out stockDisponible);
        }

        public void ActualizarStockProducto(List<int> idsProductos)
        {
            ventaDA.ActualizarStockDesdeStockTemporal(idsProductos);
        }

        public void ReponerStockTemporal(int idProducto, int cantidad)
        {
            ventaDA.ReponerStockTemporal(idProducto, cantidad);
        }

        public List<dynamic> ConsultarProductosPorVenta(int idVenta)
        {
            return ventaDA.ObtenerProductosPorVenta(idVenta);
        }

    }
}
