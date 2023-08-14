using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace Projet
{

    public partial class AddGameForPlayerWindow : Window
    {
        private Player currentPlayer;
        VideoGame videoGame = new VideoGame();

        public List<Copy> AvailableCopies { get; set; }

        //représente une fenêtre qui appelle différentes méthodes qui vont permettre de charger la liste des jeux vidéo. 
        public AddGameForPlayerWindow(Player currentPlayer)
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

        //Bouton de retour, qui redirige vers la page "LoanWindow"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LoanWindow loanWindow = new LoanWindow(currentPlayer);
            loanWindow.Show();
            Close();
        }

        //Bouton de déconnexion, redirige vers la page d'accueil "MainWindow"
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        //Bouton d'ajout qui va créer une copie d'un jeu vidéo pour la personne connecté
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            VideoGame selectedVideoGame = listVideoGames.SelectedItem as VideoGame;

            if (selectedVideoGame != null)
            {
                
                // Créer une nouvelle copie avec le currentPlayer en tant que propriétaire
                Copy newCopy = new Copy();
                newCopy.Owner = currentPlayer;
                newCopy.VideoGame = selectedVideoGame;

                try
                {
                    // Appeler la méthode Create de CopyDAO pour ajouter la copie à la base de données
                    bool success = newCopy.Create(); 

                    if (success)
                    {
                        MessageBox.Show("La copie a été créée avec succès !");
                        LoanWindow loanWindow = new LoanWindow(currentPlayer);
                        loanWindow.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Vous possédez déjà le jeu que vous souhaitez ajouter.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez un jeu vidéo.");
            }

        }

        //Permet d'afficher la liste de tout les jeux vidéos situé en base de données
        private void LoadVideoGames()
        {
            try
            {
                List<VideoGame> videoGames = videoGame.FindAll();
                listVideoGames.ItemsSource = videoGames;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading video games: " + ex.Message);
            }
        }

    }
}
