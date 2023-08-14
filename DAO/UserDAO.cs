using Projet.metier;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace Projet.DAO
{
    public class UserDAO : DAO<User>
    {
        private new string connectionString;

        //Permet d'ajouter au constructeur la connexion à la BD en passant par le fichier "App.config"
        public UserDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }

        //Permet de créer un User
        //méthode pas utilisée
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

        //Permet de supprimer un User
        //méthode pas utilisée
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

        //Permet de chercher un User avec un id donné
        //Méthode pas utilisée
        public override User Find(int id)
        {
            return null;
        }

        //Permet
        //Méthode pas utilisée
        public override bool Update(User obj)
        {
            return false;
        }
        //------------Récupération de l'idUser pour vérifier si il est admin ou connecté -----------------------//
        //Permet de récupérer l'ID d'un utilisateur en fonction de son nom d'utilisateur et de son mot de passe.
        public int? GetUserId(string username, string password)
        {
            int? userId = null;

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

        //Permet de vérifier si un utilisateur est connecté ou non 
        public bool AuthenticateUser(string username, string password)
        {
            bool isAuthenticated = false;

            try
            {
                int? userId = GetUserId(username, password);

                isAuthenticated = userId.HasValue;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la connexion à la base de données : " + ex.Message);
            }

            return isAuthenticated;
        }

        //Permet de vérifier si un utilisateur est admin ou non
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
