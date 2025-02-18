using Bodega.SolProyectoWeb.Entidades.Base;
using Bodega.SolProyectoWeb.Entidades.Core;
using Bodega.SolProyectoWeb.AccesoDatos.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Bodega.SolProyectoWeb.LogicaNegocio.Core
{
    public class CategoriaLN : BaseLN
    {
        public List<Categoria> ListarCategorias()
        {
            return new CategoriaDA().ListarCategorias();
        }
    }
}
