using Projet.DAO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Projet.metier
{
    public class VideoGame
    {

        private int idVideoGame;
        private string name;
        private int creditCost;
        private string console;
        private DateTime lastCreditUpdate;
        private List<Booking> listBookingsVideoGame;
        private List<Copy> listCopyVideoGame;
        private VideoGameDAO videoGameDAO;

        //---- Constructeur ----//
        public VideoGame(int idVideoGame, string name, int creditCost, string console, List<Booking> listBookingsVideoGame, List<Copy> listCopyVideoGame)
        {
            this.idVideoGame = idVideoGame;
            this.name = name;
            this.creditCost = creditCost;
            this.console = console;
            this.listBookingsVideoGame = listBookingsVideoGame;
            this.listCopyVideoGame = listCopyVideoGame;
        }

        public VideoGame()
        {
 
        }

        public VideoGame(int idVideoGame, string name, int creditCost, string console)
        {
            this.idVideoGame = idVideoGame;
            this.name = name;
            this.creditCost = creditCost;
            this.console = console;
        }

        //---- Getter et Setter ----//
        public int IdVideoGame { get { return idVideoGame; } set { idVideoGame = value; } }
        public string Name { get { return name; } set { name = value; } }
        public int CreditCost { get { return creditCost; } set { creditCost = value; } }
        public string Console { get { return console; } set { console = value; } }
        public List<Booking> ListBookingsVideoGame { get { return listBookingsVideoGame; } set { listBookingsVideoGame = value; } }
        public List<Copy> ListCopyVideoGame { get { return listCopyVideoGame; } set { listCopyVideoGame = value; } }
        public DateTime LastCreditUpdate { get { return lastCreditUpdate; } set { lastCreditUpdate = value; } }

        //Liste des consoles pour ajouter en admin un jeu vidéo
        public static ObservableCollection<string> Consoles { get; set; } = new ObservableCollection<string>
            {
                "PlayStation 5",
                "Playstation 4",
                "PSP",
                "Xbox Series X",
                "Xbox 360",
                "Xbox One",
                "Nintendo Switch",
                "Wii U",
                "3DS",
            };

        //Liste des crédits pour ajouter un jeu vidéo en admin
        public static ObservableCollection<int> Credits { get; set; } = new ObservableCollection<int>
            {
                1,2,3,4,5,6,7,8,9,10
            };

        //---- Méthodes supplémentaires ----//
        //Permet la prise en compte d'une réservation pour un jeu vidéo, en trouvant une copie disponible du jeu,
        //en créant un prêt pour cette copie, et en mettant à jour les crédits du prêteur en conséquence.
        public void SelectBooking(CopyDAO copyDAO, BookingDAO bookingDAO, Player player, LoanDAO loanDAO)
        {
            List<Copy> listCopyVideoGame = copyDAO.FindCopiesByVideoGame(this, player);
            List<Booking> listBookingsVideoGame = bookingDAO.GetAllBookingsForVideoGame(this);

            // Trouve une copie disponible pour ce jeu vidéo
            Copy availableCopy = null;
            foreach (Copy copy in listCopyVideoGame)
            {
                if (copy.IsAvailable(loanDAO))
                {
                    availableCopy = copy;
                    break; // Sort de la boucle dès que l'on trouve la première copie disponible.
                }
            }

            // Si aucune copie n'est disponible
            if (availableCopy == null)
            {
                MessageBox.Show("Désolé, aucune copie disponible pour le moment. La réservation s'effectue et vous recevrez une copie de votre jeu quand la location actuelle sera terminée.");
                return;
            }

            // Prend la première réservation dans la liste
            Booking selectedBooking = null;
            foreach (Booking booking in listBookingsVideoGame)
            {
                if (booking != null)
                {
                    selectedBooking = booking;
                    break;
                }
            }

            if (selectedBooking != null)
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = startDate.AddDays(selectedBooking.NumberOfWeeks * 7);
                Player lender = availableCopy.Owner;  // Utilisation directe de l'owner de la copie disponible

                Loan newLoan = new Loan
                {
                    Copy = availableCopy,
                    StartDate = startDate,
                    EndDate = endDate,
                    Ongoing = true,
                    Borrower = player,
                    Lender = lender
                };

                // Sauvegarder le prêt en base de données
                if (loanDAO.Create(newLoan))
                {
                    MessageBox.Show("Le prêt a été créé avec succès !");
                    bool test = bookingDAO.Delete(selectedBooking);
                    VideoGame vg = Find(selectedBooking.VideoGame.IdVideoGame);
                    int creditsBooking = vg.CreditCost;
                    creditsBooking = creditsBooking * selectedBooking.NumberOfWeeks;
                    lender.Credit += creditsBooking;
                    lender.UpdateCredit();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la création du prêt.");
                }
            }
            else
            {
                MessageBox.Show("Aucune réservation trouvée pour ce jeu.");
            }
        }


        //---- Méthodes qui font référence au DAO ----//
        public List<VideoGame> FindAll()
        {
            VideoGameDAO videoGameDAO = new VideoGameDAO();
            return videoGameDAO.FindAll();
        }

        public bool UpdateCredit()
        {
            if (videoGameDAO == null)
            {
                videoGameDAO = new VideoGameDAO();
            }
            return videoGameDAO.UpdateCredit(this);
        }

        public bool Create()
        {
            VideoGameDAO videoGameDAO = new VideoGameDAO();
            return videoGameDAO.Create(this);
        }

        public bool Delete()
        {
            VideoGameDAO videoGameDAO = new VideoGameDAO();
            return videoGameDAO.Delete(this);
        }

        public VideoGame Find(int idVideoGame)
        {
            VideoGameDAO videoGameDAO = new VideoGameDAO();
            return videoGameDAO.Find(this.idVideoGame);
        }

    }
}
