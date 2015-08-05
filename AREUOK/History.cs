
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
using Android.Database.Sqlite; 

namespace AREUOK
{
	[Activity (Label = "R-U-OK", Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class History : Activity
	{
		RUOKDatabase db;
		Android.Database.ICursor cursor;
		ListView listView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView(Resource.Layout.History);
			listView = FindViewById<ListView>(Resource.Id.listView1);
			db = new RUOKDatabase(this);

			Button BackHome = FindViewById<Button> (Resource.Id.button1);
			BackHome.Click += delegate {
				//create an intent to go to the next screen
				Intent intent = new Intent(this, typeof(Home));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history and go back to home screen
				StartActivity(intent);
			};

			Button DeleteButton = FindViewById<Button> (Resource.Id.button2);
			DeleteButton.Click += delegate {
				//create an intent to go to the next screen
				db.WritableDatabase.ExecSQL("DROP TABLE IF EXISTS RUOKData");
				db.WritableDatabase.ExecSQL(RUOKDatabase.create_table_sql);
				//restart this activity in order to update the view
				Intent intent = new Intent(this, typeof(History));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history and go back to home screen
				StartActivity(intent);
			};

			//query database and link to the listview
			cursor = db.ReadableDatabase.RawQuery("SELECT * FROM RUOKData ORDER BY _id DESC", null); // cursor query
			StartManagingCursor(cursor);

			// which columns map to which layout controls
			string[] fromColumns = new string[] {"date_time", "ergebnis"};
			int[] toControlIDs = new int[] {Android.Resource.Id.Text1, Android.Resource.Id.Text2};

			// use a SimpleCursorAdapter
			listView.Adapter = new SimpleCursorAdapter (this, Android.Resource.Layout.SimpleListItem2, cursor, fromColumns, toControlIDs);
			listView.ItemClick += OnListItemClick;
		}

		protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
		{
			var obj = listView.Adapter.GetItem(e.Position);
			var curs = (Android.Database.ICursor)obj;
			var text = curs.GetString(1); // 'date_time' is column 1
			var text2 = curs.GetString(2); // 'score' is column 2
			Android.Widget.Toast.MakeText(this, text + " " + text2, Android.Widget.ToastLength.Short).Show();
			//System.Console.WriteLine("Clicked on " + text);
		}

		protected override void OnDestroy ()
		{
			StopManagingCursor(cursor);
			cursor.Close();
			base.OnDestroy();
		}
	}
}

