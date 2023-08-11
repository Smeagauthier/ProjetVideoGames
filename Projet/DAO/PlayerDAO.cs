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
    public class PlayerDAO : DAO<Player>
    {
        private string connectionString;

        public PlayerDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }

        public override bool Create(Player obj)
        {
            return false;
        }

        public override bool Delete(Player obj)
        {
            return false;
        }

        public override Player Find(int id)
        {
            Player player = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Player WHERE idPlayer = @id", connection);
                    cmd.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player = new Player();
                            {
                                player.IdPlayer = reader.GetInt32(reader.GetOrdinal("idPlayer")); // Utiliser GetOrdinal pour obtenir l'index de la colonne
                                player.Credit = reader.GetInt32(reader.GetOrdinal("credit"));
                                player.Pseudo = reader.GetString(reader.GetOrdinal("pseudo"));
                                player.RegistrationDate = reader.GetDateTime(reader.GetOrdinal("registrationDate"));
                                player.DateOfBirth = reader.GetDateTime(reader.GetOrdinal("dateOfBirth"));
                            };
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur sql s'est produite!");
            }
            return player;
        }




        public List<Player> FindAll()
        {
            List<Player> listAllPlayers = new List<Player>();
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Player", connection);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Player player = new Player();
                            {
                                player.IdPlayer = reader.GetInt32("idPlayer");
                                player.Credit = reader.GetInt32("credit");
                                player.Pseudo = reader.GetString("pseudo");
                                player.RegistrationDate = reader.GetDateTime("registrationDate");
                                player.DateOfBirth = reader.GetDateTime("dateOfBirth");
                            };
                            listAllPlayers.Add(player);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur sql s'est produite!");
            }
            return listAllPlayers;
        }

        public override bool Update(Player obj)
        {
            return false;
        }

        public bool CreatePlayer(Player player)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Début de la transaction
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand playerCommand = new SqlCommand(
                        "INSERT INTO dbo.Player (idUser, credit, pseudo, registrationDate, dateOfBirth) " +
                        "VALUES (@idUser, @credit, @pseudo, @registrationDate, @dateOfBirth)",
                        connection,
                        transaction
                    );
                    playerCommand.Parameters.AddWithValue("@idUser", player.IdUser);
                    playerCommand.Parameters.AddWithValue("@credit", player.Credit);
                    playerCommand.Parameters.AddWithValue("@pseudo", player.Pseudo);
                    playerCommand.Parameters.AddWithValue("@registrationDate", player.RegistrationDate);
                    playerCommand.Parameters.AddWithValue("@dateOfBirth", player.DateOfBirth);


                    // Exécuter les commandes SQL
                    int playerResult = playerCommand.ExecuteNonQuery();

                    if (playerResult > 0)
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
                    MessageBox.Show("Erreur lors de la création du joueur : " + ex.Message);
                }
            }

            return success;
        }
        

        public Player FindByUserId(int userId)
        {
            Player player = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        "SELECT * FROM dbo.Player WHERE idUser = @userId",
                        connection);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player = new Player();
                            {
                                player.IdPlayer = reader.GetInt32(reader.GetOrdinal("idPlayer"));
                                player.Credit = reader.GetInt32(reader.GetOrdinal("credit"));
                                player.Pseudo = reader.GetString(reader.GetOrdinal("pseudo"));
                                player.RegistrationDate = reader.GetDateTime(reader.GetOrdinal("registrationDate"));
                                player.DateOfBirth = reader.GetDateTime(reader.GetOrdinal("dateOfBirth"));
                            };
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur SQL s'est produite !");
            }
            return player;
        }

        //---- Méthode du cadeau d'anniverssaire (2crédits) ----//
        public bool AddBirthDayBonus(int playerId, int bonusCredits)
        {
            bool success = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();

                    SqlCommand updateCreditsCommand = new SqlCommand(
                        "UPDATE dbo.Player SET credit = credit + @bonusCredits WHERE idPlayer = @playerId",
                        connection);
                    updateCreditsCommand.Parameters.AddWithValue("@bonusCredits", bonusCredits);
                    updateCreditsCommand.Parameters.AddWithValue("@playerId", playerId);

                    int result = updateCreditsCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        success = true;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Erreur lors de l'ajout du bonus d'anniversaire : " + ex.Message);
            }
            return success;
        }

        public bool UpdateCredit(Player player)
        {
            bool success = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Player SET credit = @credit WHERE idPlayer = @idPlayer"; // Utilisez idPlayer au lieu de idUser
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@credit", player.Credit);
                command.Parameters.AddWithValue("@idPlayer", player.IdPlayer); // Utilisez idPlayer au lieu de idUser

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    success = rowsAffected > 0; 
                }
                catch (Exception ex)
                {
                    // Gérer les erreurs d'accès à la base de données
                    Console.WriteLine("Erreur lors de la mise à jour du joueur : " + ex.Message);
                }
            }
            return success;
        }

        public Player FindOwnerByIdVideoGame(int idVideoGame)
        {
            Player owner = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT P.* FROM dbo.Player P INNER JOIN dbo.Copy C ON P.idPlayer = C.owner WHERE C.idVideoGame = @idVideoGame", connection);
                    cmd.Parameters.AddWithValue("idVideoGame", idVideoGame);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.Read())
                        {
                           
                            owner = new Player()
                            {
                                IdPlayer = reader.GetInt32(reader.GetOrdinal("idPlayer")),
                                Credit = reader.GetInt32(reader.GetOrdinal("credit")),
                                Pseudo = reader.GetString(reader.GetOrdinal("pseudo")),
                                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("registrationDate")),
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("dateOfBirth"))
                            };
                        } 
                    }
                }
            }
            catch (SqlException sqle)
            {
                throw new Exception("Une erreur sql s'est produite dans le DAO!"+sqle.Message);
            }
            //MessageBox.Show("owner : " + owner.IdPlayer + " - " + owner.Pseudo); 
            return owner;
        }
    }
    


}
                

