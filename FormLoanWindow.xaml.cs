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
    public partial class FormLoanWindow : Window
    {
        private Player currentPlayer;
        private PlayerDAO playerDAO;
        private string connectionString;
        public FormLoanWindow(Player currentplayer, VideoGame selectedVideoGame)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
            this.currentPlayer = currentplayer;

            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"{credits}";
            }

            if (selectedVideoGame != null)
            {
                // Affichez les détails du jeu vidéo où vous le souhaitez, par exemple:
                txtVideoGameDetails.Text = $"Selected Video Game: {selectedVideoGame.Name}\nConsole: {selectedVideoGame.Console}\nCredit Cost: {selectedVideoGame.CreditCost}";
            }
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
            
        }
    }
}
