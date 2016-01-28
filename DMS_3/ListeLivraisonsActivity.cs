
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DMS_3.BDD;
namespace DMS_3
{
	
	[Activity (Label = "ListeLivraisonsActivity")]			
	public class ListeLivraisonsActivity : Activity
	{	

		string[] grp = new string[10];
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.ListeLivraisons);

			//Mise dans un Array des Groupage
			DBRepository dbr = new DBRepository ();
			//req
			//var grp = db.Query<ToDoTask> ("SELECT * FROM ToDoTask WHERE StatutLivraison = '0' AND typeMission='L' AND typeSegment='LIV'  AND Userandsoft = ?  GROUP BY groupage",ApplicationData.UserAndsoft);
			var i = 0;
			foreach (var item in grp){
				
				grp[i] = item.groupage;
				grp++;
				i++;
			}

//			switch (true) {
//			case:
//				break;
//			default:
//				break;
//			}



		}
	}
}

