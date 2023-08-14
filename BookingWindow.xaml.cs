using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Media;

namespace Projet
{
    public partial class BookingWindow : Window
    {
        private Player currentPlayer;

        //Affichage de la fenêtre qui fait appel à la méthode "LoadVideoGames" et qui affiche également les crédits
        //du joueur connecté
        public BookingWindow(Player currentPlayer)
        {
            InitializeComponent();
            this.currentPlayer = currentPlayer;

            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"{credits}";
            }

            LoadVideoGames();
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

        //Permet de récupérer le jeu selectionné, si celui-ci n'est pas "null" on ouvre une nouvelle fenêtre 
        //En passant en paramètre le joueur actuel et le jeu selectionné et de les afficher
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

        //Permet de faire appel à la méthode FindAll qui va afficher tout les jeux vidéos
        //Permet également de selectionner un jeu affiché dans la listeBox
        private void LoadVideoGames()
        {
            try
            {
                VideoGame videoGame = new VideoGame();
                List<VideoGame> videoGames = videoGame.FindAll();
                listVideoGames.ItemsSource = videoGames;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des jeux vidéos : " + ex.Message);
            }
        }
    }
}
