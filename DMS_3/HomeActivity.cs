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
using Android.Locations;
using System.Json;
using Java.Text;
using DMS_3.BDD;
using SQLite;
using Environment = System.Environment;

namespace DMS_3
{
	[Activity (Label = "HomeActivity",Theme = "@android:style/Theme.Black.NoTitleBar",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class HomeActivity : Activity, ILocationListener
	{
		TextView lblTitle;
		TextView peekupBadgeText;
		TextView newMsgBadgeText;
		TextView deliveryBadgeText;

		RelativeLayout deliveryBadge;
		RelativeLayout peekupBadge;
		RelativeLayout newMsgBadge;

		LocationManager locMgr;

		//bool Is_thread_Running = false;

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

			//click button
			LinearLayout btn_Livraison = FindViewById<LinearLayout> (Resource.Id.columnlayout1_1);
			btn_Livraison.Click += delegate { btn_Livraison_Click();};

			//btn deconnexion, userlogin false et update

			//TEST
			DBRepository dbr = new DBRepository ();
			var IntegData = dbr.InsertDataPosition("codeLivraison","numCommande","refClient","nomPayeur","nomExpediteur","adresseExpediteur","villeExpediteur","CpExpediteur","dateExpe","nomClient","adresseLivraison","villeLivraison","CpLivraison","dateHeure","poids","2","4","instrucLivraison","L","LIV","groupage","ADRCom","ADRGrp","0","CR",DateTime.Now.Day,"Datemission",1,"planDeTransport",Data.userAndsoft,"nomClientLivraison","villeClientLivraison",null);
			var IntegData1 = dbr.InsertDataPosition("codeLivraison","numCommande","refClient","nomPayeur","nomExpediteur","adresseExpediteur","villeExpediteur","CpExpediteur","dateExpe","nomClient","adresseLivraison","villeLivraison","CpLivraison","dateHeure","poids","2","4","instrucLivraison","L","LIV","groupage1","ADRCom","ADRGrp","0","CR",DateTime.Now.Day,"Datemission",2,"planDeTransport",Data.userAndsoft,"nomClientLivraison","villeClientLivraison",null);
		
			//LANCEMENT DU SERVICE
			StartService (new Intent (this, typeof(ProcessDMS)));
		
		}

		protected override void OnStart()
		{	

			base.OnStart();

		}


		protected override void OnResume()
		{
			base.OnResume();
			//Afficher ou non les badges

			// initialize location manager
			locMgr = GetSystemService (Context.LocationService) as LocationManager;

			if (locMgr.AllProviders.Contains (LocationManager.NetworkProvider)
				&& locMgr.IsProviderEnabled (LocationManager.NetworkProvider)) {
				locMgr.RequestLocationUpdates (LocationManager.NetworkProvider, 2000, 1, this);
			} else {
				Toast.MakeText (this, "GPS Désactiver!", ToastLength.Long).Show ();
			}

		
//			Thread ThreadAppInteg = new Thread(new ThreadStart(this.Threadapp));
//			ThreadAppInteg.Start();
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
		protected override void OnStop()
		{	

			File.AppendAllText(Data.log_file, "OnStop le "+DateTime.Now.ToString("G")+"\n");
			base.OnStop();
		}

		protected override void OnRestart()
		{	

			File.AppendAllText(Data.log_file, "OnRestart le "+DateTime.Now.ToString("G")+"\n");
			base.OnRestart();
		}

		void btn_Livraison_Click ()
		{
			StartActivity(typeof(ListeLivraisonsActivity));
		}
	}
}

