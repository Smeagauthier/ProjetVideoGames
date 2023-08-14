using Projet.DAO;
using Projet.metier;
using System.Configuration;
using System.Windows;

namespace Projet
{

    public partial class Home : Window
    {

        private Player currentPlayer;
        private VideoGame videoGame;

        //Permet d'afficher la fenêtre, et également les crédits du joueur connecté dans le header
        //Une autorisation a été ajoutée, si les crédits sont supérieurs à 0
        // il peut cliquer vers la redirection des réservations
        // sinon le bouton se grise, et l'utilisateur ne peut pas y accédé
        public Home(Player currentPlayer)
        {
            InitializeComponent();
            this.currentPlayer = currentPlayer;

            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"Credits : {credits}";
                btnBooking.IsEnabled = currentPlayer.LoanAllowed();
            }

        }

        //Bouton qui redirige vers la page de prêt en utilisant un objet player en paramètre
        //afin de récupérer ses données sur la page redirigée
        private void LoanButton_Click(object sender, RoutedEventArgs e)
        {
            LoanWindow loanWindow = new LoanWindow(currentPlayer);
            loanWindow.Show();
            Close();
        }
        //Bouton de redirection vers la fenêtre "BookingWindow"
        //Le joueur est envoyé en paramètre afin de récupérer ses données sur la page redirigée
        private void BookingButton_Click(object sender, RoutedEventArgs e)
        {
            BookingWindow bookingWindow = new BookingWindow(currentPlayer);
            bookingWindow.Show();
            Close();
        }
        //Bouton de déconnexion, permet de rediriger vers la page de connexion "MainWindow"
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        //Bouton de redirection vers la page qui affiche la liste des réservations
        private void ListBookingButton_Click(object sender, RoutedEventArgs e)
        {
            ListBooking listBooking = new ListBooking(currentPlayer, videoGame);
            listBooking.Show();
            Close();
        }

        //Bouton de redirection verrs la page qui affiche la list des jeux vidéo
        private void ListVideoGamesButton_Click(object sender, RoutedEventArgs e)
        {
            ListVideoGame listvideoGame = new ListVideoGame(currentPlayer);
            listvideoGame.Show();
            Close();
        }

    }
}
