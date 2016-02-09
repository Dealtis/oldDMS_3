﻿using System;
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
using Android.Support.V4;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using DMS_3.BDD;
using Java.Text;
using SQLite;
using Xamarin;
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
			LinearLayout btn_Enlevement = FindViewById<LinearLayout> (Resource.Id.columnlayout1_2);
			LinearLayout btn_Message = FindViewById<LinearLayout> (Resource.Id.columnlayout2_1);

			btn_Livraison.Click += delegate { btn_Livraison_Click();};

			btn_Enlevement.Click += delegate { btn_Enlevement_Click ();};
			btn_Livraison.LongClick += Btn_Livraison_LongClick;
			btn_Message.Click += delegate { btn_Message_Click();};

			//btn deconnexion, userlogin false et update


			//TEST
			DBRepository dbr = new DBRepository ();
			var IntegData = dbr.InsertDataPosition("codeLivraison","numCommande","refClient","nomPayeur","nomExpediteur","adresseExpediteur","villeExpediteur","CpExpediteur","dateExpe","nomClient","adresseLivraison","villeLivraison","CpLivraison","dateHeure","poids","2","4","instrucLivraison","L","LIV","groupage","ADRCom","ADRGrp","0","CR",DateTime.Now.Day,"Datemission",0,"planDeTransport",Data.userAndsoft,"nomClientLivraison","villeClientLivraison",null);
			var IntegData1 = dbr.InsertDataPosition("codeLivraison","numCommande","refClient","nomPayeur","nomExpediteur","adresseExpediteur","villeExpediteur","CpExpediteur","dateExpe","nomClient","adresseLivraison","villeLivraison","CpLivraison","dateHeure","poids","2","4","instrucLivraison","C","RAM","groupage1","ADRCom","ADRGrp","0","CR",DateTime.Now.Day,"Datemission",1,"planDeTransport",Data.userAndsoft,"nomClientLivraison","villeClientLivraison",null);
			//var IntegData2 = dbr.InsertDataPosition("codeLivraison","numCommande","refClient","nomPayeur","nomExpediteur","adresseExpediteur","villeExpediteur","CpExpediteur","dateExpe","nomClient","adresseLivraison","villeLivraison","CpLivraison","dateHeure","poids","2","4","instrucLivraison","L","LIV","groupage1","ADRCom","ADRGrp","0","CR",DateTime.Now.Day,"Datemission",2,"planDeTransport",Data.userAndsoft,"nomClientLivraison","villeClientLivraison",null);

			//Xamarin Insight
			Insights.Initialize("d3afeb59463d5bdc09194186b94fc991016faf1f", this);
			Insights.Identify(Data.userAndsoft,"Name",Data.userAndsoft);
			//LANCEMENT DU SERVICE
			StartService (new Intent (this, typeof(ProcessDMS)));
		
		}

		void Btn_Livraison_LongClick (object sender, View.LongClickEventArgs e)
		{
			
			RunOnUiThread (() => {
				try {
					Data.Instance.InsertData ();
					AndHUD.Shared.ShowSuccess(this, "Mise à jour réussite!", MaskType.Clear, TimeSpan.FromSeconds(2));
				} catch (Exception ex) {
					Console.WriteLine ("\n"+ex);
					AndHUD.Shared.ShowError(this, "Error : "+ex, MaskType.Black, TimeSpan.FromSeconds(2));
				}
			}
			);
		}

		protected override void OnStart()
		{	

			base.OnStart();

		}


		protected override void OnResume()
		{
			base.OnResume();
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
			Intent intent = new Intent (this, typeof(ListeLivraisonsActivity));
			intent.PutExtra("TYPE","LIV");
			this.StartActivity (intent);
			this.OverridePendingTransition (Resource.Animation.abc_slide_in_top,Resource.Animation.abc_slide_out_bottom);
		}

		void btn_Enlevement_Click ()
		{
			Intent intent = new Intent (this, typeof(ListeLivraisonsActivity));
			intent.PutExtra("TYPE","RAM");
			this.StartActivity (intent);
			this.OverridePendingTransition (Resource.Animation.abc_slide_in_top,Resource.Animation.abc_slide_out_bottom);
		}

		void btn_Message_Click ()
		{
			Intent intent = new Intent (this, typeof(MessageActivity));
			this.StartActivity (intent);
			this.OverridePendingTransition (Resource.Animation.abc_slide_in_top,Resource.Animation.abc_slide_out_bottom);
		}
	}
}

