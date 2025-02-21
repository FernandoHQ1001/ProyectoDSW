using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodega.SolProyectoWeb.Entidades.Core
{
    public class Usuario
    {
        public int id_usuario { get; set; }
        public String nombre { get; set; }
        public String apellido { get; set; }
        public String telefono { get; set; }
        public String dni { get; set; }
        public String correo { get; set; }
        public String contrasena { get; set; }
        public String cargo { get; set; }
    }
}
