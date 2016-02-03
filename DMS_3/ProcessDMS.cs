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
namespace DMS_3
{
	[Service]
	public class ProcessDMS : Service, ILocationListener
	{
		System.Threading.Timer _timer;
		String datedujour;
		LocationManager locMgr;
		public override void OnStart (Android.Content.Intent intent, int startId)
		{
			base.OnStart (intent, startId);

			Log.Debug ("SimpleService", "SimpleService started");

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
				}

			}

			Console.WriteLine ("\nTask InsertData done");
			File.AppendAllText(Data.log_file, "Task InsertData done"+DateTime.Now.ToString("t")+"\n");

		}

		void  ComPosNotifMsg ()
		{
			//recupération des messages wervice
			//insertion en base
			//recupation des messages / notifications / POS GPS
			//Post sur le webservice
			//maj du badge
			Console.WriteLine ("\nTask ComPosGps done");
			File.AppendAllText(Data.log_file, "Task ComPosGps done"+DateTime.Now.ToString("t")+"\n");
		}

		void  ComWebService ()
		{
			//récupération des données dans la BDD
			string dbPath = System.IO.Path.Combine (Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection (dbPath);
			var table = db.Table<TableStatutPositions> ();

			foreach (var item in table) {
				try {
					string _url = "http://dms.jeantettransport.com/api/livraisongroupage";
					var webClient = new WebClient ();
					webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
					//TODO
					//webClient.UploadString (_url, item.datajson);
					//Sup pde la row dans statut pos
					var row = db.Get<TableStatutPositions>(item.Id);
					db.Delete(row);
				} catch (Exception e) {
					Console.WriteLine (e);
				}
			}
			Console.WriteLine ("\nTask ComWebService done");
			File.AppendAllText(Data.log_file, "Task ComWebService done"+DateTime.Now.ToString("t")+"\n");
		}

		public void OnLocationChanged (Android.Locations.Location location)
		{
			Data.GPS = location.Latitude.ToString() +";"+ location.Longitude.ToString();
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
	}
}