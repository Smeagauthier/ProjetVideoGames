using Projet.DAO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projet.metier
{
    public class Player : User
    {
        private int idPlayer;
        private int credit;
        private string pseudo;
        private DateTime registrationDate;
        private DateTime dateOfBirth;
        private User user;
        private List<Booking> listBookingsPlayer;
        private List<Copy> listCopy;
        private List<Loan> listLender;
        private List<Loan> listBorrower;
        private PlayerDAO playerDAO;
        private DateTime lastBonusDate;//Variable utilisée pour créer la condition des 2 crédits (1x par jour/ans)
        private int idVideoGame;

        public static Player CurrentPlayer { get; set; }




        public Player()
        {
            playerDAO = new PlayerDAO();
        }

        public Player(int idPlayer,int credit,string pseudo,DateTime registrationDate,DateTime dateOfBirth,User user)
        {
            this.idPlayer = idPlayer;
            this.credit = credit;   
            this.pseudo = pseudo;  
            this.registrationDate = registrationDate;
            this.dateOfBirth = dateOfBirth;
            this.user = user;
            string connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
            this.playerDAO = new PlayerDAO();
            this.idVideoGame = idVideoGame; 
        }

        public Player(string userName, string password) : base(userName, password)
        {
        }

        public int IdPlayer {
            get { return idPlayer; }
            set { idPlayer = value; }
        }

        public int Credit
        {
            get { return credit; }
            set { credit = value; }
        }

        public string Pseudo
        {
            get { return pseudo; }
            set { pseudo = value; }
        }

        public DateTime RegistrationDate
        {
            get { return registrationDate; }
            set { registrationDate = value; }
        }

        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
        }

        public User User
        {
            get { return user; }
            set { user = value; }
        }

        public List<Booking> ListBookingsPlayer
        {
            get { return listBookingsPlayer; }
            set { listBookingsPlayer = value; }
        }

        public List<Copy> ListCopy
        {
            get { return listCopy; }
            set { listCopy = value; }
        }

        public List<Loan> ListLender
        {
            get { return listLender; }
            set { listLender = value; }
        }

        public List<Loan> ListBorrower
        {
            get { return listBorrower; }
            set { listBorrower = value; }
        }

        public DateTime LastBonusDate
        {
            get { return lastBonusDate; }
            set { lastBonusDate = value; }
        }
        //****************** Méthodes ****************** //
        public bool LoanAllowed()
        {
            return Credit > 0;
        }

        // ---- Cadeau 2 crédits anniversaire ---- //
        public void AddBirthDayBonus()
        {
            if (IsBirthday())
            {
                Credit += 2;
                playerDAO.UpdateCredit(this); // Appel à votre classe PlayerDAO pour mettre à jour le joueur dans la base de données
                MessageBox.Show("Joyeux anniversaire ! Vous avez reçu un bonus de 2 crédits !");
            }
        }

        private bool IsBirthday()
        {
            DateTime today = DateTime.Today;
            return today.Month == DateOfBirth.Month && today.Day == DateOfBirth.Day;
        }
        // ----- ---- //

        public bool UpdateCredit(Player player)
        {
            PlayerDAO playerDAO = new PlayerDAO();
            return playerDAO.UpdateCredit(this);
        }

        public Player Find(int idPlayer)
        {
            PlayerDAO playerDAO = new PlayerDAO();
            return playerDAO.Find(this.idPlayer);
        }

        public Player FindOwnerByIdVideoGame(int idVideoGame)
        {
            PlayerDAO playerDAO = new PlayerDAO();
            return playerDAO.FindOwnerByIdVideoGame(idVideoGame);
        }

    }
}
