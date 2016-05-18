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
using Android.Telephony;
using Android.Support.V7;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using DMS_3.BDD;
using Java.Text;
using SQLite;
using Xamarin;
using Environment = System.Environment;
using Thread = Java.Lang.Thread;

namespace DMS_3
{
	[Activity (Label = "HomeActivity",Theme = "@android:style/Theme.Black.NoTitleBar",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class HomeActivity : Activity
	{
		TextView lblTitle;
		TextView peekupBadgeText;
		TextView newMsgBadgeText;
		TextView deliveryBadgeText;
		TextView txtLivraison;
		TextView txtEnlevement;
		RelativeLayout deliveryBadge;
		RelativeLayout peekupBadge;
		RelativeLayout newMsgBadge;
		System.Timers.Timer indicatorTimer;
		System.Timers.Timer serviceTimer;
		public ProcessDMSBinder binder;
		ProcessDMSConnection processDMSConnection;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Home);


			//DECLARATION DES ITEMS
			lblTitle = FindViewById<TextView>(Resource.Id.lblTitle);
			peekupBadgeText = FindViewById<TextView>(Resource.Id.peekupBadgeText);
			newMsgBadgeText = FindViewById<TextView>(Resource.Id.newMsgBadgeText);
			txtLivraison = FindViewById<TextView>(Resource.Id.txtLivraison);
			txtEnlevement = FindViewById<TextView>(Resource.Id.txtEnlevement);
			deliveryBadgeText = FindViewById<TextView>(Resource.Id.deliveryBadgeText);
			deliveryBadge = FindViewById<RelativeLayout>(Resource.Id.deliveryBadge);
			peekupBadge = FindViewById<RelativeLayout>(Resource.Id.peekupBadge);
			newMsgBadge = FindViewById<RelativeLayout>(Resource.Id.newMsgBadge);
			peekupBadge.Visibility = ViewStates.Gone;
			deliveryBadge.Visibility = ViewStates.Gone;
			newMsgBadge.Visibility = ViewStates.Gone;


			//click button
			LinearLayout btn_Livraison = FindViewById<LinearLayout> (Resource.Id.columnlayout1_1);
			LinearLayout btn_Enlevement = FindViewById<LinearLayout> (Resource.Id.columnlayout1_2);
			LinearLayout btn_Message = FindViewById<LinearLayout> (Resource.Id.columnlayout2_1);
			LinearLayout btn_Config = FindViewById<LinearLayout> (Resource.Id.columnlayout4_2);


			btn_Livraison.Click += delegate { btn_Livraison_Click();};
			btn_Enlevement.Click += delegate { btn_Enlevement_Click ();};
			//btn_Livraison.LongClick += Btn_Livraison_LongClick;
			btn_Config.LongClick += Btn_Config_LongClick;
			btn_Message.Click += delegate { btn_Message_Click();};

			//FONTS
			txtLivraison.SetTypeface (Data.LatoBlack, Android.Graphics.TypefaceStyle.Normal);
			txtEnlevement.SetTypeface (Data.LatoBlack, Android.Graphics.TypefaceStyle.Normal);

			//Xamarin Insight
			Insights.Initialize("982a3c876c6a53932848ed500da432bb3dada603", this);
			Insights.Identify(Data.userAndsoft,"Name",Data.userAndsoft);

		
			if (processDMSConnection != null)
				binder = processDMSConnection.Binder;

			var ProcessServiceIntent = new Intent ("com.dealtis.dms_3.ProcessDMS");
			processDMSConnection = new ProcessDMSConnection (this);
			ApplicationContext.BindService (ProcessServiceIntent, processDMSConnection, Bind.AutoCreate);

			//LANCEMENT DU SERVICE
			if (Data.userAndsoft == null || Data.userAndsoft == "") {
				
			} else {
				if (!Data.Is_Service_Running) {
					//Data.CheckService = new Thread(OnServiceTimerHandler);
				}
			}
		}

		void Btn_Livraison_LongClick (object sender, View.LongClickEventArgs e)
		{			
			RunOnUiThread (() => {
				try {
					Data.Instance.InsertData ();
					AndHUD.Shared.ShowSuccess(this, "Mise à jour réussit!", MaskType.Clear, TimeSpan.FromSeconds(2));
				} catch (Exception ex) {
					Console.WriteLine ("\n"+ex);
					AndHUD.Shared.ShowError(this, "Error : "+ex, MaskType.Black, TimeSpan.FromSeconds(2));
				}
			});
		}

		void Btn_Config_LongClick (object sender, View.LongClickEventArgs e)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);

