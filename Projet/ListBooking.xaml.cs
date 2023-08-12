using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace Projet
{
    public partial class ListBooking : Window
    {
        private Player currentPlayer;
        private VideoGame videoGame;
        private PlayerDAO playerDAO;
        private string connectionString;


        public ListBooking(Player currentPlayer, VideoGame videoGame)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
            this.currentPlayer = currentPlayer;
            playerDAO = new PlayerDAO();

            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"Crédits : {credits}";

                LoadBookingsForPlayer();
            }
        }

       private void LoadBookingsForPlayer()
        {
            Booking booking = new Booking();
            List<Booking> bookings = booking.FindBookingsForPlayer(currentPlayer.IdPlayer); 
            lstBookings.ItemsSource = bookings;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home(currentPlayer);
            home.Show();
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer l'objet Booking correspondant à l'élément sélectionné dans le ListBox
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
                    bool updateSuccess = playerDAO.UpdateCredit(currentPlayer);

                    if (updateSuccess)
                    {
                        // Supprimer la réservation
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
