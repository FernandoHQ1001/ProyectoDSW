using Bodega.SolProyectoWeb.Entidades.Core;
using Bodega.SolProyectoWeb.LogicaNegocio.Core;
using Bodega.SolProyectoWeb.AccesoDatos.Core;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bodega.SolProyectoWeb.ClienteWeb.Controllers
{
    public class VentasController : Controller
    {
        private VentaLN ventaLN = new VentaLN();

        // Método para mostrar la vista de registrar venta
        public ActionResult RegistrarVenta()
        {
            //Inicializar el stock temporal con los datos actuales de la tabla Producto
            ventaLN.InicializarStockTemporal();
            ViewBag.MetodosDePago = new SelectList(ventaLN.ObtenerMetodoDePago(), "id_metodo_pago", "tipo_metodo");
            ViewBag.Productos = ventaLN.ObtenerProductos();
            return View();
        }

        // Método para registrar la venta (POST)
        [HttpPost]
        public ActionResult RegistrarVenta(Venta venta, List<Pedido> pedidos)
        {
            if (string.IsNullOrEmpty(venta.dni_cliente) || venta.id_metodo_pago == null
                || venta.fecha == null)
            {
                ModelState.AddModelError("", "Por favor, complete todos los campos obligatorios.");
            }
            if (ModelState.IsValid && pedidos != null && pedidos.Any())
            {
                //Usuario usuarioActual = Session["Usuario"] as Usuario;

                /*
                if (usuarioActual != null)
                {
                    venta.id_usuario = usuarioActual.id_usuario;
                    System.Diagnostics.Debug.WriteLine("ID del usuario actual: " + venta.id_usuario);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("El usuario actual es nulo. Revisa la sesión.");
                }
                */

                int idVenta = ventaLN.RegistrarVenta(venta, pedidos);
                // Verificar que la venta fue registrada correctamente
                System.Diagnostics.Debug.WriteLine("ID de la venta registrada: " + idVenta);
                // Obtener los IDs de productos en la lista de pedidos
                List<int> idsProductos = pedidos.Select(p => p.id_producto).ToList();

                // Llamar al método para actualizar el stock
                try
                {
                    ventaLN.ActualizarStockProducto(idsProductos);

                    if (idVenta > 0)
                    {
                        TempData["RegistroExitoso"] = true; // Almacena una variable temporal para indicar éxito
                        System.Diagnostics.Debug.WriteLine("Venta registrada con éxito. Redirigiendo...");
                        return RedirectToAction("RegistrarVenta");
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores en caso de fallo al actualizar el stock
                    ModelState.AddModelError("", "Error al actualizar stock: " + ex.Message);
                    ViewBag.Productos = new SelectList(ventaLN.ObtenerProductos(), "id_producto", "nombre");
                    ViewBag.MetodosDePago = new SelectList(ventaLN.ObtenerMetodoDePago(), "id_metodo_pago", "tipo_metodo");
                    return View(venta);
                }

                // Redirigir solo si la venta es exitosa y tiene productos
                if (idVenta > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Confirmación de venta - Productos en pedido: " + pedidos.Count());
                    return RedirectToAction("Confirmacion", new { id = idVenta });
                }
                else
                {
                    // Si no hay productos en los pedidos
                    ModelState.AddModelError("", "Debe ingresar al menos un producto para realizar la venta.");
                }
            }

            // En caso de que el modelo no sea válido o haya fallado el registro o la actualización de stock
            ViewBag.Productos = new SelectList(ventaLN.ObtenerProductos(), "id_producto", "nombre");
            ViewBag.MetodosDePago = new SelectList(ventaLN.ObtenerMetodoDePago(), "id_metodo_pago", "tipo_metodo");

            // Si hay un error, permanece en la vista de registro de venta con los mensajes de error
            return View(venta);
        }

        public JsonResult VerificarStock(int idProducto, int cantidad)
        {
            bool stockSuficiente;
            int stockDisponible;


            try
            {
                // Verificar el stock temporal
                stockSuficiente = ventaLN.VerificarStockTemporal(idProducto, cantidad, out stockDisponible);
            }
            catch (Exception ex)
            {
                // Log de error y retorno de mensaje específico
                return Json(new { success = false, message = "Error al verificar el stock: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }

            if (stockSuficiente)
            {
                return Json(new { success = true, message = "Stock disponible." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = $"Stock insuficiente para el producto seleccionado. (Cantidad Disponible: {stockDisponible})" }, JsonRequestBehavior.AllowGet);
            }
        }

        private VentaDA ventaDA = new VentaDA();

        public JsonResult EliminarProductoDePedido(int idProducto, int cantidad)
        {
            try
            {
                // Llama al método que restaura el stock
                ventaDA.RestaurarStockTemporal(idProducto, cantidad);
                return Json(new { success = true, message = "Producto eliminado y stock restaurado correctamente." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al restaurar el stock: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ReponerStockTemporal(int idProducto, int cantidad)
        {
            try
            {
                // Lógica para reponer el stock en StockTemporal
                ventaLN.ReponerStockTemporal(idProducto, cantidad);
                return Json(new { success = true, message = "Stock actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al reponer el stock: " + ex.Message });
            }
        }
        public ActionResult Confirmacion(int id)
        {
            return View();
        }

        public ActionResult ConsultarVentas(string criterioBusqueda, string valorBusqueda)
        {
            try
            {
                List<Venta> ventas;

                // se verifica si el usuario selecciono un criterio pero no ingreso un valor de búsqueda
                if (!string.IsNullOrEmpty(criterioBusqueda) && string.IsNullOrEmpty(valorBusqueda))
                {
                    // lista vacia para que se active el modal en la vista
                    ventas = new List<Venta>();
                    ViewBag.HayResultados = false;
                }
                else if (!string.IsNullOrEmpty(criterioBusqueda) && !string.IsNullOrEmpty(valorBusqueda))
                {
                    // busqueda segun el criterio y el valor
                    ventas = ventaLN.ConsultarVentas(criterioBusqueda, valorBusqueda);
                    ViewBag.HayResultados = ventas.Any();
                }
                else
                {
                    // Muestra todas las ventas si no se selecciono ningun criterio ni valor
                    ventas = ventaLN.ObtenerTodasLasVentas();
                    ViewBag.HayResultados = ventas.Any();
                }

                return View("ConsultarVentas", ventas);
            }
            catch (Exception ex)
            {
                // establece un mensaje de error y devuelve una lista vacía
                ViewBag.Error = ex.Message;
                ViewBag.HayResultados = false;
                return View("ConsultarVentas", new List<Venta>());
            }
        }
        public ActionResult VerProductos()
        {
            return View(); // Muestra la vista vacía para ingresar el código de venta
        }

        // Acción para procesar la búsqueda de productos por código de venta
        [HttpPost]
        public ActionResult VerProductos(int codigoVenta)
        {
            var productos = ventaLN.ConsultarProductosPorVenta(codigoVenta);

            if (productos == null || productos.Count == 0)
            {
                ViewBag.MensajeError = "No se encontró una venta que coincida con el código ingresado.";
            }

            return View(productos); // Retorna los productos encontrados (o una lista vacía)
        }
    }
}
