using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.metier
{
    public class Administrator : User
    {
        private int idAdministrator;

        public Administrator(int idAdministrator)
        {
            this.idAdministrator = idAdministrator;
        }

        public Administrator()
        {

        }

        public int IdAdministrator { get { return idAdministrator; } set { idAdministrator = value; } }
    }
}
