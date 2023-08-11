using Projet.metier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projet.DAO
{
    public class BookingDAO : DAO<Booking>
    {
        public BookingDAO()
        {
        }

        public BookingDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public override bool Create(Booking obj)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Booking (bookingDate, idPlayer, idVideoGame, numberOfWeeks) VALUES (@bookingDate, @idPlayer, @idVideoGame, @numberOfWeeks);", connection);
                    cmd.Parameters.AddWithValue("@bookingDate", DateTime.Now); // Utilise la date actuelle comme date de réservation
                    cmd.Parameters.AddWithValue("@idPlayer", obj.Player.IdPlayer); // Utilise l'idPlayer du currentPlayer
                    cmd.Parameters.AddWithValue("@idVideoGame", obj.VideoGame.IdVideoGame); // Utilise l'idVideoGame du jeu vidéo sélectionné
                    cmd.Parameters.AddWithValue("@numberOfWeeks", obj.NumberOfWeeks); // Utilise le nombre de semaines
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur SQL s'est produite lors de la création de la réservation !");
            }
        }

        public override bool Delete(Booking booking)
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
                        "DELETE dbo.Booking WHERE idBooking = @idBooking",
                        connection,
                        transaction
                    );
                    updateCommand.Parameters.AddWithValue("@idBooking", booking.IdBooking);

                    // Exécuter la commande
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

        public override Booking Find(int id)
        {
            Booking booking = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Booking WHERE idBooking = @id", connection);
                    cmd.Parameters.AddWithValue("@id", id); 
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            booking = new Booking()
                            {
                                IdBooking = reader.GetInt32(reader.GetOrdinal("idBooking")),
                                BookingDate = reader.GetDateTime(reader.GetOrdinal("bookingDate")),
                                NumberOfWeeks = reader.GetInt32(reader.GetOrdinal("numberOfWeeks"))
                            };
                            int videoGameId = reader.GetInt32(reader.GetOrdinal("idVideoGame"));

                            // Utiliser VideoGameDAO pour charger les informations du jeu vidéo
                            VideoGameDAO videoGameDAO = new VideoGameDAO();
                            VideoGame videoGame = videoGameDAO.Find(videoGameId);

                            // Associer le jeu vidéo à la réservation
                            booking.VideoGame = videoGame;
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur sql s'est produite !");
            }
            return booking;
        }

        public override bool Update(Booking obj)
        {
            return false;
        }

        public List<Booking> FindByPlayer(int idPlayer)
        {
            List<Booking> bookings = new List<Booking>();
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        "SELECT b.idBooking, b.bookingDate, b.numberOfWeeks, v.name AS VideoGameName, v.creditCost " +
                        "FROM dbo.Booking b " +
                        "JOIN dbo.VideoGame v ON b.idVideoGame = v.idVideoGame " +
                        "WHERE b.idPlayer=@idPlayer",
                        connection);

                    cmd.Parameters.AddWithValue("@idPlayer", idPlayer);
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int bookingId = reader.GetInt32(reader.GetOrdinal("idBooking"));
                            DateTime bookingDate = reader.GetDateTime(reader.GetOrdinal("bookingDate"));

                            int numberOfWeeksOrdinal = reader.GetOrdinal("numberOfWeeks");
                            int numberOfWeeks = reader.IsDBNull(numberOfWeeksOrdinal) ? 0 : reader.GetInt32(numberOfWeeksOrdinal);

                            string videoGameName = reader.GetString(reader.GetOrdinal("VideoGameName"));
                            int creditCost = reader.GetInt32(reader.GetOrdinal("creditCost"));

                            Booking booking = new Booking
                            {
                                IdBooking = bookingId,
                                BookingDate = bookingDate,
                                NumberOfWeeks = numberOfWeeks,
                                VideoGame = new VideoGame
                                {
                                    Name = videoGameName,
                                    CreditCost = creditCost
                                }
                            };

                            bookings.Add(booking);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur SQL s'est produite lors de la recherche des réservations !");
            }
            return bookings;
        }
        //Créer un loan pour récupérer la liste des réservations
        public List<Booking> GetAllBookingsForVideoGame(VideoGame videoGame)
        {
            List<Booking> bookings = new List<Booking>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Booking WHERE idVideoGame = @idVideoGame", connection))
                {
                    command.Parameters.AddWithValue("@idVideoGame", videoGame.IdVideoGame);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Booking booking = new Booking
                            {
                                IdBooking = reader.GetInt32(reader.GetOrdinal("IdBooking")),
                                BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                                NumberOfWeeks = reader.GetInt32(reader.GetOrdinal("NumberOfWeeks"))
                            };
                            bookings.Add(booking);
                        }
                    }
                }
            }
            return bookings;
        }

        public List<Player> GetBookerPlayersForCopy(int idCopy)
        {
            List<Player> bookerPlayers = new List<Player>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SELECT DISTINCT p.* FROM Player p INNER JOIN Booking b ON p.idPlayer = b.idPlayer WHERE b.idVideoGame = (SELECT idVideoGame FROM Copy WHERE idCopy = @idCopy)", connection))
                    {
                        command.Parameters.AddWithValue("@idCopy", idCopy);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Player player = new Player
                                {
                                    IdPlayer = reader.GetInt32(reader.GetOrdinal("idPlayer")),
                                    Pseudo = reader.GetString(reader.GetOrdinal("pseudo")),
                                    Credit = reader.GetInt32(reader.GetOrdinal("credit")),
                                    RegistrationDate = reader.GetDateTime(reader.GetOrdinal("registrationDate")),
                                    DateOfBirth = reader.GetDateTime(reader.GetOrdinal("dateOfBirth")),
                                };
                                bookerPlayers.Add(player);
                            }
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur SQL s'est produite lors de la recherche des joueurs ayant réservé la même copie !");
            }

            return bookerPlayers;
        }

        //Méthode pour empêcher un currentPlayer de réserver 2x le même jeu
        public bool GetBookingsByPlayer(int idPlayer, int idVideoGame)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Booking WHERE idPlayer = @idPlayer AND idVideoGame = @idVideoGame", connection))
                    {
                        command.Parameters.AddWithValue("@idPlayer", idPlayer);
                        command.Parameters.AddWithValue("@idVideoGame", idVideoGame);

                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("Une erreur SQL s'est produite lors de la vérification de l'existence d'une réservation !");
            }
        }


    }
}
