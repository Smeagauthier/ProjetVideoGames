using Projet.DAO;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        //---- Méthodes supplémentaires ---- //

        //Permet de vérifier si les crédits sont supérieurs à 0
        public bool LoanAllowed()
        {
            return Credit > 0;
        }

       
        //Permet d'ajouter 2 crédits lorsque la date d'anniversaire correspond à la currentDate (date du jour)
        public void AddBirthDayBonus()
        {
            if (IsBirthday())
            {
                Credit += 2;
                playerDAO.UpdateCredit(this); // met à jour les crédits du joueur dans la base de données
                MessageBox.Show("Joyeux anniversaire ! Vous avez reçu un bonus de 2 crédits !");
            }
        }

        //Permet de vérifier si la date actuel correspond à la date d'anniversaire d'une personne
        //jour et mois, pas l'année
        private bool IsBirthday()
        {
            DateTime today = DateTime.Today;
            return today.Month == DateOfBirth.Month && today.Day == DateOfBirth.Day;
        }
        // ---- Méthodes du DAO ---- //

        public bool Create()
        {
            PlayerDAO playerDAO = new PlayerDAO();
            return playerDAO.Create(this);
        }

        public bool UpdateCredit()
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
