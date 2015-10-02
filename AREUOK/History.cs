
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

//For exporting the DB
using Java.Nio;
using Java.IO;


namespace AREUOK
{
	[Activity (Label = "R-U-OK", Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class History : Activity
	{
		MoodDatabase db;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView(Resource.Layout.History);
			db = new MoodDatabase(this);

			Button MoodTime = FindViewById<Button> (Resource.Id.buttonMoodTime);
			MoodTime.Click += delegate {
				//create an intent to go to the next screen
				Intent intent = new Intent(this, typeof(MoodTime));
				StartActivity(intent);
			};

			Button MoodPeople = FindViewById<Button> (Resource.Id.buttonMoodPeople);
			MoodPeople.Click += delegate {
				//create an intent to go to the next screen
				Intent intent = new Intent(this, typeof(MoodPeople));
				StartActivity(intent);
			};


			Button ExDB = FindViewById<Button> (Resource.Id.buttonExDB);
			ExDB.Click += delegate {
				//delete current DB and fill it with an example dataset
				db.WritableDatabase.ExecSQL("DROP TABLE IF EXISTS MoodData");
				db.WritableDatabase.ExecSQL(MoodDatabase.create_table_sql);
				//we want histograms that show bad mood when you are alone and good mood when you are with people
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('29.09.15', '07:30', 3, 0, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('29.09.15', '09:30', 3, 0, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('29.09.15', '11:30', 7, 2, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('29.09.15', '16:30', 1, 0, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('29.09.15', '20:30', 6, 2, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('30.09.15', '07:30', 3, 0, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('30.09.15', '09:30', 2, 0, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('30.09.15', '11:30', 7, 2, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('30.09.15', '16:30', 1, 0, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('30.09.15', '20:30', 6, 2, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('01.10.15', '09:30', 2, 0, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('01.10.15', '11:30', 7, 2, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('01.10.15', '13:30', 1, 0, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('01.10.15', '16:30', 8, 2, 1, 1, 1023)");
				db.WritableDatabase.ExecSQL("INSERT INTO MoodData (date, time, mood, people, what, location, QuestionFlags) VALUES ('01.10.15', '18:30', 3, 0, 1, 1, 1023)");
			};
				

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
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history 
				StartActivity(intent);
			};

			//EXPORT BUTTON TO WRITE SQLITE DB FILE TO SD CARD
			Button ExportButton = FindViewById<Button> (Resource.Id.button3);
			ExportButton.Click += delegate {
				File sd = GetExternalFilesDir(null);
				File backupDB = new File(sd, "MoodData.db"); //this is where we're going to export to
				//this is the database file
				File data = GetDatabasePath("MoodData.db");
				//Android.Widget.Toast.MakeText(this, data.AbsolutePath, Android.Widget.ToastLength.Short).Show();

				OutputStream OS = new FileOutputStream(backupDB);
				InputStream IS = new FileInputStream(data);
				//the actual copying action
				byte[] dataByte = new byte[IS.Available()];
				IS.Read(dataByte);
				OS.Write(dataByte);
				IS.Close();
				OS.Close();

				//http://developer.android.com/reference/android/content/Context.html#getExternalFilesDir%28java.lang.String%29
				//http://www.techrepublic.com/blog/software-engineer/export-sqlite-data-from-your-android-device/
			};
				


		}

		protected override void OnDestroy ()
		{
			db.Close ();
			base.OnDestroy();
		}
			
	}
}

