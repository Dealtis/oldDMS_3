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
using AndroidHUD;
using DMS_3.BDD;
namespace DMS_3
{
	[Activity (Label = "ListViewAdapterMenu")]			
	public class ListViewAdapterMenu : BaseAdapter<TablePositions> {
		private List<TablePositions> mItems;
		private Context mContext;
		string txtspinner;
		string codeanomalie;
		EditText EdittxtRem;
		string txtRem;
		ImageView _imageView;
		private AlertDialog.Builder dialog;

		public ListViewAdapterMenu(Context context,List<TablePositions> items) : base() {
			mItems = items;
			mContext = context;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override TablePositions this[int position] {  
			get { return mItems[position]; }
		}
		public override int Count {
			get { return mItems.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = convertView;
			LayoutInflater inflater = (LayoutInflater) mContext.GetSystemService(Context.LayoutInflaterService);
			int xml_type = 0;

			switch (mItems [position].StatutLivraison) {
			default:
				break;
			case "0":
				if (mItems [position].typeMission == "L") {
					xml_type = Resource.Layout.ListeViewRow;
				} else {
					xml_type = Resource.Layout.ListeViewRowEnlevement;
				}
				break;
			case "1":
				xml_type = Resource.Layout.ListeViewRowValide;
				break;
			case "2":
				xml_type = Resource.Layout.ListeViewRowAnomalie;
				break;
			}
			if(mItems[position].imgpath == "SUPPLIV"){
				//row = LayoutInflater.From (mContext).Inflate (Resource.Layout.ListeViewRowStroke,null,false);
				xml_type = Resource.Layout.ListeViewRowStroke;
			}
			row = inflater.Inflate(xml_type,parent,false);


			TextView textLeft = row.FindViewById<TextView> (Resource.Id.textleft);
			TextView textMid = row.FindViewById<TextView> (Resource.Id.textmid);
			TextView textRight = row.FindViewById<TextView> (Resource.Id.txtright);
			//Button btnvalid = row.FindViewById<Button> (Resource.Id.btn_valider);
			//Button btnanomalie = row.FindViewById<Button> (Resource.Id.btn_anomalie);

			textLeft.SetTypeface (Data.LatoBlack, Android.Graphics.TypefaceStyle.Normal);
			textMid.SetTypeface (Data.LatoBold, Android.Graphics.TypefaceStyle.Normal);
			textRight.SetTypeface (Data.LatoBold, Android.Graphics.TypefaceStyle.Normal);

			textLeft.Text = "OT: "+mItems[position].numCommande+" "+mItems[position].planDeTransport;
			textMid.Text = mItems[position].CpLivraison+"."+mItems[position].villeLivraison+"\tCol: "+mItems[position].nbrColis+" Pal:"+mItems[position].nbrPallette;
			textRight.Text = mItems [position].instrucLivraison;
			return row;
		}
		void Btnvalid_Click (int position)
		{
			dialog = new AlertDialog.Builder(mContext);
			AlertDialog alert = dialog.Create();
			dialog.SetMessage ("Voulez-vous valider cette position ?");
			//afficher le cr si CR
			//afficher le champ mémo
			//TODO
			string mémo = "Mémo à venir";
			//afficher la checkbox si partic

			dialog.SetCancelable (true);
			dialog.SetPositiveButton("Oui", delegate {
				DBRepository dbr = new DBRepository ();
				//mise du statut de la position à 1
				dbr.updatePosition(mItems[position].Id,"1","Validée",mémo,"LIVCFM",null);
				//creation du JSON
				string JSON ="{\"codesuiviliv\":\"RAMCFM\",\"memosuiviliv\":\""+mémo+"\",\"libellesuiviliv\":\"\",\"commandesuiviliv\":\""+mItems[position].numCommande+"\",\"groupagesuiviliv\":\""+mItems[position].groupage+"\",\"datesuiviliv\":\""+DateTime.Now.ToString("dd/MM/yyyy HH:mm")+"\",\"posgps\":\""+Data.GPS+"\"}";
				//création de la notification webservice // statut de position
				dbr.insertDataStatutpositions("LIVCFM","1","Commande Validée",mItems[position].numCommande,"Validée",DateTime.Now.ToString("dd/MM/yyyy HH:mm"),JSON);
				//dismiss la position
				mItems.RemoveAt(position);
				NotifyDataSetChanged();
				alert.Cancel();
			});
			dialog.SetNegativeButton("Non", delegate {
				AndHUD.Shared.ShowError(mContext, "Annulée!", AndroidHUD.MaskType.Clear, TimeSpan.FromSeconds(1));
			});
			dialog.Show ();
		}

		void BtnAnomalie_Click (int position)
		{
			dialog = new AlertDialog.Builder(mContext);

			dialog.SetTitle("Anomalie");
			var viewAD =  LayoutInflater.From (mContext).Inflate (Resource.Layout.Anomalie, null, false);
			dialog.SetView (viewAD);

			Spinner spinner = viewAD.FindViewById<Spinner> (Resource.Id.spinnerAnomalie);
			EdittxtRem = viewAD.FindViewById<EditText>(Resource.Id.edittext);
			_imageView = viewAD.FindViewById<ImageView>(Resource.Id.imageView1);
			Button button = viewAD.FindViewById<Button>(Resource.Id.openCamera);


			_imageView.Visibility = ViewStates.Gone;
			button.Visibility = ViewStates.Gone;

			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);
			var adapter = ArrayAdapter.CreateFromResource (
				mContext, Resource.Array.anomalielivraisonlist, Android.Resource.Layout.SimpleSpinnerItem);

			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;

			dialog.SetPositiveButton("Valider", delegate {
				txtRem = EdittxtRem.Text;
				switch(txtspinner)
				{
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
				case "Restaure en non traite":
					codeanomalie = "RESTNT";
					break;
				default:
					break;
				}

				DBRepository dbr = new DBRepository ();
				//mise du statut de la position à 1
				dbr.updatePosition(mItems[position].Id,"2",txtspinner,txtRem,codeanomalie,null);
				//creation du JSON
				string JSON ="{\"codesuiviliv\":\""+codeanomalie+"\",\"memosuiviliv\":\""+txtRem+"\",\"libellesuiviliv\":\""+txtspinner+"\",\"commandesuiviliv\":\""+mItems[position].numCommande+"\",\"groupagesuiviliv\":\""+mItems[position].groupage+"\",\"datesuiviliv\":\""+DateTime.Now.ToString("dd/MM/yyyy HH:mm")+"\",\"posgps\":\""+Data.GPS+"\"}";
				//création de la notification webservice // statut de position
				dbr.insertDataStatutpositions(codeanomalie,"2",txtspinner,mItems[position].numCommande,txtRem,DateTime.Now.ToString("dd/MM/yyyy HH:mm"),JSON);
				//dismiss la position
				mItems.RemoveAt(position);
				NotifyDataSetChanged();

			});
			dialog.SetNegativeButton("Non", delegate {
				AndHUD.Shared.ShowError(mContext, "Annulée!", AndroidHUD.MaskType.Clear, TimeSpan.FromSeconds(1));
			});
			dialog.Show ();
		}

		private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
			txtspinner = string.Format ("{0}", spinner.GetItemAtPosition (e.Position));

			if (txtspinner == "Restaure en non traite" || txtspinner == "Choisir une anomalie" ) {
				EdittxtRem.Visibility = Android.Views.ViewStates.Gone;
			} else {
				EdittxtRem.Visibility = Android.Views.ViewStates.Visible;
			}
		}



	}
}
