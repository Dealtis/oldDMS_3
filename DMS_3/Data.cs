using System;
using System.IO;
using Java.IO;
using System.Net;
using Android.Graphics;
using System.Threading;
using File = System.IO.File;
using Console = System.Console;
using SQLite;
using DMS_3.BDD;
using Xamarin;
using System.Json;
using Android.Media;

namespace DMS_3
{
	class Data
	{	

		//Instance
		private static Data instance;
		//DATA User
		public static string userAndsoft;
		public static string userTransics;

		//Log DATA
		public static string log_file;

		//GPS
		public static string GPS;

		//PHOTO
		public static Java.IO.File _file;
		public static Java.IO.File _dir;
		public static Bitmap bitmap;
		String datedujour;
		public static Data Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Data();
				}
				return instance;
			}
		}

		public bool  UploadFile(string FtpUrl, string fileName, string userName, string password,string UploadDirectory)
		{
			try{
				string PureFileName = new FileInfo(fileName).Name;
				String uploadUrl = String.Format("{0}{1}/{2}", FtpUrl,UploadDirectory,PureFileName);
				FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
				req.Proxy = null;
				req.Method = WebRequestMethods.Ftp.UploadFile;
				req.Credentials = new NetworkCredential(userName,password);
				req.UseBinary = true;
				req.UsePassive = true;
				byte[] data = System.IO.File.ReadAllBytes(fileName);
				req.ContentLength = data.Length;
				System.IO.Stream stream = req.GetRequestStream();
				stream.Write(data, 0, data.Length);
				stream.Close();
				FtpWebResponse res = (FtpWebResponse)req.GetResponse();
				File.AppendAllText(Data.log_file,"Upload file"+fileName+" good\n");
				Console.Out.Write("Upload file"+fileName+" good\n");
				return true;

			} catch (Exception ex) {
				Insights.Report(ex);
				File.AppendAllText(Data.log_file,"Upload file"+fileName+" error :"+ex+"\n");
				Console.Out.Write("Upload file"+fileName+" error\n");
				Thread.Sleep(TimeSpan.FromMinutes(2));
				UploadFile (FtpUrl, fileName, userName, password, UploadDirectory);
				return false;
			}
		}

		public void  InsertData ()
		{	
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			DBRepository dbr = new DBRepository ();
			datedujour = DateTime.Now.ToString("yyyyMMdd");

			//récupération de donnée via le webservice
			string content_integdata = "[]";
			try {
				string _url = "http://dms.jeantettransport.com/api/commande?codechauffeur=" + Data.userTransics + "&datecommande=" + datedujour + "";
				var webClient = new WebClient ();
				webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
				content_integdata = webClient.DownloadString (_url);
				Console.Out.WriteLine ("\nWebclient integdata Terminé");
				//intégration des données dans la BDD
				JsonArray jsonVal = JsonArray.Parse (content_integdata) as JsonArray;
				var jsonArr = jsonVal;
				foreach (var row in jsonArr) {
					bool checkpos = dbr.pos_AlreadyExist(row["numCommande"],row["groupage"]);
					Console.WriteLine ("\n"+checkpos+" "+row["userandsoft"]);
					if (!checkpos) {
						var IntegUser = dbr.InsertDataPosition(row["codeLivraison"],row["numCommande"],row["refClient"],row["nomPayeur"],row["nomExpediteur"],row["adresseExpediteur"],row["villeExpediteur"],row["CpExpediteur"],row["dateExpe"],row["nomClient"],row["adresseLivraison"],row["villeLivraison"],row["CpLivraison"],row["dateHeure"],row["poids"],row["nbrPallette"],row["nbrColis"],row["instrucLivraison"],row["typeMission"],row["typeSegment"],row["groupage"],row["ADRCom"],row["ADRGrp"],"0",row["CR"],DateTime.Now.Day,row["Datemission"],row["Ordremission"],row["planDeTransport"],Data.userAndsoft,row["nomClientLivraison"],row["villeClientLivraison"],null);
						Console.WriteLine ("\n"+IntegUser);
					}
				}

			} catch (Exception ex) {
				content_integdata = "[]";
				Console.WriteLine ("\n"+ex);
				Insights.Report(ex);
			}

			//maj des badges fonctions
			//TODO

			//verification des groupages et suppression des cloturer

			//select des grp's
			string content_grpcloture = String.Empty;
			var tablegroupage = db.Query<TablePositions> ("SELECT groupage FROM TablePositions group by groupage");
			foreach (var row in tablegroupage)
			{
				string numGroupage = row.groupage;
				try {
					string _urlb = "http://dms.jeantettransport.com/api/groupage?voybdx="+ numGroupage+"";
					var webClient = new WebClient ();
					webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
					content_grpcloture = webClient.DownloadString (_urlb);
					JsonArray jsonVal = JsonArray.Parse (content_grpcloture) as JsonArray;
					var jsonArr = jsonVal;
					foreach (var item in jsonArr) {						
						if (item["numCommande"] == "CLO") {
							//suppression du groupage en question si clo
							var suppgrp = dbr.supp_grp(numGroupage);
						}
					}
				}
				catch (Exception ex) {
					content_grpcloture = "[]";
					Console.WriteLine ("\n"+ex);
					Insights.Report(ex);
				}

			}

			Console.WriteLine ("\nTask InsertData done");
			File.AppendAllText(Data.log_file, "Task InsertData done"+DateTime.Now.ToString("t")+"\n");

		}
	}
}

