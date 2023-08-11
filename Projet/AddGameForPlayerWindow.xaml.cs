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
    
    public partial class AddGameForPlayerWindow : Window
    {
        private Player currentPlayer;
        private PlayerDAO playerDAO;
        private string connectionString;
        public List<Copy> AvailableCopies { get; set; }

        public AddGameForPlayerWindow(Player currentPlayer)
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
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LoanWindow loanWindow = new LoanWindow(currentPlayer);
            loanWindow.Show();
            Close();
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

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

        private void LoadVideoGames()
        {
            try
            {
                VideoGameDAO videoGameDAO = new VideoGameDAO();
                List<VideoGame> videoGames = videoGameDAO.FindAll();
                listVideoGames.ItemsSource = videoGames;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading video games: " + ex.Message);
            }
        }

    }
}
