using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.Res;


namespace AREUOK
{
	[Activity (Label = "R-U-OK", MainLauncher = true, Icon = "@drawable/icon")]
	public class Home : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Home);
		
			Button Questionnaire = FindViewById<Button> (Resource.Id.button1);
			Questionnaire.Click += delegate {
				//create an intent to go to the next screen
				Intent intent = new Intent(this, typeof(QuestionnaireFirstScreen));
				StartActivity(intent);
			};

			Button LangChange = FindViewById<Button> (Resource.Id.button2);
			LangChange.Click += delegate {
				//change language
				Android.Content.Res.Configuration conf = this.Resources.Configuration;
				//Debug output
				Console.WriteLine("Language: {0}", conf.Locale.Language.ToString());
				if (conf.Locale.Language == "de")
					conf.SetLocale(Java.Util.Locale.English);
				else
					conf.SetLocale(Java.Util.Locale.German);
				Android.Util.DisplayMetrics dm = this.Resources.DisplayMetrics;
				this.Resources.UpdateConfiguration (conf, dm);
				//Restart activity such that the changes take effect
				Intent intent = new Intent(this, typeof(Home));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history and reload the home screen
				StartActivity(intent);
			};

			Button AboutButton = FindViewById<Button> (Resource.Id.button3);
			AboutButton.Click += delegate {
				//create an intent to go to the next screen
				Intent intent = new Intent(this, typeof(About));
				StartActivity(intent);
			};
				
		}
	}
}


