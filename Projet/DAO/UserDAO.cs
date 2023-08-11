using Projet.metier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projet.DAO
{
    public class UserDAO : DAO<User>
    {
        private string connectionString;
        

        public UserDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }

        public UserDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public override bool Create(User user)
        {
            bool achieved = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand($"Insert into dbo.User(username, password) values ({user.IdUser},'{user.UserName}','{user.Password}')", connection);
                connection.Open();
                int temp = cmd.ExecuteNonQuery();
                achieved = temp > 0;
            }
            return achieved;
        }
        public override bool Delete(User user)
        {
            bool achieved = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand($"Delete from dbo.Player where idUser = ({user.IdUser})", connection);
                connection.Open();
                int temp = cmd.ExecuteNonQuery();
                achieved = temp > 0;
            }
            return achieved;
        }

        public override User Find(int id)
        {
            return null;
        }

        public override bool Update(User obj)
        {
            return false;
        }
        //------------Récupération de l'idUser pour vérifier si il est admin ou connecté -----------------------//
        public int GetUserId(string username, string password)
        {
            int userId = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT idUser FROM dbo.[User] WHERE username = @username AND password = @password";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        userId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la connexion à la base de données : " + ex.Message);
            }

            return userId;
        }

        public bool AuthenticateUser(string username, string password)
        {
            
            bool isAuthenticated = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM dbo.[User] WHERE username = @username AND password = @password";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    connection.Open();

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count > 0)
                    {
                        // Utilisateur authentifié
                        int idUser = GetUserId(username, password);
                        isAuthenticated = true;

                        // Vérifier si l'utilisateur est administrateur
                        bool isAdmin = IsUserAdmin(idUser);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la connexion à la base de données : " + ex.Message);
            }

            return isAuthenticated;
        }

        public bool IsUserAdmin(int idUser)
        {
            bool isAdmin = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM dbo.Administrator WHERE idAdministrator = @idUser";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idUser", idUser);

                    connection.Open();

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    isAdmin = count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la connexion à la base de données : " + ex.Message);
            }

            return isAdmin;
        }

        public int CreateUser(User user)
        {
            int idUser = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Début de la transaction
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand userCommand = new SqlCommand(
                        "INSERT INTO dbo.[User] (username, password) VALUES (@username, @password)",
                        connection,
                        transaction
                    );
                    userCommand.Parameters.AddWithValue("@username", user.UserName);
                    userCommand.Parameters.AddWithValue("@password", user.Password);

                    // Exécuter la commande d'insertion de l'utilisateur
                    int rowsAffected = userCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Récupérer l'idUser généré en effectuant une requête supplémentaire
                        SqlCommand getIdCommand = new SqlCommand(
                            "SELECT idUser FROM dbo.[User] WHERE username = @username",
                            connection,
                            transaction
                        );
                        getIdCommand.Parameters.AddWithValue("@username", user.UserName);

                        idUser = Convert.ToInt32(getIdCommand.ExecuteScalar());

                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Erreur lors de la création de l'utilisateur : " + ex.Message);
                }
            }

            return idUser;
        }
    }
}
