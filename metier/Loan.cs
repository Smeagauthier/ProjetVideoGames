using Projet.DAO;
using System;
using System.Collections.Generic;
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
        
        //---- Constructeur ----//
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

        //---- Getter et Setter ----//
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

        //---- Méthodes supplémentaires ----//

        //Permet d'ajouter dans une liste de jeu vidéo vide, les jeux vidéo associé au player
        public static List<VideoGame> GetPlayerVideoGames(Player player)
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

        //---- Permet de récupérer les jeu loué selon un joueur
        public List<Loan> GetPlayerLoans(Player player)
        {
            LoanDAO loanDAO = new LoanDAO();
            return loanDAO.GetPlayerLoans(player);
        }

        //Permet de calculer le retard lors du rendu d'une copie d'un jeu vidéo
        public void CalculateBalance()
        {
            if (this != null && this.Ongoing && DateTime.Now > this.EndDate)
            {
                TimeSpan delay = DateTime.Now - this.EndDate;
                int delayDays = (int)Math.Ceiling(delay.TotalDays);
                int weeks = (int)Math.Ceiling(delayDays / 7.0);
                int penalty = 0; 

                penalty = (weeks * delayDays * 5) + this.Copy.VideoGame.CreditCost;  // 5 crédits par jour de retard + le nbre de crédits du jeu vidéo
               
                // Sur base de l'ID, on recherche à quel player correspondent l'id du borrower et celui du lender pour récupérer leurs crédits
                PlayerDAO player = new PlayerDAO();
                Player borrower = player.Find(this.Borrower.IdPlayer);
                Player lender = player.Find(this.Lender.IdPlayer);

                int borrowCredits = borrower.Credit;
                int lenderCredits = lender.Credit;

                //Permet de déduire les crédits de l'emprunteur et les ajouter au prêteur
                borrower.Credit = borrowCredits - penalty;
                lender.Credit = lenderCredits + penalty;

                //Met à jour les crédits dans la base de données pour les deux joueurs
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

        //Permet de rendre un prêt, si un autre joueur à réserver le jeu que quelqu'un vient de rendre, 
        //la réservation se supprime et se convertit en prêt au prochain loueur selon la règle des priorités
        public void EndLoan()
        {
            if (this != null)
            {
                //Met fin au prêt
                this.Ongoing = false;

                // Met à jour la loan dans la base de données
                LoanDAO loanDAO = new LoanDAO();
                bool updated = loanDAO.Update(this);
                if (!updated)
                {
                    throw new Exception("Erreur lors de la mise à jour du prêt.");
                }

                // Recherche les réservations pour ce jeu
                BookingDAO bookingDAO = new BookingDAO();
                List<Booking> bookings = bookingDAO.GetAllBookingsForVideoGame(this.Copy.VideoGame);
                CopyDAO copyDAO = new CopyDAO();

                if (bookings.Count == 0)
                {
                    //MessageBox.Show("Aucune réservation en attente, le jeu est disponible.");
                }
                else if (bookings.Count == 1)
                {
                    //Transférer la réservation vers un autre joueur si celui qui possède le jeu en location le rend    
                    Player player = bookings[0].Player;
                    this.Copy.VideoGame.SelectBooking(copyDAO, bookingDAO, player, loanDAO);
                    
                }
                else
                {
                    // Transfert des réservations en fonction des règles de priorité
                    bookings.Sort((b1, b2) =>
                    {
                        // 1) Le plus de crédits sur son compte
                        int compareCredits = b1.Player.Credit.CompareTo(b2.Player.Credit);
                        if (compareCredits != 0) return compareCredits;
                        
                        // 2) Réservation la plus ancienne
                        int compareBookingDate = b1.BookingDate.CompareTo(b2.BookingDate);
                        if (compareBookingDate != 0) return compareBookingDate;
                        

                        // 3) Abonné inscrit depuis le plus longtemps
                        int compareRegistrationDate = b1.Player.RegistrationDate.CompareTo(b2.Player.RegistrationDate);
                        if (compareRegistrationDate != 0) return compareRegistrationDate;

                        // 4) Abonné le plus âgé
                        int compareDateOfBirth = b1.Player.DateOfBirth.CompareTo(b2.Player.DateOfBirth);
                        if (compareDateOfBirth != 0) return compareDateOfBirth;

                        // 5) Aléatoire
                        return new Random().Next(0, 2) == 0 ? -1 : 1;
                    });
                    Player player = bookings[0].Player;
                    this.Copy.VideoGame.SelectBooking(copyDAO, bookingDAO, player, loanDAO);
                }
            }
            else
            {
                throw new Exception("Prêt non trouvé.");
            }
        }

        //Fait référence à la méthode DeleteAllLoansForCopy
        public bool DeleteAllLoansForCopy(Copy copy)
        {
            LoanDAO loanDAO = new LoanDAO();
            return loanDAO.DeleteAllLoansForCopy(copy);
        }

    }
}
