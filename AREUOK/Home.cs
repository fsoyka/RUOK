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
	[Activity (Label = "R-U-OK", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait )]
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

			Button SettingsButton = FindViewById<Button> (Resource.Id.button2);
			SettingsButton.Click += delegate {
				Intent intent = new Intent(this, typeof(Settings));
				StartActivity(intent);
			};

			Button AboutButton = FindViewById<Button> (Resource.Id.button3);
			AboutButton.Click += delegate {
				//create an intent to go to the next screen
				Intent intent = new Intent(this, typeof(About));
				StartActivity(intent);
			};

			//check language preferences, if they are set apply them otherwise stay with the current language
			ISharedPreferences sharedPref = GetSharedPreferences("com.FSoft.are_u_ok.PREFERENCES",FileCreationMode.Private);
			String savedLanguage = sharedPref.GetString ("Language", "");
			//for debuging
		    //	Toast.MakeText (this, string.Format ("Language: {0}", savedLanguage), ToastLength.Short).Show ();

			//if there is a saved language (length > 0) and the current language is different from the saved one, then change
			Android.Content.Res.Configuration conf = this.Resources.Configuration;
			if ((savedLanguage.Length > 0) & (conf.Locale.Language != savedLanguage)){
				//set language and restart activity to see the effect
				conf.Locale = new Java.Util.Locale(savedLanguage);
				Android.Util.DisplayMetrics dm = this.Resources.DisplayMetrics;
				this.Resources.UpdateConfiguration (conf, dm);
				Intent intent = new Intent(this, typeof(Home));
				intent.SetFlags(ActivityFlags.ClearTop); //remove the history
				StartActivity(intent);
			}

			//if there is a saved name provided a personalized greeting
			String savedName = sharedPref.GetString ("Name", "");
			if (savedName.Length > 0) {
				TextView WelcomeText = FindViewById<TextView> (Resource.Id.textView2);
				WelcomeText.Text = string.Format ("{0} {1}", Resources.GetText (Resource.String.Greeting), savedName);
			}
		}
	}
}


