﻿
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using System.Threading;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Android.Content.PM;
using Android.Graphics;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using DMS_3.BDD;
namespace DMS_3
{
	[Activity (Label = "AnomalieActivity",Theme = "@android:style/Theme.Black.NoTitleBar",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class AnomalieActivity : Activity
	{	

		string id;
		int i;
		string txtspinner;
		string codeanomalie;
		EditText EdittxtRem;
		string txtRem;
		ImageView _imageView;
		TablePositions data;

		string type;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.Anomalie);

			id = Intent.GetStringExtra ("ID");
			i = int.Parse(id);

			type = Intent.GetStringExtra ("TYPE");

			DBRepository dbr = new DBRepository ();
			data = dbr.GetPositionsData (i);

			Spinner spinner = FindViewById<Spinner> (Resource.Id.spinnerAnomalie);
			EdittxtRem = FindViewById<EditText>(Resource.Id.edittext);
			_imageView = FindViewById<ImageView>(Resource.Id.imageView1);

			Button buttonvalider = FindViewById<Button>(Resource.Id.valider);

			if (IsThereAnAppToTakePictures ())
			{
				CreateDirectoryForPictures ();
				Button buttonphoto = FindViewById<Button>(Resource.Id.openCamera);
				_imageView = FindViewById<ImageView>(Resource.Id.imageView1);
				buttonphoto.Click += TakeAPicture;
			}

			buttonvalider.Click += delegate {
				Buttonvalider_Click();
			};
			;

			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);
			ArrayAdapter adapter;
			if (type == "RAM") {
				adapter = ArrayAdapter.CreateFromResource (this, Resource.Array.anomalieramasselist, Android.Resource.Layout.SimpleSpinnerItem);
			} else {
				adapter = ArrayAdapter.CreateFromResource (this, Resource.Array.anomalielivraisonlist, Android.Resource.Layout.SimpleSpinnerItem);
			}

			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;

			//REMISE  à null de la valeur photo
		}

		void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
			txtspinner = string.Format ("{0}", spinner.GetItemAtPosition (e.Position));

