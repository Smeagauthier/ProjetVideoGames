using Projet.DAO;
using Projet.metier;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace Projet
{
    public partial class FormBookingWindow : Window
    {
        private Player currentPlayer;
        private Player player;
        private VideoGame selectedVideoGame;

        //Permet d'afficher la fenêtre, d'afficher les crédits du joueur dans le header
        //Permet également si la selection du jeu vidéo n'est pas "Null" on affiche des infos sur la réservation
        //Et on fait après, appel à Loaded et FormBookingWindow :
        //cette instruction garantit que, lorsque la fenêtre est entièrement chargée,
        //la méthode FormBookingWindow_Loaded sera exécutée. 
        public FormBookingWindow(Player currentplayer, VideoGame selectedVideoGame)
        {
            InitializeComponent();
            this.currentPlayer = currentplayer;
            this.selectedVideoGame = selectedVideoGame;

            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"{credits}";
            }

            if (selectedVideoGame != null)
            {
                txtVideoGameDetails.Text = $"Nom: {selectedVideoGame.Name}\nConsole: {selectedVideoGame.Console}\nCrédits : {selectedVideoGame.CreditCost}";
            }
            Loaded += FormBookingWindow_Loaded;
        }
        //Bouton de déconnexion, permet de rediriger vers la page de connexion "MainWindow"
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        //Bouton de retour, permet de rediriger vers la page d'accueil "Home" en récupérant le joueur connecté
        //pour permettre d'afficher ses informations sur le Header du xaml
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home(currentPlayer);
            home.Show();
            Close();
        }

        //Bouton de confirmation qui va permettre de créer une nouvelle réservation lorsqu'on cliquera sur le bouton
        //cette méthode gère tout le processus de confirmation d'une réservation de jeu vidéo pour un joueur,
        //depuis la sélection du nombre de semaines jusqu'à la déduction des crédits et la mise à jour
        //de l'interface utilisateur.
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxWeeks.SelectedItem is ComboBoxItem selectedItem)
            {
                int numberOfWeeks = Convert.ToInt32(selectedItem.Content);

                // Crée une nouvelle réservation (booking) avec les informations appropriées
                Booking newBooking = new Booking();
                newBooking.BookingDate = DateTime.Now;
                newBooking.Player = currentPlayer;
                newBooking.VideoGame = selectedVideoGame;
                newBooking.NumberOfWeeks = numberOfWeeks;

                if (newBooking.Create())
                {
                    int totalCost = selectedVideoGame.CreditCost * numberOfWeeks;

                    if (currentPlayer.Credit >= totalCost)
                    {
                        currentPlayer.Credit -= totalCost;
                        currentPlayer.UpdateCredit();

                        CopyDAO copyDAO = new CopyDAO();
                        BookingDAO bookingDAO = new BookingDAO();
                        LoanDAO loanDAO = new LoanDAO();
                        selectedVideoGame.SelectBooking(copyDAO, bookingDAO, currentPlayer, loanDAO);

                        MessageBox.Show($"La réservation a bien été effectuée. \nElle vous a coûté {totalCost} crédits. \nVotre nouveau solde de crédits est de {currentPlayer.Credit} crédits");
                        int credits = currentPlayer.Credit;
                        txtCredits.Text = $"{credits}";
                        Home home = new Home(currentPlayer);
                        home.Show();
                        Close();

                    }
                    else
                    {
                        MessageBox.Show("Crédits insuffisants pour effectuer la réservation !");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur lors de la réservation");
                }
            }
        }

        //sert à initialiser et remplir un contrôle ComboBox (appelé comboBoxWeeks) avec des nombres allant de 1 à 10
        private void FormBookingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 10; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i;
                comboBoxWeeks.Items.Add(item);
            }
        }
    }
}
