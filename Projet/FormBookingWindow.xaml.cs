using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
    public partial class FormBookingWindow : Window
    {
        private Player currentPlayer;
        private PlayerDAO playerDAO;
        private string connectionString;
        private VideoGame selectedVideoGame;

        public FormBookingWindow(Player currentplayer, VideoGame selectedVideoGame)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
            this.currentPlayer = currentplayer;
            this.selectedVideoGame = selectedVideoGame;
            playerDAO = new PlayerDAO();

            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"{credits}";
            }

            if (selectedVideoGame != null)
            {
                txtVideoGameDetails.Text = $"Nom: {selectedVideoGame.Name}\nConsole: {selectedVideoGame.Console}\nCrédits : {selectedVideoGame.CreditCost}";
            }
            Loaded += FormBookingWindow_Loaded;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home(currentPlayer);
            home.Show();
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxWeeks.SelectedItem is ComboBoxItem selectedItem)
            {
                int numberOfWeeks = Convert.ToInt32(selectedItem.Content);

                // Crée une nouvelle réservation (booking) avec les informations appropriées
                Booking newBooking = new Booking();
                newBooking.BookingDate = DateTime.Now;
                newBooking.Player = currentPlayer;
                newBooking.VideoGame = selectedVideoGame;
                newBooking.NumberOfWeeks = numberOfWeeks;

                if (newBooking.Create())
                {
                    int totalCost = selectedVideoGame.CreditCost * numberOfWeeks;

                    if (currentPlayer.Credit >= totalCost)
                    {
                        currentPlayer.Credit -= totalCost;
                        playerDAO.UpdateCredit(currentPlayer);

                        CopyDAO copyDAO = new CopyDAO(connectionString);
                        BookingDAO bookingDAO = new BookingDAO(connectionString);
                        LoanDAO loanDAO = new LoanDAO();
                        selectedVideoGame.SelectBooking(copyDAO, bookingDAO,currentPlayer,loanDAO);
                        

                        MessageBox.Show($"La réservation a bien été effectuée. \nElle vous a coûté {totalCost} crédits. \nVotre nouveau solde de crédits est de {currentPlayer.Credit} crédits");
                        int credits = currentPlayer.Credit;
                        txtCredits.Text = $"{credits}";
                        Home home = new Home(currentPlayer);
                        home.Show();
                        Close();

                    }
                    else
                    {
                        MessageBox.Show("Crédits insuffisants pour effectuer la réservation !");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur lors de la réservation");
                }
            }
        }

        private void FormBookingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 10; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i;
                comboBoxWeeks.Items.Add(item);
            }
        }
    }
}
