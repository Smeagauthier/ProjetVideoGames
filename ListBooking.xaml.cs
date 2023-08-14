using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Projet
{
    public partial class ListBooking : Window
    {
        private Player currentPlayer;

        //Permet d'afficher la fenêtre, mais également les crédits du joueur connecté dans le header
        //Permet de faire appel à la méthode LoadBookingsForPlayer
        public ListBooking(Player currentPlayer, VideoGame videoGame)
        {
            InitializeComponent();
            this.currentPlayer = currentPlayer;


            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"Crédits : {credits}";

                LoadBookingsForPlayer();
            }
        }

        //Permet de stocker dans la liste toutes les réservations d'un joueur passé en paramètre (joueur actuel)
       private void LoadBookingsForPlayer()
        {
            Booking booking = new Booking();
            List<Booking> bookings = booking.FindBookingsForPlayer(currentPlayer.IdPlayer); 
            lstBookings.ItemsSource = bookings;
        }

        //Bouton de déconnexion qui redirige vers la page "MainWindow"
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        //Bouton de retour, qui redirige vers la page "Home" en passant en paramètre le joueur actuel afin de récupérer 
        //ses informations sur la page de retour.
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home(currentPlayer);
            home.Show();
            Close();
        }

        //Permet de récupérer l'objet booking selectionné dans le ListBox, et puis de le supprimer.
        // Comme le player paie en avance lors de la réservation, il est normal que les crédits soit
        //redistribué lors d'une suppression de réservation.
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupère l'objet Booking correspondant à l'élément sélectionné dans le ListBox
            Booking selectedBooking = lstBookings.SelectedItem as Booking;
            Player player = new Player();

            if (selectedBooking != null)
            {
                try
                {
                    int deletedBookingId = selectedBooking.IdBooking;

                    Booking B = new Booking();
                    B = B.Find(deletedBookingId);
                    VideoGame videoGame = new VideoGame();
                    videoGame = B.VideoGame;
                    int amount = videoGame.CreditCost * B.NumberOfWeeks;
                    currentPlayer.Credit += amount;
                    bool updateSuccess = currentPlayer.UpdateCredit();

                    if (updateSuccess)
                    {
                        // Supprime la réservation
                        bool success = selectedBooking.Delete();

                        if (success)
                        {
                            MessageBox.Show("Réservation supprimée avec succès ! Crédits ajoutés à votre compte.");
                            txtCredits.Text = $"Crédits : {currentPlayer.Credit}";
                            LoadBookingsForPlayer(); 
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression de la réservation.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la mise à jour du compte du joueur.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez une réservation");
            }
        }


    }
}
