using Projet.DAO;

namespace Projet.metier
{
    public abstract class User
    {
        private int idUser;
        private string userName;
        private string password;
        private UserDAO userDAO;
        private PlayerDAO playerDAO;

        //---- Constructeur ----//

        public User()
        {

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

        

        //---- Getter et Setter ----//
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
        //---- Méthodes supplémentaires ----//

        //Permet de récupérer l'identifiant (ID) de l'utilisateur basé sur son nom d'utilisateur (UserName) et son mot de passe (Password).//
        public int GetUserId()
        {
            return (int)userDAO.GetUserId(UserName, Password);
        }

        //Permet d'uthentifier un utilisateur en se basant sur son nom d'utilisateur et son mot de passe.
        public bool AuthenticateUser()
        {
            return userDAO.AuthenticateUser(UserName, Password);
        }

        //Permet de vérifier si un utilisateur est un admin ou non
        public bool IsUserAdmin()
        {
            int idUser = GetUserId();
            return userDAO.IsUserAdmin(idUser);
        }

        //Permet de retrouver un objet Player basé sur l'ID de l'utilisateur associé //
        public Player FindByUserId()
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
