using Projet.DAO;
using System;
using System.Collections.Generic;

namespace Projet.metier
{
    public class Booking
    {
        private int idBooking;
        private DateTime bookingDate;
        private Player player;
        private VideoGame videoGame;
        private int numberOfWeeks;

        //---- Constructeur ----//

        public Booking()
        {

        }

        public Booking(int idBooking, DateTime bookingDate, Player player, VideoGame videoGame)
        {
            this.idBooking = idBooking;
            this.bookingDate = bookingDate;
            this.player = player;
            this.videoGame = videoGame;

        }
        

        //---- Getter et Setter ----//
        public int IdBooking{ 
            get {return idBooking; } 
            set { idBooking = value; } 
        }

        public DateTime BookingDate
        {
            get { return bookingDate; }
            set { bookingDate = value; }
        }

        public Player Player
        {
            get { return player; }
            set { player = value; }
        }

        public VideoGame VideoGame
        {
            get { return videoGame; }
            set { videoGame = value; }
        }
        public int NumberOfWeeks
        {
            get { return numberOfWeeks; }
            set { numberOfWeeks = value; }
        }

        public Copy Copy 
        { 
            get { return Copy; }
            set { Copy = value; }
        }

        //---- Méthodes DAO ----

        public bool Create()
        {
            BookingDAO bookingDAO = new BookingDAO();
            return bookingDAO.Create(this);
        }

        public bool Delete()
        {
            BookingDAO bookingDAO = new BookingDAO();
            return bookingDAO.Delete(this);
        }

        public Booking Find(int idBooking)
        {
            
            BookingDAO bookingDAO = new BookingDAO();
            Booking booking = bookingDAO.Find(idBooking);
            return booking;
        }

        //---- Méthodes supplémentaires ----//
       
        //Permet de créer un bojet bookingDAO et puis de faire appel à la méthode situé dans le DAO
        //qui va permettre de retourner les bookings d'un player en utilisant un idPlayer en paramètre
        public List<Booking> FindBookingsForPlayer(int idPlayer)
        {
            BookingDAO bookingDAO = new BookingDAO();
            return bookingDAO.FindBookingsByPlayer(idPlayer);
        }

        //Fait référence à la méthode GetAllBookingsForVideoGame
        public List<Booking> GetAllBookingsForVideoGame() 
        {
            BookingDAO bookingDAO = new BookingDAO();
            return bookingDAO.GetAllBookingsForVideoGame(this.videoGame);
        }

        //Fait référence à la méthode DeleteAllBookingsForVideoGame dans le DAO
        public bool DeleteAllBookingsForVideoGame(VideoGame videoGame)
        {
            BookingDAO bookingDAO = new BookingDAO();
            return bookingDAO.DeleteAllBookingsForVideoGame(videoGame);
        }
    }
}
