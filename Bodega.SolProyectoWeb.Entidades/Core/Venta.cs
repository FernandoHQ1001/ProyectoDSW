using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodega.SolProyectoWeb.Entidades.Core
{
    public class Venta
    {
        public int id_venta { get; set; }
        public String dni_cliente { get; set; }
        public int id_metodo_pago { get; set; }
        public string nombre_metodo_pago { get; set; }
        public DateTime fecha { get; set; }
        public decimal monto_total { get; set; }
        public int id_usuario { get; set; }
    }
}
