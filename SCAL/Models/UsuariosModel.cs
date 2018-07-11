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
        public int? id;
        public string tarjeta;
        public string nombre;
        public List<string> salas_permitidas;
        public Usuario()
        {
            salas_permitidas = new List<string>();
        }
    }
    public class UsuariosModel
    {
        public const string appContext = "appContext";
        public const string sp_scal_sel_usuarios = "scal_sel_usuarios";
        public const string sp_scal_ing_usuario = "scal_ing_usuario";
        public const string sp_scal_del_usuario = "scal_del_usuario";

        public static async Task<string> SetUsuarioAsync(Usuario usuario)
        {
            var res = "";
            var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[appContext].ConnectionString);
            try
            {
                dbConn.Open();
                var cmd = new SqlCommand(sp_scal_ing_usuario, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tarjeta", usuario.tarjeta);
                cmd.Parameters.AddWithValue("@nombre", usuario.nombre);
                cmd.Parameters.AddWithValue("@salas_permitidas", string.Join(",", usuario.salas_permitidas));
                if (usuario.id != null)
                    cmd.Parameters.AddWithValue("@id", usuario.id);

                await cmd.ExecuteNonQueryAsync();
                res = "Ok";
            }
            catch (Exception ex)
            {
                res = "Error: " + ex.Message;
            }
            finally
            {
                if (dbConn.State == ConnectionState.Open)
                    dbConn.Close();
            }
            return res;
        }

        public static async Task<List<Usuario>> getUsuarios()
        {
            var res = new List<Usuario>();
            using (var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[appContext].ConnectionString))
            {
                try
                {
                    dbConn.Open();
                    using (var cmd = new SqlCommand(sp_scal_sel_usuarios, dbConn))
                    {
                        var dr = await cmd.ExecuteReaderAsync();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                res.Add(new Usuario
                                {
                                    id = (int?)dr["id"],
                                    tarjeta = (string)dr["tarjeta"],
                                    nombre = (string)dr["nombre"],
                                    salas_permitidas = dr["salas_permitidas"].ToString().Split(',').ToList()
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
        public static async Task<string> BorrarUsuarioAsync(int id)
        {
            var res = "";
            var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[appContext].ConnectionString);
            try
            {
                dbConn.Open();
                var cmd = new SqlCommand(sp_scal_del_usuario, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync();
                res = "Ok";
            }
            catch (Exception ex)
            {
                res = "Error: " + ex.Message;
            }
            finally
            {
                if (dbConn.State == ConnectionState.Open)
                    dbConn.Close();
            }
            return res;
        }
    }
}