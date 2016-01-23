
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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
			Threadapp ();
		}

		protected override void OnStart()
		{	

			base.OnStart();

		}


		protected override void OnResume()
		{
			base.OnResume();
			//Afficher ou non les badges
		}

		void Threadapp ()
		{
			if (!Is_thread_Running) {
				//execution des fonctions dans une boucle
				while (true) {
					Task.Factory.StartNew (
						
						() => {
							Console.WriteLine ( "\nHello from InsertData." );
							InsertData();
						}					
					).ContinueWith (
						t => {
							Console.WriteLine ( "\nHello from ComWebService." );
							ComWebService();
						}, TaskScheduler.FromCurrentSynchronizationContext()
						
					).ContinueWith ( 
						v => {
							Console.WriteLine ( "\nHello from ComPosGps." );
							ComPosGps();
						}, TaskScheduler.FromCurrentSynchronizationContext()
					);
				}

			}
		}

		void  InsertData ()
		{
			
		}

		void  ComWebService ()
		{

		}

		void  ComPosGps ()
		{

		}
	}
}

