﻿using System;
using System.Json;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using AndroidHUD;
using DMS_3.BDD;
using Xamarin;


namespace DMS_3
{
	[Activity (Label = "DMS_3", Icon = "@mipmap/icon",ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation, NoHistory = true)]
	public class MainActivity : Activity
	{
		Button btn_Login;
		EditText user;
		EditText password;
		TextView tableload;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Main);

			//DECLARATION DES ITEMS
			btn_Login = FindViewById<Button> (Resource.Id.btnlogin);
			user = FindViewById<EditText> (Resource.Id.user);
			password = FindViewById<EditText> (Resource.Id.password);
			tableload = FindViewById<TextView> (Resource.Id.tableload);

			if (!Data.tableuserload) {
				tableload.Text = "Table user non chargée";
				tableload.SetCompoundDrawablesWithIntrinsicBounds (Resource.Drawable.Anom,0,0,0);
			}
			//APPEL DES FONCTIONS
			btn_Login.LongClick +=  delegate {
				btn_Login_LongClick();
			};
			btn_Login.Click += delegate {
				btn_Login_Click();
			};
			
		}

		protected override void OnStart()
		{
			base.OnStart();
		}


		protected override void OnResume()
		{
			base.OnResume();
		}
		protected override void OnPause()
		{
			base.OnPause();
		}
		protected override void OnStop()
		{
			base.OnStop();
		}
		protected override void OnDestroy ()
		{
			base.OnDestroy ();
		}

		void btn_Login_Click ()
		{
			if (!(user.Text == "")) {
				//INSTANCE DBREPOSITORY
				DBRepository dbr = new DBRepository ();
				var usercheck = dbr.user_Check (user.Text.ToUpper(), password.Text);
				if (usercheck) {
					//UPDATE DE LA BDD AVEC CE USER
					AndHUD.Shared.ShowSuccess(this, "Bienvenue", MaskType.Black, TimeSpan.FromSeconds(2));
					dbr.setUserdata (user.Text.ToUpper ());
					StartActivity(typeof(HomeActivity));
				} else {
					AndHUD.Shared.ShowError(this, "Mauvais mot de passe", MaskType.Black, TimeSpan.FromSeconds(2));
				}
			} else {
				AndHUD.Shared.ShowError(this, "Champ user obligatoire", MaskType.Black, TimeSpan.FromSeconds(2));
			}
		}

		void btn_Login_LongClick ()
		{
			try {
				DBRepository dbr = new DBRepository ();
				string _url = "http://dms.jeantettransport.com/api/authen?chaufmdp=";
				var webClient = new WebClient ();
				webClient.Headers [HttpRequestHeader.ContentType] = "application/json";
				string userData = "";
				userData = webClient.DownloadString (_url);
				System.Console.WriteLine ("\n Webclient User Terminé ...");
				//GESTION DU XML
				JsonArray jsonVal = JsonArray.Parse (userData) as JsonArray;
				var jsonArr = jsonVal;
				foreach (var row in jsonArr) {
					var checkUser = dbr.user_AlreadyExist (row ["userandsoft"], row ["usertransics"], row ["mdpandsoft"], "true");
					Console.WriteLine ("\n" + checkUser + " " + row ["userandsoft"]);
					if (!checkUser) {
						var IntegUser = dbr.InsertDataUser (row ["userandsoft"], row ["usertransics"], row ["mdpandsoft"], "true");
						Console.WriteLine ("\n" + IntegUser);
					}
				}
				AndHUD.Shared.ShowSuccess(this, "Table mise à jour", MaskType.Black, TimeSpan.FromSeconds(2));
				Data.tableuserload = true;	
				tableload.Text = "Table chargée";
				tableload.SetCompoundDrawablesWithIntrinsicBounds (Resource.Drawable.Val,0,0,0);
			} catch (System.Exception ex) {
				System.Console.WriteLine (ex);
				Insights.Report (ex);
				AndHUD.Shared.ShowError (this, "Une erreur c'est produite lors du lancement, réessaie dans 5 secondes", MaskType.Black, TimeSpan.FromSeconds (5));
			}
		}
	}
}
