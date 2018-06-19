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
        public int? id;
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
        public const string sp_scal_set_sala = "scal_ing_sala";
        public const string sp_scal_del_sala = "scal_del_sala";
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
                            try
                            {
                                var client = new HttpClient();
                                client.Timeout = TimeSpan.FromSeconds(4);
                                sala.estadoLuces = client.GetStringAsync("http://" + sala.ip + "/?estadoluces").Result.Replace(Environment.NewLine,string.Empty);
                            }
                            catch
                            {
                                sala.estadoLuces = "No disponible";
                            }

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
        public static string SetSala(Sala sala)
        {
            var res = "";
            var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[appContext].ConnectionString);
            try
            {
                dbConn.Open();
                var cmd = new SqlCommand(sp_scal_set_sala, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@direccion_ip", sala.ip);
                cmd.Parameters.AddWithValue("@descripcion", sala.descripcion);
                if (sala.id != null)
                    cmd.Parameters.AddWithValue("@id", sala.id);

                cmd.ExecuteNonQuery();
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
        public static string BorrarSala(int id)
        {
            var res = "";
            var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[appContext].ConnectionString);
            try
            {
                dbConn.Open();
                var cmd = new SqlCommand(sp_scal_del_sala, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
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
        public static string EncenderLuz(string ip)
        {
            var res = "";
            var client = new HttpClient();
            try
            {
                var response = client.GetStringAsync("http://" + ip + "/?ledon").Result;
                if (response.Replace(Environment.NewLine, string.Empty) == "Luces encendidas")
                {
                    res = "Ok";
                }
                else
                {
                    res = "Error encendiendo luces en arduino";
                }
            }
            catch(Exception ex)
            {
                res = "Error: " + ex.Message;
            }
            return res;
            
        }
        public static string ApagarLuz(string ip)
        {
            var res = "";
            var client = new HttpClient();
            try
            {
                var response = client.GetStringAsync("http://" + ip + "/?ledoff").Result;
                if (response.Replace(Environment.NewLine, string.Empty) == "Luces apagadas")
                {
                    res = "Ok";
                }
                else
                {
                    res = "Error apagando luces en arduino";
                }
            }
            catch (Exception ex)
            {
                res = "Error: " + ex.Message;
            }
            return res;

        }
    }
}