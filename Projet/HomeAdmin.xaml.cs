using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
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
using System.Text.RegularExpressions;


namespace Projet
{
    
    public partial class HomeAdmin : Window
    {
        private string connectionString;
        private VideoGameDAO videoGameDAO;
        public List<VideoGame> VideoGames { get; set; }

        public HomeAdmin()
        {
            InitializeComponent();
            videoGameDAO = new VideoGameDAO();
            VideoGames = new List<VideoGame>();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Créer une instance de la classe métier VideoGame
                VideoGame videoGame = new VideoGame();

                // Appeler la méthode FindAll pour obtenir tous les jeux vidéo
                List<VideoGame> videoGames = videoGame.FindAll();

                // Affecter la liste de jeux vidéo à la source de données du contrôle ListView
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


        //---- Permet la modification du nombre de crédit qu'un jeux coûte lorsqu'on click dessus
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtenez l'objet VideoGame correspondant à l'élément sélectionné dans le ListView
            VideoGame selectedVideoGame = listView.SelectedItem as VideoGame;

            if (selectedVideoGame != null)
            {
                // Mettez à jour le crédit du jeu vidéo
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
                int creditCost = int.Parse(newGameCreditCost.Text); // Assurez-vous que ce texte ne contient que des chiffres.

                VideoGame newVideoGame = new VideoGame
                {
                    Name = name,
                    Console = console,
                    CreditCost = creditCost
                };

                if (newVideoGame.Create())
                {
                    MessageBox.Show("Le jeu vidéo a été ajouté avec succès!");
                    // Optionnellement, vous pouvez rafraîchir la liste de jeux vidéo ici.
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
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtenez l'objet VideoGame correspondant à l'élément sélectionné dans le ListView
            VideoGame selectedVideoGame = listView.SelectedItem as VideoGame;

            if (selectedVideoGame != null)
            {
                
                bool success = selectedVideoGame.Delete();

                if (success)
                {
                    MessageBox.Show("Video game delete with succes !");
                }
                else
                {
                    MessageBox.Show("Error when delete video game");
                }
            }
            else
            {
                MessageBox.Show("Select video game");
            }
        }
        //Permet de refresh la page pour afficher les nouvelles donées//
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();

        }

        private void LoadData()
        {
            try
            {
                // Créer une instance de la classe métier VideoGame
                VideoGame videoGame = new VideoGame();

                // Appeler la méthode FindAll pour obtenir tous les jeux vidéo
                List<VideoGame> videoGames = videoGame.FindAll();
                listView.ItemsSource = videoGames;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors de la récupération des jeux vidéo : " + ex.Message);
            }
        }

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


