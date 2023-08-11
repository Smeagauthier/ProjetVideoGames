using Projet.DAO;
using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Numerics;
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
    public partial class ListVideoGame : Window
    {
        private Player currentPlayer;
        private Loan loan;
        private string connectionString;
        public ListVideoGame(Player currentPlayer)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
            this.loan = new Loan(connectionString);
            this.currentPlayer = currentPlayer;

            LoadPlayerLoans();

            if (currentPlayer != null)
            {
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"{credits}";
            }
        }

        private void LoadPlayerLoans()
        {
            List<Loan> loans = loan.GetPlayerLoans(currentPlayer);
            lstLoans.ItemsSource = loans; // Lie la liste des prêts à la ListBox.
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home(currentPlayer);
            home.Show();
            Close();
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void GiveBackButton_Click(Object sender, RoutedEventArgs e)
        {
            Loan selectedLoan = lstLoans.SelectedItem as Loan;
            if (selectedLoan != null)
            {
                selectedLoan.CalculateBalance();
                PlayerDAO player = new PlayerDAO();
                /*int idBorrower = selectedLoan.Borrower.IdPlayer;
                Player borrower = player.Find(idBorrower);
                int creditsUpdate = borrower.Credit; 
                txtCredits.Text = $"{creditsUpdate}";*/
                selectedLoan.EndLoan();
                int credits = currentPlayer.Credit;
                txtCredits.Text = $"{credits}";
                // Rafraîchir la liste des prêts pour montrer les mises à jour
                LoadPlayerLoans();

            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un jeu loué avant de le rendre.");
            }
        }
    }
}
