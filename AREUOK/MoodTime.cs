
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

//for plotting
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Xamarin.Android;

namespace AREUOK
{
	[Activity (Label = "R-U-OK", Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]			
	public class MoodTime : Activity
	{
		MoodDatabase db;
		Android.Database.ICursor cursor;
		//for plotting
		private PlotView plotViewModel;
		public PlotModel MyModel { get; set; }

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			//somehow an orientation change changes the language. Therefore we check and reset the language here depending on the stored preferences
			//check language preferences, if they are set apply them otherwise stay with the current language
			ISharedPreferences sharedPref = GetSharedPreferences("com.FSoft.are_u_ok.PREFERENCES",FileCreationMode.Private);
			String savedLanguage = sharedPref.GetString ("Language", "");
			//if there is a saved language (length > 0) and the current language is different from the saved one, then change
			Android.Content.Res.Configuration conf = Resources.Configuration;
			if ((savedLanguage.Length > 0) & (conf.Locale.Language != savedLanguage)){
				//set language and restart activity to see the effect
				conf.Locale = new Java.Util.Locale(savedLanguage);
				Android.Util.DisplayMetrics dm = this.Resources.DisplayMetrics;
				this.Resources.UpdateConfiguration (conf, dm);
			}

			// Create your application here
			SetContentView(Resource.Layout.PlotScreen);
			db = new MoodDatabase(this);

			Button Back = FindViewById<Button> (Resource.Id.button1);
			Back.Click += delegate {
				//create an intent to go back to the history screen
//				Intent intent = new Intent(this, typeof(Home));
//				intent.SetFlags(ActivityFlags.ClearTop); //remove the history and go back to home screen
//				StartActivity(intent);
				OnBackPressed();
			};
				

			//Create Plot
			// http://blog.bartdemeyer.be/2013/03/creating-graphs-in-wpf-using-oxyplot/

			plotViewModel = FindViewById<PlotView>(Resource.Id.plotViewModel);

			//query database
			cursor = db.ReadableDatabase.RawQuery("SELECT date, time, mood FROM MoodData WHERE NOT mood = -1", null); // cursor query

			//read out date and time and convert back to DateTime item for plotting
//			cursor.MoveToFirst();
//			string date_temp = cursor.GetString(cursor.GetColumnIndex("date"));
//			string time_temp = cursor.GetString(cursor.GetColumnIndex("time"));
//			DateTime date_time_temp = DateTime.ParseExact (date_temp + " " + time_temp, "dd.MM.yy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
//			//print for debug
//			System.Console.WriteLine("Date Time: " + date_time_temp.ToString());

			//only continue if there is data, otherwise there will be an error
			if (cursor.Count > 0) {

				var lineSerie = new LineSeries ();

				for (int ii = 0; ii < cursor.Count; ii++) {
					cursor.MoveToPosition (ii);
					//read out date and time and convert back to DateTime item for plotting
					string date_temp = cursor.GetString (cursor.GetColumnIndex ("date"));
					string time_temp = cursor.GetString (cursor.GetColumnIndex ("time"));
					DateTime date_time_temp = DateTime.ParseExact (date_temp + " " + time_temp, "dd.MM.yy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
					//System.Console.WriteLine("Date Time: " + date_time_temp.ToString());
					//add point (date_time, mood) to line series
					lineSerie.Points.Add (new DataPoint (OxyPlot.Axes.DateTimeAxis.ToDouble (date_time_temp), (double)cursor.GetInt (cursor.GetColumnIndex ("mood"))));
				}

				PlotModel temp = new PlotModel ();
				//determine font size, either keep default or for small screens set it to a smaller size
				double dFontSize = temp.DefaultFontSize;
				double dMarkerSize = 8;
				if (Resources.DisplayMetrics.HeightPixels <= 320) {
					dFontSize = 5;
					dMarkerSize = 5;

				}
				//define axes
				var dateAxis = new OxyPlot.Axes.DateTimeAxis ();
				dateAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;
				dateAxis.StringFormat = "dd/MM HH:mm";
				dateAxis.Title = Resources.GetString (Resource.String.Time);
				dateAxis.FontSize = dFontSize; 
				temp.Axes.Add (dateAxis);
				var valueAxis = new OxyPlot.Axes.LinearAxis ();
				valueAxis.Position = OxyPlot.Axes.AxisPosition.Left;
				valueAxis.Title = Resources.GetString (Resource.String.Mood);
				valueAxis.FontSize = dFontSize; 
				valueAxis.Maximum = 10;
				valueAxis.Minimum = 0;
				valueAxis.AbsoluteMinimum = 0;
				valueAxis.AbsoluteMaximum = 10;
				valueAxis.MajorTickSize = 2;
				valueAxis.IsZoomEnabled = false;
				valueAxis.StringFormat = "0";
				temp.Axes.Add (valueAxis);
				lineSerie.MarkerType = MarkerType.Square;
				lineSerie.MarkerSize = dMarkerSize;
				lineSerie.LabelFormatString = "{1}";  //http://discussion.oxyplot.org/topic/490066-trackerformatstring-question/
				lineSerie.FontSize = dFontSize;
				temp.Series.Add (lineSerie);
				MyModel = temp;

				plotViewModel.Model = MyModel;
			}

		}

		protected override void OnDestroy ()
		{
			cursor.Close();
			db.Close ();
			base.OnDestroy();
		}
			
	}
}

