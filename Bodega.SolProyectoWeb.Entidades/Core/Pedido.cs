using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodega.SolProyectoWeb.Entidades.Core
{
    public class Pedido
    {
        public int id_pedido { get; set; }
        public int id_venta { get; set; }
        public int id_producto { get; set; }
        public int cantidad { get; set; }
        public decimal subtotal { get; set; }
    }
}
