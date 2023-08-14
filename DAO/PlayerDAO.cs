using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;

namespace Projet.DAO
{
    public class PlayerDAO : DAO<Player>
    {
        private new string connectionString;

        //Permet d'ajouter au constructeur la connexion à la BD en passant par le fichier "App.config"
        public PlayerDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }

        //Permet de créer un player lors d'une inscription
        public override bool Create(Player player)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

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

        //Méthode pas utilisée
        public override bool Delete(Player player)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand updateCommand = new SqlCommand(
                        "DELETE dbo.Player WHERE idPlayer = @idPlayer", connection, transaction);
                    updateCommand.Parameters.AddWithValue("@idPlayer", player.IdPlayer);

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
                    throw new Exception("Erreur lors de la suppression d'un joueur", ex);
                }
            }
            return success;
        }

        //Permet de chercher un joueur selon son idPlayer
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
                throw new Exception("Une erreur sql s'est produite!");
            }
            return player;
        }



        //Méthode pas utilisée
        //Permet d'afficher tout les joueurs
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

        //Méthode pas utilisée
        //Permet de mettre à jour un joueur 
        public override bool Update(Player obj)
        {
            return false;
        }

        
        //Permet de faire une recherche d'un joueur sur l'idUser, ce qui signifie
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
        //Permet de mettre à jour les crédits que possède un joueur
        public bool UpdateCredit(Player player)
        {
            bool success = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Player SET credit = @credit WHERE idPlayer = @idPlayer"; 
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@credit", player.Credit);
                command.Parameters.AddWithValue("@idPlayer", player.IdPlayer); 

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    success = rowsAffected > 0; 
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la mise à jour du joueur : " + ex.Message);
                }
            }
            return success;
        }

        //Permet de trouver la personne qui possède la copie d'un jeu vidéo 
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
            return owner;
        }
    }
    


}
                

