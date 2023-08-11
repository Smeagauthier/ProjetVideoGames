using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projet.DAO
{
    public class VideoGameDAO : DAO<VideoGame>
    {
        private string connectionString;
        public VideoGameDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }

        public override bool Create(VideoGame videoGame)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO VideoGame (Name, Console, CreditCost) VALUES (@Name, @Console, @CreditCost)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", videoGame.Name);
                    command.Parameters.AddWithValue("@Console", videoGame.Console);
                    command.Parameters.AddWithValue("@CreditCost", videoGame.CreditCost);

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    return result > 0;
                }
            }
        }

        public override bool Delete(VideoGame videoGame)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Début de la transaction
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand updateCommand = new SqlCommand(
                        "DELETE dbo.VideoGame WHERE idVideoGame = @idVideoGame",
                        connection,
                        transaction
                    );
                    updateCommand.Parameters.AddWithValue("@idVideoGame", videoGame.IdVideoGame);

                    // Exécuter la commande de mise à jour du crédit
                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                        success = true;
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Erreur lors de la supression du jeu vidéo : " + ex.Message);
                }
            }

            return success;
        }

        public override VideoGame Find(int id)
        {
            VideoGame videoGame = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.VideoGame WHERE idVideoGame = @id", connection);
                    cmd.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            videoGame = new VideoGame();
                            {
                                videoGame.IdVideoGame = reader.GetInt32("idVideoGame");
                                videoGame.Name = reader.GetString("name");
                                videoGame.CreditCost = reader.GetInt32("creditCost");
                                videoGame.Console = reader.GetString("console");
                                //videoGame.ListBookingsVideoGame = new List<Booking>(); 
                                //videoGame.ListCopyVideoGame = new List<Copy>(); 

                            };
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur sql s'est produite!");
            }
            return videoGame;
        }
        //---- Affiche la liste des jeux vidéos dans HomeAdmin ----//
        public List<VideoGame> FindAll()
        {
            List<VideoGame> videoGames = new List<VideoGame>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.VideoGame", connection);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VideoGame videoGame = new VideoGame();
                            videoGame.IdVideoGame = reader.GetInt32("idVideoGame");
                            videoGame.Name = reader.GetString("name");
                            videoGame.CreditCost = reader.GetInt32("creditCost");
                            videoGame.Console = reader.GetString("console");
                            // Assignez d'autres propriétés si nécessaires

                            videoGames.Add(videoGame);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur SQL s'est produite !");
            }

            return videoGames;
        }





        public override bool Update(VideoGame obj)
        {
            return false;
        }
        //---- Modifie le nombre de crédit dans HomeAdmin ----//
        public bool UpdateCredit(VideoGame videoGame)
        {
            bool success = false;

           /* if (videoGame.CreditCost < 1 || videoGame.CreditCost > 10)
            {
                MessageBox.Show("Le crédit doit être un chiffre entre 1 et 10 uniquement.");
                return false;
            }*/

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Début de la transaction
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand updateCommand = new SqlCommand(
                        "UPDATE dbo.VideoGame SET creditCost = @creditCost WHERE idVideoGame = @idVideoGame",
                        connection,
                        transaction
                    );
                    updateCommand.Parameters.AddWithValue("@creditCost", videoGame.CreditCost);
                    updateCommand.Parameters.AddWithValue("@idVideoGame", videoGame.IdVideoGame);

                    // Exécuter la commande de mise à jour du crédit
                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                        success = true;
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Erreur lors de la mise à jour du crédit du jeu vidéo : " + ex.Message);
                }
            }

            return success;
        }


    }
}
