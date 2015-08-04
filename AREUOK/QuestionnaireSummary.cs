
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

namespace AREUOK
{
	[Activity (Label = "R-U-OK", Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class QuestionnaireSummary : Activity
	{
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

		}
	}
}

