using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Projet.DAO;

namespace Projet.metier
{
    public abstract class User
    {
        private int idUser;
        private string userName;
        private string password;
        private UserDAO userDAO;
        private string connectionString;
        private PlayerDAO playerDAO;

        public User(string connectionString)
        {
            userDAO = new UserDAO(connectionString);
        }

        public User(UserDAO userDAO)
        {
            this.userDAO = userDAO;
        }

        public User(int idUser, string userName, string password)
        {
            this.idUser = idUser;
            this.userName = userName;
            this.password = password;
            userDAO = new UserDAO();
            playerDAO = new PlayerDAO();
        }

        public User(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
            userDAO = new UserDAO();
            playerDAO = new PlayerDAO();
        }

        public User()
        {
            userDAO = new UserDAO();
            playerDAO = new PlayerDAO();
        }

        public int IdUser
        {
            get { return idUser; }
            set { idUser = value; }
        }


        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        //****************** Méthodes ****************** //
        //---- Méthode pour la connexion des users ----//
        public int GetUserId()
        {
            return userDAO.GetUserId(UserName, Password);
        }

        public bool AuthenticateUser()
        {
            return userDAO.AuthenticateUser(UserName, Password);
        }

        public bool IsUserAdmin()
        {
            int idUser = GetUserId();
            return userDAO.IsUserAdmin(idUser);
        }

        //pour récupérer l'idUser de la table Player et User //
        public Player GetPlayer()
        {
            int idUser = GetUserId();
            if (idUser != -1) // Vérifiez si l'ID de l'utilisateur est valide
            {
                PlayerDAO playerDAO = new PlayerDAO();
                return playerDAO.FindByUserId(idUser);
            }
            return null;
        }







    }
}
