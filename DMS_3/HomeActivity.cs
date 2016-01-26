using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Net;
using System.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System.Json;

using DMS_3.BDD;
using SQLite;
using Environment = System.Environment;

namespace DMS_3
{
	[Activity (Label = "HomeActivity",Theme = "@android:style/Theme.Black.NoTitleBar",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class HomeActivity : Activity
	{
		TextView lblTitle;
		TextView peekupBadgeText;
		TextView newMsgBadgeText;
		TextView deliveryBadgeText;

		RelativeLayout deliveryBadge;
		RelativeLayout peekupBadge;
		RelativeLayout newMsgBadge;

		String datedujour;
		String datedujour_day;
		String datedujour_mouth;
		String datedujour_hour;
		String datedujour_minute;

		bool Is_thread_Running = false;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Home);

			//DECLARATION DES ITEMS
			lblTitle = FindViewById<TextView>(Resource.Id.lblTitle);
			peekupBadgeText = FindViewById<TextView>(Resource.Id.peekupBadgeText);
			newMsgBadgeText = FindViewById<TextView>(Resource.Id.newMsgBadgeText);
			deliveryBadgeText = FindViewById<TextView>(Resource.Id.deliveryBadgeText);
			deliveryBadge = FindViewById<RelativeLayout>(Resource.Id.deliveryBadge);
			peekupBadge = FindViewById<RelativeLayout>(Resource.Id.peekupBadge);
			newMsgBadge = FindViewById<RelativeLayout>(Resource.Id.deliveryBadge);

			//Mettre le lblTitle: User + versionNumber
			Context context = this.ApplicationContext;
			var version = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;

			//Lancer les threads : boucles avec les différent actions en cascade UIThread

		}

		protected override void OnStart()
		{	

			base.OnStart();

		}


		protected override void OnResume()
		{
			base.OnResume();
			//Afficher ou non les badges

			Thread ThreadAppInteg = new Thread(new ThreadStart(this.Threadapp));
			ThreadAppInteg.Start();
		}

		void Threadapp ()
		{
			if (!Is_thread_Running) {
				Is_thread_Running = true;
				while (true) {
					//Verification de la connexion
					var connectivityManager = (ConnectivityManager)GetSystemService (ConnectivityService);
					var activeConnection = connectivityManager.ActiveNetworkInfo;
					if ((activeConnection != null) && activeConnection.IsConnected) {
						//execution des fonctions dans une boucle
						Task.Factory.StartNew (

							() => {
								Console.WriteLine ("\nHello from InsertData.");
								InsertData ();
							}					
						).ContinueWith (
							t => {
								Console.WriteLine ("\nHello from ComWebService.");
								ComWebService ();
							}						
						).ContinueWith (
							v => {
								Console.WriteLine ("\nHello from ComPosGps.");
								ComPosGps ();
							}
						);
					}
					Thread.Sleep (TimeSpan.FromSeconds (60));
				}	
			
			}
		}

		void  InsertData ()
		{	
			Thread.Sleep (5000);

			string dbPath = System.IO.Path.Combine(Environment.GetFolderPath
				(Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection(dbPath);
			DBRepository dbr = new DBRepository ();
			//concaténation de la date
			if (DateTime.Now.Day < 10) {
				datedujour_day = "0" + DateTime.Now.Day;
			} else {
				datedujour_day = (DateTime.Now.Day).ToString();
			}

			if (DateTime.Now.Month < 10) {
				datedujour_mouth = "0" + DateTime.Now.Month;
			} else {
				datedujour_mouth = (DateTime.Now.Month).ToString();
			}

			if (DateTime.Now.Hour < 10) {
				datedujour_hour = "0" + DateTime.Now.Hour;
			} else {
				datedujour_hour = (DateTime.Now.Hour).ToString();
			}

			if (DateTime.Now.Minute < 10) {
				datedujour_minute = "0" + DateTime.Now.Minute;
			} else {
				datedujour_minute = (DateTime.Now.Minute).ToString();
			}

			datedujour = DateTime.Now.Year + datedujour_mouth + datedujour_mouth + datedujour_day;

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
					var checkpos = dbr.pos_AlreadyExist(row["numCommande"],row["groupage"]);
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
			var tablegroupage = db.Query<TablePosition> ("SELECT groupage FROM ToDoTask group by groupage");
			foreach (var row in tablegroupage)
			{
				string numGroupage = row.groupage;
				try {
					string _urlb = "http://dms.jeantettransport.com/api/groupage?voybdx="+ numGroupage+"";
					var webClient = new WebClient ();
					webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
					//webClient.Encoding = Encoding.UTF8;
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
		}

		void  ComWebService ()
		{
			//envoi des données des positions traités

			//envoi des messages et des positions GPS et des notif MSg
			Console.WriteLine ("\nTask ComWebService done");
		}

		void  ComPosGps ()
		{
			//récupération des donnée de message via le webservice

			//intégration des données dans la BDD

			//maj des badges
			Console.WriteLine ("\nTask ComPosGps done");
		}
	}
}

