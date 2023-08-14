using Projet.DAO;
using Projet.metier;
using System;
using System.Windows;

namespace Projet
{
    public partial class Registration : Window
    {
        private UserDAO userDAO;

        public Registration()
        {
            InitializeComponent();
        }

        //Bouton d'inscription qui fait appel à la méthode CreateUserAndPlayer
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

        //Permet de créer dans un premier temps un User qui contient un username et un password
        //Et ensuite un player lié à cet User
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
                    bool playerResult = player.Create();

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

        //Bouton de retour, qui redirige vers la page "Home" en passant en paramètre le joueur actuel afin de récupérer 
        //ses informations sur la page de retour.
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
