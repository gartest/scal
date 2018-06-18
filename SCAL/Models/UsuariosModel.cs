using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SCAL.Models
{
    public class Usuario
    {
        public int id;
        public string tarjeta;
        public string nombre;
    }
    public class UsuariosModel
    {
        public const string connectionString = "connectionString";
        public const string sp_scal_sel_usuarios = "scal_sel_usuarios";
        public static async Task<List<Usuario>> getUsuarios()
        {
            var res = new List<Usuario>();
            using (var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString))
            {
                try
                {
                    using (var cmd = new SqlCommand(sp_scal_sel_usuarios, dbConn))
                    {
                        var dr = await cmd.ExecuteReaderAsync();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                res.Add(new Usuario
                                {
                                    id = (int)dr["id"],
                                    tarjeta = (string)dr["tarjeta"],
                                    nombre = (string)dr["nombre"]
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var err = ex.Message;
                }
                finally
                {
                    if (dbConn.State == ConnectionState.Open)
                        dbConn.Close();
                }
            }
            return res;
        }
    }
}