
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
		MoodDatabase db;
		Android.Database.ICursor cursor;
		ListView listView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView(Resource.Layout.History);
			listView = FindViewById<ListView>(Resource.Id.listView1);
			db = new MoodDatabase(this);

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
				db.WritableDatabase.ExecSQL("DROP TABLE IF EXISTS MoodData");
				db.WritableDatabase.ExecSQL(MoodDatabase.create_table_sql);
				//restart this activity in order to update the view
				Intent intent = new Intent(this, typeof(History));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history and go back to home screen
				StartActivity(intent);
			};

			//query database and link to the listview
			cursor = db.ReadableDatabase.RawQuery("SELECT * FROM MoodData ORDER BY _id DESC", null); // cursor query
			//why this command is deprecated and what should be used instead: http://www.androiddesignpatterns.com/2012/07/loaders-and-loadermanager-background.html
			//http://www.vogella.com/tutorials/AndroidSQLite/article.html
			//http://www.codeproject.com/Articles/792883/Using-Sqlite-in-a-Xamarin-Android-Application-Deve
			StartManagingCursor(cursor);

			// which columns map to which layout controls
			//string[] fromColumns = new string[] {"date", "time", "mood", "people", "what", "location"};
			string[] fromColumns = new string[] {"date", "mood"};
			int[] toControlIDs = new int[] {Android.Resource.Id.Text1, Android.Resource.Id.Text2};

			// use a SimpleCursorAdapter
			listView.Adapter = new SimpleCursorAdapter (this, Android.Resource.Layout.SimpleListItem2, cursor, fromColumns, toControlIDs);
			listView.ItemClick += OnListItemClick;
		}

		protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
		{
			var obj = listView.Adapter.GetItem(e.Position);
			var curs = (Android.Database.ICursor)obj;
			var text = curs.GetString(1); // 'date' is column 1 These refer to the absolute columns in the db not the columns specified above for showing in the listview
			var text2 = curs.GetString(3); // 'time' is column 2
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

