using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using DMS_3.BDD;
using Java.Text;
using SQLite;
using Environment = System.Environment;
using Xamarin;
using Android.Media;


namespace DMS_3
{
	[Service]
	public class ProcessDMS : Service, ILocationListener
	{
		System.Threading.Timer _timer;
		String datedujour;
		LocationManager locMgr;
		String userAndsoft;
		String userTransics;
		String GPS;
		public override void OnStart (Android.Content.Intent intent, int startId)
		{
			base.OnStart (intent, startId);

			DBRepository dbr = new DBRepository ();
			userAndsoft = dbr.getUserAndsoft ();
			userTransics = dbr.getUserTransics ();

			DoStuff ();

			// initialize location manager
			locMgr = GetSystemService (Context.LocationService) as LocationManager;

			if (locMgr.AllProviders.Contains (LocationManager.NetworkProvider)
				&& locMgr.IsProviderEnabled (LocationManager.NetworkProvider)) {
				locMgr.RequestLocationUpdates (LocationManager.NetworkProvider, 2000, 1, this);
			} else {
				//Toast.MakeText (this, "GPS Désactiver!", ToastLength.Long).Show ();
			}
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();

			_timer.Dispose ();

			Log.Debug ("SimpleService", "SimpleService stopped");       
		}

		public void DoStuff ()
		{
			_timer = new System.Threading.Timer ((o) => {

				//Verification de la connexion
				var connectivityManager = (ConnectivityManager)GetSystemService (ConnectivityService);
				var activeConnection = connectivityManager.ActiveNetworkInfo;
				if ((activeConnection != null) && activeConnection.IsConnected) {
					//execution des fonctions dans une boucle
					Task.Factory.StartNew (

						() => {
							Console.WriteLine ("\nHello from InsertData.");
							InsertData ();//TOUTES LES 2 MINS
						}					
					).ContinueWith (
						t => {
							Console.WriteLine ("\nHello from ComWebService.");
							ComWebService ();//TOUTES LES 2MIN
						}						
					).ContinueWith (
						v => {
							Console.WriteLine ("\nHello from ComPosNotifMsg.");
							ComPosNotifMsg ();//TOUTES LES 2MIN
						}
					);
				}
			}, null, 0, 120000);
		}

		public override Android.OS.IBinder OnBind (Android.Content.Intent intent)
		{
			throw new NotImplementedException ();
		}
		void  InsertData ()
		{	
			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			DBRepository dbr = new DBRepository ();
			datedujour = DateTime.Now.ToString("yyyyMMdd");

			//récupération de donnée via le webservice
			string content_integdata = String.Empty;
			try {				string _url = "http://dms.jeantettransport.com/api/commande?codechauffeur=" + userTransics + "&datecommande=" + datedujour + "";
				var webClient = new WebClient ();
				webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
				content_integdata = webClient.DownloadString (_url);
				Console.Out.WriteLine ("\nWebclient integdata Terminé");
				//intégration des données dans la BDD
				JsonArray jsonVal = JsonArray.Parse (content_integdata) as JsonArray;
				var jsonArr = jsonVal;
				if (content_integdata != "[]") {
					foreach (var row in jsonArr) {
						bool checkpos = dbr.pos_AlreadyExist(row["numCommande"],row["groupage"]);
						if (!checkpos) {
							var IntegUser = dbr.InsertDataPosition(row["codeLivraison"],row["numCommande"],row["refClient"],row["nomPayeur"],row["nomExpediteur"],row["adresseExpediteur"],row["villeExpediteur"],row["CpExpediteur"],row["dateExpe"],row["nomClient"],row["adresseLivraison"],row["villeLivraison"],row["CpLivraison"],row["dateHeure"],row["poids"],row["nbrPallette"],row["nbrColis"],row["instrucLivraison"],row["typeMission"],row["typeSegment"],row["groupage"],row["ADRCom"],row["ADRGrp"],"0",row["CR"],DateTime.Now.Day,row["Datemission"],row["Ordremission"],row["planDeTransport"],userAndsoft,row["nomClientLivraison"],row["villeClientLivraison"],null);
							var resintegstatut = dbr.InsertDataStatutMessage (10,DateTime.Now,0,row["numCommande"],row["groupage"]);
							Console.WriteLine ("\n"+IntegUser);
						}
					}
				}

				//SON
				if (content_integdata == "[]") {
				} else {
					alert ();
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
					JsonValue jsonVal = JsonObject.Parse(content_grpcloture);
					//JsonArray jsonVal = JsonArray.Parse (content_grpcloture) as JsonArray;
					//var jsonArr = jsonVal;							
					if (jsonVal["etat"].ToString() == "\"CLO\""){
							//suppression du groupage en question si clo
							var suppgrp = dbr.supp_grp(numGroupage);
					}					
				}
				catch (Exception ex) {
					content_grpcloture = "[]";
					Console.WriteLine ("\n"+ex);
					Insights.Report(ex);
				}

			}

			Console.WriteLine ("\nTask InsertData done");
			//File.AppendAllText(Data.log_file, "Task InsertData done"+DateTime.Now.ToString("t")+"\n");

		}

