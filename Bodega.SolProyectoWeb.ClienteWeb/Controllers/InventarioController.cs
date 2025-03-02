using Bodega.SolProyectoWeb.AccesoDatos.Core;
using Bodega.SolProyectoWeb.Entidades.Core;
using Bodega.SolProyectoWeb.LogicaNegocio.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bodega.SolProyectoWeb.ClienteWeb.Controllers
{
    [AuthorizeCargo("Encargado de inventario")]
    public class InventarioController : Controller
    {
        // GET: Inventario/ActualizarProducto/5
        public ActionResult ActualizarProducto()
        {

            Producto producto = new Producto();

            // Obtener las categorías desde la base de datos
            var categorias = new CategoriaLN().ListarCategorias();

            // Pasar las categorías a la vista
            ViewBag.Categorias = categorias;

            return View(producto);
        }

        // POST: Inventario/CargarProducto
        [HttpPost]
        public ActionResult CargarProducto(string nombre)
        {
            // Validar si se ingreso un nombre
            if (string.IsNullOrEmpty(nombre))
            {
                TempData["Message"] = "Debe ingresar el nombre del producto que desea buscar.";
                TempData["MessageType"] = "warning"; // Advertencia
                return RedirectToAction("ActualizarProducto");
            }

            Producto producto = new ProductoLN().BuscarProductoPorNombre(nombre);

            Debug.WriteLine($"CARGAR PRODUCTO");
            Debug.WriteLine($"producto id: {producto.id_producto}");
            Debug.WriteLine($"producto nombre: {producto.nombre}");
            Debug.WriteLine($"producto stock: {producto.stock}");
            Debug.WriteLine($"producto precio: {producto.precio}");


            // Obtener las categorías 
            var categorias = new CategoriaLN().ListarCategorias();
            ViewBag.Categorias = categorias;

            if (producto != null)
            {
                return View("ActualizarProducto", producto);
            }
            else
            {
                TempData["Message"] = "No se encontró un producto que coincida con el nombre de producto ingresado.";
                TempData["MessageType"] = "error"; // Error
                ViewBag.ProductoNoEncontrado = true;
                return View("ActualizarProducto", new Producto());
            }
        }

        // POST: Inventario/ActualizarProducto/5
        [HttpPost]
        public ActionResult ActualizarProducto(Producto producto)
        {

            Debug.WriteLine($"ACTUALIZAR PRODUCTO");
            Debug.WriteLine($"producto id: {producto.id_producto}");
            Debug.WriteLine($"producto nombre: {producto.nombre}");
            Debug.WriteLine($"producto stock: {producto.stock}");
            Debug.WriteLine($"producto precio: {producto.precio}");


            try
            {
            
    

                Debug.WriteLine($"producto precio: {producto.precio}");

                if (!ModelState.IsValid)
                {
                    // Mostrar los errores de validación
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Debug.WriteLine($"Error: {modelError.ErrorMessage}");
                    }

                    // El resto de tu código para manejar el error
                }

                if (ModelState.IsValid)
                {
                    new ProductoLN().ModificarProducto(producto);
                    ViewBag.ActualizacionExitosa = true;

                    TempData["Message"] = "La actualización del producto se realizo con éxito.";
                    TempData["MessageType"] = "success"; // Éxito

                    return RedirectToAction("ActualizarProducto");
                }

                var categorias = new CategoriaLN().ListarCategorias();
                ViewBag.Categorias = categorias;

                TempData["Message"] = "Hay errores en el formulario. Por favor corrígelos.";
                TempData["MessageType"] = "error"; // Error

                return View(producto);
            }
            catch(Exception ex)
            {
                var categorias = new CategoriaLN().ListarCategorias();
                ViewBag.Categorias = categorias;

                // Mostrar el error en el TempData
                TempData["Message"] = $"Ocurrió un error al actualizar el producto: {ex.Message}";
                TempData["MessageType"] = "error"; // Error

  
                Console.WriteLine($"Error al actualizar producto: {ex.Message}");

                return View(producto);
            }
        }

        // POST: Inventario/EliminarProducto
        [HttpPost]
        public ActionResult EliminarProducto(int id_producto)
        {
            Debug.WriteLine($"producto id: {id_producto}");


            if (id_producto == 0)
            {

                TempData["Message"] = "Ingrese el producto a eliminar.";
                TempData["MessageType"] = "error"; // Error
                return RedirectToAction("ActualizarProducto");
            }

            try
            {
                var producto = new ProductoLN().BuscarProducto(id_producto);

                if (producto == null)
                {
                    TempData["Message"] = "El producto que intentas eliminar no existe.";
                    TempData["MessageType"] = "warning"; // Advertencia
                    return RedirectToAction("ActualizarProducto");
                }
                new ProductoLN().EliminarProducto(id_producto);
                TempData["Message"] = "La eliminación del producto se realizó con éxito";
                TempData["MessageType"] = "success"; // Éxito
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Ocurrió un error al intentar eliminar el producto: {ex.Message}";
                TempData["MessageType"] = "error"; // Error

                Console.WriteLine($"Error al eliminar producto: {ex.Message}");
            }

            return RedirectToAction("ActualizarProducto");
        }

        public ActionResult RegistrarProducto()
        {

            var categorias = new CategoriaLN().ListarCategorias();
            ViewBag.Categorias = new SelectList(categorias, "id_categoria", "nombre_categoria");

            return View();
        }
        [HttpPost]
        public ActionResult RegistrarProducto(Producto producto)
        {
            var usuario = (Usuario)Session["Usuario"];
            if (usuario == null)
            {
                return RedirectToAction("Login", "Login", new { mensaje = "Debes iniciar sesión para registrar productos." });
            }
            System.Diagnostics.Debug.WriteLine(($"ID Usuario desde la sesión: {usuario.id_usuario}"));
            producto.id_usuario = usuario.id_usuario;


            if (string.IsNullOrEmpty(producto.nombre) || producto.precio == 0 || string.IsNullOrEmpty(producto.id_categoria.ToString()) || producto.stock == 0 || string.IsNullOrEmpty(producto.descripcion))
            {
                ModelState.AddModelError("", "Por favor, complete todos los campos obligatorios.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productoLN = new ProductoLN();

                    // Verificar si el producto ya existe
                    var productoExistente = productoLN.BuscarProductoPorNombre(producto.nombre);
                    if (productoExistente != null)
                    {
                        ViewBag.ErrorNombreExistente = $"El producto '{producto.nombre}' ya existe en el sistema.";
                        var categorias1 = new CategoriaLN().ListarCategorias();
                        ViewBag.Categorias = new SelectList(categorias1, "id_categoria", "nombre_categoria");
                        return View(producto); // Regresa a la vista con el mensaje de error
                    }

                    // Llamar a la lógica de negocio para registrar el producto
                    var resultado = new ProductoLN().RegistrarProducto(producto);

                    if (resultado)
                    {
                        ViewBag.MensajeExitoso = "El producto fue registrado con éxito.";
                        ModelState.Clear(); // Limpia el formulario
                        var categorias2 = new CategoriaLN().ListarCategorias();
                        ViewBag.Categorias = new SelectList(categorias2, "id_categoria", "nombre_categoria");
                        return View(new Producto()); // Devuelve el formulario limpio
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error al registrar el producto.");
                    }
                }
                catch (Exception ex)
                {
                    // Manejar cualquier error y mostrarlo en la vista
                    System.Diagnostics.Debug.WriteLine($"Excepción capturada: {ex.Message}");
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }

            // Si el modelo no es válido o hubo errores, recargar categorías para la vista
            var categorias = new CategoriaLN().ListarCategorias();

            ViewBag.MensajeExitoso = "El producto fue registrado con éxito."; //temporal (despues se borra)

            ViewBag.Categorias = new SelectList(categorias, "id_categoria", "nombre_categoria");
            return View(producto);
        }

        // GET: Inventario/ConsultarInventario
        public ActionResult ConsultarInventario()
        {
            // se obtienen las categorias para el filtro
            var categorias = new CategoriaLN().ListarCategorias();
            ViewBag.Categorias = new SelectList(categorias, "id_categoria", "nombre_categoria");

            return View();
        }

        // POST: Inventario/ConsultarInventario
        [HttpPost]
        public ActionResult ConsultarInventario(string criterio, string valor)
        {
            try
            {

                if (string.IsNullOrEmpty(criterio) && !string.IsNullOrEmpty(valor))
                {
                    TempData["Message"] = "Debe seleccionar un criterio para realizar la búsqueda.";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("ConsultarInventario");
                }

                if (!string.IsNullOrEmpty(criterio) && string.IsNullOrEmpty(valor))
                {
                    TempData["Message"] = "Debe ingresar un valor para el criterio seleccionado.";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("ConsultarInventario");
                }

                List<Producto> productos;

                // Si no se selecciona un criterio o el valor está vacío, se muestran todos los productos
                if (string.IsNullOrEmpty(criterio) || string.IsNullOrEmpty(valor))
                {

                    productos = new ProductoLN().ObtenerTodosLosProductos();
                }
                else
                {
                    if (criterio == "Categoria")
                    {
                        var categorias1 = new CategoriaLN().ListarCategorias();

                        // Buscamos la categoría por nombre
                        var categoria1 = categorias1.FirstOrDefault(c => c.nombre_categoria.Equals(valor, StringComparison.OrdinalIgnoreCase));

                        if (categoria1 != null)
                        {
                            // Si se encuentra la categoría, cambiamos el valor a su id
                            valor = categoria1.id_categoria.ToString();
                        }
                        else
                        {
                            // Si no se encuentra la categoría, asignamos un valor vacío para evitar problemas en la consulta
                            valor = string.Empty;

                            TempData["Message"] = "No se encontró ninguna categoría con el nombre ingresado.";
                            TempData["MessageType"] = "error";
                            return RedirectToAction("ConsultarInventario");
                        }
                    }
                    // buscar según el criterio y el valor
                    productos = new ProductoLN().ConsultarInventario(criterio, valor);
                }

                // Recargar categorias para la vista
                var categorias = new CategoriaLN().ListarCategorias();
                ViewBag.Categorias = new SelectList(categorias, "id_categoria", "nombre_categoria");
                ViewBag.Categorias = new SelectList(categorias, "id_categoria", "nombre_categoria");

                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Ocurrió un error al consultar el inventario.";
                TempData["MessageType"] = "error";
                return RedirectToAction("ConsultarInventario");
            }
        }
    }
}
