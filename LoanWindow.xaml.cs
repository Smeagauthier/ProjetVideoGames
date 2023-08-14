using Projet.metier;
using System.Collections.Generic;
using System.Windows;

namespace Projet
{
    public partial class LoanWindow : Window
    {
        private Player currentPlayer;

        public LoanWindow(Player currentPlayer)
        {
            InitializeComponent();
            this.currentPlayer = currentPlayer;

            if (currentPlayer != null)
            {
                txtCredits.Text = $"{currentPlayer.Credit}";
                List<VideoGame> playerVideoGames = Loan.GetPlayerVideoGames(currentPlayer);
                listVideoGames.ItemsSource = playerVideoGames;
            }
        }

        //Bouton de déconnexion qui redirige vers la page "MainWindow"
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        //Bouton de retour, qui redirige vers la page "Home" en passant en paramètre le joueur actuel afin de récupérer 
        //ses informations sur la page de retour.
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home(currentPlayer);
            home.Show();
            Close();
        }

        //lorsque l'utilisateur clique sur le bouton,
        //permettant à l'utilisateur d'ajouter un jeu pour le joueur actuel.
        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            AddGameForPlayerWindow addGameForPlayerWindow = new AddGameForPlayerWindow(currentPlayer);
            addGameForPlayerWindow.Show();
            Close();
        }
    }
}
