using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Projet.DAO
{
    public class CopyDAO : DAO<Copy>
    {
        private new string connectionString;

        //Permet d'ajouter au constructeur la connexion à la BD en passant par le fichier "App.config"
        public CopyDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }

        // Permet la création d'une copie d'un jeu vidéo
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

        //Permet la suppression d'une copie d'un jeu vidéo
        //Méthode pas utilisée
        public override bool Delete(Copy copy)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand updateCommand = new SqlCommand(
                        "DELETE dbo.Copy WHERE idCopy = @idCopy", connection, transaction);
                    updateCommand.Parameters.AddWithValue("@idCopy", copy.IdCopy);

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
                    throw new Exception("Erreur lors de la suppression de la copie", ex);
                }
            }
            return success;
        }

        //Permet la recherche d'une copie selon son ID
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

        //Permet la mise à jour d'une colonne (idVideoGame) de la table Copy
        public override bool Update(Copy copy)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand updateCommand = new SqlCommand(
                        "UPDATE dbo.Copy SET idVideoGame = @idVideoGame WHERE idCopy = @idCopy", connection, transaction);
                    updateCommand.Parameters.AddWithValue("@idCopy", copy.IdCopy);
                    updateCommand.Parameters.AddWithValue("@idVideoGame", copy.VideoGame);

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
                    MessageBox.Show("Erreur lors de la mise à jour de la copie correspondant au jeu vidéo: " + ex.Message);
                }
            }

            return success;
        }


        //---- FIN DU DAO ----//

        //Récupère la liste des copies selon le jeu vidéo 
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
        //Méthode en relation avec la suppression d'un jeu vidéo dans la page ADMIN
        //Contrainte sur l'idVideoGame et idCopy dans différentes tables
        public bool DeleteAllCopiesForVideoGame(VideoGame videoGame)
        {
            //On identifie toutes les copies associées au jeu vidéo
            string selectCopiesQuery = "SELECT idCopy FROM dbo.Copy WHERE idVideoGame = @idVideoGame";

            List<int> copyIds = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(selectCopiesQuery, connection))
                {
                    command.Parameters.AddWithValue("@idVideoGame", videoGame.IdVideoGame);
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            copyIds.Add((int)reader["idCopy"]);
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            //Pour chaque ID de copie, on supprime tout les prêts associés
            foreach (int copyId in copyIds)
            {
                Copy copy = new Copy() { IdCopy = copyId };
                LoanDAO loanDAO = new LoanDAO();
                if (!loanDAO.DeleteAllLoansForCopy(copy))
                {
                    return false;
                }
            }

            //On supprime toutes les copies associées au jeu vidéo donné
            string deleteCopiesQuery = "DELETE FROM dbo.Copy WHERE idVideoGame = @idVideoGame";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(deleteCopiesQuery, connection))
                {
                    command.Parameters.AddWithValue("@idVideoGame", videoGame.IdVideoGame);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }






    }
}
