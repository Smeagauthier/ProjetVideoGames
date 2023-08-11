using Projet.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projet.metier
{
    public class Loan
    {
        private int idLoan;
        private DateTime startDate;
        private DateTime endDate;
        private bool ongoing;
        private Copy copy;
        private Player borrower;
        private Player lender;
        private string connectionString;

        public Loan()
        {

        }

        public Loan(int idLoan, DateTime startDate, DateTime endDate, bool ongoing, Copy copy)
        {
            this.idLoan = idLoan;
            this.startDate = startDate;
            this.endDate=  endDate;
            this.ongoing = ongoing;
            this.copy = copy; 
        }

        public Loan(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int IdLoan
        {
            get { return idLoan; }
            set { idLoan = value; }
        }

        public bool Ongoing
        {
            get { return ongoing; }
            set { ongoing = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        public Copy Copy
        {
            get { return copy; }    
            set { copy = value; }
        }

        public Player Borrower
        {
            get { return borrower; }
            set { borrower = value; }
        }

        public Player Lender
        {
            get { return lender; }
            set { lender = value; }
        }

        public static List<VideoGame> GetPlayerVideoGames(Player player, string connectionString)
        {
            List<VideoGame> videoGames = new List<VideoGame>();
            try
            {
                LoanDAO loanDAO = new LoanDAO();
                videoGames = loanDAO.GetPlayerVideoGames(player);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur");
            }
            return videoGames;
        }

        public List<Loan> GetPlayerLoans(Player player)
        {
            LoanDAO loanDAO = new LoanDAO();
            return loanDAO.GetPlayerLoans(player);
        }

        public void CalculateBalance()
        {
            if (this != null && this.Ongoing && DateTime.Now > this.EndDate)
            {
                TimeSpan delay = DateTime.Now - this.EndDate;
                int delayDays = (int)Math.Ceiling(delay.TotalDays);
                int weeks = (int)Math.Ceiling(delayDays / 7.0);
                int penalty = 0; 

                penalty = (weeks * delayDays * 5) + this.Copy.VideoGame.CreditCost;  // 5 crédits par jour de retard
               
                // Sur base de l'ID, rechercher à quel player correspondent l'id du borrower et celui du lender pour récupérer leurs crédits
                PlayerDAO player = new PlayerDAO();
                Player borrower = player.Find(this.Borrower.IdPlayer);
                Player lender = player.Find(this.Lender.IdPlayer);

                int borrowCredits = borrower.Credit;
                int lenderCredits = lender.Credit;

                // Déduire les crédits de l'emprunteur et les ajouter au prêteur
                borrower.Credit = borrowCredits - penalty;
                lender.Credit = lenderCredits + penalty;

                // Mise à jour des crédits dans la base de données pour les deux joueurs
                PlayerDAO playerDAO2 = new PlayerDAO();
                bool borrowerUpdated = playerDAO2.UpdateCredit(borrower);
                bool lenderUpdated = playerDAO2.UpdateCredit(lender);
                MessageBox.Show("Vous avez bien rendu le jeu, avec un retard de " + delayDays + " jours. \nVous avez une pénalité de " + penalty+" crédits. \nVotre nouveau solde de crédit est de : "+borrower.Credit+" crédits. \nLe solde sera mis à jour au redémarrage de l'application.");
                if (!borrowerUpdated || !lenderUpdated)
                {
                    throw new Exception("Erreur lors de la mise à jour des crédits des joueurs.");
                }
            }
        }

        public void EndLoan()
        {
            if (this != null)
            {
                // Mettre fin au prêt
                this.Ongoing = false;

                // Mise à jour de la loan dans la base de données
                LoanDAO loanDAO = new LoanDAO(); 
                bool updated = loanDAO.Update(this);
                if (!updated)
                {
                    throw new Exception("Erreur lors de la mise à jour du prêt.");
                }
            }
            else
            {
                throw new Exception("Prêt non trouvé.");
            }
        }
    }
}
