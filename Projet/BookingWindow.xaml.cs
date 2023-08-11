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
    public partial class BookingWindow : Window
    {
        private Player currentPlayer;
        private PlayerDAO playerDAO;
        private string connectionString;
        public BookingWindow(Player currentPlayer)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
            this.currentPlayer = currentPlayer;

            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"{credits}";
            }

            LoadVideoGames();
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

        private void VideoGameButton_Click(object sender, RoutedEventArgs e)
        {
            VideoGame selectedVideoGame = listVideoGames.SelectedItem as VideoGame;

            if (selectedVideoGame != null)
            {
                // Ouvrir la fenêtre FormBookingWindow avec les détails du jeu vidéo
                FormBookingWindow formBookingWindow = new FormBookingWindow(currentPlayer, selectedVideoGame);
                formBookingWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un jeu avant de réserver.", "Erreur de réservation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadVideoGames()
        {
            try
            {
                // Create an instance of the VideoGameDAO class
                VideoGameDAO videoGameDAO = new VideoGameDAO();

                // Call the FindAll method to get all video games
                List<VideoGame> videoGames = videoGameDAO.FindAll();

                // Set the list of video games as the ItemsSource of the ListBox
                listVideoGames.ItemsSource = videoGames;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des jeux vidéos : " + ex.Message);
            }
        }
    }
}