			if (txtspinner == "Restaure en non traite" || txtspinner == "Choisir une anomalie" ) {
				EdittxtRem.Visibility = Android.Views.ViewStates.Gone;
			} else {
				EdittxtRem.Visibility = Android.Views.ViewStates.Visible;
			}
		}

		void Buttonvalider_Click ()
		{
			if (txtspinner == "Choisir une anomalie") {
				
			} else {
				txtRem = EdittxtRem.Text;
				switch (txtspinner) {
				case "Livre avec manquant":
					codeanomalie = "LIVRMQ";
					break;
				case "Livre avec reserves pour avaries":
					codeanomalie = "LIVRCA";
					break;
				case "Livre mais recepisse non rendu":
					codeanomalie = "LIVDOC";
					break;
				case "Livre avec manquants + avaries":
					codeanomalie = "LIVRMA";
					break;
				case "Refuse pour avaries":
					codeanomalie = "RENAVA";
					break;
				case "Avise (avis de passage)":
					codeanomalie = "RENAVI";
					break;
				case "Rendu non livre : complement adresse":
					codeanomalie = "RENCAD";
					break;
				case "Refus divers ou sans motifs":
					codeanomalie = "RENDIV";
					break;
				case "Refuse manque BL":
					codeanomalie = "RENDOC";
					break;
				case "Refuse manquant partiel":
					codeanomalie = "RENMQP";
					break;
				case "Refuse non commande":
					codeanomalie = "RENDIV";
					break;
				case "Refuse cause port du":
					codeanomalie = "RENSPD";
					break;
				case "Refuse cause contre remboursement":
					codeanomalie = "RENDRB";
					break;
				case "Refuse livraison trop tardive":
					codeanomalie = "RENTAR";
					break;
				case "Rendu non justifie":
					codeanomalie = "RENNJU";
					break;
				case "Fermeture hebdomadaire":
					codeanomalie = "RENFHB";
					break;
				case "Non charge":
					codeanomalie = "RENNCG";
					break;
				case "Inventaire":
					codeanomalie = "RENINV";
					break;
				case "Ramasse pas faite":
					codeanomalie = "RAMPFT";
					break;
				case "Positions non chargees":
					codeanomalie = "RENNCG";
					break;
				case "Avis de passage":
					codeanomalie = "RENAVI";
					break;
				case "Ramasse diverse":
					codeanomalie = "RAMDIV";
					break;
				case "Restaure en non traite":
					codeanomalie = "RESTNT";
					break;

				default:
					break;
				}

				DBRepository dbr = new DBRepository ();
				//mise du statut de la position à 1
				if (txtspinner == "Restaure en non traite") {
					dbr.updatePosition (i, "0", txtspinner, txtRem, codeanomalie, null);

				} else {
					dbr.updatePosition (i, "2", txtspinner, txtRem, codeanomalie, null);
				}


				//creation du JSON
				string JSON = "{\"codesuiviliv\":\"" + codeanomalie + "\",\"memosuiviliv\":\"" + txtRem + "\",\"libellesuiviliv\":\"" + txtspinner + "\",\"commandesuiviliv\":\"" + data.numCommande + "\",\"groupagesuiviliv\":\"" + data.groupage + "\",\"datesuiviliv\":\"" + DateTime.Now.ToString ("dd/MM/yyyy HH:mm") + "\",\"posgps\":\"" + Data.GPS + "\"}";
				//création de la notification webservice // statut de position
				dbr.insertDataStatutpositions (codeanomalie, "2", txtspinner, data.numCommande, txtRem, DateTime.Now.ToString ("dd/MM/yyyy HH:mm"), JSON);


				string compImg = String.Empty;
		

				Task.Factory.StartNew (
					() => {
						try {
							var imgpath = dbr.GetPositionsData (i);
							Android.Graphics.Bitmap bmp = Android.Graphics.BitmapFactory.DecodeFile (imgpath.imgpath);
							Bitmap rbmp = Bitmap.CreateScaledBitmap (bmp, bmp.Width / 5, bmp.Height / 5, true);
							compImg = imgpath.imgpath.Replace (".jpg", "-1_1.jpg");
							using (var fs = new FileStream (compImg, FileMode.OpenOrCreate)) {
								rbmp.Compress (Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, fs);
							}
							//ftp://77.158.93.75 ftp://10.1.2.75
							Data.Instance.UploadFile ("ftp://77.158.93.75", compImg, "DMS", "Linuxr00tn", "");

						} catch (Exception ex) {
							Console.WriteLine ("\n" + ex);
						}
					}					
				);

				Intent intent = new Intent (this, typeof(ListeLivraisonsActivity));
				intent.PutExtra ("TYPE", type);
				this.StartActivity (intent);
				this.OverridePendingTransition (Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
			}
		}

		private void CreateDirectoryForPictures ()
		{
			Data._dir = new Java.IO.File (Environment.GetExternalStoragePublicDirectory (
					Environment.DirectoryPictures), "DMSIMG");
			if (!Data._dir.Exists ())
			{
				Data._dir.Mkdirs( );
			}
		}

		private bool IsThereAnAppToTakePictures ()
		{
			Intent intent = new Intent (MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities =
				PackageManager.QueryIntentActivities (intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}

		private void TakeAPicture (object sender, EventArgs eventArgs)
		{
			Intent intent = new Intent (MediaStore.ActionImageCapture);

			Data._file = new Java.IO.File (Data._dir, String.Format(""+DateTime.Now.ToString("dd_MM")+"_"+data.numCommande+".jpg", Guid.NewGuid()));
			intent.PutExtra (MediaStore.ExtraOutput, Uri.FromFile (Data._file));
			StartActivityForResult (intent, 0);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			DBRepository dbr = new DBRepository ();
			// Make it available in the gallery

			Intent mediaScanIntent = new Intent (Intent.ActionMediaScannerScanFile);
			Uri contentUri = Uri.FromFile (Data._file);
			mediaScanIntent.SetData (contentUri);
			SendBroadcast (mediaScanIntent);

			// Display in ImageView. We will resize the bitmap to fit the display.
			// Loading the full sized image will consume to much memory
			// and cause the application to crash.

			int height = Resources.DisplayMetrics.HeightPixels;
			int width = _imageView.Height ;
			Data.bitmap = Data._file.Path.LoadAndResizeBitmap (width, height);
			if (Data.bitmap != null) {
				_imageView.SetImageBitmap (Data.bitmap);
				dbr.updateposimgpath (i,Data._file.Path);
				Data.bitmap = null;
			}

			// Dispose of the Java side bitmap.
			//GC.Collect();
		}

		public override void OnBackPressed ()
		{	

			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle("Validation");
			builder.SetMessage("Voulez-vous annulée l'anomalie ?");
			builder.SetCancelable(false);
			builder.SetPositiveButton("Oui", delegate {
				if (data.StatutLivraison == "1" || data.StatutLivraison == "2") {
					Intent intent = new Intent (this, typeof(ListeTraitee));
					intent.PutExtra("TYPE",type);
					this.StartActivity (intent);
					this.OverridePendingTransition (Android.Resource.Animation.SlideInLeft,Android.Resource.Animation.SlideOutRight);
				} else {
					Intent intent = new Intent (this, typeof(ListeLivraisonsActivity));
					intent.PutExtra("TYPE",type);
					this.StartActivity (intent);
					this.OverridePendingTransition (Android.Resource.Animation.SlideInLeft,Android.Resource.Animation.SlideOutRight);
				}
			});
			builder.SetNegativeButton("Non", delegate {  });

			builder.Show();

		}
	}

	public static class BitmapHelpers
	{
		public static Bitmap LoadAndResizeBitmap (this string fileName, int width, int height)
		{
			// First we get the the dimensions of the file on disk
			BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
			BitmapFactory.DecodeFile (fileName, options);

			// Next we calculate the ratio that we need to resize the image by
			// in order to fit the requested dimensions.
			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 1;

			if (outHeight > height || outWidth > width)
			{
				inSampleSize = outWidth > outHeight
					? outHeight / height
					: outWidth / width;
			}

			// Now we will load the image and have BitmapFactory resize it for us.
			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;
			Bitmap resizedBitmap = BitmapFactory.DecodeFile (fileName, options);

			return resizedBitmap;
		}
	}
}

