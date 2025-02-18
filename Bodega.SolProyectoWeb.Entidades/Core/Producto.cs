using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodega.SolProyectoWeb.Entidades.Core
{
    public class Producto
    {
        public int id_producto { get; set; }
        public String nombre { get; set; }
        public int stock { get; set; }
        public decimal precio { get; set; }
        public String descripcion { get; set; }
        public int id_categoria { get; set; }
        public int id_usuario { get; set; }
        public string nombre_categoria { get; set; }
    }
}
