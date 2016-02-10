using System;
using System.Collections.Generic;
using System.Data;
using System.IO;


using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using DMS_3.BDD;
using Android.Text;
using Android.Views;
using Android.Widget;
using SQLite;
using AndroidHUD;


namespace DMS_3
{
	[Activity (Label = "MessageActivity",Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]			
	public class MessageActivity : Activity
	{
		private List<TableMessages> mItems;
		private ListView mListView;
		public ListeViewMessageAdapter adapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.ChatLayout);

			//LISTVIEW
			mListView = FindViewById<ListView> (Resource.Id.listViewBox);

			mItems = new List<TableMessages> ();

			string dbPath = System.IO.Path.Combine (System.Environment.GetFolderPath
				(System.Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection (dbPath);
			DBRepository dbr = new DBRepository ();

			var table = db.Query<TableMessages> ("SELECT * FROM TableMessages where codeChauffeur=?",Data.userAndsoft);
			var i = 0;

			foreach (var item in table) {
				mItems.Add (new TableMessages () {
					texteMessage = item.texteMessage,
					utilisateurEmetteur = item.utilisateurEmetteur,
					statutMessage = item.statutMessage,
					dateImportMessage = item.dateImportMessage,
					typeMessage = item.typeMessage,
					Id = item.Id
				});
				i++;
			}
// 			TODO
//			if(i > 6){
//				View view = LayoutInflater.From (this).Inflate (Resource.Layout.ListeViewDelete, null, false);
//				mListView.AddHeaderView (view);
//				view.Click += Btndeletemsg_Click;
//			}

			adapter = new ListeViewMessageAdapter (this, mItems);
			mListView.Adapter = adapter;

			//EDITTEXT
			var btnsend = FindViewById<Button>(Resource.Id.btnsend);
			btnsend.Click += Btnsend_Click;

			//STATUT DES MESSAGES RECU TO 1

			var tablemsgrecu = db.Query<TableMessages> ("SELECT * FROM TableMessages where statutMessage = 0");
			foreach (var item in tablemsgrecu) {
				var updatestatutmessage = db.Query<TableMessages> ("UPDATE TableMessages SET statutMessage = 1 WHERE statutMessage = 0");
				var resintegstatut = dbr.InsertDataStatutMessage (1,DateTime.Now,item.numMessage,"","");
			}

		}

		void  Btnsend_Click (object sender, EventArgs e){

			DBRepository dbr = new DBRepository ();
			var newmessage = FindViewById<TextView>(Resource.Id.editnewmsg);
			if (newmessage.Text == "") {

			} else {
				var resinteg = dbr.InsertDataMessage (Data.userAndsoft,"", newmessage.Text,2, DateTime.Now, 2,0);

			}



			string dbPath = System.IO.Path.Combine (System.Environment.GetFolderPath
				(System.Environment.SpecialFolder.Personal), "ormDMS.db3");
			var db = new SQLiteConnection (dbPath);


			Intent intent = new Intent (this, typeof(MessageActivity));
			this.StartActivity (intent);
			this.OverridePendingTransition (Resource.Animation.abc_slide_in_bottom,Resource.Animation.abc_slide_out_top);


		}


		void  Btndeletemsg_Click (object sender, EventArgs e){
	
		}

		public override void OnBackPressed ()
		{
			Intent intent = new Intent (this, typeof(HomeActivity));
			this.StartActivity (intent);
			this.OverridePendingTransition (Resource.Animation.abc_slide_in_bottom,Resource.Animation.abc_slide_out_top);
		}   
	}
}