		void  ComPosNotifMsg ()
		{
			//recupération des messages webservice
			//insertion en base
			//recupation des messages / notifications / POS GPS
			//Post sur le webservice
			//maj du badge

				//API GPS OK
				string _url = "http://dms.jeantettransport.com/api/leslie";
				string dbPath = System.IO.Path.Combine (System.Environment.GetFolderPath
					(System.Environment.SpecialFolder.Personal), "ormDMS.db3");
				var db = new SQLiteConnection (dbPath);

				DBRepository dbr = new DBRepository ();
				var webClient = new WebClient ();

				try {

				string content_msg = String.Empty;
					//ROUTINE INTEG MESSAGE
					try {
					
						//API LIVRER OK
						string _urlb = "http://dms.jeantettransport.com/api/leslie?codechauffeur=" + userAndsoft +"";
						var webClientb = new WebClient ();
						webClientb.Headers [HttpRequestHeader.ContentType] = "application/json";
						//webClient.Encoding = Encoding.UTF8;

						content_msg = webClientb.DownloadString (_urlb);
					} catch (Exception ex) {
						content_msg = "[]";
						Insights.Report (ex,Xamarin.Insights.Severity.Error);

					}
				if (content_msg != "[]") {
					JsonArray jsonVal = JsonArray.Parse (content_msg) as JsonArray;
					var jsonarr = jsonVal;
					foreach (var item in jsonarr) {
						if (item["texteMessage"].ToString().Length < 9) {
							var resinteg = dbr.InsertDataMessage (item ["codeChauffeur"], item ["utilisateurEmetteur"], item ["texteMessage"],0,DateTime.Now,1,item ["numMessage"]);
							var resintegstatut = dbr.InsertDataStatutMessage(0,DateTime.Now,item ["numMessage"],"","");
							alertsms ();	
						}else{
						switch(item ["texteMessage"].ToString().Substring(1,9))
							{
							case "%%SUPPLIV":
								//var updatestatt = db.Query<TablePositions>("UPDATE TablePositions SET imgpath = ? WHERE numCommande = ?","SUPPLIV",(item ["texteMessage"].ToString()).Remove((item ["texteMessage"].ToString()).Length - 3).Substring(11));
								var updatestat = dbr.updatePositionSuppliv((item ["texteMessage"].ToString()).Remove((item ["texteMessage"].ToString()).Length - 3).Substring(11));
								dbr.InsertDataStatutMessage (1,DateTime.Now,item ["numMessage"],"","");
								dbr.InsertDataMessage (item ["codeChauffeur"], item ["utilisateurEmetteur"],"La position "+(item ["texteMessage"].ToString()).Remove((item ["texteMessage"].ToString()).Length - 3).Substring(11)+" a été supprimée de votre tournée",0,DateTime.Now,1, item ["numMessage"]);
								break;
							case "%%RETOLIV":
								var updatestattretour = db.Query<TablePositions>("UPDATE TablePositions SET imgpath = null WHERE numCommande = ?",(item ["texteMessage"].ToString()).Remove((item ["texteMessage"].ToString()).Length - 3).Substring(11));
								var resstatutbis = dbr.InsertDataStatutMessage (1,DateTime.Now,item ["numMessage"],"","");
								break;
							case "%%SUPPGRP":
								var supgrp = db.Query<TablePositions>("DELETE from TablePositions where groupage = ?",(item ["texteMessage"].ToString()).Remove((item ["texteMessage"].ToString()).Length - 2).Substring(10));
								var ressupgrp = dbr.InsertDataStatutMessage (1,DateTime.Now,item ["numMessage"],"","");
								break;
							case "%%GETFLOG":
								//("ftp://77.158.93.75");
								Thread thread = new Thread(() => UploadFile("ftp://10.1.2.75",Data.log_file,"DMS","Linuxr00tn",""));
								thread.Start ();
								break;
							default:
								var resinteg = dbr.InsertDataMessage (item ["codeChauffeur"], item ["utilisateurEmetteur"], item ["texteMessage"],0,DateTime.Now,1,item ["numMessage"]);
								var resintegstatut = dbr.InsertDataStatutMessage(0,DateTime.Now,item ["numMessage"],"","");
								alertsms ();
								Console.WriteLine (item ["numMessage"].ToString());
								Console.WriteLine (resinteg);
								break;
							}
						}
					}
				}
				String datajson = string.Empty;
				String datagps=string.Empty;
				String datamsg=string.Empty;
				String datanotif=string.Empty;


				datagps = "{\"posgps\":\"" + GPS + "\",\"userandsoft\":\"" + userAndsoft + "\"}";

				var tablestatutmessage = db.Query<TableNotifications> ("SELECT * FROM TableNotifications");

				//SEND NOTIF
				foreach (var item in tablestatutmessage) {
					datanotif += "{\"statutNotificationMessage\":\"" + item.statutNotificationMessage + "\",\"dateNotificationMessage\":\"" + item.dateNotificationMessage + "\",\"numMessage\":\""+item.numMessage+"\",\"numCommande\":\""+item.numCommande+"\",\"groupage\":\""+item.groupage+"\"},";
				}

				//SEND MESSAGE
				var tablemessage = db.Query<TableMessages> ("SELECT * FROM TableMessages WHERE statutMessage = 2");
				foreach (var item in tablemessage) {
					datamsg += "{\"codeChauffeur\":\"" + item.codeChauffeur + "\",\"texteMessage\":\"" + item.texteMessage + "\",\"utilisateurEmetteur\":\""+item.utilisateurEmetteur+"\",\"dateImportMessage\":\""+item.dateImportMessage+"\",\"typeMessage\":\""+item.typeMessage+"\"},";
				}
				if(datanotif == ""){
					datanotif ="{}";
				}else{
					datanotif = datanotif.Remove(datanotif.Length - 1);
				}
				if(datamsg == ""){
					datamsg ="{}";
				}else{
					datamsg = datamsg.Remove(datamsg.Length - 1);
				}

				datajson = "{\"suivgps\":"+datagps+",\"statutmessage\":["+datanotif+"],\"Message\":["+datamsg+"]}";

				//API MSG/NOTIF/GPS
				try{
					webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
					webClient.UploadString (_url,datajson);
					foreach (var item in tablestatutmessage) {
						var resultdelete = dbr.deletenotif(item.Id);
					}
					foreach (var item in tablemessage) {
					var updatestatutmessage = db.Query<TableMessages> ("UPDATE TableMessages SET statutMessage = 3 WHERE _Id = ?",item.Id);
					}
				}
				catch (Exception e)
				{
					Insights.Report (e,Xamarin.Insights.Severity.Error);
				}
				} catch (Exception ex) {
					Insights.Report (ex,Xamarin.Insights.Severity.Error);
					Console.Out.Write(ex);
				}
			Console.WriteLine ("\nTask ComPosGps done");
		}
		

