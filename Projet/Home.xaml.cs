using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projet
{

    public partial class Home : Window
    {
        
        private Player currentPlayer;
        private PlayerDAO playerDAO;
        private string connectionString;
        private VideoGame videoGame; 

        public Home(Player currentPlayer)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
            this.currentPlayer = currentPlayer;
            playerDAO = new PlayerDAO();

            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"Credits : {credits}";
                btnBooking.IsEnabled = currentPlayer.LoanAllowed();
            }

            CreateLoanForWaitingReservations();
        }

        private void LoanButton_Click(object sender, RoutedEventArgs e)
        {
            LoanWindow loanWindow = new LoanWindow(currentPlayer);
            loanWindow.Show();
            Close();
        }

        private void BookingButton_Click(object sender, RoutedEventArgs e)
        {
            BookingWindow bookingWindow = new BookingWindow(currentPlayer);
            bookingWindow.Show();
            Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        private void ListBookingButton_Click(object sender, RoutedEventArgs e)
        {
            ListBooking listBooking = new ListBooking(currentPlayer, videoGame);
            listBooking.Show();
            Close();
        }

        private void ListVideoGamesButton_Click(object sender, RoutedEventArgs e)
        {
            ListVideoGame listvideoGame = new ListVideoGame(currentPlayer);
            listvideoGame.Show();
            Close();
        }

        private void CreateLoanForWaitingReservations()
        {
            Booking bookingsByPlayer = new Booking();
            List<Booking> bookings = bookingsByPlayer.FindBookingsForPlayer(currentPlayer.IdPlayer);

            foreach (Booking booking in bookings)
            {
                if (booking.Copy != null && !booking.Copy.Loan.Ongoing)
                {
                    List<Player> reservationPlayers = booking.GetBookerPlayersForCopy(booking.Copy.IdCopy);

                    // Utiliser la méthode pour choisir le joueur approprié
                    Player selectedPlayer = ChoosePlayerForCopy(reservationPlayers);

                    // Créer une nouvelle Loan basée sur la réservation et le joueur choisi
                    Loan newLoan = new Loan
                    {
                        Copy = booking.Copy,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(booking.NumberOfWeeks*7), // Par exemple, 7 jours de prêt
                        Ongoing = true,
                        Borrower = selectedPlayer,
                        Lender = booking.Copy.Owner
                    };

                    LoanDAO loanDAO = new LoanDAO();
                    bool loanCreated = loanDAO.Create(newLoan);

                    if (loanCreated)
                    {
                        MessageBox.Show($"Une nouvelle location a été créée pour la réservation {booking.IdBooking}. La copie a été empruntée par {selectedPlayer.Pseudo}.");
                    }
                    else
                    {
                        MessageBox.Show($"Erreur lors de la création de la location pour la réservation {booking.IdBooking}.");
                    }
                }
            }
        }

        private Player ChoosePlayerForCopy(List<Player> players)
        {
            // Règle 1 : Le plus de crédits sur son compte
            Player playerWithMostCredits = players.OrderByDescending(player => player.Credit).FirstOrDefault();
            if (playerWithMostCredits != null)
            {
                return playerWithMostCredits;
            }

            // Règle 2 : Réservation la plus ancienne
            /*Booking oldestBooking = players
                .SelectMany(player => player.)
                .OrderBy(booking => booking.BookingDate)
                .FirstOrDefault();
            if (oldestBooking != null)
            {
                return oldestBooking.Player;
            }*/

            // Règle 3 : Abonné inscrit depuis le plus longtemps
            Player oldestSubscriber = players.OrderBy(player => player.RegistrationDate).FirstOrDefault();
            if (oldestSubscriber != null)
            {
                return oldestSubscriber;
            }

            // Règle 4 : Abonné le plus âgé
            Player oldestPlayer = players.OrderBy(player => player.DateOfBirth).FirstOrDefault();
            if (oldestPlayer != null)
            {
                return oldestPlayer;
            }

            // Règle 5 : Aléatoire
            Random random = new Random();
            int randomIndex = random.Next(players.Count);
            return players[randomIndex];
        }
    }
}
