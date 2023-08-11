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
    public class CopyDAO : DAO<Copy>
    {
        public CopyDAO(){}

        public CopyDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public override bool Create(Copy copy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Copy (owner, idVideoGame) SELECT @idPlayer, @idVideoGame WHERE NOT EXISTS (SELECT 1 FROM Copy WHERE owner = @idPlayer AND idVideoGame = @idVideoGame)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idPlayer", copy.Owner.IdPlayer);
                        command.Parameters.AddWithValue("@idVideoGame", copy.VideoGame.IdVideoGame);

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la création de la copie : " + ex.Message);
            }
        }

        public override bool Delete(Copy obj)
        {
            return false;
        }

        public override Copy Find(int id)
        {
            Copy copy = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Copy WHERE idCopy = @id", connection);
                    cmd.Parameters.AddWithValue("id", id);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            copy = new Copy();
                            {                      
                                copy.IdCopy = reader.GetInt32("idCopy");
                            };
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur sql s'est produite!");
            }
            return copy;
        }

        public List<Copy> FindCopiesByVideoGame(VideoGame videoGame,Player player)
        {
            List<Copy> listCopiesByVideoGame = new List<Copy>();
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("select * from dbo.Copy where idVideoGame in (select idVideoGame from dbo.VideoGame where idVideoGame = @id)", connection);
                    cmd.Parameters.AddWithValue("@id", videoGame.IdVideoGame);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idCopy = reader.GetInt32(0);
                            int idPlayer = reader.GetInt32(1); // ID du propriétaire de la copie dans la table Copy
                            int idVideoGame = reader.GetInt32(2);

                            // ID pour obtenir l'instance appropriée de VideoGame
                            VideoGame vg = videoGame.Find(idVideoGame);

                            // ID pour obtenir l'instance appropriée de Player en utilisant idPlayer de la table Copy
                            Player owner = player.FindOwnerByIdVideoGame(idVideoGame);

                            Copy copy = new Copy(idCopy, owner, vg);
                            listCopiesByVideoGame.Add(copy);
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                throw new Exception("Une erreur sql s'est produite!"+sqle.Message);
            }
            return listCopiesByVideoGame;
        }

        public override bool Update(Copy obj)
        {
            return false;
        }
    }
}