		void  ComWebService ()
		{
			//récupération des données dans la BDD
			string dbPath = System.IO.Path.Combine (Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection (dbPath);
			var table = db.Query<TableStatutPositions> ("Select * FROM TableStatutPositions");

			foreach (var item in table) {
				try {
					string _url = "http://dms.jeantettransport.com/api/livraisongroupage";
					var webClient = new WebClient ();
					webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
					//TODO
					webClient.UploadString (_url, item.datajson);
					//Sup pde la row dans statut pos
					var row = db.Get<TableStatutPositions>(item.Id);
					db.Delete(row);
				} catch (Exception e) {
					Console.WriteLine (e);
					Insights.Report(e);
				}
			}
			Console.WriteLine ("\nTask ComWebService done");
			//File.AppendAllText(Data.log_file, "Task ComWebService done"+DateTime.Now.ToString("t")+"\n");
		}

		public void OnLocationChanged (Android.Locations.Location location)
		{
			GPS = location.Latitude.ToString() +";"+ location.Longitude.ToString();
			Data.GPS = GPS = location.Latitude.ToString() +";"+ location.Longitude.ToString();
		}
		public void OnProviderDisabled (string provider)
		{

		}
		public void OnProviderEnabled (string provider)
		{

		}
		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{

		}

		public void alert()
		{

			MediaPlayer _player;
			_player = MediaPlayer.Create(this,Resource.Raw.beep4);
			_player.Start();
		}

		public void alertsms()
		{

			MediaPlayer _player;
			_player = MediaPlayer.Create(this,Resource.Raw.msg3);
			_player.Start();
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
	}
}