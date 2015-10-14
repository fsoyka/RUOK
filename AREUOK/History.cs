
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
				//Feedback message
				Toast toast = Toast.MakeText (this, GetString (Resource.String.Done), ToastLength.Short);
				toast.SetGravity (GravityFlags.Center, 0, 0);
				toast.Show ();
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

				//Feedback message
				Toast toast = Toast.MakeText (this, GetString (Resource.String.Done), ToastLength.Short);
				toast.SetGravity (GravityFlags.Center, 0, 0);
				toast.Show ();

				//restart this activity in order to update the view
				Intent intent = new Intent(this, typeof(History));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history 
				StartActivity(intent);
			};

			//EXPORT BUTTON TO WRITE SQLITE DB FILE TO SD CARD
			Button ExportButton = FindViewById<Button> (Resource.Id.button3);
			ExportButton.Click += delegate {

				//This is for exporting the db file
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

				//Now try to export everything as a csv file
				Android.Database.ICursor cursor; 
				cursor = db.ReadableDatabase.RawQuery("SELECT * FROM MoodData ORDER BY _id DESC", null); // cursor query
				//only write a file if there are entries in the DB
				if (cursor.Count > 0) {
					backupDB = new File(sd, "MoodData.csv"); //this is where we're going to export to
					OS = new FileOutputStream(backupDB);
					//write a header in the beginning
					string header = "date; time; mood; people; what; location; pos1; pos2; pos3; pos4; pos5; neg1; neg2; neg3; neg4; neg5\n";
					byte[] bytes = new byte[header.Length * sizeof(char)];
					System.Buffer.BlockCopy(header.ToCharArray(), 0, bytes, 0, bytes.Length);
					OS.Write(bytes);

					for (int ii = 0; ii < cursor.Count; ii++) { //go through all rows
						cursor.MoveToPosition (ii);
						//now go through all columns
						for (int kk = 1; kk < cursor.ColumnCount-1; kk++) { //skip the first column since it is just the ID and the last since it's the question flags
							//[date] TEXT NOT NULL, [time] TEXT NOT NULL, [mood] INT NOT NULL, [people] INT NOT NULL, [what] INT NOT NULL, [location] INT NOT NULL, [pos1] INT, [pos2] INT , [pos3] INT, [pos4] INT, [pos5] INT, [neg1] INT, [neg2] INT , [neg3] INT, [neg4] INT, [neg5] INT, [QuestionFlags] INT NOT NULL)";
							//the first two columns are strings, the rest is int
							string tempStr;
							if (kk < 3) {
								tempStr = cursor.GetString(kk);
							}
							else {
								int tempInt = cursor.GetInt(kk);
								tempStr = tempInt.ToString();
							}
							if (kk == cursor.ColumnCount-2) //if last column, advance to next line
								tempStr += "\n";
							else
								tempStr += "; ";
							//convert to byte and write
							bytes = new byte[tempStr.Length * sizeof(char)];
							System.Buffer.BlockCopy(tempStr.ToCharArray(), 0, bytes, 0, bytes.Length);
							OS.Write(bytes);
						}
					}

					OS.Close();
					//send via email
					var email = new Intent(Intent.ActionSend);
					email.SetType("text/plain");
					//email.PutExtra(Android.Content.Intent.ExtraEmail, new string[]{"fsoyka@gmail.com"});
					email.PutExtra(Android.Content.Intent.ExtraSubject, "R-U-OK Export");
					email.PutExtra(Android.Content.Intent.ExtraText, "Beschreibung der Datenbank Einträge :\n[mood] Stimmung 0-8, -1 wenn die Erinnerung verpasst wurde\n[people] keiner-viele, 0-2 \n[what] Freizeit-Arbeit, 0-2 \n[location] Unterwegs/Daheim, 0-1 \n[pos1-5] und [neg1-5] sind die Affekt Fragen, bewertet zwischen 1-9. Einträge mit 0 sind nicht gefragt worden. Die Frage sind folgende:\nPos1: Wie fröhlich fühlen Sie sich?\nPos2: Wie optimistisch sind Sie?\nPos3: Wie zufrieden sind Sie?\nPos4: Wie entspannt sind Sie?\nPos5: 5te Frage fehlt noch\nNeg1: Wie traurig sind Sie?\nNeg2: Wie ängstlich sind Sie?\nNeg3: Wie einsam sind Sie?\nNeg4: Wie unruhig sind Sie?\nNeg5: Wie ärgerlich sind Sie?\n" );
					email.PutExtra(Android.Content.Intent.ExtraStream, Android.Net.Uri.Parse("file://" + backupDB.AbsolutePath));
					//System.Console.WriteLine(backupDB.AbsolutePath);
					StartActivity(Intent.CreateChooser(email, "Send email..."));
				}

				//Feedback message
				Toast toast = Toast.MakeText (this, GetString (Resource.String.Done), ToastLength.Short);
				toast.SetGravity (GravityFlags.Center, 0, 0);
				toast.Show ();

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

