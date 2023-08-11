using Projet.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projet.metier
{
    public class Booking
    {
        private int idBooking;
        private DateTime bookingDate;
        private Player player;
        private VideoGame videoGame;
        private int numberOfWeeks; 

        public Booking(int idBooking, DateTime bookingDate, Player player, VideoGame videoGame)
        {
            this.idBooking = idBooking;
            this.bookingDate = bookingDate;
            this.player = player;
            this.videoGame = videoGame;

        }
        public Booking()
        {

        }
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
        public List<Booking> FindBookingsForPlayer(int idPlayer)
        {
            BookingDAO bookingDAO = new BookingDAO();
            return bookingDAO.FindByPlayer(idPlayer);
        }

        public Booking Find(int idBooking)
        {
            
            BookingDAO bookingDAO = new BookingDAO();
            Booking booking = bookingDAO.Find(idBooking);
            return booking;
        }

        
    }
}
