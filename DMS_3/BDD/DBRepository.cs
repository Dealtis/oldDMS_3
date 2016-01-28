using System;
using System.Data;
using System.IO;
using SQLite;
using Android.Graphics;

namespace DMS_3.BDD
{		
	public class DBRepository
	{
		
		// CS comprennant toutes les requêtes BDD

		//CREATE BDD
		public string CreateDB()
		{
			var output = "";
			output += "Création de la BDD";
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal),"ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			output += "\nBDD crée...";
			return output;            
		}

		//CREATE TABLE
		public string CreateTable()
		{
			try
			{
				string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
					(Environment.SpecialFolder.Personal),"ormDMS.db3");
				var db = new SQLiteConnection(dbPath);
				db.CreateTable<TableUser>();
				db.CreateTable<TablePositions>();
				Console.Out.WriteLine("\nTable User Crée");
				string result = "Table crée !";
				return result;
			}
			catch (Exception ex)
			{	

				return "Erreur : " + ex.Message;

			}
		}

		//VERIF SI USER DEJA INTEGRER
		public bool user_AlreadyExist(string user_AndsoftUser, string user_TransicsUser, string user_Password, string user_UsePartic)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			bool output = false;
			var table = db.Table<TableUser>().Where (v => v.user_AndsoftUser.Equals(user_AndsoftUser)).Where (v => v.user_TransicsUser.Equals(user_TransicsUser)).Where (v => v.user_Password.Equals(user_Password)).Where (v => v.user_UsePartic.Equals(user_UsePartic));
			foreach (var item in table)
			{
				output = true;

			}
			return output;
		}

		//Insertion des DATS USER
		public string InsertDataUser(string user_AndsoftUser, string user_TransicsUser,string user_Password, string user_UsePartic)
		{
			try
			{
				string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
					(Environment.SpecialFolder.Personal), "ormDMS.db3");
				var db = new SQLiteConnection(dbPath);
				TableUser item = new TableUser();

				item.user_AndsoftUser =  user_AndsoftUser;
				item.user_TransicsUser = user_TransicsUser;
				item.user_Password = user_Password;
				item.user_UsePartic = user_UsePartic;
				db.Insert(item);

				return "Insertion" +user_AndsoftUser+" réussite";
			}
			catch (Exception ex)
			{
				return "Erreur : " + ex.Message;

			}
		}

		//Insertion des donnes des positions
		public string InsertDataPosition(string codeLivraison,string numCommande, string refClient, string nomPayeur, string nomExpediteur,string adresseExpediteur, string villeExpediteur, string CpExpediteur, string dateExpe, string nomClient, string adresseLivraison, string villeLivraison, string CpLivraison, string dateHeure, string poids, string nbrPallette, string nbrColis, string instrucLivraison, string typeMission, string typeSegment, string GROUPAGE,string AdrLiv, string AdrGrp, string statutLivraison, string CR,int dateBDD, string Datemission, int Ordremission, string planDeTransport, string Userandsoft, string nomClientLivraison, string villeClientLivraison, string imgpath)
		{
			try
			{
				string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
					(Environment.SpecialFolder.Personal), "ormDMS.db3");
				var db = new SQLiteConnection(dbPath);
				TablePositions item = new TablePositions();

				item.codeLivraison =  codeLivraison;
				item.numCommande = numCommande;
				item.nomClient =  nomClient ;
				item.refClient = refClient ;
				item.nomPayeur = nomPayeur;
				item.adresseLivraison = adresseLivraison;
				item.CpLivraison = CpLivraison;
				item.villeLivraison = villeLivraison;
				item.dateHeure = dateHeure;
				item.nbrColis = nbrColis;
				item.nbrPallette = nbrPallette;
				item.poids = poids;
				item.adresseExpediteur = adresseExpediteur;
				item.CpExpediteur = CpExpediteur;
				item.dateExpe = dateExpe;
				item.villeExpediteur = villeExpediteur;
				item.nomExpediteur = nomExpediteur;
				item.instrucLivraison = instrucLivraison;
				item.groupage = GROUPAGE;
				item.ADRLiv = AdrLiv;
				item.ADRGrp = AdrGrp;
				item.typeMission = typeMission;
				item.typeSegment = typeSegment;
				item.StatutLivraison = statutLivraison;
				item.CR = CR;
				item.dateBDD = dateBDD;
				item.Datemission = Datemission;
				item.Ordremission = Ordremission;
				item.planDeTransport = planDeTransport;
				item.Userandsoft = Userandsoft;
				item.nomClientLivraison = nomClientLivraison;
				item.villeClientLivraison = villeClientLivraison;
				item.imgpath = imgpath;
				db.Insert(item);
				return "Insertion good";
			}
			catch (Exception ex)
			{
				return "Erreur : " + ex.Message;

			}
		}

		//USER CHECK LOGIN
		public bool user_Check(string user_AndsoftUserTEXT,string user_PasswordTEXT)
		{		
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			bool output = false;

			var query = db.Table<TableUser>().Where (v => v.user_AndsoftUser.Equals(user_AndsoftUserTEXT)).Where (v => v.user_Password.Equals(user_PasswordTEXT));

			foreach (var item in query) {
				output = true;
				var row = db.Get<TableUser> (item.Id);
				row.user_IsLogin = true;
				db.Update(row);
				Console.WriteLine ("UPDATE GOOD" + row.user_IsLogin);

			}

			return output;

		}

		//USER CHECK LOGIN
		public string is_user_Log_In()
		{		
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			string output = string.Empty;
			var query = db.Table<TableUser>().Where (v => v.user_IsLogin.Equals(true));
			foreach (var item in query) {
				output = item.user_AndsoftUser;
				Console.WriteLine ("\nUSER CONNECTE" + item.user_AndsoftUser);
			}
			return output;

		}
		//setUserdata

		public string setUserdata(string UserAndsoft)
		{		
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			string output = string.Empty;
			var query = db.Table<TableUser>().Where (v => v.user_AndsoftUser.Equals(UserAndsoft));
			foreach (var item in query) {
				Data.userAndsoft = item.user_AndsoftUser;
				Data.userTransics = item.user_TransicsUser;
				output = "setUserdata good";
				Console.WriteLine ("\nUSER CONNECTE" + item.user_AndsoftUser);
			}
			return output;

		}
	
		//VERIF SI POS DEJA INTEGRER
		public bool pos_AlreadyExist(string numCommande, string groupage)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			bool output = false;
			var table = db.Table<TablePositions>().Where (v => v.numCommande.Equals(numCommande)).Where (v => v.groupage.Equals(groupage));
			foreach (var item in table)
			{
				output = true;


			}
			return output;
		}

		//suppresion d'un GRP
		public string supp_grp(string numGroupage)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			string output = "";

			var query = db.Table<TablePositions>().Where (v => v.groupage.Equals(numGroupage));
			foreach (var item in query) {
				output = item.groupage;
				var row = db.Get<TablePositions>(item.Id);
				db.Delete(row);
				Console.WriteLine ("\nDELETE GOOD" + numGroupage);
			}
			return output;
		}




			
	}
}
