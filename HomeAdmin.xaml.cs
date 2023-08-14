using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Projet
{

    public partial class HomeAdmin : Window
    {
        //Permet d'afficher la fenêtre
        public HomeAdmin()
        {
            InitializeComponent();
        }

        //Permet d'afficher tout les jeux vidéos
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //On créé une instance de la classe métier VideoGame
                VideoGame videoGame = new VideoGame();

                //On appel la méthode FindAll pour obtenir tous les jeux vidéo
                List<VideoGame> videoGames = videoGame.FindAll();

                //On affecte la liste de jeux vidéo à la source de données du contrôle ListView
                listView.ItemsSource = videoGames;
                consoleComboBox.ItemsSource = VideoGame.Consoles;
                DataContext = this;
                newGameCreditCost.ItemsSource= VideoGame.Credits;
                DataContext=this;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors de la récupération des jeux vidéo : " + ex.Message);
            }

        }


        //---- Permet la modification du nombre de crédit qu'un jeu coûte lorsqu'on click dessus
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //On obtient un jeu vidéo correspondant à l'élément sélectionné dans le ListView
            VideoGame selectedVideoGame = listView.SelectedItem as VideoGame;

            if (selectedVideoGame != null)
            {
                //On met à jour le crédit du jeu vidéo
                bool success = selectedVideoGame.UpdateCredit();

                if (success)
                {
                    MessageBox.Show("Crédit du jeu vidéo mis à jour avec succès !");
                }
                else
                {
                    MessageBox.Show("Erreur lors de la mise à jour du crédit du jeu vidéo.");
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez un jeu vidéo dans la liste.");
            }
        }
        //Permet l'ajout d'un jeu video dans admin//
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = newGameName.Text;
                string console = consoleComboBox.SelectedItem.ToString();
                int creditCost = int.Parse(newGameCreditCost.Text);

                VideoGame newVideoGame = new VideoGame
                {
                    Name = name,
                    Console = console,
                    CreditCost = creditCost
                };

                if (newVideoGame.Create())
                {
                    MessageBox.Show("Le jeu vidéo a été ajouté avec succès!");
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'ajout du jeu vidéo.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors de l'ajout du jeu vidéo : " + ex.Message);
            }
        }
        //Permet de supprimer un jeu video dans admin//
        //Dans le cas où une réservation est associée, on supprime également la réservation
        //Dans le cas où une copie est associée, on supprime également la copie
        //Dans le cas où un prêt est associée, on supprime également le prêt
        //Contrainte dans la base de données
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // On obtient l'objet VideoGame correspondant à l'élément sélectionné dans le ListView
            VideoGame selectedVideoGame = listView.SelectedItem as VideoGame;

            if (selectedVideoGame != null)
            {
                List<string> successfulDeletions = new List<string>();

                // 1. Essayez de supprimer toutes les réservations associées à ce jeu vidéo, si elles existent
                Booking booking = new Booking();
                bool bookingsDeleted = booking.DeleteAllBookingsForVideoGame(selectedVideoGame);
                if (bookingsDeleted) successfulDeletions.Add("bookings");

                // 2. Essayez de supprimer toutes les copies associées à ce jeu vidéo, si elles existent
                Copy copy = new Copy();
                bool copiesDeleted = copy.DeleteAllCopiesForVideoGame(selectedVideoGame);
                if (copiesDeleted) successfulDeletions.Add("copies");

                // 3. Puis supprimez le jeu vidéo lui-même
                bool success = selectedVideoGame.Delete();
                if (success) successfulDeletions.Add("video game");

                // Compile feedback for user
                if (successfulDeletions.Count == 3)
                {
                    MessageBox.Show("Video game, bookings, and copies all deleted successfully!");
                }
                else if (successfulDeletions.Count > 0)
                {
                    MessageBox.Show(string.Join(", ", successfulDeletions) + " deleted successfully!");
                }
                else
                {
                    MessageBox.Show("Error when deleting the video game or its associated data.");
                }
            }
            else
            {
                MessageBox.Show("Please select a video game.");
            }
        }



        //Permet de refresh la page pour afficher les nouvelles donées//
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        //Bouton de retour, permet de rediriger vers la page d'accueil "Home" en récupérant le joueur connecté
        //pour permettre d'afficher ses informations sur le Header du xaml
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();

        }

        //Permet de charger la liste des jeux vidéo
        private void LoadData()
        {
            try
            {
                // Créer une instance de la classe métier VideoGame
                VideoGame videoGame = new VideoGame();

                // Appel la méthode FindAll pour obtenir tous les jeux vidéo
                List<VideoGame> videoGames = videoGame.FindAll();
                listView.ItemsSource = videoGames;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors de la récupération des jeux vidéo : " + ex.Message);
            }
        }
        //Regex qui permet de ne saisir aucun caractère, uniquement des chiffres pour la modification des crédits
        //des jeux vidéos
        private void newGameCreditCost_regex(object sender, TextCompositionEventArgs e)
        {
            // Entrer des chiffres uniquement
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; 
            }
        }


    }
}


