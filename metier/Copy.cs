using Projet.DAO;
using System;

namespace Projet.metier
{
    public class Copy
    {
        private int idCopy;
        private Player owner;
        private VideoGame videoGame;
        private Loan loan;

        //---- Constructeur ----//
        public Copy()
        {

        }

        public Copy(int idCopy, Player owner, VideoGame videoGame)
        {
            this.idCopy = idCopy;
            this.owner = owner;
            this.videoGame = videoGame;
        }

       //---- Getter et Setter ----//

        public int IdCopy { get { return idCopy; } set { idCopy = value; } }
        public Player Owner { get { return owner; } set { owner = value; } }
        public VideoGame VideoGame { get { return videoGame; } set {videoGame = value; } }
        public Loan Loan { get { return loan; } set { loan = value; } }
        
        //---- Méthodes supplémentaires ----//
        public void ReleaseCopy()
        {

        }

        public void Borrow()
        {

        }

        //Permet de récupérer le player qui loue le jeu vidéo et de vérifier si...
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


        //---- Méthodes ----//

        //Fait référence à la méthode Create du DAO
        public bool Create()
        {
            CopyDAO copyDAO = new CopyDAO();
            return copyDAO.Create(this);
        }

        //Fait référence à la méthode DeleteAllCopiesForVideoGame du DAO
        public bool DeleteAllCopiesForVideoGame(VideoGame videoGame)
        {
            CopyDAO copyDAO = new CopyDAO();
            return copyDAO.DeleteAllCopiesForVideoGame(videoGame);
        }

        //---- ToString qui permet d'afficher toutes les infos d'une copie, sert de vérification ----//
        public override string ToString()
        {
            return $"Copy ID: {idCopy}, Owner: {owner?.UserName ?? "Aucun"}, VideoGame: {videoGame?.Name ?? "Inconnu"}, Loan Statut: {(loan?.Ongoing ?? false ? "Ongoing" : "Disponible")}";
        }



    }
}
