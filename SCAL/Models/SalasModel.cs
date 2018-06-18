using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SCAL.Models
{
    public class Sala
    {
        public int id;
        public string ip;
        public string descripcion;
    }
    public class SalaStatus : Sala
    {
        public string estadoLuces;
    }
    public class SalasModel
    {
        public const string appContext = "appContext";
        public const string sp_scal_sel_salas = "scal_sel_salas";
        public static async Task<List<Sala>> GetSalas()
        {
            var res = new List<Sala>();
            using (var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[appContext].ConnectionString))
            {
                try
                {
                    using (var cmd = new SqlCommand(sp_scal_sel_salas, dbConn))
                    {
                        var dr = await cmd.ExecuteReaderAsync();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                res.Add(new Sala
                                {
                                    id = (int)dr["id"],
                                    ip = (string)dr["ip"],
                                    descripcion = (string)dr["descripcion"]
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
        public static List<SalaStatus> GetSalasStatus()
        {
            var res = new List<SalaStatus>();
            var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[appContext].ConnectionString);
            try
            {
                dbConn.Open();
                using (var cmd = new SqlCommand(sp_scal_sel_salas, dbConn))
                {
                    var dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            var sala = new SalaStatus
                            {
                                id = (int)dr["id"],
                                ip = (string)dr["ip"],
                                descripcion = (string)dr["descripcion"]
                            };
                            var client = new HttpClient();
                            sala.estadoLuces = client.GetStringAsync("http://" + sala.ip + "/?estadoluces").Result;
                            res.Add(sala);
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
            return res;
        }
    }
}