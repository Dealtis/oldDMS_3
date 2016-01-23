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
			var table = db.Query<TableUser>("SELECT * FROM TableUser WHERE user_AndsoftUser = ? and user_TransicsUser = ? and user_Password = ? and user_UsePartic = ?",user_AndsoftUser,user_TransicsUser,user_Password,user_UsePartic);
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
			
	}
}
