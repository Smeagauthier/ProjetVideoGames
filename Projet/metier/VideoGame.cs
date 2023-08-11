using Projet.DAO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            lastCreditUpdate = new DateTime(2023, 07, 15);
        }

        public VideoGame(int idVideoGame, string name, int creditCost, string console)
        {
            this.idVideoGame = idVideoGame;
            this.name = name;
            this.creditCost = creditCost;
            this.console = console;
        }

        public int IdVideoGame { get { return idVideoGame; } set { idVideoGame = value; } }
        public string Name { get { return name; } set { name = value; } }
        public int CreditCost { get { return creditCost; } set { creditCost = value; } }
        public string Console { get { return console; } set { console = value; } }
        public List<Booking> ListBookingsVideoGame { get { return listBookingsVideoGame; } set { listBookingsVideoGame = value; } }
        public List<Copy> ListCopyVideoGame { get { return listCopyVideoGame; } set { listCopyVideoGame = value; } }
        public DateTime LastCreditUpdate
        {
            get { return lastCreditUpdate; }
            set { lastCreditUpdate = value; }
        }
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

        public void SelectBooking(CopyDAO copyDAO, BookingDAO bookingDAO, Player player, LoanDAO loanDAO)
        {
            List<Copy> listCopyVideoGame = copyDAO.FindCopiesByVideoGame(this, player);
            List<Booking> listBookingsVideoGame = bookingDAO.GetAllBookingsForVideoGame(this);

            // Trouver une copie disponible pour ce jeu vidéo
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

            // Prendre la première réservation dans la liste
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

        //---- Référence à la méthode FindAll de videoGameDAO ----//
        public List<VideoGame> FindAll()
        {
            VideoGameDAO videoGameDAO = new VideoGameDAO();
            return videoGameDAO.FindAll();
        }
        //---- Référence à la méthode UpdateCredit de VideoGameDAO ----//
        public bool UpdateCredit()
        {
            //Permet la modification du crédit 1x par semaine uniquement lorsque l'application reste ouverte
            //Si on la ferme, cela rénitialise le compteur (A corriger plus tard).
            //if ((DateTime.Now - lastCreditUpdate).TotalDays >= 7)
            //{
            // lastCreditUpdate = DateTime.Now;
            // MessageBox.Show("Dernière date = "+lastCreditUpdate);

            if (videoGameDAO == null)
            {
                videoGameDAO = new VideoGameDAO();
            }
            return videoGameDAO.UpdateCredit(this);
            //}
            //return false;
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
