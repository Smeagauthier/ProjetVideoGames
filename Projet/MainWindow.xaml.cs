using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Projet
{
    public partial class MainWindow : Window
    {
        private string connectionString;
        public int idUser { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }
        //****************** Méthodes ****************** //
        //---- Méthode du boutton click de la page Login qui vérifie si admin ou user + cadeau anniversaire ----//
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            Player player = new Player(username, password);

            bool isAuthenticated = player.AuthenticateUser();

            if (isAuthenticated)
            {
                int idUser = player.GetUserId();
                bool isAdmin = player.IsUserAdmin();

                if (isAdmin)
                {
                    HomeAdmin homeAdminWindow = new HomeAdmin();
                    homeAdminWindow.Show();
                    this.Close();
                }
                else
                {
                    Player currentPlayer = player.GetPlayer();

                    if (currentPlayer != null)
                    {
                        // Définir le joueur actuel en utilisant la propriété statique CurrentPlayer
                        Player.CurrentPlayer = currentPlayer;

                        currentPlayer.AddBirthDayBonus();
                        // Mise à jour du joueur dans la base de données
                        PlayerDAO playerDAO = new PlayerDAO();
                        playerDAO.UpdateCredit(currentPlayer);
                        Home homeWindow = new Home(currentPlayer);
                        homeWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Joueur non trouvé.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect.");
            }
        }

        //Envoie vers la page Registration lors du click sur le boutton
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Registration registrationWindow = new Registration();
            registrationWindow.Show();
            this.Close();
        }
    }
}