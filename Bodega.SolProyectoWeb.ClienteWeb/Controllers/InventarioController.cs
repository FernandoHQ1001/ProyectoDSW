using Bodega.SolProyectoWeb.Entidades.Core;
using Bodega.SolProyectoWeb.LogicaNegocio.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bodega.SolProyectoWeb.ClienteWeb.Controllers
{
    public class InventarioController : Controller
    {
        // GET: Inventario
        public ActionResult Index()
        {
            return View();
        }

        // GET: Inventario/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Inventario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventario/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Inventario/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Inventario/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Inventario/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Inventario/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
