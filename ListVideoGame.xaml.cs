using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Projet
{
    public partial class ListVideoGame : Window
    {
        private Player currentPlayer;
        private Loan loan;

        //Permet d'afficher la fenêtre, en chargeant la méthode LoadPlayerLoans et les crédits
        public ListVideoGame(Player currentPlayer)
        {
            InitializeComponent();
            this.loan = new Loan();
            this.currentPlayer = currentPlayer;

            LoadPlayerLoans();

            if (currentPlayer != null)
            {
                txtCredits.Text = $"{currentPlayer.Credit}";
            }
        }

        //Permet de récupérer les prêts selon l'id d'un joueur (le joueur connecté)
        private void LoadPlayerLoans()
        {
            List<Loan> loans = loan.GetPlayerLoans(currentPlayer);
            lstLoans.ItemsSource = loans;
        }

        //Bouton de retour, qui redirige vers la page "Home" en passant en paramètre le joueur actuel afin de récupérer 
        //ses informations sur la page de retour.
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home(currentPlayer);
            home.Show();
            Close();
        }

        //Bouton de déconnexion qui redirige vers la page "MainWindow"
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        //Bouton qui permet de rendre un jeu loué en appelant EndLoan et LoadPlayerLoans
        private void GiveBackButton_Click(Object sender, RoutedEventArgs e)
        {
            Loan selectedLoan = lstLoans.SelectedItem as Loan;
            if (selectedLoan != null)
            {
                selectedLoan.EndLoan();
                txtCredits.Text = $"{currentPlayer.Credit}";
                LoadPlayerLoans();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un jeu loué avant de le rendre.");
            }
        }
    }
}
