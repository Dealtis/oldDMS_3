using System;
using System.Data;
using System.IO;
using SQLite;
using Android.Graphics;
using Xamarin;
using System.Threading.Tasks;
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
			var db = new SQLiteConnection (dbPath);		
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
				var db = new SQLiteConnection (dbPath);
				db.CreateTable<TableUser>();
				db.CreateTable<TablePositions>();
				db.CreateTable<TableStatutPositions>();
				db.CreateTable<TableMessages>();
				db.CreateTable<TableNotifications>();
				Console.Out.WriteLine("\nTable User Crée");
				string result = "Table crée !";
				return result;
			}
			catch (SQLiteException ex)
			{	
				Insights.Report(ex);
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
			catch (SQLiteException ex)
			{
				Insights.Report(ex);
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
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;
			}
		}


		//Insertion des données Message

		public string InsertDataMessage(string codeChauffeur,string utilisateurEmetteur, string texteMessage, int statutMessage, DateTime dateImportMessage, int typeMessage, int numMessage)
		{
			try
			{
				string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
					(Environment.SpecialFolder.Personal), "ormDMS.db3");
				var db = new SQLiteConnection(dbPath);
				TableMessages item = new TableMessages();
				item.codeChauffeur = codeChauffeur;
				item.utilisateurEmetteur =  utilisateurEmetteur;				
				item.texteMessage = texteMessage;
				item.statutMessage = statutMessage;
				item.dateImportMessage = dateImportMessage;
				item.typeMessage = typeMessage;
				item.numMessage = numMessage;
				db.Insert(item);
				return "Insertion good";
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;

			}
		}

		//Insertion des données STATUT MESSAGE
		public string InsertDataStatutMessage(int statutNotificationMessage, DateTime dateNotificationMessage, int numMessage, string numCommande, string groupage)
		{
			try
			{
				string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
					(Environment.SpecialFolder.Personal), "ormDMS.db3");
				var db = new SQLiteConnection(dbPath);
				TableNotifications item = new TableNotifications();
				item.statutNotificationMessage = statutNotificationMessage;
				item.dateNotificationMessage =  dateNotificationMessage;				
				item.numMessage = numMessage;
				item.numCommande = numCommande;
				item.groupage = groupage;
				db.Insert(item);
				return "\n"+statutNotificationMessage+" "+numCommande;
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;

			}
		}

		public string insertDataStatutpositions (string codesuiviliv, string statut, string libellesuiviliv,string commandesuiviliv, string memosuiviliv, string datesuiviliv, string datajson)
		{
			try
			{
				string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
					(Environment.SpecialFolder.Personal), "ormDMS.db3");
				var db = new SQLiteConnection(dbPath);
				TableStatutPositions item = new TableStatutPositions();
				item.commandesuiviliv = commandesuiviliv;
				item.codesuiviliv = codesuiviliv;
				item.statut = statut;
				item.libellesuiviliv = libellesuiviliv;
				item.memosuiviliv = memosuiviliv;
				item.datesuiviliv = datesuiviliv;
				item.datajson = datajson;
				db.Insert(item);
				return "Insertion good";
			}
			catch (SQLiteException ex)
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

		public string updatePosition (int idposition,string statut, string txtAnomalie, string txtRemarque, string codeAnomalie, string imgpath)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			string output = "";
			var row = db.Get<TablePositions>(idposition);
			row.StatutLivraison = statut;
			db.Update(row);
			output = "UPDATE POSITIONS " + row.Id;
			return output;
		}

		public string updatePositionSuppliv (string numCommande)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			string output = "";
			var query = db.Table<TablePositions>().Where (v => v.numCommande.Equals(numCommande));
			foreach (var item in query) {
				output = "YALO";
				var row = db.Get<TablePositions> (item.Id);
				row.imgpath = "SUPPLIV";
				db.Update(row);
				Console.WriteLine ("UPDATE SUPPLIV" + row.numCommande);
			}
			return output;
		}
		//USER CHECK LOGIN
		public string is_user_Log_In()
		{		
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			string output = "false";
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

		public string getUserAndsoft()
		{		
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			string output = string.Empty;
			var query = db.Table<TableUser>().Where (v => v.user_IsLogin.Equals(true));
			foreach (var item in query) {				
				output = item.user_AndsoftUser;
			}
			return output;

		}

		public string getUserTransics()
		{		
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			string output = string.Empty;
			var query = db.Table<TableUser>().Where (v => v.user_IsLogin.Equals(true));
			foreach (var item in query) {				
				output = item.user_TransicsUser;
			}
			return output;

		}

		public string logout()
		{		
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			string output = string.Empty;
			var query = db.Table<TableUser>().Where (v => v.user_IsLogin.Equals(true));
			foreach (var item in query) {
				var row = db.Get<TableUser>(item.Id);
				row.user_IsLogin = false;
				db.Update(row);
				output = "UPDATE USER LOGOUT " + row.user_AndsoftUser;
			}
			return output;
		}

	
		//VERIF SI POS DEJA INTEGRER
		public bool pos_AlreadyExist(string numCommande, string groupage, string typeMission, string typeSegment)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			bool output = false;
			var table = db.Table<TablePositions>().Where (v => v.numCommande.Equals(numCommande)).Where (v => v.groupage.Equals(groupage)).Where (v => v.typeMission.Equals(typeMission)).Where (v => v.typeSegment.Equals(typeSegment));
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

		//supp notification
		public string deletenotif(int id)
		{
			try
			{
				string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
					(Environment.SpecialFolder.Personal),"ormDMS.db3");
				var db = new SQLiteConnection(dbPath);
				db.Delete<TableNotifications>(id);
				string result = "delete";
				return result;
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;

			}
		}

		//SELECT PAR ID
		public TablePositions GetPositionsData(int id)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);

			TablePositions data = new TablePositions ();
			var item = db.Get<TablePositions>(id);
			data.codeLivraison = item.codeLivraison;
			data. numCommande = item.numCommande;
			data. nomClient = item.nomClient;
			data. refClient = item.refClient;
			data. nomPayeur = item.nomPayeur;
			data. adresseLivraison = item.adresseLivraison;
			data. CpLivraison = item.CpLivraison;
			data. villeLivraison = item.villeLivraison;
			data. dateHeure = item.dateHeure;
			data. dateExpe = item.dateExpe;
			data. nbrColis = item.nbrColis;
			data. nbrPallette = item.nbrPallette;
			data. adresseExpediteur = item.adresseExpediteur;
			data. CpExpediteur = item.CpExpediteur;
			data. villeExpediteur = item.villeExpediteur;
			data. nomExpediteur = item.nomExpediteur;
			data. StatutLivraison = item.StatutLivraison;
			data .instrucLivraison = item.instrucLivraison;
			data. groupage = item.groupage;
			data. ADRLiv = item.ADRLiv;
			data .ADRGrp = item.ADRGrp;
			data .planDeTransport = item.planDeTransport;
			data. typeMission = item.typeMission;
			data.typeSegment = item.typeSegment;
			data.CR = item.CR;
			data .nomClientLivraison = item.nomClientLivraison;
			data .villeClientLivraison = item.villeClientLivraison;
			data .Datemission = item.Datemission;
			data. Ordremission = item.Ordremission;
			data .Userandsoft = item.Userandsoft;
			data .remarque = item.remarque;
			data .codeAnomalie = item.codeAnomalie;
			data .libeAnomalie = item.libeAnomalie;
			data.imgpath = item.imgpath;

			if (Convert.ToDouble((item.poids).Replace ('.', ',')) < 1) {
				data.poids = ((Convert.ToDouble((item.poids).Replace ('.', ','))) * 1000) + " kg";
			} else {
				data.poids = item.poids + "tonnes";
			}
			return data;
		}			

		public int GetidPrev (int id)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			int idprev;
			//get int ordremission
			var item = db.Get<TablePositions>(id);
			idprev = (item.Ordremission) - 1;
			//getordremission -1
			var query = db.Table<TablePositions>().Where (v => v.Ordremission.Equals(idprev));
			//getordremission -1
			foreach (var row in query) {
				idprev = row.Id;
			}
			if (idprev < 0) {
				idprev = 0;
			}
			return 	idprev;
		}

		public int GetidNext (int id)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			int idnext;
			//get int ordremission
			var item = db.Get<TablePositions>(id);
			idnext = (item.Ordremission)+ 1;
			var query = db.Table<TablePositions>().Where (v => v.Ordremission.Equals(idnext));
			//getordremission -1

			foreach (var row in query) {
				idnext = row.Id;
			}

			if (idnext < 0) {
				idnext = 0;
			}
			return 	idnext;
		}

		public string updateposimgpath (int i, string path)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			string output = "";
			var row = db.Get<TablePositions>(i);
			row.imgpath = path;
			db.Update(row);
			output = "UPDATE POSITIONS " + row.Id;
			return output;
		}

		public string DropTableMessage()
		{
			try
			{
				string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
					(Environment.SpecialFolder.Personal),"ormDMS.db3");
				var db = new SQLiteConnection(dbPath);
				db.DeleteAll<TableMessages>();
				string result = "delete";
				return result;
			}
			catch (SQLiteException ex)
			{
				return "Erreur : " + ex.Message;

			}
		}

		//GET NUMBER LIV RAM ET MSG

		public int SETBadges (string userandsoft)
		{
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);

			var cLIV = db.Table<TablePositions>().Where (v => v.Userandsoft.Equals(userandsoft)).Where (v => v.typeMission.Equals("L")).Where (v => v.typeSegment.Equals("LIV")).Where (v => v.StatutLivraison.Equals("0")).Count();
			var cRam = db.Table<TablePositions>().Where (v => v.Userandsoft.Equals(userandsoft)).Where (v => v.typeMission.Equals("C")).Where (v => v.typeSegment.Equals("RAM")).Where (v => v.StatutLivraison.Equals("0")).Count();
			var cMsg = db.Table<TableMessages>().Where (v => v.statutMessage.Equals(0)).Count();

			var cSUPPLIV = db.Table<TablePositions>().Where (v => v.imgpath.Equals("SUPPLIV")).Count();

			Data.Instance.setLivraisonIndicator (cLIV - cSUPPLIV);
			Data.Instance.setEnlevementIndicator (cRam);
			Data.Instance.setMessageIndicator (cMsg);
			return 	0;
		}
	}
}
