namespace Projet.metier
{
    public class Administrator : User
    {
        private int idAdministrator;

        //---- Constructeur ----//

        public Administrator()
        {

        }

        public Administrator(int idAdministrator)
        {
            this.idAdministrator = idAdministrator;
        }

        //---- Getter et setter ----//

        public int IdAdministrator { get { return idAdministrator; } set { idAdministrator = value; } }


    }
}
