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
    public class TarjetaModel
    {
        public const string connectionString = "connectionString";
        public const string sp_scal_ing_tarjeta = "scal_ing_tarjeta";
        public static async Task<string> GuardarIdTarjeta(string id)
        {
            var res = "";
            using (var dbConn = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString))
            {
                try
                {
                    using (var cmd = new SqlCommand(sp_scal_ing_tarjeta, dbConn))
                    {
                        var dr = await cmd.ExecuteNonQueryAsync();
                        res = "OK";
                    }
                }
                catch (Exception ex)
                {
                    res = ex.Message;
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