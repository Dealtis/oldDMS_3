using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Net;
using System.Net;
using System.Text;
using Android.Support.V7.App;
using Android.Util;
using DMS_3.BDD;

using AndroidHUD;

using System.Json;

namespace DMS_3
{
	[Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true,ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation)]
	public class SplashActivity : AppCompatActivity
	{
		static readonly string TAG = "X:" + typeof (SplashActivity).Name;

		public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
		{
			base.OnCreate(savedInstanceState, persistentState);
		}

		protected override void OnResume()
		{
			base.OnResume();

			Task startupWork = new Task(() =>
				{
					//INSTANCE DBREPOSITORY
					DBRepository dbr = new DBRepository ();
					//CREATION DE LA BDD
					dbr.CreateDB();

					//CREATION DES TABLES
					dbr.CreateTable();

					//TEST DE CONNEXION
					var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);


					bool App_Connec = false;
					while (!App_Connec) {
						var activeConnection = connectivityManager.ActiveNetworkInfo;
						if ((activeConnection != null) && activeConnection.IsConnected) {
							try {
								string _url = "http://dms.jeantettransport.com/api/authen?chaufmdp=";
								var webClient = new WebClient();
								webClient.Headers [HttpRequestHeader.ContentType] = "application/json";

								string userData="";
								userData = webClient.DownloadString(_url);
								System.Console.WriteLine ("\n Webclient User Terminé ...");

								//GESTION DU XML
								JsonArray jsonVal = JsonArray.Parse (userData) as JsonArray;
								var jsonArr = jsonVal;
								foreach (var row in jsonArr) {
									var checkUser = dbr.user_AlreadyExist(row["userandsoft"],row["usertransics"],row["mdpandsoft"],"true");
									Console.WriteLine ("\n"+checkUser+" "+row["userandsoft"]);
									if (!checkUser) {
										var IntegUser = dbr.InsertDataUser(row["userandsoft"],row["usertransics"],row["mdpandsoft"],"true");
										Console.WriteLine ("\n"+IntegUser);
									}
								}
								App_Connec = true;
							} catch (System.Exception ex) {
								System.Console.WriteLine (ex);
								App_Connec = false;
								AndHUD.Shared.ShowError(this, "Une erreur c'est produite lors du lancement, réessaie dans 5 secondes", MaskType.Black, TimeSpan.FromSeconds(5));
							}
						}else{
							App_Connec = false;
							AndHUD.Shared.ShowError(this, "Pas de connexion, réessaie dans 5 secondes", MaskType.Black, TimeSpan.FromSeconds(5));
							Thread.Sleep(5000);
						}
					}
				});

			startupWork.ContinueWith(t =>
				{
					//Is a user login ?
					DBRepository dbr = new DBRepository ();
					var user_Login = dbr.is_user_Log_In();
					if (!(user_Login=string.Empty)) {
						Data.userAndsoft = user_Login;
						StartActivity(new Intent(Application.Context, typeof (HomeActivity)));
					}else{
						StartActivity(new Intent(Application.Context, typeof (MainActivity)));
					}				
				}, TaskScheduler.FromCurrentSynchronizationContext());

			startupWork.Start();
		}
	}
}

