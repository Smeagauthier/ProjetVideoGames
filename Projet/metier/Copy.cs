using Projet.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet.metier
{
    public class Copy
    {
        private int idCopy;
        private Player owner;
        private VideoGame videoGame;
        private Loan loan;

        public Copy(int idCopy, Player owner, VideoGame videoGame)
        {
            this.idCopy = idCopy;
            this.owner = owner;
            this.videoGame = videoGame;
        }

        public Copy()
        {

        }

        public int IdCopy { get { return idCopy; } set { idCopy = value; } }
        public Player Owner { get { return owner; } set { owner = value; } }
        public VideoGame VideoGame { get { return videoGame; } set {videoGame = value; } }
        public Loan Loan { get { return loan; } set { loan = value; } }
        

        public void ReleaseCopy()
        {

        }

        public void Borrow()
        {

        }

        public bool IsAvailable(LoanDAO loanDAO)
        {
            Loan currentLoan = loanDAO.GetCurrentLoanForCopy(this);

            if (currentLoan == null)
            {
                // Si aucune loan n'est associée à cette copie, elle est disponible
                return true;
            }

            if (currentLoan.EndDate < DateTime.Now || !currentLoan.Ongoing)
            {
                // Si la loan associée est terminée ou non en cours, la copie est disponible
                return true;
            }

            return false;
        }



        public bool Create()
        {
            CopyDAO copyDAO = new CopyDAO();
            return copyDAO.Create(this);
        }
       
        public override string ToString()
        {
            return $"Copy ID: {idCopy}, Owner: {owner?.UserName ?? "Aucun"}, VideoGame: {videoGame?.Name ?? "Inconnu"}, Loan Statut: {(loan?.Ongoing ?? false ? "Ongoing" : "Disponible")}";
        }



    }
}
