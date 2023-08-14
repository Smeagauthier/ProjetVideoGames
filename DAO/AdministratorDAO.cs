using Projet.metier;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Projet.DAO
{
    public class AdministratorDAO : DAO<Administrator>
    {
        // !! LES COMMENTAIRES DANS LES METHODES DAO SONT UNIQUEMENT EXPLIQUEE DANS CETTE CLASSE DAO !!!! (Redondance...)
        private new string connectionString;
        //Elle utilise une chaîne de connexion pour établir une connexion à une base de données,
        //et cette chaîne de connexion est récupérée à partir d'un fichier de configuration.
        public AdministratorDAO() 
        {
            connectionString = ConfigurationManager.ConnectionStrings["VideoGames"].ConnectionString;
        }

        //Permet de créer un admin en utilisant un objet en paramètre
        //Méthode pas utilisée
        public override bool Create(Administrator obj)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    // Création d'une nouvelle instance de SqlCommand.requête et utilisation d'une condition sur l'idAdmin
                    SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Administrator (idAdministrator) VALUES (@idAdministrator);", connection);
                    //Remplace l'@id par l'id founi
                    cmd.Parameters.AddWithValue("@idAdministrator", obj.IdAdministrator);
                    //Ouverture de la connexion à la base de données
                    connection.Open();
                    //Retourne le nombre de ligne affectée
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            //Gestion des exceptions 
            catch (SqlException)
            {
                throw new Exception("Une erreur SQL s'est produite lors de la création de la réservation !");
            }
        }

        //Permet de supprimer un admin en utilisant un objet en paramètre
        //Méthode pas utilisée
        public override bool Delete(Administrator obj)
        {
            // Indicateur pour savoir si la suppression a réussi.
            bool success = false;

            //Ouverture de la connexion à la base de données.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Transaction SQL.
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // requête SQL de suppression.
                    SqlCommand updateCommand = new SqlCommand(
                        "DELETE dbo.Administrator WHERE idAdministrator = @idAdministrator",connection,transaction);

                    // Associe l'ID de l'administrateur à la requête.
                    updateCommand.Parameters.AddWithValue("@idAdministrator", obj.IdAdministrator);

                    // Retourne le nombre de ligne affectées
                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    // Si une ou plusieurs lignes sont affectées, valide la transaction.
                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                        success = true;
                    }
                    else // Sinon, annule la transaction.
                    {
                        transaction.Rollback();
                    }
                }
                // En cas d'erreur, annule la transaction et renvoye l'erreur.
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Erreur lors de la suppression de l'admin", ex);
                }
            }

            // Renvoie le résultat de la suppression.
            return success;
        }


        //Permet de trouver un admin selon son ID
        public override Administrator Find(int id)
        {
            //création d'un objet administrator
            Administrator administrator = null;
            try
            {
                //connexion à la base de données
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    // Création d'une nouvelle instance de SqlCommand.requête et utilisation d'une condition sur l'idAdmin
                    SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Administrator WHERE idAdministrator = @id", connection);
                    //Remplace l'@id par l'id fourni 
                    cmd.Parameters.AddWithValue("id", id);
                    //Ouverture de la connexion à la base de données
                    connection.Open();
                    // Exécution de la requête SQL et obtention d'un SqlDataReader pour lire les résultats.
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            administrator = new Administrator();
                            {
                                administrator.IdAdministrator = reader.GetInt32("idAdministrator");
                            };
                        }
                    }
                }
            }
            //Gestion des exceptions lors de l'utilisation de la méthode
            catch (SqlException)
            {
                throw new Exception("Une erreur sql s'est produite!");
            }
            return administrator;
        }

        //Permet de mettre à jour un admin en utilisant un objet en paramètre
        //Méthode pas utilisée
        //La table admin contient uniquement un id.
        public override bool Update(Administrator obj)
        {
            return false;
        }
    }
}
