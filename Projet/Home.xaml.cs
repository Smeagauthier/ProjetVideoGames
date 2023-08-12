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

    }
}
