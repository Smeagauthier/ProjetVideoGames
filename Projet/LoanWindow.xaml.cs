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
    public partial class LoanWindow : Window
    {
        private Player currentPlayer;
        private PlayerDAO playerDAO;
        private string connectionString;
        public LoanWindow(Player currentPlayer)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
            this.currentPlayer = currentPlayer;

            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"{credits}";
                List<VideoGame> playerVideoGames = Loan.GetPlayerVideoGames(currentPlayer, connectionString);
                listVideoGames.ItemsSource = playerVideoGames;
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

        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            AddGameForPlayerWindow addGameForPlayerWindow = new AddGameForPlayerWindow(currentPlayer);
            addGameForPlayerWindow.Show();
            Close();
        }



    }
}
