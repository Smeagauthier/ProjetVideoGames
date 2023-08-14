using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Projet.DAO
{
    public class LoanDAO : DAO<Loan>
    {
        public new string connectionString;

        //Permet d'ajouter au constructeur la connexion à la BD en passant par le fichier "App.config"
        public LoanDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }

        //Permet de créer un prêt
        public override bool Create(Loan loan)
         {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Loan (StartDate, EndDate, Ongoing, Borrower, Lender, IdCopy) VALUES (@StartDate, @EndDate, @Ongoing, @Borrower, @Lender, @IdCopy)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = loan.StartDate;
                        cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = loan.EndDate;
                        cmd.Parameters.Add("@Ongoing", SqlDbType.Bit).Value = loan.Ongoing;
                        cmd.Parameters.Add("@Borrower", SqlDbType.Int).Value = loan.Borrower.IdPlayer;
                        cmd.Parameters.Add("@Lender", SqlDbType.Int).Value = loan.Copy.Owner.IdPlayer; 
                        cmd.Parameters.Add("@IdCopy", SqlDbType.Int).Value = loan.Copy.IdCopy;

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        return result > 0;  
                    }
                }
            }
        //Permet la suppression d'un prêt selon son id
        //pas utilisée
        public override bool Delete(Loan loan)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand updateCommand = new SqlCommand(
                        "DELETE dbo.Loan WHERE idLoan = @idLoan", connection, transaction);
                    updateCommand.Parameters.AddWithValue("@idBooking", loan.IdLoan);

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
                    throw new Exception("Erreur lors de la suppression de la réservation", ex);
                }
            }
            return success;
        }

        //Permet de faire une mise à jour complète d'une ligne dans la table "Loan"
        public override bool Update(Loan loan)
        {
            using (SqlConnection conn = new(connectionString))
            {
                string query = @"
                UPDATE Loan
                SET StartDate = @StartDate, 
                    EndDate = @EndDate,
                    Ongoing = @Ongoing,
                    IdCopy = @IdCopy
                WHERE idLoan = @idLoan";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StartDate", loan.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", loan.EndDate);
                    cmd.Parameters.AddWithValue("@Ongoing", loan.Ongoing);
                    cmd.Parameters.AddWithValue("@IdCopy", loan.Copy.IdCopy);
                    cmd.Parameters.AddWithValue("@idLoan", loan.IdLoan);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //Permet de chercher un prêt (ses valeurs) selon un id donné
        //pas utilisée
        public override Loan Find(int id)
        {
            Loan loan = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Loan WHERE idLoan = @id", connection);
                    cmd.Parameters.AddWithValue("id", id);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            loan = new Loan();
                            {
                                loan.IdLoan = reader.GetInt32("idLoan");
                                loan.StartDate = reader.GetDateTime("startDate");
                                loan.EndDate = reader.GetDateTime("endDate");
                                loan.Ongoing = reader.GetBoolean("ongoing");
                            };
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur sql s'est produite!");
            }
            return loan;
        }
        // ---- FIN DU DAO ----//

        //Permet de récupérer et retourner tous les jeux vidéo associés au possesseur du jeu vidéo (owner).
        public List<VideoGame> GetPlayerVideoGames(Player player)
        {
            List<VideoGame> videoGames = new List<VideoGame>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT * FROM VideoGame WHERE idVideoGame IN (SELECT idVideoGame FROM Copy WHERE owner = @idPlayer)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@idPlayer", player.IdPlayer);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        VideoGame videoGame = new VideoGame();
                        videoGame.IdVideoGame = reader.GetInt32(reader.GetOrdinal("idVideoGame"));
                        videoGame.Name = reader.GetString(reader.GetOrdinal("name"));
                        videoGame.Console = reader.GetString(reader.GetOrdinal("console"));
                        videoGame.CreditCost = reader.GetInt32(reader.GetOrdinal("creditcost"));
                        videoGames.Add(videoGame);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return videoGames;
        }

        //Permet de récupérer et retourner tous les prêts (Loan) actifs associés à celui qui emprunte la copie du jeu vidéo
        //et si le "en cours" est sur 1 c'est que la copie est en cour de prêt.
        public List<Loan> GetPlayerLoans(Player player)
        {
            List<Loan> loans = new List<Loan>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT L.*, C.*, VG.* 
            FROM Loan L
            INNER JOIN Copy C ON L.idCopy = C.idCopy
            INNER JOIN VideoGame VG ON C.idVideoGame = VG.idVideoGame
            WHERE 
            L.borrower = @playerId AND L.Ongoing = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@PlayerId", SqlDbType.Int).Value = player.IdPlayer;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Loan loan = new Loan
                            {
                                IdLoan = reader.GetInt32(reader.GetOrdinal("idLoan")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                Ongoing = reader.GetBoolean(reader.GetOrdinal("Ongoing")),
                                Borrower = new Player { IdPlayer = reader.GetInt32(reader.GetOrdinal("Borrower")) },
                                Lender = new Player { IdPlayer = reader.GetInt32(reader.GetOrdinal("Lender")) }

                            };

                            Copy copy = new Copy
                            {
                                IdCopy = reader.GetInt32(reader.GetOrdinal("idCopy")),
                            };

                            VideoGame videoGame = new VideoGame
                            {
                                IdVideoGame = reader.GetInt32(reader.GetOrdinal("idVideoGame")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CreditCost = reader.GetInt32(reader.GetOrdinal("CreditCost")),
                                Console = reader.GetString(reader.GetOrdinal("Console")),
                            };

                            copy.VideoGame = videoGame;
                            loan.Copy = copy;

                            loans.Add(loan);
                        }

                    }
                }
            }
            return loans;
        }
        //Elle permet de récupérer le prêt actuel associé à une copie spécifique de jeu vidéo.
        public Loan GetCurrentLoanForCopy(Copy copy)
        {
            Loan currentLoan = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Loan WHERE idCopy = @idCopy AND Ongoing = 1", connection))
                    {
                        cmd.Parameters.AddWithValue("@idCopy", copy.IdCopy);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentLoan = new Loan
                                {
                                    IdLoan = reader.GetInt32(reader.GetOrdinal("idLoan")),
                                    Copy = copy,
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                    Ongoing = reader.GetBoolean(reader.GetOrdinal("Ongoing")),
                                    Borrower = new Player { IdPlayer = reader.GetInt32(reader.GetOrdinal("Borrower")) },
                                    Lender = new Player { IdPlayer = reader.GetInt32(reader.GetOrdinal("Lender")) }
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Une erreur SQL s'est produite: " + e.Message);
            }

            return currentLoan;
        }

        //Permet de supprimer les prêts selon l'id de la copie
        //Fait référence à la suppresion d'un jeu vidéo (Contraintes...)
        //Supprimer un jeu vidéo => une copie => un prêt
        public bool DeleteAllLoansForCopy(Copy copy)
        {
            string deleteQuery = "DELETE FROM dbo.Loan WHERE idCopy = @idCopy";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@idCopy", copy.IdCopy);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        return true; // Retournez true si la suppression a réussi
                    }
                    catch (Exception)
                    {
                        return false; // Retournez false si une erreur se produit
                    }
                }
            }

        }

    }
}
