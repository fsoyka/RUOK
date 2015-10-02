
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
	[Activity (Label = "R-U-OK", Icon = "@drawable/icon")]			
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
			cursor = db.ReadableDatabase.RawQuery("SELECT date, time, mood FROM MoodData", null); // cursor query

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
				if (Resources.DisplayMetrics.HeightPixels <= 320)
					dFontSize = 8;
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
				lineSerie.MarkerSize = 8;
				lineSerie.LabelFormatString = "{1}";  //http://discussion.oxyplot.org/topic/490066-trackerformatstring-question/
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

