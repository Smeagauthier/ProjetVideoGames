using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace Projet.DAO
{
    public class BookingDAO : DAO<Booking>
    {
        private string connectionString;
        //Permet d'ajouter au constructeur la connexion à la BD en passant par le fichier "App.config"
        public BookingDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }

        //Création d'une réservation en bd
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
                    cmd.Parameters.AddWithValue("@numberOfWeeks", obj.NumberOfWeeks); // Utilise le nombre de semaines de la réservation (Formulaire)
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

        //Suppression d'une réservation
        public override bool Delete(Booking booking)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand updateCommand = new SqlCommand(
                        "DELETE dbo.Booking WHERE idBooking = @idBooking",
                        connection,
                        transaction
                    );
                    updateCommand.Parameters.AddWithValue("@idBooking", booking.IdBooking);

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
        //Recherche d'une réservation selon un id fourni
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

                            VideoGameDAO videoGameDAO = new VideoGameDAO();
                            VideoGame videoGame = videoGameDAO.Find(videoGameId);

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
        //Met à jour  le nombre de semaine de la réservation selectionnée par ID (passage en paramètre par objet)
        //NON UTILISEE AILLEURS
        public override bool Update(Booking booking)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand updateCommand = new SqlCommand(
                        "UPDATE dbo.Booking SET numberOfWeeks = @numberOfWeeks WHERE idBooking = @idBooking",
                        connection,
                        transaction
                    );
                    updateCommand.Parameters.AddWithValue("@numberOfWeeks", booking.NumberOfWeeks);
                    updateCommand.Parameters.AddWithValue("@idBooking", booking.IdBooking);

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
                    MessageBox.Show("Erreur lors de la mise à jour du nombres de semaines : " + ex.Message);
                }
            }

            return success;
        }

        //---- FIN DU DAO ---- //

        //Sert à récupérer toutes les réservations associées à un jeu vidéo (pour les afficher)
        public List<Booking> GetAllBookingsForVideoGame(VideoGame videoGame)
        {
            //Création d'une liste vide qui stockera les réservations récupérées depuis la bd
            List<Booking> bookings = new List<Booking>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Booking WHERE idVideoGame = @idVideoGame", connection))
                {
                    command.Parameters.AddWithValue("@idVideoGame", videoGame.IdVideoGame);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //Pour chaque ligne, création d'une réservation en remplissant avec les données de la bd
                        while (reader.Read())
                        {
                            Booking booking = new Booking
                            {
                                IdBooking = reader.GetInt32(reader.GetOrdinal("IdBooking")),
                                BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                                NumberOfWeeks = reader.GetInt32(reader.GetOrdinal("NumberOfWeeks")),
                                Player = new Player { IdPlayer = reader.GetInt32(reader.GetOrdinal("IdPlayer")) },
                                VideoGame = new VideoGame { IdVideoGame = reader.GetInt32(reader.GetOrdinal("IdVideoGame")) }
                            };
                            bookings.Add(booking);
                        }
                    }
                }
            }
            //On retourne la liste des réservations afin de tous les afficher
            return bookings;
        }

        //Permet de toutes les réservations (Booking) associées à un joueur spécifique, identifié par son idPlayer
        public List<Booking> FindBookingsByPlayer(int idPlayer)
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
                            // Récupère les données en bd
                            int bookingId = reader.GetInt32(reader.GetOrdinal("idBooking"));
                            DateTime bookingDate = reader.GetDateTime(reader.GetOrdinal("bookingDate"));
                            int numberOfWeeksOrdinal = reader.GetOrdinal("numberOfWeeks");
                            int numberOfWeeks = reader.IsDBNull(numberOfWeeksOrdinal) ? 0 : reader.GetInt32(numberOfWeeksOrdinal);
                            string videoGameName = reader.GetString(reader.GetOrdinal("VideoGameName"));
                            int creditCost = reader.GetInt32(reader.GetOrdinal("creditCost"));

                            //Création d'une nouvelle réservation qui s'ajoutera à la liste des réservations
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

        //Permet de supprimer les réservations selon un jeu vidéo
        public bool DeleteAllBookingsForVideoGame(VideoGame videoGame)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("DELETE FROM dbo.Booking WHERE idVideoGame = @idVideoGame", connection))
                {
                    command.Parameters.AddWithValue("@idVideoGame", videoGame.IdVideoGame);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

    }
}
