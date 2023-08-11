using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.DAO
{
    public class LoanDAO : DAO<Loan>
    {
        private string connectionString;

        public LoanDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }

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




        public override bool Delete(Loan obj)
        {
            return false;
        }

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

        public List<Loan> FindLoansLenderByPlayer(Player player)
        {
            List<Loan> listLoansLenderByPlayer = new List<Loan>();
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    string query = @"
                SELECT L.* 
                FROM dbo.Loan AS L
                JOIN dbo.Copy AS C ON L.IdCopy = C.IdCopy
                WHERE L.lender = C.owner AND C.owner = @id";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("id", player.IdPlayer);

                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Loan loan = new Loan
                            {
                                IdLoan = reader.GetInt32("idLoan"),
                            };

                            listLoansLenderByPlayer.Add(loan);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur sql s'est produite!");
            }
            return listLoansLenderByPlayer;
        }


        public List<Loan> FindLoansBorrowerByPlayer(Player player)
        {
            List<Loan> listLoansBorrowerByPlayer = new List<Loan>();
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Loan WHERE borrower IN (SELECT idPlayer FROM dbo.Player WHERE idPlayer = @id)", connection);
                    cmd.Parameters.AddWithValue("id", player.IdPlayer);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Loan loan = new Loan();
                            {
                                loan.IdLoan = reader.GetInt32("idLoan");
                            };
                            listLoansBorrowerByPlayer.Add(loan);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur sql s'est produite!");
            }
            return listLoansBorrowerByPlayer;
        }

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
                                // remplissez les propriétés du prêt en supposant que vous ayez des colonnes correspondantes
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

        public List<Loan> FindLoansByBooking(int idBooking)
        {
            List<Loan> loans = new List<Loan>();
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                                    "SELECT l.idLoan, l.ongoing " +
                                    "FROM dbo.Loan l " +
                                    "JOIN dbo.Copy c ON l.idCopy = c.idCopy " +
                                    "JOIN dbo.Booking b ON c.idVideoGame = b.idVideoGame " +
                                    "WHERE b.idBooking = @idBooking", connection); cmd.Parameters.AddWithValue("@idBooking", idBooking);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Loan loan = new Loan
                            {
                                IdLoan = reader.GetInt32(reader.GetOrdinal("idLoan")),
                                Ongoing = reader.GetBoolean(reader.GetOrdinal("ongoing"))
                            };

                            loans.Add(loan);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur SQL s'est produite lors de la recherche des prêts pour la réservation !");
            }
            return loans;
        }

    }
}
