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
    public class Log
    {
        public string ip_sala;
        public string nombre_sala;
        public string id_tarjeta;
        public string nom_usuario;
        public string resultado;
        public DateTime f_registro;
    }
    public class TarjetaModel
    {
        public const string appContext = "appContext";
        public const string sp_scal_ing_tarjeta = "scal_ing_log_rfid";
        public const string sp_scal_sel_log = "scal_sel_log";
        public static async Task<string> GuardarLogRFID(string ip, string id_tarjeta, string resultado)
        {
            var res = "";
            using (var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[appContext].ConnectionString))
            {
                try
                {
                    dbConn.Open();
                    using (var cmd = new SqlCommand(sp_scal_ing_tarjeta, dbConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ip", ip);
                        cmd.Parameters.AddWithValue("@id_tarjeta", id_tarjeta);
                        cmd.Parameters.AddWithValue("@resultado", resultado);
                        var dr = await cmd.ExecuteNonQueryAsync();
                        res = "OK";
                    }
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
            }         
            return res;
        }

        internal static async Task<List<Log>> GetLogsAsync()
        {
            var res = new List<Log>();
            using (var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[appContext].ConnectionString))
            {
                try
                {
                    dbConn.Open();
                    using (var cmd = new SqlCommand(sp_scal_sel_log, dbConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var dr = await cmd.ExecuteReaderAsync();
                        if (dr.HasRows)
                        {
                            while (await dr.ReadAsync())
                            {
                                res.Add(new Log {
                                    ip_sala = dr["ip"].ToString().Trim(),
                                    nombre_sala = dr["descripcion"].ToString().Trim(),
                                    id_tarjeta = dr["id_tarjeta"].ToString().Trim(),
                                    nom_usuario = dr["nombre"].ToString().Trim(),
                                    resultado = dr["resultado"].ToString().Trim(),
                                    f_registro = DateTime.Parse(dr["f_registro"].ToString().Trim())
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var err = "Error: " + ex.Message;
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