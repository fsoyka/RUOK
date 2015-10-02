
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
	[Activity (Label = "R-U-OK", Icon = "@drawable/icon", ConfigurationChanges =  Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]			
	public class MoodPeople : Activity
	{
		MoodDatabase db;
		Android.Database.ICursor cursor;
		//for plotting
		private PlotView plotViewModelLeft;
		private PlotView plotViewModelRight;
		public PlotModel MyModelLeft { get; set; }
		public PlotModel MyModelRight { get; set; }

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView(Resource.Layout.PlotDoubleScreen);
			db = new MoodDatabase(this);

			Button Back = FindViewById<Button> (Resource.Id.button1);
			Back.Click += delegate {
				//create an intent to go back to the history screen
//				Intent intent = new Intent(this, typeof(History));
//				intent.SetFlags(ActivityFlags.ClearTop); //remove the history and go back to home screen
//				StartActivity(intent);
				OnBackPressed();
			};
				

			//CREATE HISTOGRAM FOR PEOPLE CONTEXT ON THE LEFT
			plotViewModelLeft = FindViewById<PlotView>(Resource.Id.plotViewModelLeft);

			//select all mood values for which people was 0 (alone)
			cursor = db.ReadableDatabase.RawQuery("SELECT mood FROM MoodData WHERE people = 0", null); // cursor query


			if (cursor.Count > 0) {
				
				//initialize with 9 zero entries
				int[] histArray = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
				//go through each entry and create the histogram count
				for (int ii = 0; ii < cursor.Count; ii++) {
					cursor.MoveToPosition (ii);
					int mood_temp = cursor.GetInt (0); //get mood from database
					histArray [mood_temp] += 1; //increase histogram frequency by one
					//System.Console.WriteLine("Mood: " + mood_temp.ToString() + " Freq: " + histArray [mood_temp].ToString());
				}

				PlotModel temp = new PlotModel ();
				//determine font size, either keep default or for small screens set it to a smaller size
				double dFontSize = temp.DefaultFontSize;
				if (Resources.DisplayMetrics.HeightPixels <= 320)
					dFontSize = 8;

				//define axes

				//we need 9 categories for the histogram since we score the mood between 0 and 8
				var categoryAxis1 = new OxyPlot.Axes.CategoryAxis ();
//			categoryAxis1.GapWidth = 0;
				categoryAxis1.LabelField = "Label";
//			categoryAxis1.MinorStep = 1;
				categoryAxis1.Position = OxyPlot.Axes.AxisPosition.Bottom;
				categoryAxis1.ActualLabels.Add ("0");
				categoryAxis1.ActualLabels.Add ("1");
				categoryAxis1.ActualLabels.Add ("2");
				categoryAxis1.ActualLabels.Add ("3");
				categoryAxis1.ActualLabels.Add ("4");
				categoryAxis1.ActualLabels.Add ("5");
				categoryAxis1.ActualLabels.Add ("6");
				categoryAxis1.ActualLabels.Add ("7");
				categoryAxis1.ActualLabels.Add ("8");

//			categoryAxis1.AbsoluteMaximum = 10;
//			categoryAxis1.Maximum = 10;
				categoryAxis1.StringFormat = "0";
				categoryAxis1.IsPanEnabled = false;
				categoryAxis1.IsZoomEnabled = false;
				categoryAxis1.FontSize = dFontSize;
				categoryAxis1.Title = Resources.GetString (Resource.String.Mood);
				temp.Axes.Add (categoryAxis1);

				var linearAxis1 = new OxyPlot.Axes.LinearAxis ();
				linearAxis1.AbsoluteMinimum = 0;
				linearAxis1.AbsoluteMaximum = histArray.Max () * 1.2; //this has to be a bit higher than the highest frequency of the histogram
				linearAxis1.Minimum = 0;
				linearAxis1.Maximum = histArray.Max () * 1.2;
//			linearAxis1.MaximumPadding = 0.1;
//			linearAxis1.MinimumPadding = 0;
				linearAxis1.Position = OxyPlot.Axes.AxisPosition.Left;
				linearAxis1.FontSize = dFontSize;
				linearAxis1.IsZoomEnabled = false;
				linearAxis1.StringFormat = "0.0";
				linearAxis1.Title = Resources.GetString (Resource.String.Frequency);
				temp.Axes.Add (linearAxis1);

				var columnSeries1 = new ColumnSeries ();
				//http://forums.xamarin.com/discussion/20809/is-there-no-plotview-which-is-in-oxyplot-compent-in-xamarin-android
				//add data
				foreach (int i in histArray) {
					columnSeries1.Items.Add (new ColumnItem (i, -1));
				}
				columnSeries1.LabelFormatString = "{0}";
				columnSeries1.FontSize = dFontSize;
				temp.Series.Add (columnSeries1);


				temp.Title = Resources.GetString (Resource.String.Alone);
				temp.TitleFontSize = dFontSize;
				MyModelLeft = temp;

				plotViewModelLeft.Model = MyModelLeft;
			}

			//CREATE HISTOGRAM FOR PEOPLE CONTEXT ON THE RIGHT
			plotViewModelRight = FindViewById<PlotView>(Resource.Id.plotViewModelRight);

			//select all mood values for which people was 0 (alone)
			cursor = db.ReadableDatabase.RawQuery("SELECT mood FROM MoodData WHERE NOT people = 0", null); // cursor query

			//only continue if there is data, otherwise there will be an error
			if (cursor.Count > 0) {

				//initialize with 9 zero entries
				int[] histArrayRight = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
				//go through each entry and create the histogram count
				for (int ii = 0; ii < cursor.Count; ii++) {
					cursor.MoveToPosition (ii);
					int mood_temp = cursor.GetInt (0); //get mood from database
					histArrayRight [mood_temp] += 1; //increase histogram frequency by one
					//System.Console.WriteLine("Mood: " + mood_temp.ToString() + " Freq: " + histArray [mood_temp].ToString());
				}

				PlotModel tempRight = new PlotModel ();
				double dFontSize = tempRight.DefaultFontSize;
				if (Resources.DisplayMetrics.HeightPixels <= 320)
					dFontSize = 8;
				

				//define axes

				//we need 9 categories for the histogram since we score the mood between 0 and 8
				var categoryAxisRight = new OxyPlot.Axes.CategoryAxis ();
				categoryAxisRight.LabelField = "Label";
				categoryAxisRight.Position = OxyPlot.Axes.AxisPosition.Bottom;
				categoryAxisRight.ActualLabels.Add ("0");
				categoryAxisRight.ActualLabels.Add ("1");
				categoryAxisRight.ActualLabels.Add ("2");
				categoryAxisRight.ActualLabels.Add ("3");
				categoryAxisRight.ActualLabels.Add ("4");
				categoryAxisRight.ActualLabels.Add ("5");
				categoryAxisRight.ActualLabels.Add ("6");
				categoryAxisRight.ActualLabels.Add ("7");
				categoryAxisRight.ActualLabels.Add ("8");
				categoryAxisRight.StringFormat = "0";
				categoryAxisRight.IsPanEnabled = false;
				categoryAxisRight.IsZoomEnabled = false;
				categoryAxisRight.FontSize = dFontSize;
				categoryAxisRight.Title = Resources.GetString (Resource.String.Mood);
				tempRight.Axes.Add (categoryAxisRight);

				var linearAxisRight = new OxyPlot.Axes.LinearAxis ();
				linearAxisRight.AbsoluteMinimum = 0;
				linearAxisRight.AbsoluteMaximum = histArrayRight.Max () * 1.2; //this has to be a bit higher than the highest frequency of the histogram
				linearAxisRight.Minimum = 0;
				linearAxisRight.Maximum = histArrayRight.Max () * 1.2;
				linearAxisRight.Position = OxyPlot.Axes.AxisPosition.Left;
				linearAxisRight.FontSize = dFontSize;
				linearAxisRight.IsZoomEnabled = false;
				linearAxisRight.StringFormat = "0.0";
				linearAxisRight.Title = Resources.GetString (Resource.String.Frequency);
				tempRight.Axes.Add (linearAxisRight);

				var columnSeriesRight = new ColumnSeries ();
				//http://forums.xamarin.com/discussion/20809/is-there-no-plotview-which-is-in-oxyplot-compent-in-xamarin-android
				//add data
				foreach (int i in histArrayRight) {
					columnSeriesRight.Items.Add (new ColumnItem (i, -1));
				}
				columnSeriesRight.LabelFormatString = "{0}";
				columnSeriesRight.FontSize = dFontSize;
				tempRight.Series.Add (columnSeriesRight);


				tempRight.Title = Resources.GetString (Resource.String.WithPeople);
				tempRight.TitleFontSize = dFontSize;
				MyModelRight = tempRight;

				plotViewModelRight.Model = MyModelRight;
			}

		}

		protected override void OnDestroy ()
		{
			cursor.Close();
			db.Close ();
			base.OnDestroy();
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			//plotViewModelLeft.ActualModel.Title = Resources.GetString(Resource.String.Alone);
			System.Console.WriteLine(Resources.GetString(Resource.String.Alone));
			//somehow the orientation change changes the language. Try to fix this by saving the language and re-setting it manually everytime the orientation changes
		}
			
	}
}

