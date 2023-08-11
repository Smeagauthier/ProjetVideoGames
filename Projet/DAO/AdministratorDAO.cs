using Projet.metier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.DAO
{
    public class AdministratorDAO : DAO<Administrator>
    {
        public AdministratorDAO() {}
        public override bool Create(Administrator obj)
        {
            return false;
        }

        public override bool Delete(Administrator obj)
        {
            return false;
        }

        public override Administrator Find(int id)
        {
            Administrator administrator = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Administrator WHERE idAdmin = @id", connection);
                    cmd.Parameters.AddWithValue("id", id);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            administrator = new Administrator();
                            {
                                administrator.IdAdministrator = reader.GetInt32("idAdmin");
                            };
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur sql s'est produite!");
            }
            return administrator;
        }

        public override bool Update(Administrator obj)
        {
            return false;
        }
    }
}
