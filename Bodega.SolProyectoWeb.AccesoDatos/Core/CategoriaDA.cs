using Bodega.SolProyectoWeb.Entidades.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodega.SolProyectoWeb.AccesoDatos.Core
{
    public class CategoriaDA
    {
        // Cambiar a que devuelva una lista de Categoria
        public List<Categoria> ListarCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();
            string connectionString = ConfigurationManager.ConnectionStrings["cnnSql"].ConnectionString;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("listarCategorias", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    // Crear el objeto Categoria y agregarlo a la lista
                    categorias.Add(new Categoria
                    {
                        id_categoria = Convert.ToInt32(dr["id_categoria"]),
                        nombre_categoria = dr["nombre_categoria"].ToString() // Asegúrate de que el nombre de la columna sea correcto
                    });
                }
            }
            return categorias;
        }
    }
}
