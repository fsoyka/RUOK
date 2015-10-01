
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

//for plotting
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Xamarin.Android;

namespace AREUOK
{
	[Activity (Label = "R-U-OK", Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class History : Activity
	{
		MoodDatabase db;
		Android.Database.ICursor cursor;
		//for plotting
		private PlotView plotViewModel;
		public PlotModel MyModel { get; set; }

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView(Resource.Layout.History);
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

			//Create Plot
			// http://blog.bartdemeyer.be/2013/03/creating-graphs-in-wpf-using-oxyplot/

			plotViewModel = FindViewById<PlotView>(Resource.Id.plotViewModel);

			//query database
			cursor = db.ReadableDatabase.RawQuery("SELECT date, time, mood FROM MoodData", null); // cursor query

			//read out date and time and convert back to DateTime item for plotting
//			cursor.MoveToFirst();
//			string date_temp = cursor.GetString(cursor.GetColumnIndex("date"));
//			string time_temp = cursor.GetString(cursor.GetColumnIndex("time"));
//			DateTime date_time_temp = DateTime.ParseExact (date_temp + " " + time_temp, "dd.MM.yy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
//			//print for debug
//			System.Console.WriteLine("Date Time: " + date_time_temp.ToString());

			var lineSerie = new LineSeries();

			for(int ii = 0; ii < cursor.Count; ii++) {
				cursor.MoveToPosition(ii);
				//read out date and time and convert back to DateTime item for plotting
				string date_temp = cursor.GetString(cursor.GetColumnIndex("date"));
				string time_temp = cursor.GetString(cursor.GetColumnIndex("time"));
				DateTime date_time_temp = DateTime.ParseExact (date_temp + " " + time_temp, "dd.MM.yy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
				//System.Console.WriteLine("Date Time: " + date_time_temp.ToString());
				//add point (date_time, mood) to line series
				lineSerie.Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(date_time_temp),(double)cursor.GetInt(cursor.GetColumnIndex("mood"))));
			}

			PlotModel temp = new PlotModel();
			//define axes
			var dateAxis = new OxyPlot.Axes.DateTimeAxis();
			dateAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;
			dateAxis.StringFormat = "dd/MM HH:mm";
			dateAxis.Title = "Time";
			//dateAxis.FontSize = 8;  //TODO Fix font size for small devices
			temp.Axes.Add(dateAxis);
			var valueAxis = new OxyPlot.Axes.LinearAxis ();
			valueAxis.Position = OxyPlot.Axes.AxisPosition.Left;
			valueAxis.Title = "Mood";
			//valueAxis.FontSize = 8;
			valueAxis.Maximum = 8.5;
			valueAxis.Minimum = 0;
			valueAxis.AbsoluteMinimum = 0;
			valueAxis.AbsoluteMaximum = 8.5;
			valueAxis.MajorTickSize = 2;
			valueAxis.IsZoomEnabled = false;
			valueAxis.StringFormat = "0";
			temp.Axes.Add(valueAxis);
			lineSerie.MarkerType = MarkerType.Square;
			lineSerie.MarkerSize = 8;
			lineSerie.LabelFormatString = "{1}";  //http://discussion.oxyplot.org/topic/490066-trackerformatstring-question/
			temp.Series.Add(lineSerie);
			MyModel = temp;

			plotViewModel.Model = MyModel;

		}

		protected override void OnDestroy ()
		{
			cursor.Close();
			db.Close ();
			base.OnDestroy();
		}
			
	}
}

