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
using Android.Telephony;
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
		System.Timers.Timer indicatorTimer;
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
			btn_Livraison.LongClick += Btn_Livraison_LongClick;
			btn_Config.LongClick += Btn_Config_LongClick;
			btn_Message.Click += delegate { btn_Message_Click();};


			//Xamarin Insight
			Insights.Initialize("d3afeb59463d5bdc09194186b94fc991016faf1f", this);
			Insights.Identify(Data.userAndsoft,"Name",Data.userAndsoft);

			//LANCEMENT DU SERVICE
			StartService (
				new Intent (this, typeof(ProcessDMS)).PutExtra("userAndsoft",Data.userAndsoft).PutExtra("userTransics",Data.userTransics)		
			);
		
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
				File.AppendAllText(Data.log_file, "[LOGOUT]Coupure du service le "+DateTime.Now.ToString("G")+"\n");
				StopService (
					new Intent (this, typeof(ProcessDMS)).PutExtra("userAndsoft",Data.userAndsoft).PutExtra("userTransics",Data.userTransics)		
				);
				Intent intent = new Intent (this, typeof(MainActivity));
				this.StartActivity (intent);
				this.OverridePendingTransition (Resource.Animation.abc_slide_in_top,Resource.Animation.abc_slide_out_bottom);

			});
			builder.Show();
		}

		protected override void OnStart()
		{	

			base.OnStart();

		}


		protected override void OnResume()
		{
			base.OnResume();


			var version = this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;
			lblTitle.Text = Data.userAndsoft + " " + version;
			indicatorTimer = new System.Timers.Timer();
			indicatorTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnIndicatorTimerHandler);
			indicatorTimer.Interval = 1000;
			indicatorTimer.Enabled = true;
			indicatorTimer.Start();
		}

		void OnIndicatorTimerHandler (object sender, System.Timers.ElapsedEventArgs e)
		{
			//SET des badges
			DBRepository dbr = new DBRepository ();
			dbr.SETBadges(Data.userAndsoft);

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

		public override void OnBackPressed ()
		{
			
		}
	}
}