			builder.SetTitle("Deconnexion");

			builder.SetMessage("Voulez-vous vous déconnecter ?");
			builder.SetCancelable(false);
			builder.SetPositiveButton("Annuler", delegate {  });
			builder.SetNegativeButton("Déconnexion", delegate {
				DBRepository dbr = new DBRepository ();
				dbr.logout();
				Data.userAndsoft = null;
				Data.userTransics = null;
				File.AppendAllText(Data.log_file, "["+DateTime.Now.ToString("t")+"]"+"[LOGOUT]Coupure du service le "+DateTime.Now.ToString("G")+"\n");
				StopService (new Intent (this, typeof(ProcessDMS)));
				Data.Is_Service_Running = false;
				Intent intent = new Intent (this, typeof(MainActivity));
				this.StartActivity (intent);
				//this.OverridePendingTransition (Resource.Animation.abc_slide_in_top,Resource.Animation.abc_slide_out_bottom);
			});
			builder.Show();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		void OnServiceTimerHandler ()
		{
			while (true) {				
				Thread.Sleep (120000);
			}
				
		}
		protected override void OnResume()
		{
			base.OnResume();
			DBRepository dbr = new DBRepository ();
			if (dbr.is_user_Log_In() == "false") {
				Intent intent = new Intent (this, typeof(MainActivity));
				this.StartActivity (intent);
				//this.OverridePendingTransition (Resource.Animation.abc_slide_in_top,Resource.Animation.abc_slide_out_bottom);
			}
			var t = DateTime.Now.ToString ("dd_MM_yy");
			string dir_log = (Android.OS.Environment.GetExternalStoragePublicDirectory (Android.OS.Environment.DirectoryDownloads)).ToString ();
			//Shared Preference
			ISharedPreferences pref = Application.Context.GetSharedPreferences ("AppInfo", FileCreationMode.Private);
			string log = pref.GetString ("Log", String.Empty);
			//GetTelId
			TelephonyManager tel = (TelephonyManager)this.GetSystemService (Context.TelephonyService);
			var telId = tel.DeviceId;
			//Si il n'y a pas de shared pref
			if (log == String.Empty) {
				Data.log_file = Path.Combine (dir_log, t + "_" + telId + "_log.txt");
				ISharedPreferencesEditor edit = pref.Edit ();
				edit.PutString ("Log", Data.log_file);
				edit.Apply ();
			} else {
				//il y a des shared pref
				Data.log_file = pref.GetString("Log", String.Empty);
				if (!(Data.log_file.Substring(26,Math.Min(Data.log_file.Length,2)).Equals(DateTime.Now.Day.ToString("00")))) {
					File.Delete(Data.log_file);
					Data.log_file = Path.Combine (dir_log, t+"_"+telId+"_log.txt");
					ISharedPreferencesEditor edit = pref.Edit();
					edit.PutString("Log",Data.log_file);
					edit.Apply();
					Data.log_file = pref.GetString("Log", String.Empty);
				}

			}

			var user = dbr.getUserAndsoft ();
			dbr.setUserdata (user);
			dbr.SETBadges(Data.userAndsoft);

			var version = this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;
			lblTitle.Text = Data.userAndsoft + " " + version;
			indicatorTimer = new System.Timers.Timer();

			indicatorTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnIndicatorTimerHandler);
			indicatorTimer.Interval = 1000;
			indicatorTimer.Enabled = true;
			indicatorTimer.Start();

//			if (!Data.CheckService.IsAlive) {
//				Data.CheckService.Start();
//			}
		}

		void OnIndicatorTimerHandler (object sender, System.Timers.ElapsedEventArgs e)
		{
			//cacher les badges si inférieur à 1 else afficher et mettre le nombre
			if (Data.Instance.getLivraisonIndicator () < 1) {
				RunOnUiThread (() =>deliveryBadge.Visibility = ViewStates.Gone);
			} else {
				RunOnUiThread (() =>deliveryBadgeText.Text = Data.Instance.getLivraisonIndicator ().ToString());
				RunOnUiThread (() =>deliveryBadge.Visibility = ViewStates.Visible);
			}

			if (Data.Instance.getEnlevementIndicator () < 1) {
				RunOnUiThread (() =>peekupBadge.Visibility = ViewStates.Gone);
			} else {
				RunOnUiThread (() =>peekupBadgeText.Text = Data.Instance.getEnlevementIndicator ().ToString());
				RunOnUiThread (() =>peekupBadge.Visibility = ViewStates.Visible);
			}

			if (Data.Instance.getMessageIndicator () < 1) {
				RunOnUiThread (() => newMsgBadge.Visibility = ViewStates.Gone);
			} else {
				RunOnUiThread (() => newMsgBadgeText.Text = Data.Instance.getMessageIndicator ().ToString ());
				RunOnUiThread (() => newMsgBadge.Visibility = ViewStates.Visible);
			}
		}

		protected override void OnStop()
		{	
			indicatorTimer.Stop ();
			//File.AppendAllText(Data.log_file, "["+DateTime.Now.ToString("t")+"]"+"OnStop le "+DateTime.Now.ToString("G")+"\n");
			base.OnStop();
		}

		protected override void OnRestart()
		{
			File.AppendAllText(Data.log_file, "["+DateTime.Now.ToString("t")+"]"+"OnRestart le "+DateTime.Now.ToString("G")+"\n");
			base.OnRestart();
		}

		void btn_Livraison_Click ()
		{
			Intent intent = new Intent (this, typeof(ListeLivraisonsActivity));
			intent.PutExtra("TYPE","LIV");
			this.StartActivity (intent);
			Finish();
			//this.OverridePendingTransition (Resource.Animation.abc_slide_in_top,Resource.Animation.abc_slide_out_bottom);
		}

		void btn_Enlevement_Click ()
		{
			Intent intent = new Intent (this, typeof(ListeLivraisonsActivity));
			intent.PutExtra("TYPE","RAM");
			this.StartActivity (intent);
			Finish();
			//this.OverridePendingTransition (Resource.Animation.abc_slide_in_top,Resource.Animation.abc_slide_out_bottom);
		}

		void btn_Message_Click ()
		{
			Intent intent = new Intent (this, typeof(MessageActivity));
			this.StartActivity (intent);
			Finish();
			//this.OverridePendingTransition (Resource.Animation.abc_slide_in_top,Resource.Animation.abc_slide_out_bottom);
		}

		public override void OnBackPressed ()
		{			
		}

		class ProcessDMSConnection : Java.Lang.Object, IServiceConnection
		{
			HomeActivity activity;
			ProcessDMSBinder binder;

			public ProcessDMSBinder Binder {
				get {
					return binder;
				}
			}
			public ProcessDMSConnection (HomeActivity activity)
			{
				this.activity = activity;
			}
			public void OnServiceConnected (ComponentName name, IBinder service)
			{
				var demoServiceBinder = service as ProcessDMSBinder;
				if (demoServiceBinder != null) {
					var binder = (ProcessDMSBinder)service;
					activity.binder = binder;
					if (Data.userAndsoft == null || Data.userAndsoft == "") {

					} else {
						Data.Is_Service_Running = true;
					}
					this.binder = (ProcessDMSBinder)service;
				} else {
					File.AppendAllText(Data.log_file, "["+DateTime.Now.ToString("t")+"]"+"[SERVICE] binder none : service non lancé "+DateTime.Now.ToString("G")+"\n");
				}
			}
			public void OnServiceDisconnected (ComponentName name)
			{
				Data.Is_Service_Running = false;
			}
		}
	}
}

