
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
using Java.IO;

namespace AREUOK
{
	[Activity (Label = "R-U-OK", Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class QuestionnaireSummary : Activity
	{
		RUOKDatabase dbRUOK;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.QuestionnaireSummary);
			int Ergebnis = Intent.GetIntExtra ("ergebnis", 0);

			TextView ErgebnisText = FindViewById<TextView> (Resource.Id.textView3);
			ErgebnisText.Text = string.Format ("{0} {1}", Resources.GetText (Resource.String.total_score), Ergebnis);

			Button ContinueHome = FindViewById<Button> (Resource.Id.button1);
			ContinueHome.Click += delegate {
				//create an intent to go to the next screen
				Intent intent = new Intent(this, typeof(Home));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history and go back to home screen
				StartActivity(intent);
			};

			//Toast.MakeText (this, string.Format ("{0}", DateTime.Now.ToString("dd.MM.yy HH:mm")), ToastLength.Short).Show();

			ContentValues insertValues = new ContentValues();
			insertValues.Put("date_time", DateTime.Now.ToString("dd.MM.yy HH:mm"));
			insertValues.Put("ergebnis", Ergebnis);
			dbRUOK = new RUOKDatabase(this);
			dbRUOK.WritableDatabase.Insert ("RUOKData", null, insertValues);

			//The following two function were used to understand the usage of SQLite. They are not needed anymore and I just keep them in case I wanna later look back at them.
			//InitializeSQLite3(Ergebnis);
			//ReadOutDB ();
		}

		private void InitializeSQLite3(int Ergebnis) {

			File databaseFile = GetDatabasePath("names.db");
			databaseFile.Mkdirs();
			databaseFile.Delete (); //take this out after the DB is initialized

			bool temp = databaseFile.Exists (); //even when all the app data is deleted, the .db file still exists
			//System.Console.WriteLine("Exists: {0}", temp.ToString());
			SQLiteDatabase database = SQLiteDatabase.OpenOrCreateDatabase(databaseFile, null);

			//in case the file exists, we have already created the table. Otherwise we still need to do so
			//if (!temp)

			//take this out after the DB is initialized
				database.ExecSQL("create table user(id integer primary key autoincrement," +
					"first text not null, last text not null, " + 
					"username text not null,  password text not null, ergebnis int not null)");							
				
			database.ExecSQL("insert into user(first,last,username, password, ergebnis) " +
				"values('Bertie','Ahern','bahern','celia123', '" + Ergebnis.ToString() + "')");
			
		}

		private void ReadOutDB() {
			File databaseFile = GetDatabasePath("names.db");
			SQLiteDatabase db = SQLiteDatabase.OpenOrCreateDatabase(databaseFile, null);
			Android.Database.ICursor projCursor = db.Query("user", null, null, null, null, null, null);
			projCursor.MoveToFirst ();
			int idColIndex = projCursor.GetColumnIndex ("ergebnis");
			int ErgebnisFromDBFirst = projCursor.GetInt(idColIndex);
			projCursor.MoveToLast ();
			int ErgebnisFromDBLast = projCursor.GetInt(idColIndex);
			//Toast.MakeText (this, string.Format ("Ergebnis First: {0} Ergebnis Last: {1}", ErgebnisFromDBFirst, ErgebnisFromDBLast), ToastLength.Short).Show ();

			idColIndex = projCursor.GetColumnIndex ("password");
			string PassFromDB = projCursor.GetString (idColIndex);
			//Toast.MakeText (this, string.Format ("Password: {0}", PassFromDB), ToastLength.Short).Show ();
		}

	}
}

