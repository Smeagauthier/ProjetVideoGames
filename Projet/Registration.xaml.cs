using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
    
    public partial class Registration : Window
    {
        private string connectionString;
        private UserDAO userDAO;
        private PlayerDAO playerDAO;
        public Registration()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
            userDAO = new UserDAO(connectionString);
            playerDAO = new PlayerDAO();

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string pseudo = txtPseudo.Text;
            DateTime dateOfBirth = dpDateOfBirth.SelectedDate.GetValueOrDefault();

            bool created = CreateUserAndPlayer(username, password, pseudo, dateOfBirth);

            if (created)
            {
                MessageBox.Show("Inscription réussie !");
                MainWindow mainWindow = new();
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Erreur lors de l'inscription.");
            }
        }


        private bool CreateUserAndPlayer(string username, string password, string pseudo, DateTime dateOfBirth)
        {
            bool success = false;

            try
            {
                // Créer un nouvel utilisateur de type Player
                Player player = new Player(username, password);

                // Appeler la méthode de création d'utilisateur dans UserDAO
                int idUser = userDAO.CreateUser(player);

                if (idUser > 0)
                {
                    // Maintenant, vous avez déjà un joueur associé à l'utilisateur
                    player.IdUser = idUser;
                    player.Credit = 10;
                    player.Pseudo = pseudo;
                    player.RegistrationDate = DateTime.Now;
                    player.DateOfBirth = dateOfBirth;

                    // Appeler la méthode de création de joueur dans PlayerDAO
                    bool playerResult = playerDAO.CreatePlayer(player);

                    if (playerResult)
                    {
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la création de l'utilisateur et du joueur : " + ex.Message);
            }

            return success;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

    }
}
