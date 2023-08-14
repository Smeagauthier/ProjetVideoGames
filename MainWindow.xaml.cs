using Projet.DAO;
using Projet.metier;
using System.Windows;

namespace Projet
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //---- Méthode du boutton click de la page Login qui vérifie si admin ou user + cadeau anniversaire ----//
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            Player player = new Player(username, password);

            bool isAuthenticated = player.AuthenticateUser();

            if (isAuthenticated)
            {
                bool isAdmin = player.IsUserAdmin();

                if (isAdmin)
                {
                    HomeAdmin homeAdminWindow = new HomeAdmin();
                    homeAdminWindow.Show();
                    this.Close();
                }
                else
                {
                    Player currentPlayer = player.FindByUserId();

                    if (currentPlayer != null)
                    {
                        // Défini le joueur actuel en utilisant la propriété CurrentPlayer
                        Player.CurrentPlayer = currentPlayer;

                        currentPlayer.AddBirthDayBonus();

                        // Mise à jour du joueur dans la base de données
                        currentPlayer.UpdateCredit();

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

        // Envoie vers la page Registration lors du click sur le boutton
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Registration registrationWindow = new Registration();
            registrationWindow.Show();
            this.Close();
        }
    }
}
